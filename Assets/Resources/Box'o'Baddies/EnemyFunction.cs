using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunction : MonoBehaviour
{
    internal int EnemyValue; //initialising variables needed
    internal float MoveSpeed, MaxHP, ArmourVal, CurrentHP, DamageDealt;
    private GameManagerStuff GameManager;
    private EnemySpawning ESp;
    private GameObject[,] CGG;
    private Vector3 ResPos;
    private GameObject ChosenTile, maincamera, Ally;
    public List<GameObject> FinalPath = new List<GameObject>(), OpenTiles = new List<GameObject>(), ClosedTiles = new List<GameObject>();
    private GameObject killzonepathing;
    internal AudioClip DeathSound, DamageSound, KillZoneSound, TeleportSound;
    internal enum EnemyID //defineing enemy type
    {
        Default, Teleport, Phasing,
        Assasin, Knight, Mother,
        Shielded, Charger, Regenerator,
        Undead, Bonus, Support,
        Child, Resurrecting, Boss //resurrecting is a state for undead do not directly call
    }
    internal EnemyID CurrentEnemyID; //this enemies type]
    private void Awake()
    {
        maincamera = GameObject.Find("Main Camera");
        TeleportSound = (AudioClip)Resources.Load("Audio/Sound Effects/RandomSfx/pot");
        KillZoneSound = (AudioClip)Resources.Load("Audio/Sound Effects/RandomSfx/lvlup");
        DeathSound = (AudioClip)Resources.Load("Audio/Sound Effects/AbstractPackSFX/Files/AbstractSfx/39");
        DamageSound = (AudioClip)Resources.Load("Audio/Sound Effects/AbstractPackSFX/Files/AbstractSfx/73");
        CGG = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid(); //finding components needed
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        ESp = GameObject.Find("EnemyController").GetComponent<EnemySpawning>();
        killzonepathing = new GameObject("killzone pathing");  //create a game ovbject past the last tile of the grid to aim for to ensure death by killzone collision
        Ally = gameObject;
    }
    void Start()  // Use this for initialization
    {
        CurrentHP = MaxHP;
        if (CurrentEnemyID == EnemyID.Regenerator || CurrentEnemyID == EnemyID.Support)
        {
            StartCoroutine(EnemySpecialActions("Healing"));
        }
        killzonepathing.transform.position = new Vector3(transform.position.x, transform.position.y, GameObject.Find("KillZone").transform.position.z - 10);
        killzonepathing.transform.SetParent(GameObject.Find("FragmentEncapsulation").transform); //clean containment
    }
    void Update() //Update is called once per frame
    {
        if (FinalPath.Count > 0)
        {
            foreach (var item in FinalPath)
            {
                Debug.DrawLine(item.transform.position + GetExtents(item), transform.position, Color.red, 2);
            }
        }
        switch (CurrentEnemyID) //unique functions per enemy type
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
                GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, Mathf.PingPong(Time.time / 4, 1));
                StandardMovement();
                break;
            case EnemyID.Support:
                if (GameObject.Find("EnemyController").transform.childCount > 2)
                {
                    float tempDist = 999999;
                    foreach (Transform enemy in GameObject.Find("EnemyController").transform)
                    {
                        if (enemy.name != "FragmentEncapsulation" && enemy.name != "Support" && Vector3.Distance(gameObject.transform.position, enemy.position) <= tempDist && enemy.GetComponent<EnemyFunction>().CurrentHP +2 <= enemy.GetComponent<EnemyFunction>().MaxHP)
                        {
                            tempDist = Vector3.Distance(gameObject.transform.position, enemy.position);
                            Ally = enemy.gameObject;
                        }
                    }
                }
                else
                {
                    Ally = gameObject;
                }
                if (Ally = gameObject)
                {
                    StandardMovement();
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = ((Ally.transform.position - Vector3.back) - transform.position).normalized * MoveSpeed;
                }
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
                    if (UnityEngine.Random.Range(0, 20) <= 1)
                    {
                        AudioSource.PlayClipAtPoint(DamageSound, maincamera.transform.position, 0.03f);
                    }
                    DamageDealt = GameManagerStuff.Damage;
                    CurrentHP += (DamageDealt / (ArmourVal / GameManagerStuff.ArmourPiercingPC)) / 10;
                    GameManager.DisplayValue(((DamageDealt / (ArmourVal / GameManagerStuff.ArmourPiercingPC)) / 10), gameObject.transform.position);
                    GameManager.FragmentEnemy(gameObject, 1, 1);
                    GameManagerStuff.Currency += GameManagerStuff.Bonus;
                    if (CurrentEnemyID == EnemyID.Teleport)
                    {
                        Teleport(gameObject);
                    }
                }
            }
        }
        if (CurrentHP <= 0)
        {
            AudioSource.PlayClipAtPoint(DeathSound, maincamera.transform.position, 0.03f);
            if (CurrentEnemyID == EnemyID.Mother)
            {
                ESp.SpawnBaddie(EnemyID.Child, transform.position + Vector3.right * 5);
                ESp.SpawnBaddie(EnemyID.Child, transform.position + Vector3.left * 5);
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
            if (FinalPath.Count <= 0) //if path is not written
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))//raycast starting grid tile
                {
                    ChosenTile = hit.transform.gameObject; //this tile is the tile we are working from
                }
                else
                {
                    ChosenTile = null;
                    Destroy(gameObject); //this unit has gotten knocked off the grid and does not have a path generated
                }
                if (ChosenTile != null)
                {
                    var i = 0;
                    do //loop until the last tile is the killzonepathing, ie, the end of the path
                    {
                        i++;
                        if (GetGridCoords(ChosenTile).x + 1 >= 0 && GetGridCoords(ChosenTile).x + 1 <= CGG.GetLength(0) - 1) //is the tile to the right out of bounds?
                        {
                            CheckForOpenTile(CGG[(int)GetGridCoords(ChosenTile).x + 1, (int)GetGridCoords(ChosenTile).y]); //is this tile built upon?
                        }
                        if (GetGridCoords(ChosenTile).x - 1 >= 0 && GetGridCoords(ChosenTile).x - 1 <= CGG.GetLength(0) - 1)//is the tile to the left out of bounds?
                        {
                            CheckForOpenTile(CGG[(int)GetGridCoords(ChosenTile).x - 1, (int)GetGridCoords(ChosenTile).y]);
                        }
                        if (GetGridCoords(ChosenTile).y + 1 >= 0 && GetGridCoords(ChosenTile).y + 1 <= CGG.GetLength(1) - 1)//is the tile above out of bounds?
                        {
                            CheckForOpenTile(CGG[(int)GetGridCoords(ChosenTile).x, (int)GetGridCoords(ChosenTile).y + 1]);
                        }
                        if (GetGridCoords(ChosenTile).y - 1 >= 0 && GetGridCoords(ChosenTile).y - 1 <= CGG.GetLength(1) - 1)//is the tile below out of bounds? (out of the grid array size, ie there is no tile below the last row of tiles)
                        {
                            CheckForOpenTile(CGG[(int)GetGridCoords(ChosenTile).x, (int)GetGridCoords(ChosenTile).y - 1]);
                        }
                        AddTile(); // get the best tile from opentiles and add it to the final path
                        OpenTiles.Clear();
                        if (i >= (CGG.GetLength(0) - 1) * (CGG.GetLength(1) - 1)) //if the loop gets out of control or the enemy gets stuck between closed tiles, the closed tiles are reset and the loop is broken and restarted
                        {
                            ClosedTiles.Clear();
                            ClosedTiles.Add(hit.transform.gameObject); // this stops 2 long final path loops, still creates longer pathing problems though

                            break;
                        }
                    } while (ChosenTile != killzonepathing); // while the path is not finished repeat
                }
            }
            else //if path is already written
            {
                Vector3 moveto = new Vector3(FinalPath[0].transform.position.x, transform.position.y, FinalPath[0].transform.position.z); //height fix for velocity of enemies. having the to position at the same height reduces stuttering in the movement
                if (Vector3.Distance(FinalPath[0].transform.position + GetExtents(FinalPath[0]), new Vector3(transform.position.x, 0, transform.position.z)) <= 0.5f && FinalPath[0].name != "killzonepathing") //if the enemy has reached the next tile remove it
                {
                    if (!ClosedTiles.Contains(FinalPath[0]))
                    {
                        ClosedTiles.Add(FinalPath[0]);
                    }
                    FinalPath.Remove(FinalPath[0]); //note that this will never remove killzone pathing because the enemy game object this script is attached to, will be destroyed before it ever hits this mark
                }
                if (CurrentEnemyID == EnemyID.Charger) //chargers have different movement
                {
                    var CurrentMoveSpeed = Mathf.PingPong(Time.time, MoveSpeed/4); // pingpong speed of charger
                    if (FinalPath.Count > 0 && CurrentMoveSpeed >= 1) //there is one instance where at this point the path can contain nothing, if thisis the case there is nothing to move to, so stand still and then get a new path because path.coun == 0
                    {
                        print(CurrentMoveSpeed);
                        GetComponent<Rigidbody>().velocity = ((moveto + GetExtents(FinalPath[0])) - transform.position).normalized * CurrentMoveSpeed; //aim for the middle of each tile
                    }
                }
                if (FinalPath.Count > 0)//there is one instance where at this point the path can contain nothing, if thisis the case there is nothing to move to, so stand still and then get a new path because path.coun == 0
                {
                    GetComponent<Rigidbody>().velocity = ((moveto + GetExtents(FinalPath[0])) - transform.position).normalized * MoveSpeed;
                }
                foreach (GameObject item in FinalPath) //since you build the path has a tile selected been built upon?
                {
                    bool flag = false;
                    foreach (Transform item2 in item.transform)
                    {
                        if (item2.name == "Tower")
                        {
                            FinalPath.Clear(); //yes so rewrite path
                            flag = true;
                            break; //break inner loop
                        }
                    }
                    if (flag)
                    {
                        break; //break outer loop
                    }
                }
            }
    }
    Vector2 GetGridCoords(GameObject Tile) //get the grid reference coordinates of the Tile called
    {
        bool flag = false; //for cutting the second loop
        int x = 0, y = 0;
        for (x = 0; x <= (CGG.GetLength(0) - 1); x++) //for each x on grid
        {
            for (y = 0; y <= (CGG.GetLength(1) - 1); y++) //for each y on x
            {
                if (CGG[x, y] == Tile.transform.gameObject) //is this x,y this Tile?
                {
                    flag = true; // also break xloop
                    break; //yes x and y are now set correctly to correspond to the Tiles position in the grid array, beak y loop
                }
            }
            if (flag) //when break yloop also break xloop BEFORE any additional counting
            {
                break;
            }
        }
        return new Vector2(x, y);
    }
    void CheckForOpenTile(GameObject Tile) // check if the Tile called is a viable Tile to move to
    {
        OpenTiles.Add(Tile); //add Tile to list
        if (ClosedTiles.Count > 0)
        {
            foreach (GameObject item in ClosedTiles)
            {
                if (item == Tile)
                {
                    if (OpenTiles.Contains(Tile))
                    {
                        OpenTiles.Remove(Tile);
                    }
                }
            }
        }
        if (Tile.transform.childCount > 0) //does Tile have children?
        {
            foreach (Transform item in Tile.transform)
            {
                if (item.name == "Tower") //foreach child is child tower? ie Tile is built upon
                {
                    if (OpenTiles.Contains(Tile))
                    {
                        OpenTiles.Remove(Tile);//Tile is invalid to move to
                    }
                    if (!ClosedTiles.Contains(Tile))
                    {
                        ClosedTiles.Add(Tile);
                    }
                }
            }
        }
    }
    void AddTile() //add Tile from open Tile list
    {
        if (OpenTiles.Count > 0) //are there any Tiles in openTiles list?
        {
            float distance = 99999999; // starting distence vlaue must be larger than potential distence between Tiles
            foreach (GameObject item in OpenTiles) //for each possible Tile
            {
                    if (GetGridCoords(item).y == 0) //is this Tile at the end of the grid?
                    {
                        FinalPath.Add(CGG[(int)GetGridCoords(item).x, 0]); // yes it is, end the path witht he killzonepathing gameobject
                        ChosenTile = killzonepathing; //this breaks the path building loop
                        break;
                    }
                    else //the Tile isnt the end Tile
                    {
                        if (Vector3.Distance(item.transform.position, killzonepathing.transform.position) <= distance && !ClosedTiles.Contains(item)) //is the distance between this Tile  and the destination Tile smaller than all previous Tiles
                        {
                            distance = Vector3.Distance(item.transform.position, killzonepathing.transform.position); //overwrite smallest distance
                            ChosenTile = item; // select Tile as best choice
                        }
                    }
            }
            if (!FinalPath.Contains(ChosenTile))
            {
                FinalPath.Add(ChosenTile); //add Tile to path
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (String.Equals(col.transform.name, "KillZone")) // if the enemy makes it to the end of the grid and collides with the killzone
        {
            AudioSource.PlayClipAtPoint(KillZoneSound, maincamera.transform.position, 0.03f);
            GameManager.FragmentEnemy(gameObject, 10, 15); //spawn fragments
            Destroy(killzonepathing);
            Destroy(gameObject); //obj let through destory enemy
            if (GameManagerStuff.Population > 0) //lost a population
            {
                GameManagerStuff.Population--;
            }
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (System.String.Equals(col.transform.name, "Projectile"))
        {
            if (CurrentEnemyID == EnemyID.Teleport) //teleports on damage taken
            {
                Teleport(gameObject);
            }
            if (CurrentEnemyID == EnemyID.Shielded) //This unit is immune to tower damage
            {
                DamageDealt = 0;
            }
            else
            {
                DamageDealt = col.transform.parent.GetComponent<TowerBehaviour>().GetDamage(); //get tower damage
            }
            if (UnityEngine.Random.Range(0, 20) <= 1)
            {
                AudioSource.PlayClipAtPoint(DamageSound, maincamera.transform.position, 0.03f);
            }
            CurrentHP += (DamageDealt / (ArmourVal / col.transform.parent.GetComponent<TowerBehaviour>().armourpiercingpc))/10;
            if (CurrentHP <= 0)
            {
                GameManagerStuff.Currency += col.transform.parent.GetComponent<TowerBehaviour>().BonusGold;
            }
            GameManager.DisplayValue(((DamageDealt / (ArmourVal / col.transform.parent.GetComponent<TowerBehaviour>().armourpiercingpc)) / 10), gameObject.transform.position);
            GameManager.FragmentEnemy(gameObject, 1, 1);
            if (col.gameObject != null) //Destroy the bullet on impact, it also has a destroy timer hence the if isnotnull
            {
                Destroy(col.gameObject);
            }
        }
    }
    internal void EnemyType(EnemyID id) //sets id passed statistics
    {
        CurrentEnemyID = id;
        switch (CurrentEnemyID)
        {
            case EnemyID.Teleport:
                ReAssignTypeVal(Color.white, id.ToString(), 20, 5, enemyvalue:20);
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
            case EnemyID.Support:
                ReAssignTypeVal(Color.clear, id.ToString(), 12, 8, 10, 0);
                break;
            case EnemyID.Mother:
                ReAssignTypeVal(new Color32(255, 181, 197, 255), id.ToString(), 12, 15);
                break;
            case EnemyID.Shielded:
                ReAssignTypeVal(new Color32(0, 0, 128, 255), id.ToString(), 12, 5, 9);
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
                BonusParent.gameObject.transform.SetParent(transform.parent);
                transform.SetParent(BonusParent.transform);
                break;
            case EnemyID.Child:
                if (UnityEngine.Random.value > .5f)
                {
                    ReAssignTypeVal(new Color32(135, 206, 255, 0), id.ToString(), 7, 5, 2, 0.5f, 4);
                }
                else
                {
                    ReAssignTypeVal(new Color32(255, 105, 180, 0), id.ToString(), 10, 4, 1, 0.5f, 4);
                }
                break;
            default:
                ReAssignTypeVal(new Color32(65, 90, 190, 255));
                break;
        }
    }
    internal void ReAssignTypeVal(Color32 Color, string name = "Base Baddie", int movespeed = 15, int maxhp = 10, int enemyvalue = 10, float armourvalue = 1, int scalar = 10) //enemy type attributes setting
    {
        MoveSpeed = movespeed;
        MaxHP = maxhp + (GameObject.Find("EnemyController").GetComponent<EnemySpawning>().OnStage * 1.0025f); //difficulty scaling with level
        EnemyValue = enemyvalue;
        ArmourVal = armourvalue + (GameObject.Find("EnemyController").GetComponent<EnemySpawning>().OnStage * 1.0001f);
        transform.localScale *= scalar;
        transform.name = name;
        GetComponent<Renderer>().material.color = Color;
        transform.position += Vector3.up * gameObject.GetComponent<Renderer>().bounds.size.y / 2;
    }
    internal IEnumerator EnemySpecialActions(string Pass)
    {
        if (System.String.Equals(Pass, "Healing")) //heal 1 damage to healer enemy type every 3 seconds
        {
            if (CurrentEnemyID == EnemyID.Regenerator)
            {
                yield return new WaitForSeconds(3); //spawn interval
                CurrentHP++;
                GameManager.DisplayValue(1, transform.position);
                StartCoroutine(EnemySpecialActions("Healing"));
            }
            else if (CurrentEnemyID == EnemyID.Support)
            {
                yield return new WaitForSeconds(3);
                if (Ally.GetComponent<EnemyFunction>().CurrentHP + 2 <= Ally.GetComponent<EnemyFunction>().MaxHP)
                {
                    Ally.GetComponent<EnemyFunction>().CurrentHP += 2;
                }
                GameManager.DisplayValue(1, Ally.transform.position);
                StartCoroutine(EnemySpecialActions("Healing"));
            }
        }
    }
    internal Vector3 GetExtents(GameObject gameObj) //one of the 20 game objects i have to use this to get extents has no renderer attached, this is a lazy way to ignore that
    {
        try 
        {
            return gameObj.GetComponent<Renderer>().bounds.extents;
        }
        catch (Exception)
        {
            return Vector3.zero;
        }
    }
    internal void Teleport(GameObject gt) //teleportin function, moves enemy and resets pathing list
    {
        FinalPath.Clear();
        ClosedTiles.Clear();
        Vector3 temp = gt.transform.position.ParameterChange(X: (UnityEngine.Random.Range(0, 110)), Z: (UnityEngine.Random.Range(gt.transform.position.z, 190)));
        gt.transform.position = temp;
        killzonepathing.transform.position = new Vector3(temp.x, temp.y, killzonepathing.transform.position.z);
        AudioSource.PlayClipAtPoint(TeleportSound, maincamera.transform.position, 0.03f);
    }
}