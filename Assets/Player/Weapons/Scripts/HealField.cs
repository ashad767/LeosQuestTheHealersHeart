using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealField : MonoBehaviour
{
    public float time;

    private void Update()
    {
        time += Time.deltaTime;

        if(time > 15)
        {
            Destroy(gameObject);
        }
    }
}
