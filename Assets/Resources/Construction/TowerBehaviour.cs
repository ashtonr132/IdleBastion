﻿using UnityEngine;
using System.Collections;
using System;

public class TowerBehaviour : MonoBehaviour {

    internal int Cost = 100, Range = 50, ProjectileSpeed = 5, BonusGold = 0;
    internal float FireRate = 0.25f, Accuracy = 1.25f, Damage = -1, armourpiercingpc = 0;
    private GameObject EnemyController, Target = null;
    internal static GameObject LastTowerSelected;
    private GameManagerStuff GameManager;
    internal enum TowerID
    {
        Default
    }
    internal TowerID CurrentTowerID;
    void Start() //Use this for initialization
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        EnemyController = GameObject.Find("EnemyController");
        if (GameManagerStuff.Currency - Cost < 0)
        {
            GameManager.PushToEventLog("Not Enough Gold.");
            Destroy(gameObject);
        }
        else
        {
            GameManagerStuff.Currency -= Cost;
        }
        StartCoroutine(Fire());
        LastTowerSelected = gameObject;
    }
    void Update()//Update is called once per frame
    {
        foreach (Transform enemy in EnemyController.transform) //target enemy
        {
            if (enemy != null)
            {
                if (Target != null)
                {
                    if (Vector3.Distance(enemy.position, transform.position) < Vector3.Distance(Target.transform.position, transform.position))
                    {
                        Target = enemy.gameObject;
                    }
                }
                else
                {
                    Target = enemy.gameObject;
                }
            }
        }
    }
    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(FireRate);
        if (Target != null && Vector3.Distance(Target.transform.position, transform.position) < Range)
        {
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.transform.name = "Projectile";
            Projectile.transform.localScale *= 3;
            Rigidbody rb = Projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Projectile.GetComponent<SphereCollider>().isTrigger = true;
            Projectile.transform.position = gameObject.transform.position + new Vector3(0, gameObject.GetComponent<MeshCollider>().bounds.size.y);
            Projectile.GetComponent<Rigidbody>().velocity = (((UnityEngine.Random.onUnitSphere / Accuracy) + Target.transform.position) - Projectile.transform.position) * ProjectileSpeed;
            Projectile.transform.SetParent(gameObject.transform);
            Destroy(Projectile, ProjectileSpeed/2);
        }
        StartCoroutine(Fire());
    }
    internal void SetTowerType(TowerID ID) //different tower types set here
    {
        switch (ID)
        {
            default: //Basic
                Damage = -1; Cost = 100; Range = 50; ProjectileSpeed = 5; FireRate = 0.25f; 
                transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color32(100, 50, 0, 255);
                    break;
        }
    }
    public float GetDamage() //damage this tower currently does
    {
        return Damage;
    }
}
