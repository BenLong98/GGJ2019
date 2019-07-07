using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HealthController : NetworkBehaviour
{
    [SyncVar] public int currHealth;
    public int startingHealth;

    public bool isDead;                                           
    public bool damaged;                                          



    void Awake()
    {
        currHealth = startingHealth;
    }



    public virtual void TakeDamage(int amount)
    {
        currHealth -= amount;

        if (currHealth <= 0 && !isDead)
        {
            RpcDeath();
        }
    }

    [ClientRpc]
    public virtual void RpcDeath()
    {

    }
}