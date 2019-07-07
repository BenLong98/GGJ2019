using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class WallCreator : NetworkBehaviour {

    [SerializeField] GameObject[] buildingObjects = new GameObject[3];
    [SerializeField] GameObject[] buildingPlaceholders= new GameObject[3];
    [SerializeField] Transform spawnPoint;
    [SerializeField] int selectedBuilding = 1;

    Toolbar tb;

    private Grid grid;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
        tb = FindObjectOfType<Toolbar>();
    }

    private void Update() {

        if (!isLocalPlayer) { return; }


        if (tb.ON)
        {
            return;
        }
        else
        {
            BuildModeON();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedBuilding = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBuilding = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBuilding = 3;
        }
    }

    public void BuildModeON() {
        // Place building
        float distance = 5.5f;
        Vector3 direction = (Camera.main.transform.position + Camera.main.transform.forward * distance - this.transform.position).normalized;
        Debug.DrawLine(this.transform.position, Camera.main.transform.position + Camera.main.transform.forward * distance, Color.red);
        Vector3 endPoint;

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, direction, out hit, distance))
        {
            endPoint = hit.point;
        }
        else
        {
            // No hit.point to pull from, calculate where the bullet would have gone to
            Vector3 theoretical = this.transform.position + (direction * distance);
            endPoint = theoretical;
        }
        endPoint = grid.GetNearestPointOnGrid(endPoint);

        if (Input.GetMouseButtonDown(0))
        {
            switch (selectedBuilding)
            {
                case 1:
                    CmdBuildingPiece(selectedBuilding, endPoint + new Vector3(0, 0.31f, 0));
                    break;
                case 2:
                    CmdBuildingPiece(selectedBuilding, endPoint + new Vector3(0, 1.6f, 0));
                    break;
                case 3:
                    CmdBuildingPiece(selectedBuilding, endPoint + new Vector3(0, 1.6f, 0f));
                    break;
                default:
                    break;
            }
        }

        UpdateGhostBuild(selectedBuilding, endPoint);
    }

    public void UpdateGhostBuild(int selectBuilding, Vector3 endpoint) {
        if (isLocalPlayer)
        {
            switch (selectBuilding)
            {
                case 1:
                    buildingPlaceholders[0].SetActive(true);
                    buildingPlaceholders[1].SetActive(false);
                    buildingPlaceholders[2].SetActive(false);

                    buildingPlaceholders[0].transform.position = endpoint + new Vector3(0, 0.31f, 0);
                    if (buildingPlaceholders[0].transform.position.y < 0)
                    {
                        buildingPlaceholders[0].transform.position = new Vector3(buildingPlaceholders[0].transform.position.x, 0, buildingPlaceholders[0].transform.position.z);
                    }
                    SetRotation(buildingPlaceholders[0]);
                    break;
                case 2:
                    buildingPlaceholders[0].SetActive(false);
                    buildingPlaceholders[1].SetActive(true);
                    buildingPlaceholders[2].SetActive(false);


                    buildingPlaceholders[1].transform.position = endpoint + new Vector3(0, 1.6f, 0);
                    if (buildingPlaceholders[1].transform.position.y < 0)
                    {
                        buildingPlaceholders[1].transform.position = new Vector3(buildingPlaceholders[1].transform.position.x, 0, buildingPlaceholders[1].transform.position.z);
                    }
                    SetRotation(buildingPlaceholders[1]);

                    break;
                case 3:
                    buildingPlaceholders[0].SetActive(false);
                    buildingPlaceholders[1].SetActive(false);
                    buildingPlaceholders[2].SetActive(true);


                    buildingPlaceholders[2].transform.position = endpoint + new Vector3(0f, 1.6f, 0f);
                    if (buildingPlaceholders[2].transform.position.y < 0)
                    {
                        buildingPlaceholders[2].transform.position = new Vector3(buildingPlaceholders[2].transform.position.x, 0, buildingPlaceholders[2].transform.position.z);
                    }
                    SetRotation(buildingPlaceholders[2]);

                    break;

                default:
                    break;

            }
        }
    }

    private void SetRotation(GameObject buildingPiece) {
        var vec = this.transform.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        buildingPiece.transform.eulerAngles = vec;
    }

    private GameObject getBuildingType(int buildingID) {
        switch (buildingID)
        {
            case 1:
                return buildingObjects[0];
            case 2:
                return buildingObjects[1];
            case 3:
                return buildingObjects[2];
            default:
                return null;
        }
    }


    [Command]
    public void CmdBuildingPiece(int buildingID, Vector3 placePoint) {

        GameObject go = Instantiate(getBuildingType(buildingID));
        SetRotation(go);

        NetworkServer.Spawn(go);

        go.GetComponent<WallController>().CallPosition(placePoint);
        go.GetComponent<WallController>().CallResizeBuilding();
    }
}
