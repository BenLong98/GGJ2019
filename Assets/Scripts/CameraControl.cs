using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : NetworkBehaviour {

    bool cameraLocked = false;
    bool firstPerson = false;

    GameObject firstPersonViewModel;
    Transform firstPersonBarrel;
    GameObject thirdPersonViewModel;
    Transform thirdPersonBarrel;

    public Transform barrel;

    Vector3 thirdPersonOffset = new Vector3(1, 1, -3);

    float y;
    float x;

    float sensitivity = 1f;

    GameObject player;


	void Start()
    {
        if (!transform.parent.GetComponent<PlayerController>().isLocalPlayer)
        {
           transform.Find("Main Camera").gameObject.SetActive(false);
        }
        
        firstPersonViewModel = this.transform.GetChild(0).GetChild(0).gameObject;
        firstPersonBarrel = this.transform.GetChild(0).GetChild(0).GetChild(0);
        thirdPersonViewModel = this.transform.parent.Find("heavyAvatar").gameObject;
        thirdPersonBarrel = this.transform.parent.Find("heavyAvatar").Find("pelvis").Find("waist").GetChild(0).Find("r_shoulder").GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);

        barrel = thirdPersonBarrel;

        // default camera lock to true
        ToggleCameraLock(true);
    }
	
    // Used for input. Done every frame
	void Update()
    {
        // Inputs
        if (Input.GetKeyDown(KeyCode.LeftAlt)) { ToggleCameraLock(); }
        if (Input.GetKeyDown(KeyCode.I)) { sensitivity += 2f; } // sensitivity controls in case we need them in presentation
        if (Input.GetKeyDown(KeyCode.O)) { sensitivity -= 2f; }
        if (Input.GetKeyDown(KeyCode.Tab)) { ToggleFirstPerson(); }
        
        // Camera control
        if (cameraLocked)
        {
            y -= Input.GetAxis("Mouse Y") * sensitivity;
            x += Input.GetAxis("Mouse X") * sensitivity;
        }

        // Cap the vertical
        y = Mathf.Clamp(y, -80, 70);
    }

    // Used for visuals and camera movement. Done at the end of every frame (when we finish calculating stoof)
    void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(y, x, 0f);

        // Camera wall checker
        if (Camera.main && this.transform.parent.GetComponent<Toolbar>().isLocalPlayer)
        {
            Vector3 check = Task.CanSee(this.transform.position, Camera.main.transform.position, 3f);
            if (check != Vector3.zero)
            {
                Camera.main.transform.position = check;
            }
            else
            {
                if (firstPerson)
                {
                    Camera.main.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    Camera.main.transform.localPosition = thirdPersonOffset;
                }
            }
        }
    }



    /// <summary> Overload to actually 'toggle' </summary>
    void ToggleFirstPerson() { if (firstPerson) { ToggleFirstPerson(false); } else { ToggleFirstPerson(true); } }

    /// <summary> Turns the camera's viewpoint from first to third person, or back again </summary>
    void ToggleFirstPerson(bool _bool)
    {
        if (transform.parent.GetComponent<PlayerController>().isLocalPlayer) {
            if (_bool)
            {
                Camera.main.transform.localPosition = new Vector3(0f, 0f, 0f);
                firstPerson = true;
                firstPersonViewModel.SetActive(true);
                thirdPersonViewModel.SetActive(false);
                barrel = firstPersonBarrel;
            }
            else
            {
                Camera.main.transform.localPosition = thirdPersonOffset;
                firstPerson = false;
                firstPersonViewModel.SetActive(false);
                thirdPersonViewModel.SetActive(true);
                barrel = thirdPersonBarrel;
            }
        }
    }

    /// <summary> Overload to actually 'toggle' </summary>
    void ToggleCameraLock() { if (cameraLocked) { ToggleCameraLock(false); } else { ToggleCameraLock(true); } }

    /// <summary> Turns the camera lock on and off </summary>
    void ToggleCameraLock(bool _bool)
    {
        if (_bool)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraLocked = false;
        }
    }
}
