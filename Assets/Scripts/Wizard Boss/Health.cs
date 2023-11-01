using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Because I'm only allowed to drag & drop gameObjects in SerializeFields (or public variables), and this script is not in Wizard gameObject,
    // I need to make a reference to 'Wizard.cs' by referencing an actual gameObject (Wizard), then using the script (Wizard.cs)
    [SerializeField] private Wizard wizard; // referencing the script
    
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
        slider.value = wizard.currentHealth / wizard.maxHealth;

        if(slider.value <= slider.minValue )
        {
            fillImage.enabled = false;
        }
    }
}
