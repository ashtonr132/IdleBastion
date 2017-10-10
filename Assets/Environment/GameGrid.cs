using UnityEngine;

public class GameGrid : MonoBehaviour {

    private int Width = 12, Height = 20;
    private GameObject gameGrid;
    private GameObject[,] grid;
    public Material[] Textures;

    void Awake ()
    {
        grid = new GameObject[Width, Height];
        gameGrid = GameObject.Find("GameGrid");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                grid[x, y] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Destroy(grid[x, y].GetComponent<MeshCollider>());
                grid[x, y].transform.rotation = new Quaternion(0, 180, 0, 0); //for somea reason textures display upside down on unity's default Planes?
                var textrRan = Random.value;
                if (textrRan > 0.95f)
                {
                    grid[x, y].GetComponent<Renderer>().material = Textures[1]; //cutelilflow
                }
                else if (textrRan < 0.4f && textrRan > 0.1f)
                {
                    grid[x, y].GetComponent<Renderer>().material = Textures[3]; //wavy grass
                }
                else if (textrRan < 0.05f)
                {
                    grid[x, y].GetComponent<Renderer>().material = Textures[0]; //leafy grass
                }
                else 
                {
                    grid[x, y].GetComponent<Renderer>().material = Textures[2]; //plain grass
                }
                grid[x, y].transform.SetParent(gameGrid.transform);
                grid[x, y].transform.name = ("GridPiece " + x + (" , ") + y); //naming convention
                grid[x, y].transform.position = new Vector3(grid[x, y].transform.position.x + (x * 10), 0, grid[x, y].transform.position.y + (y * 10)); //grid spacing, coords* dimensions of grid pieces
                //add building script for towers
            }
        }
        GameObject killZone = GameObject.CreatePrimitive(PrimitiveType.Cube); //create a cube
        Destroy(killZone.GetComponent<Renderer>()); // makes invisible
        killZone.transform.SetParent(gameGrid.transform); //Parent this as part of the GameGrid
        killZone.transform.name = "KillZone";
        killZone.transform.position = grid[5, 0].transform.position - new Vector3(-5, 0, 5); //where murder zone go?
        killZone.transform.localScale += new Vector3(120, 100, 1); //big ol murder zone
        killZone.AddComponent<Rigidbody>();
        killZone.GetComponent<Rigidbody>().useGravity = false;
        killZone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ 
                                                       | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    public GameObject[,] getGrid() //pass a grid ref
    {
        return grid;
    }
}
