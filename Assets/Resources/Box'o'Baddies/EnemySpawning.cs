using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawning : MonoBehaviour
{
    private GameObject[,] grid;
    private int spawnInterval = 2, onStage = 0;
    private GameManagerStuff gameManager;
    List<string[]> Stages = new List<string[]>();

    // Use this for initialization
    void Start()
    {
        grid = GameObject.Find("GameGrid").GetComponent<CreateGameGrid>().GetGrid();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        generateStages();
        StartCoroutine(SpawningInterval());
    }
    private void Update()
    {
        if (transform.childCount == 0) //is Stage finished completed?
        {
            onStage++;
            gameManager.PushToEventLog("Stage" + onStage);
            StartCoroutine(SpawningInterval());
        }
    }

    private IEnumerator SpawningInterval()
    {
        for (int i = 0; i < Stages[0].Length; i++)
        {
            SpawnBaddie((Stages[onStage])[i]);
            yield return new WaitForSeconds(spawnInterval); //spawn interval
        }
    }

    public void SpawnBaddie(string type) 
    {
        SpawnBaddie(type, grid[Random.Range(0, 11), 19].transform.position + Vector3.up * 10);
    }

    public void SpawnBaddie(string type, Vector3 spawnPos)
    {
        GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Enemy = gameManager.AssignComponents(type, tempMesh.GetComponent<MeshFilter>().mesh, new Material(Shader.Find("Unlit/Color")), true); Destroy(tempMesh);
        Enemy.transform.position = spawnPos;
        Enemy.transform.SetParent(gameObject.transform); //Orderliness
        Enemy.AddComponent<WhatBaddieDo>();
        Collider col2 = Enemy.AddComponent<BoxCollider>();
        col2.isTrigger = true;  
        Enemy.GetComponent<WhatBaddieDo>().EnemyType(type);
    }

    private void generateStages()
    {
        string[] Stage0 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Stages.Add(Stage0);
        string[] Stage1 = new string[] { "Default", "Default", "Knight", "Default", "Default", "Knight", "Default", "Default", "Knight", "Boss" }; Stages.Add(Stage1);
        string[] Stage2 = new string[] { "Default", "Assasin", "Assasin", "Default", "Assasin", "Default", "Default", "Assasin", "Default", "Boss" }; Stages.Add(Stage2);
        string[] Stage3 = new string[] { "Mother", "Default", "Mother", "Default", "Default", "Mother", "Default", "Mother", "Default", "Boss" }; Stages.Add(Stage3);
        string[] Stage4 = new string[] { "Default", "Default", "Default", "Default", "Bonus", "Default", "Default", "Default", "Default", "Boss" }; Stages.Add(Stage4);
        string[] Stage5 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Stages.Add(Stage5);
        string[] Stage6 = new string[] { "Default", "Default", "Knight", "Default", "Default", "Knight", "Default", "Default", "Knight", "Boss" }; Stages.Add(Stage6);
        string[] Stage7 = new string[] { "Default", "Assasin", "Mother", "Default", "Assasin", "Default", "Default", "Assasin", "Default", "Boss" }; Stages.Add(Stage7);
        string[] Stage8 = new string[] { "Mother", "Knight", "Mother", "Default", "Default", "Mother", "Knight", "Mother", "Knight", "Boss" }; Stages.Add(Stage8);
        string[] Stage9 = new string[] { "Boss", "Boss", "Mother", "Assasin", "Bonus", "Assasin", "Bonus", "Mother", "Knight", "Boss" }; Stages.Add(Stage9);
    }
}
