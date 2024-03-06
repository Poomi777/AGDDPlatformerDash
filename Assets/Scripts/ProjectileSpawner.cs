using System.Collections;
using System.Collections.Generic;
using AGDDPlatformer;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour, IResettable
{
    public GameObject projectilePrefab;
    public float spawnRate = 2f;
    public float initialDelay = 3f;
    public bool isStraightMovingProjectile = true;
    public Transform spawnPoint;
    public Transform aimPoint;


    private float lastSpawnTime = 0f;
    private bool canShoot = false;

    
    void Start()
    {
        GameManager.instance.resettableGameObjects.Add(this);
        StartCoroutine(DelayStartShoot(initialDelay));
    }
    
    void Update()
    {
        if (canShoot && lastSpawnTime >= spawnRate)
        {
            SpawnProjectile();
            lastSpawnTime = 0f;
        }

        lastSpawnTime += Time.deltaTime;
    }

    IEnumerator DelayStartShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnProjectile();
        canShoot = true; // Enable spawning after the delay
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

            Projectile projectileScript = newProjectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.isStraightMovingProjectile = isStraightMovingProjectile;

                Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    if (isStraightMovingProjectile && aimPoint != null)
                    {
                        Vector2 direction = (aimPoint.position - spawnPoint.position).normalized;
                        rb.velocity = direction * projectileScript.speed;

                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        newProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
            }
        }

        else
            {
                Debug.LogError("Projectile prefab is not assigned to the spawner.");
            }
    }

    public void resetGameObject()
    {
        canShoot = false;
        lastSpawnTime = 0f;
        StartCoroutine(DelayStartShoot(initialDelay));
    }

    public bool isDestructible()
    {

        return false;
    }
}
