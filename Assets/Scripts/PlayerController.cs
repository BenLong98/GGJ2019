using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PlayerClass { Heavy, Assault, Ninja }


[RequireComponent(typeof(MovementControl))]
public class PlayerController : HealthController {
    
    GameObject deathPanel;

    [SyncVar] public int money = 0;
    Text moneyTextBox;

    public PlayerClass MyClass;
    /// <summary> Movement speed </summary>
    public float speed { set { this.GetComponent<MovementControl>().speed = value; } }
    public float jumppower { set { this.GetComponent<MovementControl>().jumpPower = value; } }

    [SerializeField] bool isBuilding = false;
    [SerializeField] int toggleCount = 1;


    public virtual void Awake()
    {
    }
    
	void Start()
    {
        deathPanel = GameObject.FindWithTag("deathPanel");
        moneyTextBox =  GameObject.FindWithTag("moneyTextBox").GetComponent<Text>();
        
    }



    public override void OnServerDeath()
    {
        print("server player death");
        RpcGameLost();
    }

    [ClientRpc] 
    void RpcGameLost() {
        print("rpc game lost");
        deathPanel = GameObject.FindWithTag("PlayerUI").transform.Find("DeathPanel").gameObject;
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    [ClientRpc]
    public override void RpcDeath()
    {
        print("rpc death player");
        //foreach (MeshRenderer rend in gameObject.GetComponentsInChildren<MeshRenderer>())
        //{
        //    rend.enabled = false;
        //}

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    

    public void AddMoney(int _amount)
    {
        print("adding money "+_amount);
        SetMoney(money + _amount);
        print(this.transform.name);
    }

    public void SetMoney(int _amount)
    {
        money = _amount;

        if (money < 0)
        {
            money = 0;
        }

        if (moneyTextBox != null)
        {
            moneyTextBox.text = ":$" + money;
        }

    }

    public void TakeMoney(int _amount)
    {
        if (money>=_amount)
        {
            SetMoney(money - _amount);
        }
        else
        {
            print("You don't have money for that!");
        }
    }

}
