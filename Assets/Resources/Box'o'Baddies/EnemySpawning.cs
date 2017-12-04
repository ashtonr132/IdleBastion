using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    internal GameObject[,] Grid;
    private int OnStage, LevelDifficulty;
    private GameManagerStuff GameManager;
    List<EnemyFunction.EnemyID[]> Stages = new List<EnemyFunction.EnemyID[]>();

    void Start() //Use this for initialization
    {
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
            Stages.Add(GenerateStage());
            for(int i = 0; i <= Interval; i++)
            {
                yield return new WaitForSeconds(1);
                if ((Interval - i) == 0)
                {
                    GameManager.PushToEventLog("Stage " + OnStage.ToString());
                }
                else
                {
                    GameManager.PushToEventLog("Next Stage in " + (Interval - i).ToString());
                }
            }
            StartCoroutine(Spawning("SpawnStage", 3));
        }
    }
    internal void SpawnBaddie(EnemyFunction.EnemyID id, Vector3? spawnPos = null) //Quick setup for objects 
    {
        GameObject GridPieceTemp = Grid[Random.Range(0, 21), 22]; //one less than the grid size
        spawnPos = spawnPos ?? new Vector3(GridPieceTemp.transform.position.x + (GridPieceTemp.GetComponent<Renderer>().bounds.size.x / 2), GridPieceTemp.transform.position.y, GridPieceTemp.transform.position.z);
        GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Enemy = GameManager.AssignComponents(id.ToString(), tempMesh.GetComponent<MeshFilter>().mesh, new Material(Shader.Find("Unlit/Color")), true); Destroy(tempMesh);
        Enemy.transform.position = (Vector3)spawnPos + Vector3.up;
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<EnemyFunction>();
        Enemy.GetComponent<EnemyFunction>().EnemyType(id);
    }
    private EnemyFunction.EnemyID[] GenerateStage()
    {
        LevelDifficulty = (int)((5 * Mathf.Sin((2 * Mathf.PI * OnStage) / -3)) + (5 * OnStage) + 3);
        int totalLevelSet = 0;
        List<EnemyFunction.EnemyID> temp = new List<EnemyFunction.EnemyID>();
        while (totalLevelSet < LevelDifficulty)
        {
            int tempint = Random.Range(1, 11);
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
        EnemyFunction.EnemyID[] temparray= new EnemyFunction.EnemyID[temp.Count];
        for (int i = 0; i < temp.Count; i++)
        {
            temparray[i] = temp[i];
        }
        return temparray;             
        // EnemyFunction.EnemyID[] Stage0 = new EnemyFunction.EnemyID[] {EnemyFunction.EnemyID.Assasin,EnemyFunction.EnemyID.Bonus, EnemyFunction.EnemyID.Boss, EnemyFunction.EnemyID.Charger, EnemyFunction.EnemyID.Default, EnemyFunction.EnemyID.Knight, EnemyFunction.EnemyID.Mother, EnemyFunction.EnemyID.Phasing, EnemyFunction.EnemyID.Regenerator, EnemyFunction.EnemyID.Shielded, EnemyFunction.EnemyID.Teleport, EnemyFunction.EnemyID.Undead}; Stages.Add(Stage0); //Planned Stages
    }
}
