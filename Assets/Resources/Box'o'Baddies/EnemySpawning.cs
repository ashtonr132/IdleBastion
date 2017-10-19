using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    private GameObject[,] Grid;
    private int SpawnInterval = 2, OnStage = 0;
    private GameManagerStuff GameManager;
    List<string[]> Stages = new List<string[]>();

    //Use this for initialization
    void Start()
    {
        Grid = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        generateStages();
        StartCoroutine(Spawning(SpawnInterval));
    }
    private void Update()
    {
        if (transform.childCount == 0) //is Stage finished completed?
        {
            if(!(OnStage >= Stages.Count -1))
            {
                OnStage++;
                StartCoroutine(Spawning(SpawnInterval));
                GameManager.PushToEventLog("Stage " + OnStage);
            }
            else
            {
                //start infinite mode
            }
        }
    }
    private IEnumerator Spawning(int Interval)
    {
        for (int i = 0; i < Stages[OnStage].Length; i++)
        {
            SpawnBaddie((Stages[OnStage])[i]);
            yield return new WaitForSeconds(Interval); //spawn interval
        }
    }
    public void SpawnBaddie(string type) 
    {
        SpawnBaddie(type, Grid[Random.Range(0, 11), 19].transform.position);
    }
    public void SpawnBaddie(string type, Vector3 spawnPos)
    {
        GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Enemy = GameManager.AssignComponents(type, tempMesh.GetComponent<MeshFilter>().mesh, new Material(Shader.Find("Unlit/Color")), true); Destroy(tempMesh);
        Enemy.transform.position = spawnPos + new Vector3(0, Enemy.GetComponent<Renderer>().bounds.size.y/2 + 1, 0);
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<WhatBaddieDo>();
        Enemy.GetComponent<WhatBaddieDo>().EnemyType(type);
    }
    private void generateStages()
    {
        string[] Stage0 = new string[] {"Default" }; Stages.Add(Stage0);
    }
}
