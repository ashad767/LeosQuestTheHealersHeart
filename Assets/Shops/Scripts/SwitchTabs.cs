using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchTabs : MonoBehaviour
{
    private RectTransform currentLayout;

    #region Layouts to Display
    [SerializeField] private RectTransform itemsUI;
    [SerializeField] private RectTransform upgradesUI;
    
    [SerializeField] private RectTransform items_Weapon;
    [SerializeField] private RectTransform items_Eqpmt;
    [SerializeField] private RectTransform items_Special;

    [SerializeField] private RectTransform upgrades_Skills;
    [SerializeField] private RectTransform upgrades_Weapon;
    #endregion


    [SerializeField] private GameObject shop;

    private int currentLayoutTracker;

    private void Start()
    {
        currentLayout = itemsUI;
        currentLayoutTracker = 0;

        // delete later
        currentLayout.gameObject.SetActive(true);
        GetComponent<ScrollRect>().content = currentLayout;
    }

    public void ClickTab(int btnNumber)
    {
        currentLayout.gameObject.SetActive(false);

        Re_Render(btnNumber);

        RectTransform[] layouts = { itemsUI, upgradesUI, items_Weapon, items_Eqpmt, items_Special, upgrades_Skills, upgrades_Weapon };
        currentLayout = layouts[btnNumber];
        currentLayoutTracker = btnNumber;

        currentLayout.gameObject.SetActive(true);
        GetComponent<ScrollRect>().content = currentLayout;
    }

    private void Re_Render(int btnNumber)
    {
        // Coming FROM 2 big black btns
        if (currentLayoutTracker <= 1 && btnNumber > 1)
        {
            switch (btnNumber)
            {
                case 2:
                    shop.GetComponent<ItemsW_Manager>().checkIfPurchaseable();
                    break;

                case 3:
                    shop.GetComponent<ItemsE_Manager>().checkIfPurchaseable();
                    break;

                case 4:
                    shop.GetComponent<ItemsS_Manager>().checkIfPurchaseable();
                    break;

                case 5:
                    shop.GetComponent<UpgradesSkillsManager>().checkIfUpgradeable();
                    break;

                case 6:
                    shop.GetComponent<UpgradesWeaponsManager>().checkIfUpgradeable();
                    break;
            }
        }
    }

}
