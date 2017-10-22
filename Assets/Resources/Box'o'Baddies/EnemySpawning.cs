using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    internal GameObject[,] Grid;
    private int OnStage = 0;
    private GameManagerStuff GameManager;
    List<EnemyFunction.EnemyID[]> Stages = new List<EnemyFunction.EnemyID[]>();

    void Start() //Use this for initialization
    {
        Grid = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        GenerateStages();
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
                if (i == Stages[OnStage].Length - 2)
                {
                    StartCoroutine(Spawning("NextStage", 3));
                }
            }
            OnStage++;
        }
        else if(System.String.Equals(Pass, "NextStage")) //Prepare for next stage
        {
            Interval += GameObject.Find("EnemyController").transform.childCount - 1; //Contains one non enemy child
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
        Enemy.transform.position = (Vector3)spawnPos + new Vector3(0, Enemy.GetComponent<Renderer>().bounds.size.y/2 + 1, 0);
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<EnemyFunction>();
        Enemy.GetComponent<EnemyFunction>().EnemyType(id);
    }
    private void GenerateStages()
    {
        EnemyFunction.EnemyID[] Stage0 = new EnemyFunction.EnemyID[] { EnemyFunction.EnemyID.Charger }; Stages.Add(Stage0);
        // EnemyFunction.EnemyID[] Stage0 = new EnemyFunction.EnemyID[] {EnemyFunction.EnemyID.Assasin,EnemyFunction.EnemyID.Bonus, EnemyFunction.EnemyID.Boss, EnemyFunction.EnemyID.Charger, EnemyFunction.EnemyID.Default, EnemyFunction.EnemyID.Knight, EnemyFunction.EnemyID.Mother, EnemyFunction.EnemyID.Phasing, EnemyFunction.EnemyID.Regenerator, EnemyFunction.EnemyID.Shielded, EnemyFunction.EnemyID.Teleport, EnemyFunction.EnemyID.Undead}; Stages.Add(Stage0); //Planned Stages
    }
}
