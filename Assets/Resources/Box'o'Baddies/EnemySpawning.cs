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
        string[] Stage0 = new string[] { "Charger", "Mother", "Assasin", "Knight", "Shielded", "Regenerator", "Bonus", "Undead", "Default", "Boss" }; Stages.Add(Stage0);
    }
}
