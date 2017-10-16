using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingControls : MonoBehaviour
{
    private GameObject[,] grid;
    private GameObject selGridPiece, indicator;
    private CreateGameGrid CGG;
    private Vector3 indPlacement;
    private GameManagerStuff gameManager;
    private string[] TowerType;

    // Use this for initialization
    void Start()
    {
        CGG = gameObject.GetComponent<CreateGameGrid>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        grid = CGG.GetGrid();
        TowerType[0] = "Default";
    }

    // Update is called once per frame
    void Update()
    {
        if (indicator != null)
        {
            //TowerType[(int)(Input.GetAxis("Mouse ScrollWheel") * 10)];
            indicator.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.black, Color.white, Mathf.PingPong(Time.time, 1)); //indicator flashing
            float scaler = (Mathf.PingPong(Time.time, 1) + 9)/10; //scaler value of between 0.9 to 1 and back
            indicator.transform.localScale = new Vector3(scaler, scaler, 1); //pulsing scale
            indicator.transform.position = indPlacement - indicator.GetComponent<Renderer>().bounds.size / 2 + Vector3.up; //dynamic repositioning so that the indicator is always in the middle of the square
        }
        if (Input.GetMouseButtonDown(1)) // right clicked
        {
            RaycastHit hit; //ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                Destroy(indicator);
                if (hit.transform.gameObject == indicator) // on indicator
                {
                    if (indicator.transform.parent.Find("Tower") == null) //is square built on already?
                    {
                        GameObject Tower = gameManager.AssignComponents("Tower", ((GameObject)Resources.Load("Construction/Tower")).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, (Material)Resources.Load("Construction/TowerMat"), true);
                        Tower.AddComponent<TowerBehaviour>();
                        Tower.GetComponent<Rigidbody>().isKinematic = true;
                        Tower.transform.position = indicator.transform.position;
                        Tower.transform.SetParent(indicator.transform.parent);
                        gameManager.TowersBuilt++;
                    }
                    else
                    {
                        gameManager.PushToEventLog("This square has already been constructed upon.");
                    }
                }
                foreach (GameObject gridpiece in grid) //on grid piece
                {
                    if (hit.transform.gameObject == gridpiece)
                    {
                        selGridPiece = hit.transform.gameObject; //which grid piece
                        indicator = gameManager.AssignComponents("Indicator", CGG.DoMesh(), new Material(Shader.Find("Standard")));
                        indicator.transform.Rotate(90, 0, 0);
                        indicator.transform.SetParent(gridpiece.transform);
                        indPlacement = selGridPiece.transform.position;
                        indPlacement += selGridPiece.GetComponent<Renderer>().bounds.size / 2;
                    }
                }
            }
        }
    }
}