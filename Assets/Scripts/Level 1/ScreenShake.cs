using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Vector3 originalCameraPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.25f;
    //private float dampingSpeed = 1f;

    private void Awake()
    {
        originalCameraPosition = transform.localPosition;
    }

    public void Shake()
    {
        originalCameraPosition = transform.localPosition;
        shakeDuration = 0.5f; // Adjust the duration as needed
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            // Smoothly interpolate towards a random position inside a circle
            transform.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalCameraPosition; // Reset to the original position
        }
    }

}
