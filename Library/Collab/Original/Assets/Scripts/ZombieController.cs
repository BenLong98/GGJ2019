using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour {

	[HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
	[HideInInspector] public GameObject target;

	// time between zombies evaluating things, basically their reaction time
	public float evaluationTime = 1f;


	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

		Invoke("EvaluateTarget",evaluationTime);
	}

	void EvaluateTarget() {
		GameObject closestPlayer = FindClosestTag("Player",gameObject, judgeByCanSee);
		
		if (closestPlayer != target) {
			ChangeTarget(closestPlayer);
		}

		GameObject partBetween = null;

		/*RaycastHit hit;
		Vector3 rayDirection = pos1.position - pos2.position;
		if (Physics.Raycast (transform.position, rayDirection, hit)) {
			if (hit.transform.tag == "structure") {
				partBetween = 
			}
		}
	*/


		Invoke("EvaluateTarget",evaluationTime);
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
	/// <param name="judgeByCanSee">optional: false, determines if findclosest</param>
	/// 
	GameObject FindClosestTag(string tag, GameObject obj, bool judgeByCanSee = false) {
		float closestDist = -1f;
		GameObject closestObject = null;
		
		foreach (GameObject searchObject in GameObject.FindGameObjectsWithTag(tag))
		{
			float dist = Vector3.Distance(searchObject.transform.position, obj.transform.position);
			if (closestObject==null || closestDist== -1f || dist<closestDist) {
                if (!judgeByCanSee || (judgeByCanSee && CanSee())){
                    closestDist = dist;
                    closestObject = searchObject;
                }
			}
		}
		
		return closestObject;
	}

	/// <summary>
	/// Determines if any object with given tag can be found between a source and target
	/// </summary>
	/// <param name="tag">the tag of the objects we want to see, e.g 'player'</param>
	/// <param name="pos">the source position</param>
	/// <param name="pos2">the target position</param>
	/// <returns></returns>
	bool CanSee(string tag, Vector3 pos, Vector3 pos2) {
		RaycastHit hit;
		Vector3 rayDirection = pos1.position - pos2.position;
		if (Physics.Raycast (transform.position, rayDirection, hit)) {
			if (hit.transform.tag == tag) {
				return true;
			} else {
				return false;
			}
		}
	}



	void Awake() {

	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			agent.destination = target.transform.position;
		}
	}

	void ChangeTarget(GameObject tgt) {
		target = tgt;
	}


}
