using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class darknessManager : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject bossSkeleton;
    [SerializeField] private GameObject MC;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject MC_PointLight;
    
    // I have to keep track of the existing mini-enemies in this list so that I can update their respective material type when the darkening effect activates/deactivates
    public List<GameObject> spawnedMiniEnemies = new List<GameObject>();

    private Material originalMaterial;
    [SerializeField] private Material darkenMaterial;

    public bool isDark = false;

    // Start is called before the first frame update
    void Start()
    {
        // this stores the original material type of the sprites. To be used when the darkening effect fades away.
        originalMaterial = background.GetComponent<SpriteRenderer>().material;
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

    public void activateDarkness()
    {
        isDark = true;
        background.GetComponent<SpriteRenderer>().material = darkenMaterial;
        bossSkeleton.GetComponent<SpriteRenderer>().material = darkenMaterial;
        
        directionalLight.SetActive(true);
        StartCoroutine(activateDarknessIntensity());

        MC_PointLight.SetActive(true);
        MC_PointLight.transform.SetParent(MC.transform);
        MC_PointLight.GetComponent<Light>().intensity = 0.75f;
    }

    private IEnumerator activateDarknessIntensity()
    {
        float timer = 0f;
        float transitionToDarknessDuration = 2.7f;
        float originalIntensity = 1f;
        float darknessIntensity = 0.15f;

        while (timer < transitionToDarknessDuration)
        {
            directionalLight.GetComponent<Light>().intensity = Mathf.Lerp(originalIntensity, darknessIntensity, timer/transitionToDarknessDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void de_activateDarkness()
    {
        StartCoroutine(de_activateDarknessIntensity()); // I have to incrementally deactivate the darkness and THEN reset the sprite material types, or else it would just instantly light up with no gradual process.
    }

    private IEnumerator de_activateDarknessIntensity()
    {
        float timer = 0f;
        float transitionToOriginalDuration = 2.7f;
        float darknessIntensity = 0.15f;
        float originalIntensity = 1f;
        
        while (timer < transitionToOriginalDuration)
        {
            directionalLight.GetComponent<Light>().intensity = Mathf.Lerp(darknessIntensity, originalIntensity, timer / transitionToOriginalDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        isDark = false;
        background.GetComponent<SpriteRenderer>().material = originalMaterial;
        bossSkeleton.GetComponent<SpriteRenderer>().material = originalMaterial;
        directionalLight.SetActive(false);
        MC_PointLight.SetActive(false);
    }
}
