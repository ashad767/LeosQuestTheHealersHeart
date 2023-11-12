using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSpawner : MonoBehaviour
{
    //public GameObject lightningPrefab;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    StartCoroutine(SpawnLightning());
    //}

    //IEnumerator SpawnLightning()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(Random.Range(2f, 4f)); // Adjust the delay as needed

    //        // Get the camera's frustum size in world units
    //        Camera mainCamera = Camera.main;
    //        float frustumHeight = 2.0f * mainCamera.orthographicSize;
    //        float frustumWidth = frustumHeight * mainCamera.aspect;

    //        // Calculate the position at the top of the camera frustum
    //        Vector3 spawnPosition = new Vector3(
    //            Random.Range(-frustumWidth / 2f, frustumWidth / 2f),
    //            mainCamera.transform.position.y + frustumHeight / 2f,
    //            0f
    //        );

    //        // Instantiate the lightning prefab at the calculated position
    //        Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);
    //    }
    //}
}
