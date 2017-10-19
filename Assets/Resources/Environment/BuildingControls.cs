using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    private GameObject[,] Grid;
    private GameObject SelGridPiece, Indicator;
    private CreateGameGrid CGG;
    private Vector3 IndPlacement;
    private GameManagerStuff GameManager;

    // Use this for initialization
    void Start()
    {
        CGG = gameObject.GetComponent<CreateGameGrid>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        Grid = CGG.GetGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Indicator != null)
        {
            Indicator.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.black, Color.white, Mathf.PingPong(Time.time, 1)); //indicator flashing
            float scaler = (Mathf.PingPong(Time.time, 1) + 9)/10; //scaler value of between 0.9 to 1 and back
            Indicator.transform.localScale = new Vector3(scaler, scaler, 1); //pulsing scale
            Indicator.transform.position = IndPlacement - Indicator.GetComponent<Renderer>().bounds.size / 2 + Vector3.up; //dynamic repositioning so that the indicator is always in the middle of the square
        }
        if (Input.GetMouseButtonDown(1)) //right clicked
        {
            RaycastHit hit; //ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                Destroy(Indicator);
                if (hit.transform.gameObject == Indicator) //on indicator
                {
                    if (Indicator.transform.parent.Find("Tower") == null) //is square built on already?
                    {
                        GameObject Tower = GameManager.AssignComponents("Tower", ((GameObject)Resources.Load("Construction/Tower")).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, (Material)Resources.Load("Construction/TowerMat"), true);
                        GameObject TowerRoof = GameManager.AssignComponents("TowerRoof", ((GameObject)Resources.Load("Construction/TowerRoof")).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, new Material(Shader.Find("Standard")), false);
                        TowerRoof.transform.position = Tower.transform.position + new Vector3(0, (int)(Tower.GetComponent<Collider>().bounds.size.y/4), -Tower.transform.position.z/2);
                        TowerRoof.transform.SetParent(Tower.transform);
                        Tower.AddComponent<TowerBehaviour>();
                        Tower.GetComponent<TowerBehaviour>().SetTowerType("Default");
                        Tower.GetComponent<Rigidbody>().isKinematic = true;
                        Tower.transform.position = Indicator.transform.position;
                        Tower.transform.SetParent(Indicator.transform.parent);
                        GameManager.TowersBuilt++;
                    }
                    else
                    {
                        GameManager.PushToEventLog("This square has already been constructed upon.");
                    }
                }
                foreach (GameObject gridpiece in Grid) //on grid piece
                {
                    if (hit.transform.gameObject == gridpiece)
                    {
                        SelGridPiece = hit.transform.gameObject; //which grid piece
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