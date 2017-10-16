using UnityEngine;

public class CreateGameGrid : MonoBehaviour
{
    private int Width = 12, Height = 20;
    private GameObject[,] grid;

    void Awake()
    {
        grid = new GameObject[Width, Height];
        Mesh mesh = DoMesh();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                grid[x, y] = new GameObject("GridPiece " + x + (" , ") + y, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                grid[x, y].GetComponent<MeshFilter>().mesh = mesh;
                grid[x, y].GetComponent<MeshCollider>().convex = true;
                grid[x, y].GetComponent<MeshCollider>().sharedMesh = mesh; //Meshcoll needed for raycast identifying
                grid[x, y].transform.localScale *= 10;
                var textrRan = Random.value;
                if (textrRan > 0.95f)
                {
                    grid[x, y].GetComponent<MeshRenderer>().material = (Material)Resources.Load("Environment/LittleFlowerMat");
                }
                else if (textrRan < 0.4f && textrRan > 0.1f)
                {
                    grid[x, y].GetComponent<MeshRenderer>().material = (Material)Resources.Load("Environment/WavyGrassMat");
                }
                else if (textrRan < 0.05f)
                {
                    grid[x, y].GetComponent<MeshRenderer>().material = (Material)Resources.Load("Environment/LeafyGrassMat");
                }
                else
                {
                    grid[x, y].GetComponent<MeshRenderer>().material = (Material)Resources.Load("Environment/PlainGrassMat");
                }
                grid[x, y].transform.SetParent(transform);
                grid[x, y].transform.position = new Vector3(grid[x, y].transform.position.x + (x * 10), 0, grid[x, y].transform.position.y + (y * 10)); //grid spacing, coords* dimensions of grid pieces
                grid[x, y].transform.Rotate(90, 0, 0);
            }
        }
        GameObject killZone = new GameObject("KillZone", typeof(Rigidbody), typeof(BoxCollider)); //create a cube
        killZone.transform.SetParent(transform); //Parent this as part of the GameGrid
        killZone.transform.position = grid[5, 0].transform.position - new Vector3(-5, 0, 5); //where murder zone go?
        killZone.transform.localScale += new Vector3(120, 100, 1); //big ol murder zone
        killZone.GetComponent<Rigidbody>().useGravity = false;
        killZone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
                                                       | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    public GameObject[,] GetGrid() //pass a grid ref
    {
        return grid;
    }

    public Mesh DoMesh()
    {
        int[] myTriangles = { 0, 3, 1, 0, 2, 3 };
        Vector3[] myVertices = { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) }; //Mesh Verticies
        Vector2[] myUVs = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) }; // Plane UVMaps
        Mesh mesh = new Mesh() { name = "PlanarMesh", vertices = myVertices, triangles = myTriangles, uv = myUVs };
        mesh.RecalculateNormals();
        return mesh;
    }
}
