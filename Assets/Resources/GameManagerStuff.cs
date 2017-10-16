using UnityEngine;
using UnityEngine.UI;

public class GameManagerStuff : MonoBehaviour
{
    public int CurrencyAmount = 0, TotalLifeTimeClicks = 0, EnemiesKilled = 0, TowersBuilt = 0, Population = 1, DamageDealt = -1;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TotalLifeTimeClicks++;
        }
    }

    public GameObject AssignComponents(string name, Mesh mesh, Material mat, bool needsRB = false) //setting up new game objects quickly
    {
        GameObject outGO = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        outGO.GetComponent<MeshFilter>().mesh = mesh;
        outGO.GetComponent<MeshRenderer>().material = mat;
        outGO.GetComponent<MeshCollider>().sharedMesh = mesh;
        outGO.GetComponent<MeshCollider>().convex = true;
        if (needsRB == true)
        {
            outGO.AddComponent<Rigidbody>();
            outGO.GetComponent<Rigidbody>().useGravity = false;
            outGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        return outGO;
    }
    public void PushToEventLog(string inString)
    {
        Text EventLog = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        EventLog.text = "\n" + inString + EventLog.text;
    }
}
