using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    private GameObject[,] Grid;
    private GameObject SelGridPiece, Indicator;
    private CreateGameGrid CGG;
    private Vector3 IndPlacement;
    private GameManagerStuff GameManager;
    
    void Start()// Use this for initialization
    {
        CGG = gameObject.GetComponent<CreateGameGrid>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        Grid = CGG.GetGrid();
    }
    void Update()// Update is called once per frame
    {
        if (Indicator != null)
        {
            Indicator.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.black, Color.white, Mathf.PingPong(Time.time, 1)); //Indicator flashing
            float scaler = (Mathf.PingPong(Time.time, 1) + 9)/10; //Scaler value of between 0.9 to 1 and back
            Indicator.transform.localScale = new Vector3(scaler, scaler, 1); //Pulsing scale
            Indicator.transform.position = IndPlacement - Indicator.GetComponent<Renderer>().bounds.size / 2 + Vector3.up; //Dynamic repositioning so that the indicator is always in the middle of the square
        }
        if (Input.GetMouseButtonDown(1)) //Right clicked
        {
            RaycastHit hit; //Ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                Destroy(Indicator);
                if (hit.transform.gameObject == Indicator) //On indicator
                {
                    if (Indicator.transform.parent.Find("Tower") == null) //Is square built on already?
                    {
                        GameObject Tower = GameManager.AssignComponents("Tower", ((GameObject)Resources.Load("Construction/Tower")).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, (Material)Resources.Load("Construction/TowerMat"), true);
                        GameObject TowerRoof = GameManager.AssignComponents("TowerRoof", ((GameObject)Resources.Load("Construction/TowerRoof")).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, new Material(Shader.Find("Standard")), false);
                        TowerRoof.transform.position = Tower.transform.position + new Vector3(0, (int)(Tower.GetComponent<Collider>().bounds.size.y/4), -Tower.transform.position.z/2);
                        TowerRoof.transform.SetParent(Tower.transform);
                        Tower.AddComponent<TowerBehaviour>();
                        Tower.GetComponent<TowerBehaviour>().SetTowerType(TowerBehaviour.TowerID.Default);
                        Tower.GetComponent<Rigidbody>().isKinematic = true;
                        Tower.transform.position = Indicator.transform.position + Indicator.transform.parent.GetComponent<Renderer>().bounds.extents;
                        Tower.transform.SetParent(Indicator.transform.parent);
                        Tower.transform.localScale /= 1.25f;
                        GameManagerStuff.TowersBuilt++;
                    }
                    else
                    {
                        GameManager.PushToEventLog("Tower Selected.");
                        TowerBehaviour.LastTowerSelected = Indicator.transform.parent.GetChild(0).gameObject;
                    }
                }
                foreach (GameObject gridpiece in Grid) //On grid piece
                {
                    if (hit.transform.gameObject == gridpiece)
                    {
                        SelGridPiece = hit.transform.gameObject; //Which grid piece
                        Indicator = GameManager.AssignComponents("Indicator", CGG.DoMesh(), new Material(Shader.Find("Standard")));
                        Indicator.transform.Rotate(90, 0, 0);
                        Indicator.transform.SetParent(gridpiece.transform);
                        IndPlacement = SelGridPiece.transform.position;
                        IndPlacement += SelGridPiece.GetComponent<Renderer>().bounds.size / 2;
                    }
                }
            }
        }
    }
}