using System.Collections;
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
        SpawnBaddie(doBaddieOrder());
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
    private string doBaddieOrder()
    {
        string[] baddies = { "Boss", "Assasin", "Knight", "Mother", "Bonus", "Default"};
        //                      0        1         2         3         4          5
        var holdMeDaddy = (Random.value * baddies.Length);
        return baddies[4];
    }
}
