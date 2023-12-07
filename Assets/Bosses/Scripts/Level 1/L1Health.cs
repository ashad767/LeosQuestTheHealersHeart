using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class L1Health : MonoBehaviour
{
    [SerializeField] private L1BossMovement minotaur;
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
        if(minotaur != null)
        {
            slider.value = minotaur.currentHealth / minotaur.maxHealth;
        }

        if (slider.value <= slider.minValue)
        {
            fillImage.enabled = false;
        }
    }
}
