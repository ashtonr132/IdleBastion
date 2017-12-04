using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunction : MonoBehaviour
{
    internal int EnemyValue, DamageDealt, x = 0, y = 0;
    internal float MoveSpeed, MaxHP, ArmourVal, CurrentHP;
    private GameManagerStuff GameManager;
    private EnemySpawning ESp;
    private GameObject[,] CGG;
    private Vector3 ResPos;
    private List<GameObject> FinalPath = new List<GameObject>(), OpenTiles = new List<GameObject>();
    private GameObject killzonepathing;
    internal enum EnemyID
    {
        Default, Teleport, Phasing,
        Assasin, Knight, Mother,
        Shielded, Charger, Regenerator,
        Undead, Bonus,
        Child, Resurrecting, Boss //resurrecting is a state for undead do not directly call
    }
    internal EnemyID CurrentEnemyID;
    void Start()  // Use this for initialization
    {
        CurrentHP = MaxHP;
        CGG = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        ESp = GameObject.Find("EnemyController").GetComponent<EnemySpawning>();
        if (CurrentEnemyID == EnemyID.Regenerator)
        {
            StartCoroutine(EnemyActions("Healing"));
        }
        killzonepathing = new GameObject("killzone pathing");
        killzonepathing.transform.position = new Vector3(transform.position.x, transform.position.y, GameObject.Find("KillZone").transform.position.z);
        killzonepathing.transform.SetParent(GameObject.Find("FragmentEncapsulation").transform);
    }
    void Update() //Update is called once per frame
    {
        if (FinalPath.Count > 0)
        {
            foreach (var item in FinalPath)
            {
                Debug.DrawLine(item.transform.position, transform.position, Color.red, 99);
            }
        }
        switch (CurrentEnemyID)
        {
            case EnemyID.Bonus:
                transform.Rotate(Vector3.right * UnityEngine.Random.value * 5);
                gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 1)); //Color pulsing
                StandardMovement();
                break;
            case EnemyID.Charger:
                StandardMovement();
                break;
            case EnemyID.Regenerator:
                gameObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color32(250, 128, 114, 255), new Color32(0, 128, 0, 255), Mathf.PingPong(Time.time, 1.5f));
                StandardMovement();
                break;
            case EnemyID.Phasing:
                Color col = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, Mathf.PingPong(Time.time / 2, 1));
                StandardMovement();
                break;
            case EnemyID.Resurrecting:
                Collider GOCollider = GetComponent<Collider>();
                GOCollider.enabled = false;
                gameObject.transform.position = new Vector3(ResPos.x, Mathf.Lerp((ResPos + (Vector3.down * 30)).y, ResPos.y, Time.time / 20), ResPos.z);
                if (gameObject.transform.position.y >= ResPos.y)
                {
                    CurrentEnemyID = EnemyID.Default;
                    GOCollider.enabled = true;
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                }
                break;
            default:
                StandardMovement();
                break;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; //Ray stuff for damage text popups
            if ((Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)))
            {
                if (hit.transform == transform) //Is the ray hitting this transform?
                {
                    CurrentHP += GameManagerStuff.Damage / ArmourVal;
                    GameManager.DisplayValue((GameManagerStuff.Damage / ArmourVal).ToString(), gameObject.transform.position);
                    GameManager.FragmentEnemy(gameObject, 1, 1);
                    if (CurrentEnemyID == EnemyID.Teleport)
                    {
                        gameObject.transform.position = gameObject.transform.position.ParameterChange(X: (UnityEngine.Random.Range(0, 110)), Z: (UnityEngine.Random.Range(gameObject.transform.position.z, 190)));
                    }
                }
            }
        }
        if (CurrentHP <= 0)
        {
            if (CurrentEnemyID == EnemyID.Mother)
            {
                ESp.SpawnBaddie(EnemyID.Child, transform.position + Vector3.right * 3);
                ESp.SpawnBaddie(EnemyID.Child, transform.position + Vector3.left * 3);
                Destroy(killzonepathing);
                Destroy(gameObject);
            }
            else if (CurrentEnemyID == EnemyID.Undead)
            {
                ResPos = gameObject.transform.position;
                CurrentHP = MaxHP;
                CurrentEnemyID = EnemyID.Resurrecting;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                GetComponent<Renderer>().material.color = new Color32(152, 251, 152, 255);
                GameManager.FragmentEnemy(gameObject, 10, 15);
            }
            else
            {
                GameManager.FragmentEnemy(gameObject, 10, 15);
                GameManagerStuff.EnemiesKilled++;
                Destroy(killzonepathing);
                Destroy(gameObject);
            }
            GameManagerStuff.Currency += EnemyValue;
        }
    }
    private void StandardMovement()
    {
        if (FinalPath.Count == 0)
        {
            RaycastHit hit;
            GameObject chosentile;
            Physics.Raycast(transform.position, Vector3.down, out hit);
            chosentile = hit.transform.gameObject;
                do
                {
                    GetGridCoords(chosentile);
                    if (x + 1 >= 0 && x + 1 <= CGG.GetLength(0) - 1)
                    {
                        CheckForOpenTile(CGG[x + 1, y]);
                    }
                    if (x - 1 >= 0 && x - 1 <= CGG.GetLength(0) - 1)
                    {
                        CheckForOpenTile(CGG[x - 1, y]);
                    }
                    if (y + 1 >= 0 && y + 1 <= CGG.GetLength(1) - 1)
                    {
                        CheckForOpenTile(CGG[x, y + 1]);
                    }
                    if (y - 1 >= 0 && y - 1 <= CGG.GetLength(1) - 1)
                    {
                        CheckForOpenTile(CGG[x, y - 1]);
                    }
                    if (OpenTiles.Count > 0)
                    {
                        float distance = 9999;
                        foreach (GameObject item in OpenTiles)
                        {
                            if (item == CGG[x, 0])
                            {
                                FinalPath.Add(CGG[x, 0]);
                                chosentile = killzonepathing;
                            }
                            else
                            {
                                if (Vector3.Distance(item.transform.position, killzonepathing.transform.position) <= distance)
                                {
                                    distance = Vector3.Distance(item.transform.position, killzonepathing.transform.position);
                                    chosentile = item;
                                }
                            }
                        }
                        FinalPath.Add(chosentile);
                    }
                } while (chosentile != killzonepathing);
        }
        else
        {
            if (Vector3.Distance(FinalPath[0].transform.position, new Vector3(transform.position.x, 0, transform.position.z)) <= 8f)
            {
                FinalPath.Remove(FinalPath[0]);
            }
            if (FinalPath.Count > 0)
            {
                if (CurrentEnemyID == EnemyID.Charger)
            {
                try
                {
                    var CurrentMoveSpeed = Mathf.PingPong(Time.time * 100, MoveSpeed / Mathf.Sqrt(CurrentHP) + 3); if (FinalPath[0] != killzonepathing)
                        if (FinalPath[0] != killzonepathing)
                        {
                            GetComponent<Rigidbody>().velocity = ((FinalPath[0].transform.position + FinalPath[0].GetComponent<Renderer>().bounds.extents) - transform.position) * CurrentMoveSpeed;
                        }
                        else
                        {
                            GetComponent<Rigidbody>().velocity = (FinalPath[0].transform.position - transform.position) * CurrentMoveSpeed;
                        }
                }
                catch (Exception)
                {
                    throw;
                }
            }
                if (FinalPath[0] != killzonepathing)
                {
                    GetComponent<Rigidbody>().velocity = ((FinalPath[0].transform.position + FinalPath[0].GetComponent<Renderer>().bounds.extents) - transform.position) * MoveSpeed;
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = (FinalPath[0].transform.position - transform.position) * MoveSpeed;
                }
                foreach (GameObject item in FinalPath)
                {
                    bool flag = false;
                    foreach (Transform item2 in item.transform)
                    {
                        if (item.name == "Tower")
                        {
                            FinalPath.Clear();
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
        }
    }
    void GetGridCoords(GameObject tile)
    {
        bool flag = false;
        for (x = 0; x < (CGG.GetLength(0) - 1); x++)
        {
            for (y = 0; y < (CGG.GetLength(1) - 1); y++)
            {
                if (CGG[x, y] == tile.transform.gameObject)
                {
                    flag = true;
                    break;

                }
            }
            if (flag)
            {
                break;
            }
        }
    }
    void CheckForOpenTile(GameObject tile)
    {
        OpenTiles.Add(tile);
        if (tile.transform.childCount > 0)
        {
            foreach (Transform item in tile.transform)
            {
                if (item.name == "Tower")
                {
                    OpenTiles.Remove(tile);
                }
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (String.Equals(col.transform.name, "KillZone"))
        {
            GameManager.FragmentEnemy(gameObject, 10, 15);
            Destroy(killzonepathing);
            Destroy(gameObject); //obj let through
            if (GameManagerStuff.Population > 1)
            {
                GameManagerStuff.Population--;
            }
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (System.String.Equals(col.transform.name, "Projectile"))
        {
            if (CurrentEnemyID == EnemyID.Teleport)
            {
                gameObject.transform.position = gameObject.transform.position.ParameterChange(X: (UnityEngine.Random.Range(0, 110)), Z: (UnityEngine.Random.Range(gameObject.transform.position.z, 190)));
            }
            if (CurrentEnemyID == EnemyID.Shielded) //This unit is immune to tower damage
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
    internal void EnemyType(EnemyID id) //Typecast statistics
    {
        CurrentEnemyID = id;
        switch (CurrentEnemyID)
        {
            case EnemyID.Teleport:
                ReAssignTypeVal(Color.white, id.ToString(), movespeed:20, enemyvalue:20);
                break;
            case EnemyID.Phasing:
                ReAssignTypeVal(Color.black, id.ToString(), maxhp: 4, enemyvalue: 15);
                gameObject.GetComponent<Renderer>().material = (Material)(Resources.Load("Box'o'Baddies/FragmentMat"));
                break;
            case EnemyID.Boss:
                ReAssignTypeVal(Color.red, id.ToString(), 10, enemyvalue:250, armourvalue:3, scalar:25);
                break;
            case EnemyID.Assasin:
                ReAssignTypeVal(Color.yellow, id.ToString(), 35, 5, 20, scalar:6);
                break;
            case EnemyID.Knight:
                ReAssignTypeVal(Color.grey, id.ToString(), 10, 8, 30, 2.5f, 12);
                break;
            case EnemyID.Mother:
                ReAssignTypeVal(new Color32(255, 181, 197, 255), id.ToString(), 12, 5);
                break;
            case EnemyID.Shielded:
                ReAssignTypeVal(new Color32(0, 0, 128, 255), id.ToString(), 12, 5, 15);
                break;
            case EnemyID.Charger:
                ReAssignTypeVal(new Color32(0, 255, 255, 255), id.ToString(), 50);
                break;
            case EnemyID.Regenerator:
                ReAssignTypeVal(new Color32(0, 128, 0, 255), id.ToString(), enemyvalue: 45);
                break;
            case EnemyID.Undead:
                ReAssignTypeVal(new Color32(65, 90, 190, 255), id.ToString(), enemyvalue: 10);
                break;
            case EnemyID.Bonus:
                ReAssignTypeVal(Color.black, id.ToString(), 100, 2, 200, 1, 8);
                GameObject BonusParent = new GameObject(gameObject.name);
                BonusParent.gameObject.transform.SetParent(gameObject.transform.parent);
                gameObject.transform.SetParent(BonusParent.transform);
                break;
            case EnemyID.Child:
                if (UnityEngine.Random.value > .5f)
                {
                    ReAssignTypeVal(new Color32(135, 206, 255, 0), id.ToString(), 7, 15, 2, 0.5f, 4);
                }
                else
                {
                    ReAssignTypeVal(new Color32(255, 105, 180, 0), id.ToString(), 10, 10, 1, 0.5f, 4);
                }
                break;
            default:
                ReAssignTypeVal(new Color32(65, 90, 190, 255));
                break;
        }
    }
    internal void ReAssignTypeVal(Color32 Color, string name = "DefaultBaddie", int movespeed = 15, int maxhp = 10, int enemyvalue = 10, float armourvalue = 1, int scalar = 10)
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
    internal IEnumerator EnemyActions(string Pass, Vector3 Pos = new Vector3())
    {
        if (System.String.Equals(Pass, "Healing"))
        {
            yield return new WaitForSeconds(3); //spawn interval
            CurrentHP++;
            GameManager.DisplayValue("+1", gameObject.transform.position);
            StartCoroutine(EnemyActions("Healing"));
        }
    }
}