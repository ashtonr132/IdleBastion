using System.Collections;
using UnityEngine;

public class EnemyFunction : MonoBehaviour
{
    private int EnemyValue, DamageDealt;
    private float MoveSpeed, MaxHP, ArmourVal, CurrentHP;
    private string IdString;
    private GameManagerStuff GameManager;
    private EnemySpawning ESp;
    private Vector3 ResPos;
    
    void Start()  // Use this for initialization
    {
        CurrentHP = MaxHP;
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        ESp = GameObject.Find("EnemyController").GetComponent<EnemySpawning>();
        if (IdString == "Regenerator")
        {
            StartCoroutine(EnemyActions("Healing"));
        }
    }
    void Update() //Update is called once per frame
    {
        if (IdString == "Bonus") //Movement
        {
            transform.Rotate(Vector3.right * UnityEngine.Random.value * 5);
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 1)); //Color pulsing
            StandardMovement();
        }
        else if (IdString == "Charger")
        {
            var currentMoveSpeed = Mathf.PingPong(Time.time *100, (MoveSpeed / CurrentHP)*10);
            gameObject.GetComponent<Rigidbody>().velocity = -Vector3.forward * currentMoveSpeed; //Enemy movement
        }
        else if(IdString == "Regenerator")
        {
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color32(250, 128, 114, 255), new Color32(0, 128, 0, 255), Mathf.PingPong(Time.time, 1.5f));
            StandardMovement();
        }
        else if (IdString == "Phasing")
        {
            Color col = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, Mathf.PingPong(Time.time/2, 1));
            StandardMovement(); 
        }
        else if (IdString == "Resurrecting")
        {
            Collider col = GetComponent<Collider>();
            col.enabled = false;
            gameObject.transform.position = new Vector3(ResPos.x, Mathf.Lerp((ResPos + (Vector3.down * 30)).y, ResPos.y, Time.time/20), ResPos.z);
            if (gameObject.transform.position.y >= ResPos.y)
             {
                IdString = "Default";
                col.enabled = true;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            }
        }
        else
        {
            StandardMovement();
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; //Ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                if (hit.transform == transform) //Is the ray hitting this transform?
                {
                    CurrentHP += GameManager.DamageDealt / ArmourVal;
                    GameManager.DisplayValue((GameManager.DamageDealt / ArmourVal).ToString(), gameObject.transform.position);
                    GameManager.FragmentEnemy(gameObject, 1, 1);
                }
            }
        }
        if (CurrentHP <= 0)
        {
            if (IdString == "Mother")
            {
                ESp.SpawnBaddie("Child", transform.position + Vector3.right * 3);
                ESp.SpawnBaddie("Child", transform.position + Vector3.left * 3);
                Destroy(gameObject);
            }
            else if (IdString == "Undead")
            {
                ResPos = gameObject.transform.position;
                CurrentHP = MaxHP;
                IdString = "Resurrecting";
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                GetComponent<Renderer>().material.color = new Color32(152, 251, 152, 255);
                GameManager.FragmentEnemy(gameObject, 10, 15);
            }
            else
            {
                GameManager.FragmentEnemy(gameObject, 10, 15);
                Destroy(gameObject);
            }
            GameManager.CurrencyAmount += EnemyValue;
        }
    }
    private void StandardMovement()
    {
        gameObject.GetComponent<Rigidbody>().velocity = -Vector3.forward * MoveSpeed; //Enemy movement
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.name == "KillZone")
        {
            GameManager.FragmentEnemy(gameObject, 10, 15);
            Destroy(gameObject); //obj let through
            if (GameManager.Population > 1)
            {
                GameManager.Population--;
            }
        }
        /*else if (col.gameObject.GetComponent<WhatBaddieDo>().IdString == "Blah")
        {
            //make them change movement out of the way
        }*/
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.transform.name == "Projectile")
        {
            if (IdString == "Shielded") //This unit is immune to tower damage
            {
                DamageDealt = 0;
            }
            else
            {
                DamageDealt = col.transform.parent.GetComponent<TowerBehaviour>().GetDamage(); //Get tower damage
            }
            CurrentHP += DamageDealt / ArmourVal;
            GameManager.DisplayValue(DamageDealt.ToString(), gameObject.transform.position);
            GameManager.FragmentEnemy(gameObject, 1, 1);
            if (col.gameObject != null) //Destroy the bullet on impact, it also has a destroy timer hence the if isnotnull
            {
                Destroy(col.gameObject);
            }
        }
    }
    public void EnemyType(string type) //Typecast statistics
    {
        IdString = type;
        switch (type)
        {
            //Support, all units on screen take less damage, this unit takes 2x damage
            //Cloner, clones itself
            //Teleporter, teleports itself when clicked, back in the y axis and any direction in the xaxis
            case "Phasing":
                ReAssignTypeVal(Color.black, type, maxhp: 4, enemyvalue: 15);
                gameObject.GetComponent<Renderer>().material = (Material)(Resources.Load("Box'o'Baddies/FragmentMat"));
                break;
            /*case "Teleport":
                ReAssignTypeVal(Color.red, type, 10, 10, 250, 3, 25);
                break; */
            case "Boss":
                ReAssignTypeVal(Color.red, type, 10, 10, 250, 3, 25);
                break;
            case "Assasin":
                ReAssignTypeVal(Color.yellow, type, 35, 5, 20, 1, 6);
                break;
            case "Knight":
                ReAssignTypeVal(Color.grey, type, 10, 8, 30, 2.5f, 12);
                break;
            case "Mother":
                ReAssignTypeVal(new Color32(255, 181, 197, 255), type, 12, 5, 10, 1, 10);
                break;
            case "Shielded":
                ReAssignTypeVal(new Color32(0, 0, 128, 255), type, 12, 5, 15, 1, 10);
                break;
            case "Charger":
                ReAssignTypeVal(new Color32(0, 255, 255, 255), type, 40);
                break;
            case "Regenerator":
                ReAssignTypeVal(new Color32(0, 128, 0, 255), type, enemyvalue: 45);
                break;
            case "Undead":
                ReAssignTypeVal(new Color32(65, 90, 190, 255), type, enemyvalue: 10);
                break;
            case "Bonus":
                ReAssignTypeVal(Color.black, type, 100, 2, 200, 1, 8);
                GameObject BonusParent = new GameObject(gameObject.name);
                BonusParent.gameObject.transform.SetParent(gameObject.transform.parent);
                gameObject.transform.SetParent(BonusParent.transform);
                break;
            case "Child":
                if (UnityEngine.Random.value > .5f)
                {
                    ReAssignTypeVal(new Color32(135, 206, 255, 0), type, 7, 15, 2, 0.5f, 4);
                }
                else
                {
                    ReAssignTypeVal(new Color32(255, 105, 180, 0), type, 10, 10, 1, 0.5f, 4);
                }
                break;
            default:
                ReAssignTypeVal(new Color32(65, 90, 190, 255));
                break;
        }
    }
    private void ReAssignTypeVal(Color32 Color, string name = "DefaultBaddie", int movespeed = 15, int maxhp = 10, int enemyvalue = 10, float armourvalue = 1, int scalar = 10)
    {
        MoveSpeed = movespeed;
        MaxHP = maxhp;
        EnemyValue = enemyvalue;
        ArmourVal = armourvalue;
        gameObject.transform.localScale *= scalar;
        gameObject.name = name;
        gameObject.GetComponent<Renderer>().material.color = Color;
        gameObject.transform.position += Vector3.up * gameObject.GetComponent<Renderer>().bounds.size.y / 2;
    }
    private IEnumerator EnemyActions(string Pass, Vector3 Pos = new Vector3())
    {
        if (Pass == "Healing")
        {
            yield return new WaitForSeconds(3); //spawn interval
            CurrentHP++;
            GameManager.DisplayValue("+1", gameObject.transform.position);
            StartCoroutine(EnemyActions("Healing"));
        }
    }
}
