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
    public void PushToEventLog(string inString) // put text into the scrolling game log
    {
        Text EventLog = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        EventLog.text = "\n" + inString + EventLog.text;
    }
    public void DisplayValue(string Display, Vector3 DisplayPosition) // popup text
    {
        GameObject DamageTextInstance = (GameObject)Instantiate((GameObject)Resources.Load("Box'o'Baddies/DamageValueParent"), Camera.main.WorldToScreenPoint(DisplayPosition), Quaternion.identity); //Position is wrong
        DamageTextInstance.transform.GetChild(0).GetComponent<Text>().text = Display; //set text to displayup to two deicmal places only
        DamageTextInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); //text objects display via canvas
        AnimatorClipInfo[] clipInfo = DamageTextInstance.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0); // how long is text bounce anim?
        Destroy(DamageTextInstance, clipInfo[0].clip.length);//kill after bounce anim ends
    }
}
