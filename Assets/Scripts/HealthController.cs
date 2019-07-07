using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HealthController : NetworkBehaviour
{
    [SyncVar] public int currHealth = 100;
    public int startingHealth = 100;
    public bool isDead;                                           
    public bool damaged;                                          



    void Awake()
    {
        currHealth = startingHealth;
    }

    public void PrintIsLocal()
    {
        if (this.isLocalPlayer)
        {
            print("IS LOCAL");
        }
        else
        {
            print("NOT local");
        }
    }




    public virtual void TakeDamage(int amount, NetworkInstanceId attacker)
    {
        print("this is being called now on "+gameObject.name);
        //print(amount + " " + attacker);
        currHealth -= amount;

        //print(currHealth);

        if (GetComponent<ParticleSystem>() != null) {
            GetComponent<ParticleSystem>().Play();
        }


        if (currHealth <= 0 && !isDead)
        {
            // the following code will only be run for player objects,
            // they're the only things that have money
            GameObject localAttacker = NetworkServer.FindLocalObject(attacker);
            if (localAttacker && localAttacker.GetComponent<PlayerController>()){

                localAttacker.GetComponent<PlayerController>().AddMoney(1);
            }

            // call rpc and on server death
            if (NetworkServer.active) {
                RpcDeath();
                OnServerDeath();
            }
            
        }
    }
    

    [Command]
    public void CmdTakeDamage(int amount, NetworkInstanceId attacker) {
        print("used to take damage here");
        //TakeDamage(amount, attacker);
    }

    public virtual void OnServerDeath() {
        //print()
        //NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    public virtual void RpcDeath()
    {
        //NetworkServer.Destroy(gameObject);
    }

    
}