using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawning : MonoBehaviour {

    public GameObject Enemies;
    private GameObject[,] grid;
    private int spawnInterval = 2;
    List<string[]> Levels = new List<string[]>();

    // Use this for initialization
    void Start ()
    {
        grid = GameObject.Find("GameGrid").GetComponent<GameGrid>().getGrid();
        generateLevels();
        StartCoroutine(spawningInterval());
    }

    public IEnumerator spawningInterval()
    {
        for (int i = 0; i < Levels[0].Length; i++)
        {
            yield return new WaitForSeconds(spawnInterval); //spawn interval
            SpawnBaddie((Levels[0])[i]);
        }
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

    private void generateLevels()
    {
        string[] Level0 = new string[] { "Mother", "Assasin", "Knight", "Bonus", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level0);
        string[] Level1 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level1);
        string[] Level2 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level2);
    }
}
