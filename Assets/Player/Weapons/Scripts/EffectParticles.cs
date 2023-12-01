using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectParticles : MonoBehaviour
{
    public string effectName;
    public float effectDuration;
    private void Update()
    {
        Debug.Log(name + " has effect " + effectName);

        effectDuration -= Time.deltaTime;

        if(effectDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
