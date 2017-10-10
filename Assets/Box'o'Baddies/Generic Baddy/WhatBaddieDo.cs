using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WhatBaddieDo : MonoBehaviour
{

    private int moveSpeed = 10, enemyValue = 1;
    private float maxHP = 5, currentHP, armourVal = 1;
    private string idString;
    public GameObject DamageTextParent;
    private GameObject canvas;
    private GameManagerStuff gameManager;

    // Use this for initialization
    void Start()
    {
        currentHP = maxHP;
        canvas = GameObject.Find("Canvas");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
    }

    // Update is called once per frame
    void Update()
    {
        doMovement();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; //ray stuff for damage text popups
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if ((Physics.Raycast(ray, out hit)))
            {
                if (hit.transform == transform) //is the ray hitting this transform?
                {
                    currentHP = currentHP + gameManager.DamageDealt/armourVal;
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
                GameObject.Find("EnemyController").GetComponent<EnemySpawning>().SpawnBaddie("Child", transform.position + Vector3.right * 3);
                GameObject.Find("EnemyController").GetComponent<EnemySpawning>().SpawnBaddie("Child", transform.position + Vector3.left * 3);
            }
            
            Destroy(gameObject);
            gameManager.CurrencyAmount += enemyValue;
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
            Physics.IgnoreCollision(col.transform.GetComponent<Collider>(), transform.GetComponent<Collider>(), true); // for now ignore all other enemies, later make them change movement out of the way
        }
    }
    private void doMovement()
    {
        if(idString == "Bonus")
        {
            gameObject.transform.parent.transform.Translate(-Vector3.forward * Time.deltaTime * moveSpeed); //enemy movement
            transform.Rotate(Vector3.right * Random.value * 5);
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            transform.Translate(-Vector3.forward * Time.deltaTime * moveSpeed); //enemy movement
        }
    }

    public void EnemyType(string type)
    {
        idString = type;
        switch (type)
        {
            case "Boss":
                moveSpeed = 5;
                maxHP = 10;
                enemyValue = 25;
                armourVal = 3; //min 1
                gameObject.transform.localScale = Vector3.one * 25f;
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                gameObject.name = ("Boss " + gameObject.transform.parent.childCount);
                break;

            case "Assasin":
                moveSpeed = 35;
                maxHP = 5;
                enemyValue = 2;
                armourVal = 1; //min 1
                gameObject.transform.localScale = Vector3.one * 6f;
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                gameObject.name = ("Assasin " + gameObject.transform.parent.childCount);
                break;

            case "Knight":
                moveSpeed = 10;
                maxHP = 8;
                enemyValue = 3;
                armourVal = 2.5f; //min 1
                gameObject.transform.localScale = Vector3.one * 12f;
                gameObject.GetComponent<Renderer>().material.color = Color.gray;
                gameObject.name = ("Knight " + gameObject.transform.parent.childCount);
                break;

            case "Mother":
                moveSpeed = 12;
                maxHP = 5;
                enemyValue = 0;
                armourVal = 1; //min 1
                gameObject.transform.localScale = Vector3.one * 10f;
                gameObject.GetComponent<Renderer>().material.color = new Color32(255, 181, 197, 0); // pink1
                gameObject.name = ("Mother " + gameObject.transform.parent.childCount);
                break;

            case "Bonus":
                moveSpeed = 100;
                maxHP = 2;
                enemyValue = 25;
                armourVal = 1; //min 1
                gameObject.transform.localScale = Vector3.one * 8f;
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                gameObject.name = ("Bonus" + gameObject.transform.parent.childCount);

                GameObject BonusParent = new GameObject();
                BonusParent.gameObject.name = gameObject.name;
                BonusParent.gameObject.transform.SetParent(gameObject.transform.parent);
                gameObject.transform.SetParent(BonusParent.transform);
                break;

            case "Child":
                armourVal = 0.5f; //min 1
                gameObject.transform.localScale = Vector3.one * 4f;
                Color childColVal;
                if (Random.value > .5f)
                {
                    childColVal = new Color32(135, 206, 255, 0); // sky blue
                    enemyValue = 2;
                    moveSpeed = 7;
                    maxHP = 3;
                }
                else
                {
                    childColVal = new Color32(255, 105, 180, 0); // hot pink
                    enemyValue = 1;
                    maxHP = 2;
                    moveSpeed = 10;
                }
                gameObject.GetComponent<Renderer>().material.color = childColVal;
                gameObject.name = ("Child " + gameObject.transform.parent.childCount);
                break;

            default:
                moveSpeed = 15;
                maxHP = 10;
                enemyValue = 1;
                armourVal = 1; //min 1
                gameObject.transform.localScale = Vector3.one * 10f;
                gameObject.name = ("GenericBoringBaddie " + gameObject.transform.parent.childCount);
                break;
        }
    }
}
