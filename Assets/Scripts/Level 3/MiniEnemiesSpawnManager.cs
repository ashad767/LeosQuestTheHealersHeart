using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesSpawnManager : MonoBehaviour
{
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;

    // Prefabs
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject miniZombiePrefab;
    [SerializeField] private GameObject miniSkeletonPrefab;
    [SerializeField] private GameObject lightningExplosionPrefab;

    [SerializeField] private darknessManager darknessManager;

    // Audio
    [SerializeField] AudioSource lightningAudio;

    private int numberOfMiniEnemies = 3;
    private int miniEnemiesRemaining = 0;

    // Singleton instance
    public static MiniEnemiesSpawnManager Instance { get; private set; }

    void Awake()
    {
        // Ensure there is only one instance of MiniEnemiesSpawnManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSpawning()
    {
        if(miniEnemiesRemaining == 0)
        {
            StartCoroutine(SpawnLightningAndEnemies());
        }
    }

    private IEnumerator SpawnLightningAndEnemies()
    {
        for (int i = 0; i < numberOfMiniEnemies; i++)
        {
            SpawnLightning();
            yield return new WaitForSeconds(0.1f);
        }
        miniEnemiesRemaining = numberOfMiniEnemies; // resetting the 'miniEnemiesRemaining' variable so that in each iteration in spawnMiniEnemies() coroutine (in L3BossMovement.cs), the 'StartSpawning()' function doesn't get called
    }

    private void SpawnLightning()
    {
        // Get the camera's frustum size in world units
        Camera mainCamera = Camera.main;
        float frustumHeight = 2.0f * mainCamera.orthographicSize; // mainCamera.orthographicSize gives HALF of the vertical size of the camera view. Multiplying by 2 gives the full height of the camera view.
        float frustumWidth = frustumHeight * mainCamera.aspect; // mainCamera.aspect is (width / height). Since we already have the frustum height from previous line, we can do -> (width / height) * frustumHeight, which cancels the heights and gives us the width of the camera view.

        // Calculate the position within the width of the camera's frustum and at the top of the camera frustum
        Vector2 spawnPosition = new Vector2(
            Random.Range(-frustumWidth / 2f, frustumWidth / 2f),
            mainCamera.transform.position.y + frustumHeight / 2f
            );

        // Instantiate the lightning prefab at the calculated position
        GameObject lightning = Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);
        lightning.transform.localScale = new Vector3(4f, Random.Range(0.5f, 9.5f), 1f); // take a random y-scale for the lightning
        Destroy(lightning, animLength[0].length);

        spawnLightningExplosion(lightning);

        // each mini-zombie or skeleton has a 50% chance of being spawned
        if (Random.Range(0f, 1f) <= 0.5f)
        {
            spawnMiniZombie(lightning);
        }
        else
        {
            spawnMiniSkeleton(lightning);
        }
    }

    private void spawnLightningExplosion(GameObject lightning)
    {
        GameObject lightningExplosion = Instantiate(lightningExplosionPrefab, lightning.transform.position, Quaternion.identity);
        lightningAudio.Play();

        // I want to spawn the explosion at the bottom of the lightning sprite. Vector2(0, -1) seemed a pretty good localPosition
        lightningExplosion.transform.SetParent(lightning.transform);
        lightningExplosion.transform.localPosition = Vector2.down;
        lightningExplosion.transform.SetParent(null);
        
        Destroy(lightningExplosion, animLength[1].length);
    }

    private void spawnMiniZombie(GameObject lightning)
    {
        GameObject miniZombie = Instantiate(miniZombiePrefab, lightning.transform.position, Quaternion.identity);
        miniZombie.GetComponent<MiniZombieMovement>().darknessManager = darknessManager;
        darknessManager.spawnedMiniEnemies.Add(miniZombie); // need to add this spawned mini-zombie to the darknessManager's 'spawnedMiniEnemies' list to keep track of the existing mini-zombie gameobject and whether or not to apply the darkening material. THIS GAMEOBJECT GETS REMOVED FROM THE 'spawnedMiniEnemies' LIST WHEN IT DIES (inside 'MiniZombieMovement.cs')

        // I want to spawn the mini-zombie at the bottom of the lightning sprite. Vector2(0, -1) seemed a pretty good localPosition
        miniZombie.transform.SetParent(lightning.transform);
        miniZombie.transform.localPosition = Vector2.down;
        miniZombie.transform.SetParent(null);
        miniZombie.GetComponent<MiniZombieMovement>().MC = MC;
    }

    private void spawnMiniSkeleton(GameObject lightning)
    {
        GameObject miniSkeleton = Instantiate(miniSkeletonPrefab, lightning.transform.position, Quaternion.identity);
        miniSkeleton.GetComponent<MiniSkeletonMovement>().darknessManager = darknessManager;
        darknessManager.spawnedMiniEnemies.Add(miniSkeleton); // need to add this spawned mini-skeleton to the darknessManager's 'spawnedMiniEnemies' list to keep track of the existing mini-skeleton gameobject and whether or not to apply the darkening material. THIS GAMEOBJECT GETS REMOVED FROM THE 'spawnedMiniEnemies' LIST WHEN IT DIES (inside 'MiniSkeletonMovement.cs')

        // I want to spawn the mini-skeleton at the bottom of the lightning sprite. Vector2(0, -1) seemed a pretty good localPosition
        miniSkeleton.transform.SetParent(lightning.transform);
        miniSkeleton.transform.localPosition = Vector2.down;
        miniSkeleton.transform.SetParent(null);
        miniSkeleton.GetComponent<MiniSkeletonMovement>().MC = MC;
    }

    public void MiniEnemyKilled()
    {
        miniEnemiesRemaining--;
    }
}
