using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerStuff : MonoBehaviour
{
    public int CurrencyAmount = 0, TotalLifeTimeClicks = 0, EnemiesKilled = 0, TowersBuilt = 0, Population = 1, DamageDealt = -1;
    private GameObject Canvas, FragmentEncapsulation;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
        FragmentEncapsulation = new GameObject("FragmentEncapsulation");
        FragmentEncapsulation.transform.SetParent(GameObject.Find("EnemyController").transform);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TotalLifeTimeClicks++;
        }
    }

    public GameObject AssignComponents(string name, Mesh mesh, Material mat, bool needsRB = false) //Setting up new game objects quickly
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
    public void PushToEventLog(string inString) //Put text into the scrolling game log
    {
        Text EventLog = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        EventLog.text = "\n" + inString + EventLog.text;
    }
    public void FragmentEnemy(GameObject GameObjectPos, int FragMin, int FragMax)
    {
        var num = Random.Range(FragMin, FragMax);
        for (int i = 0; i < num; i++)
        {
            GameObject Fragment = AssignComponents("EnemyFragment", GameObjectPos.GetComponent<MeshFilter>().mesh, (Material)Resources.Load("Box'o'Baddies/FragmentMat"), false);
            //Fragment.GetComponent<Renderer>().material.SetFloat("_Mode", 3); unity5 bug, the material will not update until checked in the inspector if changed this way, i had to use a premade material instead
            var scale = Mathf.Clamp(Random.value * 2, 0.25f, 2);
            Fragment.transform.position = new Vector3(GameObjectPos.transform.position.x + Random.Range(-GameObjectPos.GetComponent<Renderer>().bounds.size.x / 2, GameObjectPos.GetComponent<Renderer>().bounds.size.x / 2), GameObjectPos.transform.position.y + GameObjectPos.GetComponent<Renderer>().bounds.size.y, GameObjectPos.transform.position.z + Random.Range(-GameObjectPos.GetComponent<Renderer>().bounds.size.z / 2, GameObjectPos.GetComponent<Renderer>().bounds.size.z / 2));
            Fragment.transform.localScale = new Vector3(scale, scale, scale);
            Fragment.GetComponent<Renderer>().material.color = new Color(GameObjectPos.GetComponent<Renderer>().material.color.r, GameObjectPos.GetComponent<Renderer>().material.color.g, GameObjectPos.GetComponent<Renderer>().material.color.b, 1);
            Destroy(Fragment.GetComponent<MeshCollider>()); Fragment.AddComponent<BoxCollider>(); Fragment.AddComponent<Rigidbody>();
            Fragment.GetComponent<Rigidbody>().AddForce(Random.Range(-500f, 500f), Random.Range(-1000, 1000), Random.Range(-500, 500));
            Fragment.transform.SetParent(FragmentEncapsulation.transform);
            StartCoroutine(FadeOut(Fragment, 0.15f));
        }
    }
    public void DisplayValue(string Display, Vector3 DisplayPosition) //Popup text
    {
        DisplayPosition = Camera.main.WorldToScreenPoint(DisplayPosition) + (Vector3.right * 70) + (new Vector3(Random.value, Random.value, Random.value)) * 15; //World coords to pixel coords for the ui canvas plus an angle skew adjustment and a random 15 wide random change
        Vector2 canvasCentre = new Vector2(Canvas.GetComponent<RectTransform>().sizeDelta.x / 2, Canvas.GetComponent<RectTransform>().sizeDelta.y / 2); //Canvas centre for reference
        GameObject DamageTextInstance = (GameObject)Instantiate((GameObject)Resources.Load("Box'o'Baddies/DamageValueParent"), -(canvasCentre - new Vector2(DisplayPosition.x, DisplayPosition.y)), Quaternion.identity); //Position is wrong
        DamageTextInstance.transform.GetChild(0).GetComponent<Text>().text = Display;
        DamageTextInstance.transform.SetParent(Canvas.transform, false); //Text objects display via canvas
        AnimatorClipInfo[] clipInfo = DamageTextInstance.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0); //How long is text bounce anim?
        Destroy(DamageTextInstance, clipInfo[0].clip.length); //Destroy after bounce anim ends
    }
    public IEnumerator FadeOut(GameObject FadeMe, float WaitTime)
    {
        Color PreFadeColor = FadeMe.GetComponent<Renderer>().material.color;
        yield return new WaitForSeconds(WaitTime);
        if (PreFadeColor.a >= 0)
        {
            FadeMe.GetComponent<Renderer>().material.color = new Color(PreFadeColor.r, PreFadeColor.g, PreFadeColor.b, PreFadeColor.a - 0.01f);
            StartCoroutine(FadeOut(FadeMe, WaitTime));
        }
        else
        {
            Destroy(FadeMe);
        }
    }
}
