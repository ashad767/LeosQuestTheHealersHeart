using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static UnityEditor.Progress;

public class ShopManager : MonoBehaviour
{
    private int playerCoins = 1000;
    public TextMeshProUGUI playerCoinsTxt;
    
    public CreateItem_SO[] SO_itemList;
    public ItemInfo[] itemInfoList;
    public Button[] upgradeBtns;
    private readonly float[] multipliers = { 1.25f, 1.5f, 1.75f, 2f, 2.25f };

    // Declare a jagged array of int arrays {pointer-to-next-upgrade-bar, cost}
    public int[][] arrayOfArrays = new int[5][]
    {
        new int[] { 0, 15 },
        new int[] { 0, 20 },
        new int[] { 0, 30 },
        new int[] { 0, 55 },
        new int[] { 0, 85 },
    };

    private int nextUpgradeBarIndex = 0;
    private int costIndex = 1;


    // Start is called before the first frame update
    void Start()
    {
        playerCoinsTxt.text = "Coins: " + playerCoins.ToString();

        // Uncomment this reset the shop data
        //Reset();
        
        LoadItems();
        checkIfUpgradeable();
    }

    private void Reset()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetString(i.ToString(), "");
        }
    }

    // Used by the onClick() function when pressing the "return" button
    public void SaveData()
    {
        for (int i = 0; i < 5; i++)
        {
            string[] pointer_and_cost = new string[2];
            pointer_and_cost[0] = arrayOfArrays[i][nextUpgradeBarIndex].ToString();
            pointer_and_cost[1] = arrayOfArrays[i][costIndex].ToString();

            PlayerPrefs.SetString(i.ToString(), string.Join(",", pointer_and_cost));
        }
        
        PlayerPrefs.Save();
    }

    private void LoadItems()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            ItemInfo currentItem = itemInfoList[i];

            currentItem.itemImg.sprite = SO_itemList[i].itemImg;
            currentItem.itemImg.GetComponent<RectTransform>().sizeDelta = new Vector2(SO_itemList[i].imgWidth, 64f);

            currentItem.itemNameTxt.text = SO_itemList[i].itemName;
            currentItem.descriptionTxt.text = SO_itemList[i].description;

            if(PlayerPrefs.GetString(i.ToString()) != "")
            {
                string[] pointer_and_cost = PlayerPrefs.GetString(i.ToString()).Split(",");
                arrayOfArrays[i][nextUpgradeBarIndex] = int.Parse(pointer_and_cost[nextUpgradeBarIndex]);
                arrayOfArrays[i][costIndex] = int.Parse(pointer_and_cost[costIndex]);
            }
            else
            {
                arrayOfArrays[i][nextUpgradeBarIndex] = arrayOfArrays[i][nextUpgradeBarIndex];
                arrayOfArrays[i][costIndex] = arrayOfArrays[i][costIndex];
            }
            
            
            InitializeUpgradeBars(i);

            if (arrayOfArrays[i][nextUpgradeBarIndex] <= 4)
            {
                currentItem.nextUpgradeInfoTxt.text = "* " + multipliers[arrayOfArrays[i][nextUpgradeBarIndex]].ToString() + "x increase on next upgrade";

                // DELETE THIS LATER (THIS IS ONLY FOR RESETTING THE BASE COSTS)
                //SO_itemList[i].cost = baseCost[i];

                currentItem.costTxt.text = arrayOfArrays[i][costIndex].ToString() + " Coins";
            }
        }
    }

    private void InitializeUpgradeBars(int item)
    {
        // Before this for-loop, when the player would upgrade an item(s) and leave and then open the shop again, the upgrade bars would all be black.
        // Then if the player were to upgrade that item(s) again, the upgrade bars would start turning green from the value of "pointerToNextUpgradeBar", but leaving all the bars before it black.
        // This for-loop is to initialize the upgrade bars and check which upgrade bars are already green and display it from the start of the scene.
        // It iterates over all the upgrade bars of the specific shop item passed as an argument from LoadItems() named "item".
        for (int upgradeBar = 0; upgradeBar < arrayOfArrays[item][nextUpgradeBarIndex]; upgradeBar++)
        {
            itemInfoList[item].upgradeBarsList[upgradeBar].color = Color.green;
        }
    }

    private void checkIfUpgradeable()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            Button currentUpgradeBtn = upgradeBtns[i];

            if (arrayOfArrays[i][nextUpgradeBarIndex] > 4)
            {
                currentUpgradeBtn.interactable = false;
                currentUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "<s><i>Maxed Out</i></s>";

                itemInfoList[i].nextUpgradeInfoTxt.text = "";
                itemInfoList[i].costTxt.text = "";
            }

            else
            {
                // Update the "nextUpgradeInfoTxt" to tell the player by how much the buff will be increased by for the next item upgrade
                itemInfoList[i].nextUpgradeInfoTxt.text = "* " + multipliers[arrayOfArrays[i][nextUpgradeBarIndex]].ToString() + "x increase on next upgrade";

                if (playerCoins >= arrayOfArrays[i][costIndex])
                {
                    currentUpgradeBtn.interactable = true;
                    currentUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "UPGRADE";
                }

                else
                {
                    currentUpgradeBtn.interactable = false;
                    currentUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Insufficient Coins";
                }
            }
            
        }
    }

    // Used by the onClick() function in the Inspector window
    public void Upgrade(int btnNumber)
    {
        if(playerCoins >= arrayOfArrays[btnNumber][costIndex])
        {
            // Decrease the player's coins (top right corner)
            playerCoins -= arrayOfArrays[btnNumber][costIndex];
            playerCoinsTxt.text = "Coins: " + playerCoins.ToString();

            // Changes the color of the upgrade bar that the "pointerToNextUpgradeBar" index is pointing to, to green
            itemInfoList[btnNumber].upgradeBarsList[arrayOfArrays[btnNumber][nextUpgradeBarIndex]].color = Color.green;

            // Increases upgrade bar price for next item upgrade bar (STARTING FROM 2ND BAR SINCE FIRST BAR IS ALREADY SET AS THE BASE COST IN "CreateItem_SO.cs").
            // So for this, when the player upgrades to the last bar (and thus all 5 bars turn green), this will run and display the cost, but then immediately be an empty string because of the call to "checkIfUpgradeable()".
            // "SO_itemList[btnNumber].cost" will still hold a price as an integer value as if there were a 6th upgrade bar.
            float nextUpgradeBarPrice = arrayOfArrays[btnNumber][costIndex] + arrayOfArrays[btnNumber][costIndex] * multipliers[arrayOfArrays[btnNumber][nextUpgradeBarIndex]];
            arrayOfArrays[btnNumber][costIndex] = ConvertToMultiplesOf5(nextUpgradeBarPrice);
            itemInfoList[btnNumber].costTxt.text = arrayOfArrays[btnNumber][costIndex].ToString() + " Coins";

            // Point to the next upgrade bar (as an integer)
            arrayOfArrays[btnNumber][nextUpgradeBarIndex]++;

            // Check again which items the player can upgrade to
            checkIfUpgradeable();
        }
    }

    private int ConvertToMultiplesOf5(float number)
    {
        return Mathf.RoundToInt(number / 5f) * 5;
    }

}