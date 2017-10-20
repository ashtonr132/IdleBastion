using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    private GameObject[,] Grid;
    private int OnStage = 0;
    private GameManagerStuff GameManager;
    List<string[]> Stages = new List<string[]>();

    void Start() //Use this for initialization
    {
        Grid = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        GenerateStages();
        StartCoroutine(Spawning("NextStage", 3)); //Start stageloop ienum
    }
    public IEnumerator Spawning(string Pass, float Interval)
    {
        if (Pass == "SpawnStage") //Spawn enemies in onstage stage
        {
            for (int i = 0; i <= Stages[OnStage].Length - 1; i++)
            {
                yield return new WaitForSeconds(Interval);
                SpawnBaddie(Stages[OnStage][i]);
                if (i == Stages[OnStage][i].Length - 2)
                {
                    StartCoroutine(Spawning("NextStage", 3));
                }
            }
            OnStage++;
        }
        else if(Pass == "NextStage") //Prepare for next stage
        {
            Interval += GameObject.Find("EnemyController").transform.childCount - 1; //Contains one non enemy child
            for(int i = 0; i < Interval; i++)
            {
                yield return new WaitForSeconds(1);
                GameManager.PushToEventLog("Next Stage in " + (Interval - i).ToString());
            }
            GameManager.PushToEventLog("Stage " + OnStage.ToString());
            StartCoroutine(Spawning("SpawnStage", 1));
        }
    }
    public void SpawnBaddie(string type)
    {
        SpawnBaddie(type, Grid[Random.Range(0, 11), 19].transform.position);
    }
    public void SpawnBaddie(string type, Vector3 spawnPos) //Quick setup for objects 
    {
        GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Enemy = GameManager.AssignComponents(type, tempMesh.GetComponent<MeshFilter>().mesh, new Material(Shader.Find("Unlit/Color")), true); Destroy(tempMesh);
        Enemy.transform.position = spawnPos + new Vector3(0, Enemy.GetComponent<Renderer>().bounds.size.y/2 + 1, 0);
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<EnemyFunction>();
        Enemy.GetComponent<EnemyFunction>().EnemyType(type);
    }
    private void GenerateStages()
    {
        string[] Stage0 = new string[] {"Charger" }; Stages.Add(Stage0); //Planned Stages
    }
}
