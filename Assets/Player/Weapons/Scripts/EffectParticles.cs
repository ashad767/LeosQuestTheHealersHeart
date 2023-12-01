using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectParticles : MonoBehaviour
{
    public string effectName;
    public float effectDuration;
    

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer parentSpriteRenderer;

    private void Start()
    {
        parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private void Update()
    {
        effectDuration -= Time.deltaTime;
        spriteRenderer.sprite = parentSpriteRenderer.sprite;
        transform.position = transform.parent.position;

        Debug.Log(spriteRenderer.material);
        Debug.Log(parentSpriteRenderer.material);

        if (effectDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
