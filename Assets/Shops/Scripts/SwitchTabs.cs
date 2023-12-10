using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchTabs : MonoBehaviour
{
    private RectTransform currentLayout;

    // the 2 big black btns
    [SerializeField] private RectTransform itemsUI;
    [SerializeField] private RectTransform upgradesUI;
    
    // the btns under "Items" tabs
    [SerializeField] private RectTransform items_Weapon;
    [SerializeField] private RectTransform items_Eqpmt;
    [SerializeField] private RectTransform items_Special;

    // the btns under "Upgrades" tabs
    [SerializeField] private RectTransform upgrades_Skills;
    [SerializeField] private RectTransform upgrades_Weapon;

    private void Start()
    {
        currentLayout = itemsUI;

        // delete later
        currentLayout.gameObject.SetActive(true);
        GetComponent<ScrollRect>().content = currentLayout;
    }

    public void ClickTab(int btnNumber)
    {
        currentLayout.gameObject.SetActive(false);

        RectTransform[] layouts = { itemsUI, upgradesUI, items_Weapon, items_Eqpmt, items_Special, upgrades_Skills, upgrades_Weapon };
        currentLayout = layouts[btnNumber];

        currentLayout.gameObject.SetActive(true);
        GetComponent<ScrollRect>().content = currentLayout;
    }

}
