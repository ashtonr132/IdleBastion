using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour {

    public GameObject Enemies;
    private GameObject[,] grid;
    private int spawnInterval = 2;


    // Use this for initialization
    void Start ()
    {
        grid = GameObject.Find("GameGrid").GetComponent<GameGrid>().getGrid();
        StartCoroutine(spawningInterval());
        
    }

    public IEnumerator spawningInterval()
    {
        SpawnBaddie(generateLevels(List.));
        yield return new WaitForSeconds((Random.value * 3) + spawnInterval); //spawn interval
        StartCoroutine(spawningInterval()); //restart ienum
    }

    public void SpawnBaddie(string type)
    {
        SpawnBaddie(type, grid[Random.Range(0, 11), 19].transform.position + Vector3.up *10);
    }

    public void SpawnBaddie(string type, Vector3 spawnPos)
    {
            GameObject Goonie = (GameObject)Instantiate(Enemies, spawnPos, Quaternion.identity);
            Goonie.transform.SetParent(gameObject.transform); //less clutter if you can close the GOtree, also keeps them all together
            Goonie.GetComponent<WhatBaddieDo>().EnemyType(type);
    }
    private List<string[]> generateLevels()
    {
        List<string[]> Levels = new List<string[]>();
        string[] Level0 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level0);
        string[] Level1 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level1);
        string[] Level2 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level2);
        return Levels;
    }
}
