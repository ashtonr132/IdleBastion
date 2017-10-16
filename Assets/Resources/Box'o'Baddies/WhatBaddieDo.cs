using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WhatBaddieDo : MonoBehaviour
{
    private int moveSpeed = 10, enemyValue = 1;
    private float maxHP = 5, armourVal = 1, currentHP;
    private string idString;
    private GameObject DamageTextParent, canvas;
    private GameManagerStuff gameManager;
    private EnemySpawning eSp;

    // Use this for initialization
    void Start()
    {
        currentHP = maxHP;
        canvas = GameObject.Find("Canvas");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        eSp = GameObject.Find("EnemyController").GetComponent<EnemySpawning>();
        DamageTextParent = (GameObject)Resources.Load("Box'o'Baddies/DamageValueParent");
    }

    // Update is called once per frame
    void Update()
    {
        if (idString == "Bonus") // Movement
        {
            gameObject.transform.parent.transform.Translate(-Vector3.forward * Time.deltaTime * moveSpeed); //enemy movement
            transform.Rotate(Vector3.right * Random.value * 5);
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 1)); //color pulsing
        }
        else
        {
            transform.Translate(-Vector3.forward * Time.deltaTime * moveSpeed); //enemy movement
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; //ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                if (hit.transform == transform) //is the ray hitting this transform?
                {
                    currentHP += gameManager.DamageDealt / armourVal;
                    GameObject DamageTextInstance = (GameObject)Instantiate(DamageTextParent, Input.mousePosition - canvas.transform.localPosition + new Vector3(75 + Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.identity);
                    DamageTextInstance.transform.GetChild(0).GetComponent<Text>().text = (Mathf.Round((gameManager.DamageDealt / armourVal) * Mathf.Pow(10.0f, (float)2)) / Mathf.Pow(10.0f, (float)2)).ToString(); //set text to displayup to two deicmal places only
                    DamageTextInstance.transform.SetParent(canvas.transform, false); //text objects display via canvas
                    AnimatorClipInfo[] clipInfo = DamageTextInstance.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0); // how long is bounce anim?
                    Destroy(DamageTextInstance, clipInfo[0].clip.length);//kill after bounce anim ends
                }
            }
        }
        if (currentHP <= 0)
        {
            if (transform.GetComponent<WhatBaddieDo>().idString == "Mother")
            {
                eSp.SpawnBaddie("Child", transform.position + Vector3.right * 3);
                eSp.SpawnBaddie("Child", transform.position + Vector3.left * 3);
            }
            gameManager.CurrencyAmount += enemyValue;
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.name == "KillZone")
        {
            Destroy(gameObject); // obj let through
            if (gameManager.Population > 1)
            {
                gameManager.Population--;
            }
        }
        else if (col.gameObject.GetComponent<WhatBaddieDo>())
        {
            // make them change movement out of the way
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.name == "Projectile")
        {
            currentHP += col.transform.parent.GetComponent<TowerBehaviour>().GetDamage() / armourVal;
            GameObject DamageTextInstance = (GameObject)Instantiate(DamageTextParent, Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(75 + Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))), Quaternion.identity);
            DamageTextInstance.transform.GetChild(0).GetComponent<Text>().text = (Mathf.Round((col.transform.parent.GetComponent<TowerBehaviour>().GetDamage() / armourVal) * Mathf.Pow(10.0f, (float)2)) / Mathf.Pow(10.0f, (float)2)).ToString(); //set text to displayup to two deicmal places only
            DamageTextInstance.transform.SetParent(canvas.transform, false); //text objects display via canvas
            AnimatorClipInfo[] clipInfo = DamageTextInstance.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0); // how long is bounce anim?
            Destroy(DamageTextInstance, clipInfo[0].clip.length);//kill after bounce anim ends
            if (col.gameObject != null)
            {
                Destroy(col.gameObject);
            }
        }
    }

    public void EnemyType(string type) //typecast statistics
    {
        idString = type;
        switch (type)
        {
            // Support, Speed Boost for the next enemy spawned
            // Support, all units on screen take less damage, this unit takes 2x damage
            // Cloner, clones itself
            // Charger, moves in bursts
            // Teleporter, teleports itself when clicked
            // Undead, Respawns once after death
            // Deflector, takes no damage from towers
            // Shielded, takes no damage from clicks
            // Flying
            // Regenerating
            // Almost Invisible
            // Support, Healing 
            case "Boss":
                ReAssignTypeVal(Color.red, 10, 10, 25, 3, 25, type);
                break;

            case "Assasin":
                ReAssignTypeVal(Color.yellow, 35, 5, 2, 1, 6, type);
                break;

            case "Knight":
                ReAssignTypeVal(Color.grey, 10, 8, 3, 2.5f, 12, type);
                break;

            case "Mother":
                ReAssignTypeVal(new Color32(255, 181, 197, 0), 12, 5, 1, 1, 10, type);
                break;

            case "Bonus":
                ReAssignTypeVal(Color.black, 100, 2, 25, 1, 8, type);
                GameObject BonusParent = new GameObject(gameObject.name);
                BonusParent.gameObject.transform.SetParent(gameObject.transform.parent);
                gameObject.transform.SetParent(BonusParent.transform);
                break;

            case "Child":
                if (Random.value > .5f)
                {
                    ReAssignTypeVal(new Color32(135, 206, 255, 0), 7, 3, 2, 0.5f, 4, type);
                }
                else
                {
                    ReAssignTypeVal(new Color32(255, 105, 180, 0), 10, 2, 1, 0.5f, 4, type);
                }
                break;

            default:
                ReAssignTypeVal(new Color32(65, 90, 190, 255));
                break;
        }
    }
    private void ReAssignTypeVal(Color32 Color, int movespeed = 15, int maxhp = 10, int enemyvalue = 1, float armourvalue = 1, int scalar = 10, string name = "DefaultBaddie")
    {
        moveSpeed = movespeed;
        maxHP = maxhp;
        enemyValue = enemyvalue;
        armourVal = armourvalue;
        gameObject.transform.localScale *= scalar;
        gameObject.name = name;
        gameObject.GetComponent<Renderer>().material.color = Color;
        gameObject.transform.position += Vector3.up * gameObject.GetComponent<Renderer>().bounds.size.y / 2;
    }
}
