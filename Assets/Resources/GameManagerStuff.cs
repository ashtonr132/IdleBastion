using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerStuff : MonoBehaviour
{
    internal static float TotalLifeTimeClicks, EnemiesKilled, TowersBuilt, Currency = 0, Population = 1, Damage = -1, Cost = 10, Bonus = 0, ArmourPiercingPC = 10, Score = 0;
    private GameObject Canvas, FragmentEncapsulation;
    private Transform PlayerUI;
    private bool isRunning = false;

    private void Start()
    {
        PlayerUI = GameObject.Find("Player").transform;
        Canvas = GameObject.Find("Canvas");
        FragmentEncapsulation = new GameObject("FragmentEncapsulation"); //unity tidyness
        FragmentEncapsulation.transform.position = new Vector3(0, 0, 0); //out of the way
        FragmentEncapsulation.transform.SetParent(GameObject.Find("EnemyController").transform);
    }
    void Update()
    {
        if (Population < 1 && isRunning == false) // if you lost the game and the coroutine isnt already running
        {
            PushToEventLog("You Have Died!");
            StartCoroutine(RestartGame());
        }
        Population += Time.deltaTime/55; //add population and currency over time
        Currency += Time.deltaTime/5;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) //counts clicks
        {
            TotalLifeTimeClicks++;
        }
        if (PlayerUI.gameObject.activeSelf) //update ui when active
        {
            PlayerUI.GetChild(1).GetChild(0).GetComponent<Text>().text = "Population : " + ((int)Population).ToString();
            PlayerUI.GetChild(2).GetChild(0).GetComponent<Text>().text = "Total Clicks : " + TotalLifeTimeClicks.ToString();
            PlayerUI.GetChild(3).GetChild(0).GetComponent<Text>().text = "Currency : " + ((int)Currency).ToString();
            PlayerUI.GetChild(4).GetChild(0).GetComponent<Text>().text = "Towers Built : " + TowersBuilt.ToString();
            PlayerUI.GetChild(5).GetChild(0).GetComponent<Text>().text = "Ememies Killed : " + EnemiesKilled.ToString();
            PlayerUI.GetChild(6).GetChild(0).GetComponent<Text>().text = "Score : " + Score.ToString();
        }
    }
    internal GameObject AssignComponents(string name, Mesh mesh, Material mat, bool needsRB = false) //Setting up new game objects quickly
    {
        GameObject outGO = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        outGO.GetComponent<MeshFilter>().mesh = mesh;
        outGO.GetComponent<MeshRenderer>().material = mat;
        outGO.GetComponent<MeshCollider>().sharedMesh = mesh;
        outGO.GetComponent<MeshCollider>().convex = true;
        if (needsRB)
        {
            outGO.AddComponent<Rigidbody>();
            outGO.GetComponent<Rigidbody>().useGravity = false;
            outGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        return outGO;
    }
    internal void PushToEventLog(string inString) //Put text into the scrolling game log
    {
        if (isRunning == false || inString.Contains("Restarting"))
        {
            Text EventLog = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            if (EventLog.text.Length + inString.Length <= (int)(65535 / 4)) //unity text cimponent has a max verticy limit of 65535, each character take 4 verticies, so if text length >= max length clear text
            {
                EventLog.text = "\n" + inString + EventLog.text;
            }
            else
            {
                EventLog.text = "Event Log Full! \n Event Log Cleared.";
                PushToEventLog(inString);
            }
        }
    }
    internal void FragmentEnemy(GameObject GameObjectPos, int FragMin, int FragMax) //this function was inspired by the example game CubeWorld in Casual game development, however i have written my own version with the functionality i required
    {
        var num = UnityEngine.Random.Range(FragMin, FragMax);
        for (int i = 0; i < num; i++)
        {
            GameObject Fragment = AssignComponents("EnemyFragment", GameObjectPos.GetComponent<MeshFilter>().mesh, (Material)Resources.Load("Box'o'Baddies/FragmentMat"), false);
            //Fragment.GetComponent<Renderer>().material.SetFloat("_Mode", 3); unity5 bug, the material will not update until checked in the inspector if changed this way, i had to use a premade material instead
            var scale = Mathf.Clamp(UnityEngine.Random.value * 2, 0.25f, 2);
            Fragment.transform.position = new Vector3(GameObjectPos.transform.position.x + UnityEngine.Random.Range(-GameObjectPos.GetComponent<Renderer>().bounds.size.x / 2, GameObjectPos.GetComponent<Renderer>().bounds.size.x / 2), GameObjectPos.transform.position.y + GameObjectPos.GetComponent<Renderer>().bounds.size.y, GameObjectPos.transform.position.z + UnityEngine.Random.Range(-GameObjectPos.GetComponent<Renderer>().bounds.size.z / 2, GameObjectPos.GetComponent<Renderer>().bounds.size.z / 2));
            Fragment.transform.localScale = new Vector3(scale, scale, scale);
            Fragment.GetComponent<Renderer>().material.color = new Color(GameObjectPos.GetComponent<Renderer>().material.color.r, GameObjectPos.GetComponent<Renderer>().material.color.g, GameObjectPos.GetComponent<Renderer>().material.color.b, 1);
            Destroy(Fragment.GetComponent<MeshCollider>()); Fragment.AddComponent<BoxCollider>(); Fragment.AddComponent<Rigidbody>();
            Fragment.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-500, 500));
            Fragment.transform.SetParent(FragmentEncapsulation.transform);
            Fragment.GetComponent<BoxCollider>().size /= 2.5f;
            StartCoroutine(FadeOut(Fragment, 0.15f));
        }
    }
    internal void DisplayValue(float Display, Vector3 DisplayPosition) //Popup text
    {
        GameObject DamageTextInstance = (GameObject)Instantiate((GameObject)Resources.Load("Box'o'Baddies/DamageValueParent")); //Position is wrong
        DamageTextInstance.transform.SetParent(Canvas.transform, false); //Text objects display via canvas
        DamageTextInstance.transform.position = DisplayPosition + Vector3.right * 25 + Vector3.one * UnityEngine.Random.Range(-15, +15);
        string Temp = "";
        if (Display > 0)
        {
            Temp = "+";
        }
        DamageTextInstance.transform.GetChild(0).GetComponent<Text>().text = Temp + decimal.Round((decimal)Display, 2, MidpointRounding.AwayFromZero).ToString();
        AnimatorClipInfo[] clipInfo = DamageTextInstance.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0); //How long is text bounce anim?
        Destroy(DamageTextInstance, clipInfo[0].clip.length); //Destroy after bounce anim ends
    }
    internal IEnumerator FadeOut(GameObject FadeMe, float WaitTime) //slowly up the transparency value of an object until it is transparent then destroy it
    {
        Color PreFadeColor = FadeMe.GetComponent<Renderer>().material.color; //this function was inspired by the example game CubeWorld in Casual game development, however i have written my own version with the functionality i required
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
    internal IEnumerator RestartGame()
    {
        isRunning = true; //as to not start multiple coroutines
        for (int i = 8; i > 0; i--) //restarting game warning
        {
            PushToEventLog("Game Restarting In : " + i);
            yield return new WaitForSeconds(1);
        }
        TotalLifeTimeClicks = 0; //resetting static values that need reset
        EnemiesKilled = 0;
        TowersBuilt = 0;
        Currency = 0;
        Population = 1;
        Score = 0;
        TowerBehaviour.LastTowerSelected = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload game scene
        isRunning = false; //should not be necisarry but precautionary/completion reasons
    }
    internal void NotEnoughGold() //commonly used phrasing
    {
        PushToEventLog("Not Enough Gold");
    }
    internal bool CanAfford(float f) //quickmath
    {
        if (f <= Currency)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
public static class HelperClass //inspired by lecturer Konstantinos Dokos, rewritten by me from memory/necisary functionality
{
    internal static Vector3 ParameterChange(this Vector3 Vec, float? X = null, float? Y = null, float? Z = null) //Quick rewrite of readonly variable usings optional parameters as nullables to keep uneeded variables the same
    {
        return new Vector3(X ?? Vec.x, Y ?? Vec.y,  Z ?? Vec.z); //If is not referred called with, is null, if is null usses the pre-existing value
    }
}
