using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    internal GameObject[,] Grid;
    private int OnStage, LevelDifficulty;
    private GameManagerStuff GameManager;
    List<EnemyFunction.EnemyID[]> Stages = new List<EnemyFunction.EnemyID[]>();
    private AudioClip spawnsound;
    void Start() //Use this for initialization
    {
        spawnsound = (AudioClip)Resources.Load("Audio/Sound Effects/AbstractPackSFX/Files/AbstractSfx/17");
        Grid = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        StartCoroutine(Spawning("NextStage", 3)); //Start stageloop ienum
    }
    internal IEnumerator Spawning(string Pass, float Interval)
    {
        if (System.String.Equals(Pass, "SpawnStage")) //Spawn enemies in onstage stage
        {
            for (int i = 0; i <= Stages[OnStage].Length - 1; i++)
            {
                yield return new WaitForSeconds(Interval);
                SpawnBaddie(Stages[OnStage][i]);
                if (i == (Stages[OnStage].Length -1))
                {
                    StartCoroutine(Spawning("NextStage", 3));
                }
            }
            OnStage++;
        }
        else if(System.String.Equals(Pass, "NextStage")) //Prepare for next stage
        {
            for (int i = 0; i <= Interval; i++)
            {
                yield return new WaitUntil(() => GameObject.Find("EnemyController").transform.childCount == 1); //wait for current stage enemies to be destroyed
                if ((Interval - i) == 0)
                {
                    Stages.Add(GenerateStage());
                    GameManager.PushToEventLog("Stage " + OnStage.ToString());
                }
                else if (OnStage != 0 && i == 0)
                {
                    GameManager.PushToEventLog("Stage Completed \nGold + " + OnStage * 10);
                    GameManagerStuff.Currency += (OnStage * 3);
                    GameManager.PushToEventLog("Next Stage in " + (Interval - i).ToString());
                    GameManagerStuff.Score += LevelDifficulty;
                }
                else
                {
                    GameManager.PushToEventLog("Next Stage in " + (Interval - i).ToString());
                    yield return new WaitForSeconds(1);
                }
            }
            StartCoroutine(Spawning("SpawnStage", (3 * Mathf.Pow(0.8f, OnStage) + 0.25f))); //time between each unit spawns decreases as the stage increases, never dips below 0.25f
        }
    }
    internal void SpawnBaddie(EnemyFunction.EnemyID id, Vector3? spawnPos = null) //Quick setup for objects 
    {
        GameObject GridPieceTemp = Grid[UnityEngine.Random.Range(0, 21), 22]; //one less than the grid size
        spawnPos = spawnPos ?? new Vector3(GridPieceTemp.transform.position.x + (GridPieceTemp.GetComponent<Renderer>().bounds.size.x / 2), GridPieceTemp.transform.position.y, GridPieceTemp.transform.position.z + (GridPieceTemp.GetComponent<Renderer>().bounds.size.y / 2));
        GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Cube); //grab a cube mesh
        GameObject Enemy = GameManager.AssignComponents(id.ToString(), tempMesh.GetComponent<MeshFilter>().mesh, new Material(Shader.Find("Unlit/Color")), true);
        Destroy(tempMesh);
        Enemy.transform.position = (Vector3)spawnPos + Vector3.up;
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<EnemyFunction>();
        Enemy.GetComponent<EnemyFunction>().EnemyType(id);
        AudioSource.PlayClipAtPoint(spawnsound, GameObject.Find("Main Camera").transform.position, 0.02f);
        GameManager.PushToEventLog("Spawning " + Enemy.name);
    }
    private EnemyFunction.EnemyID[] GenerateStage()
    {
        LevelDifficulty = (int)((5 * Mathf.Sin((2 * Mathf.PI * OnStage) / -3)) + (5 * OnStage) + 3); //kwikmaffs
        int totalLevelSet = 0;
        List<EnemyFunction.EnemyID> temp = new List<EnemyFunction.EnemyID>();
        while (totalLevelSet < LevelDifficulty) //build level selecing enemies on the fly to fufill the levels difficulty cap
        {
            int tempint = UnityEngine.Random.Range(1, 11);
            switch (tempint)
            {
                default:
                    temp.Add(EnemyFunction.EnemyID.Default);
                    break;
                case 2:
                    temp.Add(EnemyFunction.EnemyID.Charger);
                    break;
                case 3:
                    temp.Add(EnemyFunction.EnemyID.Teleport);
                    break;
                case 4:
                    temp.Add(EnemyFunction.EnemyID.Regenerator);
                    break;
                case 5:
                    temp.Add(EnemyFunction.EnemyID.Shielded);
                    break;
                case 6:
                    temp.Add(EnemyFunction.EnemyID.Phasing);
                    break;
                case 7:
                    temp.Add(EnemyFunction.EnemyID.Mother);
                    break;
                case 8: 
                    temp.Add(EnemyFunction.EnemyID.Knight);
                    break;
                case 9:
                    temp.Add(EnemyFunction.EnemyID.Undead);
                    break;
                case 10:
                    temp.Add(EnemyFunction.EnemyID.Assasin);
                    break;
            }
            totalLevelSet += tempint;
        }
        EnemyFunction.EnemyID[] temparray = new EnemyFunction.EnemyID[temp.Count];
        for (int i = 0; i < temp.Count; i++)
        {
            temparray[i] = temp[i];
        }
        return temparray;
        //code for spawning specific stages, tutorial stage? or just bugtesting
        //EnemyFunction.EnemyID[] Stage0 = new EnemyFunction.EnemyID[] {EnemyFunction.EnemyID.Assasin,EnemyFunction.EnemyID.Bonus, EnemyFunction.EnemyID.Boss, EnemyFunction.EnemyID.Charger, EnemyFunction.EnemyID.Default, EnemyFunction.EnemyID.Knight, EnemyFunction.EnemyID.Mother, EnemyFunction.EnemyID.Phasing, EnemyFunction.EnemyID.Regenerator, EnemyFunction.EnemyID.Shielded, EnemyFunction.EnemyID.Teleport, EnemyFunction.EnemyID.Undead}; Stages.Add(Stage0); //Planned Stages
    }
}
