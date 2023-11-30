using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class L4Health : MonoBehaviour
{
    // Because I'm only allowed to drag & drop gameObjects in SerializeFields (or public variables), and this script is not attached to Wizard gameObject (it's attached to Slider, which is also not a prefab),
    // I need to make a reference to 'Wizard.cs' by referencing an actual gameObject (Wizard), then using the script (Wizard.cs). So I dragged & dropped the Wizard game object to this SerializeField, then using its script Wizard.cs
    [SerializeField] private L4BossMovement fireDemon; // referencing the script Wizard.cs, not the game object

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
        if (fireDemon != null)
        {
            slider.value = fireDemon.currentHealth / fireDemon.maxHealth;
        }

        if (slider.value <= slider.minValue)
        {
            fillImage.enabled = false;
        }
    }
}
