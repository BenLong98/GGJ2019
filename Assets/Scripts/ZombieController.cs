using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class ZombieController : HealthController {

	[HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    public float thrust = 40f;
    public GameObject target;

	

	WaveController waveController;

    Animator anim;

	// the distance from the target that 
	public float stopDistance = 1f;

	public int damage = 1;

	// time between zombies evaluating things, basically their reaction time
	public float evaluationTime = 1f;
    public float animationRandomizerTimer = 0.5f;

	bool canAttack = true;

    [SerializeField] GameObject limbs;



    void Awake()
    {
        anim = this.GetComponent<Animator>();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

	// Use this for initialization
	void Start () {
		
		waveController = GameObject.FindWithTag("wavecontroller").GetComponent<WaveController>();

		target = FindClosestTag("Player", gameObject, true);
		if (target == null)  target = FindClosestTag("Player", gameObject);
		

		Invoke("EvaluateTarget", evaluationTime);
        Invoke("RandomRunAnimation", animationRandomizerTimer);
	}

	void EvaluateTarget() {
		// get the closest "Player" tagged object, that we can see (judgeByCanSee true)
		GameObject closestPlayer = FindClosestTag("Player",gameObject, true);
		
		// if we found a closestplayer (judging by whether we can see them)
		if (closestPlayer != null) {
			ChangeTarget(closestPlayer);
		}

		else {
			closestPlayer = FindClosestTag("Player",gameObject);

			if (closestPlayer!=null) {
				// find a part between the zombie and the closestplayer
				GameObject partBetween = null;

				RaycastHit hit;
				Vector3 rayDirection = closestPlayer.transform.position - transform.position;
				if (Physics.Raycast (transform.position, rayDirection, out hit)) {
					if (hit.transform.tag == "structure") {
						partBetween = hit.transform.gameObject;
					}
				}

				if (partBetween != null) {
					ChangeTarget(partBetween);
				} 
				else if (target == null) {
					ChangeTarget(target);
				}
			}

		}

		if (target != null) {
			float distanceFromTarget = Vector3.Distance(target.transform.position,transform.position);
			if (target.GetComponent<HealthController>() && distanceFromTarget<=stopDistance+0.25f) {
				
				StartCoroutine( Attack() );
			}
		}
	


		Invoke("EvaluateTarget",evaluationTime);
	}

	IEnumerator Attack() {
		canAttack = false;
		//print("There's a trigger right here ready for playing the attack animation");
		anim.SetTrigger("Attack");

		// replace invoke with an animation trigger or some such nonsense, or just tune the number
		Invoke("AttackLanded", 1f);

		yield return new WaitForEndOfFrame();
	}

	void AttackLanded() {
		canAttack = true;
		if (target!=null && target.GetComponent<HealthController>()!=null) {
			if (NetworkServer.active) {
				target.GetComponent<HealthController>().TakeDamage(damage, netId);
			}
			if (NetworkClient.active) {
				target.GetComponent<HealthController>().CmdTakeDamage(damage, netId);
			}
		}
	}

    void RandomRunAnimation()
    {
        anim.SetInteger("RunIndex", Random.Range(0, 4));

        Invoke("RandomRunAnimation", animationRandomizerTimer);
    }

	

	// Find closest player you can see with RayCast OR 
	// objects in between the zombie and the target group of players.
	// 
	// if zombie has wall as target, will only switch target if they can hit a player by raycast, or the wall is destroyed

	/// <summary>
	/// Finds the object with the given tag, closest to the given gameobject
	/// </summary>
	/// <param name="tag">The tag of the other thing to search for</param>
	/// <param name="obj">The object that's looking through the tagged objects</param>
	/// <param name="judgeByCanSee">optional: false, determines if we only get objects we can see</param>
	/// 
	GameObject FindClosestTag(string tag, GameObject obj, bool judgeByCanSee = false) {
		float closestDist = -1f;
		GameObject closestObject = null;
		
		foreach (GameObject searchObject in GameObject.FindGameObjectsWithTag(tag))
		{
			float dist = Vector3.Distance(searchObject.transform.position, obj.transform.position);
			// if we haven't found anything yet or this is the closest thing AND...
			if (closestObject==null || closestDist== -1f || dist<closestDist) {
				// ... either we're not judging by whether we can see, or we are AND can see the thing
				if(!judgeByCanSee || (judgeByCanSee && Task.CanSee(searchObject,obj.transform.position,searchObject.transform.position) )) {
					closestDist = dist;
					closestObject = searchObject;
				}
			}
		}
		
		return closestObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			agent.destination = target.transform.position;
			agent.stoppingDistance = stopDistance;
		}

        anim.SetFloat("Speed", agent.velocity.magnitude);
	}

	void ChangeTarget(GameObject tgt) {
		if (target != tgt) {
			target = tgt;
		}
	}

    [ClientRpc]
    public override void RpcDeath()
    {
		print("rpc death is being called on this zombie on all clients");
		
		if (waveController!=null) {
			print("wave controller is not null");
			waveController.zombieDied(this);
		}	
		NetworkServer.Destroy(gameObject);
		//base.RpcDeath();
    }

	public override void OnServerDeath() {
        if (Random.Range(0, 10) == 1)
        {
            string[] pickups = new string[] { "PickupChaingun", "PickupRocketLauncher" };

            int index = Random.Range(0, pickups.Length);
            string chosenPickup = pickups[index];

            GameObject drop = Instantiate(Resources.Load<GameObject>("Pickups/" + chosenPickup));
            drop.transform.eulerAngles = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
            drop.transform.position = transform.position;

            NetworkServer.Spawn(drop);
        }




        //Spawn Ragdoll
        GameObject ragdoll = Instantiate(Resources.Load<GameObject>("ZombieRagdoll"));
        // ragdoll.transform.eulerAngles = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
        ragdoll.transform.position = transform.position;
        ragdoll.transform.rotation = transform.rotation;

        float randY = Random.Range(0, .5f);
        float randX = Random.Range(-.5f, .5f);

        ragdoll.GetComponentInChildren<Rigidbody>().AddForce(-transform.forward + new Vector3(randX, randY, 0) * 200, ForceMode.Impulse);
        

        NetworkServer.Spawn(ragdoll);


        //Destroy the game object
        //NetworkServer.Destroy(gameObject);
		base.OnServerDeath();
    }

}
