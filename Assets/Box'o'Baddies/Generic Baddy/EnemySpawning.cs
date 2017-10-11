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
        for (int j = 0; j < Levels.Count; j++)
        {
            for (int i = 0; i < Levels[0].Length; i++)
            {
            yield return new WaitForSeconds(spawnInterval); //spawn interval
            SpawnBaddie((Levels[j])[i]);
            }
            
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
        string[] Level0 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level0);
        string[] Level1 = new string[] { "Default", "Default", "Knight",  "Default", "Default", "Knight",  "Default", "Default", "Knight",  "Boss" }; Levels.Add(Level1);
        string[] Level2 = new string[] { "Default", "Assasin", "Assasin", "Default", "Assasin", "Default", "Default", "Assasin", "Default", "Boss" }; Levels.Add(Level2);
        string[] Level3 = new string[] { "Mother", "Default", "Mother", "Default", "Default", "Mother", "Default", "Mother", "Default", "Boss" }; Levels.Add(Level3);
        string[] Level4 = new string[] { "Default", "Default", "Default", "Default", "Bonus", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level4);
        string[] Level5 = new string[] { "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Default", "Boss" }; Levels.Add(Level5);
        string[] Level6 = new string[] { "Default", "Default", "Knight", "Default", "Default", "Knight", "Default", "Default", "Knight", "Boss" }; Levels.Add(Level6);
        string[] Level7 = new string[] { "Default", "Assasin", "Mother", "Default", "Assasin", "Default", "Default", "Assasin", "Default", "Boss" }; Levels.Add(Level7);
        string[] Level8 = new string[] { "Mother", "Knight", "Mother", "Default", "Default", "Mother", "Knight", "Mother", "Knight", "Boss" }; Levels.Add(Level8);
        string[] Level9 = new string[] { "Boss", "Boss", "Mother", "Assasin", "Bonus", "Assasin", "Bonus", "Mother", "Knight", "Boss" }; Levels.Add(Level9);

    }
}
