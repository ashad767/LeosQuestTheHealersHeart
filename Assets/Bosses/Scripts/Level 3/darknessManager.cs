using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class darknessManager : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject bossSkeleton;
    [SerializeField] private GameObject MC;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject MC_PointLight;

    public Tilemap[] tilemapsToChange;
    
    // I have to keep track of the existing mini-enemies in this list so that I can update their respective material type when the darkening effect activates/deactivates
    public List<GameObject> spawnedMiniEnemies = new List<GameObject>();

    private Material originalMaterial;

    private Material originalTilemapMaterial;
    [SerializeField] private Material darkenMaterial;

    public bool isDark = false;

    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = bossSkeleton.GetComponent<SpriteRenderer>().material;

        originalTilemapMaterial = tilemapsToChange[0].GetComponent<TilemapRenderer>().material; 

    }

    private void Update()
    {
        renderMaterialForMiniEnemies(spawnedMiniEnemies);
    }

    private void renderMaterialForMiniEnemies(List<GameObject> spawnedMiniEnemies)
    {
        if(spawnedMiniEnemies != null)
        {
            for (int i = 0; i < spawnedMiniEnemies.Count; i++)
            {
                if(spawnedMiniEnemies[i] != null)
                {
                    if (isDark)
                    {
                        spawnedMiniEnemies[i].GetComponent<SpriteRenderer>().material = darkenMaterial;
                    }
                    else
                    {
                        spawnedMiniEnemies[i].GetComponent<SpriteRenderer>().material = originalMaterial;
                    }
                }
            }
        }
    }

    public void activateDarkness()
    {
        isDark = true;
        foreach(Tilemap tilemap in tilemapsToChange)
        {
            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
            renderer.material = darkenMaterial;
        }

        if(bossSkeleton != null)
        {
            bossSkeleton.GetComponent<SpriteRenderer>().material = darkenMaterial;
        }
        
        directionalLight.SetActive(true);
        StartCoroutine(activateDarknessIntensity());

        MC_PointLight.SetActive(true);
        MC_PointLight.transform.SetParent(MC.transform);
        MC_PointLight.GetComponent<Light>().intensity = 0.7f;
    }

    private IEnumerator activateDarknessIntensity()
    {
        float timer = 0f;
        float transitionToDarknessDuration = 2.7f;
        float originalIntensity = 1f;
        float darknessIntensity = 0.03f;

        while (timer < transitionToDarknessDuration)
        {
            directionalLight.GetComponent<Light>().intensity = Mathf.Lerp(originalIntensity, darknessIntensity, timer/transitionToDarknessDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // the "transitionToOriginalDuration" float is for having different durations to de-activate darkness.
    // One for normal duration (5 seconds) ; Another for when the boss dies while it's dark and I want to quickly turn on the lights (2 seconds)
    public void de_activateDarkness(float transitionToOriginalDuration)
    {
        StartCoroutine(de_activateDarknessIntensity(transitionToOriginalDuration)); // I have to incrementally deactivate the darkness and THEN reset the sprite material types, or else it would just instantly light up with no gradual progress.
    }

    private IEnumerator de_activateDarknessIntensity(float transitionToOriginalDuration)
    {
        float timer = 0f;
        float darknessIntensity = 0.03f;
        float originalIntensity = 1f;
        
        while (timer < transitionToOriginalDuration)
        {
            directionalLight.GetComponent<Light>().intensity = Mathf.Lerp(darknessIntensity, originalIntensity, timer / transitionToOriginalDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        isDark = false;

        foreach (Tilemap tilemap in tilemapsToChange)
        {
            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
            renderer.material = originalTilemapMaterial;
        }
        if (bossSkeleton != null)
        {
            bossSkeleton.GetComponent<SpriteRenderer>().material = originalMaterial;
        }
        
        directionalLight.SetActive(false);
        MC_PointLight.SetActive(false);
    }
}
