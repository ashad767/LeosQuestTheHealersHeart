using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class L3Health : MonoBehaviour
{
    [SerializeField] private L3BossMovement moros;
    private Slider slider;
    [SerializeField] private Image fillImage;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Since boss gameobject gets destroyed and this script is attached to Slider, not boss, it would throw a null exception error if I didn't put this if-statement
        if (moros != null)
        {
            slider.value = moros.currentHealth / moros.maxHealth;
        }

        if (slider.value <= slider.minValue)
        {
            fillImage.enabled = false;
        }
    }
}
