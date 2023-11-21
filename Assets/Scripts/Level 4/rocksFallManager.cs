using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocksFallManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rocksFallPrefabs;
    
    Camera mainCamera;
    float frustumHeight;
    float frustumWidth;

    // Start is called before the first frame update
    void Start()
    {
        // Get the camera's frustum size in world units
        mainCamera = Camera.main;
        frustumHeight = 2.0f * mainCamera.orthographicSize; // mainCamera.orthographicSize gives HALF of the vertical size of the camera view. Multiplying by 2 gives the full height of the camera view.
        frustumWidth = frustumHeight * mainCamera.aspect; // mainCamera.aspect is (width / height). Since we already have the frustum height from previous line, we can do -> (width / height) * frustumHeight, which cancels the heights and gives us the width of the camera view.
    }

    public void spawnRocks()
    {
        StartCoroutine(spawnRocksFunc());
    }

    private IEnumerator spawnRocksFunc()
    {
        int howManyRocksToDrop = (int)Random.Range(5f, 10f);

        for (int i = 0; i < howManyRocksToDrop; i++)
        {
            float whichRockWillFall = Random.Range(0f, 1f);

            if (whichRockWillFall <= 0.33f)
            {
                Instantiate(rocksFallPrefabs[0], getRandomRockSpawnPosition(), Quaternion.identity);
            }
            else if (whichRockWillFall > 0.33f && whichRockWillFall <= 0.66f)
            {
                Instantiate(rocksFallPrefabs[1], getRandomRockSpawnPosition(), Quaternion.identity);
            }
            else
            {
                Instantiate(rocksFallPrefabs[2], getRandomRockSpawnPosition(), Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
        }
    }

    private Vector2 getRandomRockSpawnPosition()
    {
        float mainCameraPositionX = mainCamera.transform.position.x;
        float mainCameraPositionY = mainCamera.transform.position.y;

        // Calculate the position within the width of the camera's frustum and at the top of the camera frustum
        return new Vector2(
            Random.Range((-frustumWidth / 2.7f) + mainCameraPositionX, (frustumWidth / 2.7f) + mainCameraPositionX),
            mainCameraPositionY + (frustumHeight / 1.5f)
            );

    }
}
