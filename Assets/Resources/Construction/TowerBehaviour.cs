using UnityEngine;
using System.Collections;
using System;

public class TowerBehaviour : MonoBehaviour {

    private int Damage = -1, Cost = 10, Range = 50, ProjectileSpeed = 5, Accuracy;
    private float FireRate = 0.25f;
    private GameObject enemyController;
    private GameObject target = null;
    private GameManagerStuff GameManager;

    // Use this for initialization
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        enemyController = GameObject.Find("EnemyController");
        if (GameManager.CurrencyAmount - Cost < 0)
        {
            GameManager.PushToEventLog("This exceeds youre current gold.");
            Destroy(gameObject);
        }
        else
        {
            GameManager.CurrencyAmount -= Cost;
        }
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform enemy in enemyController.transform)
        {
            if (enemy != null)
            {
                if (target != null)
                {
                    if (Vector3.Distance(enemy.position, transform.position) < Vector3.Distance(target.transform.position, transform.position))
                    {
                        target = enemy.gameObject;
                    }
                }
                else
                {
                    target = enemy.gameObject;
                }
            }
        }
    }
    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(FireRate);
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < Range)
        {
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.transform.localScale *= 3;
            Projectile.AddComponent<Rigidbody>();
            Projectile.GetComponent<Rigidbody>().useGravity = false;
            Projectile.GetComponent<SphereCollider>().isTrigger = true;
            Projectile.transform.position = gameObject.transform.position + new Vector3(0, gameObject.GetComponent<MeshCollider>().bounds.size.y);
            Projectile.GetComponent<Rigidbody>().velocity = ((target.transform.position - Vector3.forward)- Projectile.transform.position) * ProjectileSpeed;
            Projectile.transform.SetParent(gameObject.transform);
            Projectile.transform.name = "Projectile";
            Destroy(Projectile, 2);
        }
        StartCoroutine(Fire());
    }
    private void SetTowerType(String type)
    {
        switch (type)
        {
            case "Fire":
                break;
            default:
                break;
        }
    }
    public int GetDamage()
    {
        return Damage;
    }
}
