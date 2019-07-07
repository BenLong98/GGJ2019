using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovementControl : NetworkBehaviour {

    [SyncVar] Vector3 syncPos;

    Toolbar tb;
    PlayerController pc;
    CharacterController cc;
    Animator anim;

    float gravity = 22f;

    public float jumpPower = 10f;
    public float speed = 5f;

    float x = 0f;
    float z = 0f;
    float y = 0f;

    bool requestJump = false;

    Vector3 lastMovement = Vector3.zero;
    bool lastGrounded = false;



	void Awake()
    {
        tb = this.transform.root.GetComponent<Toolbar>();
        pc = this.transform.root.GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
	}
	
    // Used for input
	void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            requestJump = true;
        }
	}

    // Used for physics and movement
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Vector3 movement = lastMovement;

            // Gravity
            if (cc.isGrounded)
            {
                movement.y = -(gravity * Time.deltaTime); //apply a single tick of gravity to make sure player is always grounded
            }
            else
            {
                movement.y = movement.y - (gravity * Time.deltaTime);
            }

            // Normalize inputs
            Vector3 input = new Vector3(x, 0f, z);
            input = input.normalized;

            // Planar movement
            if (cc.isGrounded)
            {
                if (x != 0)
                {
                    movement.x = input.x * speed;
                }
                else
                {
                    movement.x = lastMovement.x * 0.75f;
                }

                if (z != 0)
                {
                    movement.z = input.z * speed;
                }
                else
                {
                    movement.z = lastMovement.z * 0.75f;
                }
            }
            else
            {
                movement.x = ((input.x * speed) * 0.05f + lastMovement.x * 0.95f);
                movement.z = ((input.z * speed) * 0.05f + lastMovement.z * 0.95f);
            }

            // Jumping
            if (cc.isGrounded && requestJump)
            {
                movement.y = jumpPower;
                requestJump = false;
                anim.SetBool("Grounded", false);
            }

            // Continuous jump if they hold space when they land on ground
            if (!lastGrounded && cc.isGrounded && Input.GetButton("Jump"))
            {
                movement.y = jumpPower;
                requestJump = false;
                anim.SetBool("Grounded", false);
            }

            // Save it before you mutilate it
            lastMovement = movement;
            lastGrounded = cc.isGrounded;

            // Rotate the player to face the camera's facing direction
            /*this.transform.rotation = Quaternion.Lerp(
                this.transform.rotation,
                Quaternion.Euler(0f, Camera.main.transform.parent.transform.rotation.eulerAngles.y, 0f),
                8f * Time.deltaTime);*/

            // Rotate the player to face the aim position
            Vector3 aim = tb.GetAim(); // get aim pos
            Vector3 dir = aim - this.transform.position; // get relative vector
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up); // get rotation towards that point

            Vector3 newPlayerRotation = rot.eulerAngles;
            newPlayerRotation.x = 0f;
            newPlayerRotation.z = 0f;

            this.transform.rotation = Quaternion.Lerp(
                this.transform.rotation,
                Quaternion.Euler(newPlayerRotation),
                8f * Time.deltaTime);

            // Rotate the movement Vector to face the camera's facing direction
            movement = Quaternion.Euler(0f, Camera.main.transform.parent.transform.rotation.eulerAngles.y, 0f) * movement;

            // Finalize movement
            cc.Move(movement * Time.deltaTime);

            // ----------
            // ANIMATIONS
            // ----------

            if (x != 0 || z != 0)
            {
                anim.SetBool("Running", true);
            }
            else
            {
                anim.SetBool("Running", false);
            }

            if (!lastGrounded && cc.isGrounded)
            {
                anim.SetBool("Grounded", true);
            }
        }
    }
}
