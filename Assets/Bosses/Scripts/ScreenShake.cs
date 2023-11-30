using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Vector3 originalCameraPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.25f;

    private void Awake()
    {
        originalCameraPosition = transform.position;
    }

    public void Shake()
    {
        originalCameraPosition = transform.position;
        shakeDuration = 0.5f; // Adjust the duration as needed
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            // Smoothly interpolate towards a random position inside a circle
            transform.position = originalCameraPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.position = originalCameraPosition; // Reset to the original position
        }
    }

}
