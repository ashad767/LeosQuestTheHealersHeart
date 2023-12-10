using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesWeaponsManager : MonoBehaviour
{
    [SerializeField] private Player player;
    public TextMeshProUGUI playerCoinsTxt;

    public CreateUpgrades_SO[] SO_itemList;
    public ItemInfo_UpgradesWeapons[] itemInfoList;
    public Button[] upgradeBtns;

    private readonly string[] desc = { "Basic", "Intermediate", "Advanced", "Expert" };
    private readonly float[] multipliers = { 1.5f, 2f, 3f };
    private readonly string key = "UW_";

    #region Images
    // sword: 9, 45, 111, 136
    // bow: 514, 551, 619, 522
    // magic: 256, 322, 355, 259
    [SerializeField] private Sprite[] sprites;
    private Sprite[][] spritesInit;
    #endregion

    #region pointer-to-next-upgrade-bar ; cost
    // Declare a jagged array of int arrays {pointer-to-next-upgrade-bar, cost}
    private int[][] upgradeItems_PointerCost_Pairs = new int[3][]
    {
        new int[] { 0, 100 },
        new int[] { 0, 200 },
        new int[] { 0, 250 },
    };

    private int nextUpgradeBarIndex = 0;
    private int costIndex = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        spritesInit = new Sprite[3][]
        {
            new Sprite[] { sprites[0], sprites[1], sprites[2], sprites[3] },
            new Sprite[] { sprites[4], sprites[5], sprites[6], sprites[7] },
            new Sprite[] { sprites[8], sprites[9], sprites[10], sprites[11] }
        };

        // Uncomment this to reset the shop data
        //Reset();

        LoadItems();
        checkIfUpgradeable();
    }

    private void Reset()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            PlayerPrefs.SetString(key + i.ToString(), "");
        }
    }

    // Used by the onClick() function when pressing the "return" button
    public void SaveData()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            string[] pointer_and_cost = new string[2];
            pointer_and_cost[0] = upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex].ToString();
            pointer_and_cost[1] = upgradeItems_PointerCost_Pairs[i][costIndex].ToString();

            PlayerPrefs.SetString(key + i.ToString(), string.Join(",", pointer_and_cost));
        }

        PlayerPrefs.Save();
    }

    private void LoadItems()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            ItemInfo_UpgradesWeapons currentItem = itemInfoList[i];
            currentItem.itemNameTxt.text = SO_itemList[i].itemName;


            // If there are already data values from previous interaction with the shop, initialize them here
            if (PlayerPrefs.GetString(key + i.ToString()) != "")
            {
                string[] pointer_and_cost = PlayerPrefs.GetString(key + i.ToString()).Split(",");
                upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex] = int.Parse(pointer_and_cost[nextUpgradeBarIndex]);
                upgradeItems_PointerCost_Pairs[i][costIndex] = int.Parse(pointer_and_cost[costIndex]);

                currentItem.itemImg.sprite = spritesInit[i][upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex]]; // might go outOfBounds
                currentItem.descriptionTxt.text = desc[upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex]];
            }

            // If this is the first time the player interacts with the shop, initialize the data with the default values
            else
            {
                currentItem.itemImg.sprite = spritesInit[i][0];
                currentItem.descriptionTxt.text = desc[0];
                upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex] = upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex];
                upgradeItems_PointerCost_Pairs[i][costIndex] = upgradeItems_PointerCost_Pairs[i][costIndex];
            }

            InitializeUpgradeBars(i);

            if (upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex] <= SO_itemList.Length - 1)
            {
                currentItem.costTxt.text = upgradeItems_PointerCost_Pairs[i][costIndex].ToString() + " Coins";
            }
        }
    }

    private void InitializeUpgradeBars(int item)
    {
        // Before this for-loop, when the player would upgrade an item(s) and leave and then open the shop again, the upgrade bars would all be black.
        // Then if the player were to upgrade that item(s) again, the upgrade bars would start turning green from the value of "pointerToNextUpgradeBar", but leaving all the bars before it black.
        // This for-loop is to initialize the upgrade bars and check which upgrade bars are already green and display it from the start of the scene.
        // It iterates over all the upgrade bars of the specific shop item passed as an argument from LoadItems() named "item".
        for (int upgradeBar = 0; upgradeBar < upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex]; upgradeBar++)
        {
            itemInfoList[item].upgradeBarsList[upgradeBar].color = Color.green;
        }
    }

    public void checkIfUpgradeable()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            Button currentUpgradeBtn = upgradeBtns[i];

            if (upgradeItems_PointerCost_Pairs[i][nextUpgradeBarIndex] > SO_itemList.Length - 1)
            {
                currentUpgradeBtn.interactable = false;
                currentUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "<s><i>Maxed Out</i></s>";

                itemInfoList[i].costTxt.text = "";
            }

            else
            {
                if (player.coins >= upgradeItems_PointerCost_Pairs[i][costIndex])
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
    public void Upgrade(int item)
    {
        int itemCost = upgradeItems_PointerCost_Pairs[item][costIndex];

        if (player.RemoveCoins(itemCost))
        {
            // Decrease the player's coins (top right corner)
            playerCoinsTxt.text = "Coins: " + player.coins.ToString();

            // Changes the color of the upgrade bar that the "pointerToNextUpgradeBar" index is pointing to, to green
            itemInfoList[item].upgradeBarsList[upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex]].color = Color.green;

            // Increases upgrade bar price for next item upgrade bar (STARTING FROM 2ND BAR SINCE FIRST BAR IS ALREADY SET AS THE BASE COST IN "CreateItem_SO.cs").
            // So for this, when the player upgrades to the last bar (and thus all 5 bars turn green), this will run and display the cost, but then immediately be an empty string because of the call to "checkIfUpgradeable()".
            // NOTE: for "multipliers[upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex]]", "nextUpgradeBarIndex" is for the bar that just turned green from line above
            itemCost = ConvertToMultiplesOf5(itemCost + (itemCost * multipliers[upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex]]));
            upgradeItems_PointerCost_Pairs[item][costIndex] = itemCost;
            itemInfoList[item].costTxt.text = itemCost.ToString() + " Coins";

            // Point to the next upgrade bar (as an integer)
            upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex]++;

            int nextBar = upgradeItems_PointerCost_Pairs[item][nextUpgradeBarIndex];
            itemInfoList[item].itemImg.sprite = spritesInit[item][nextBar];
            itemInfoList[item].descriptionTxt.text = desc[nextBar];

            // Check again which items the player can upgrade to
            checkIfUpgradeable();
        }
    }

    private int ConvertToMultiplesOf5(float number)
    {
        return Mathf.RoundToInt(number / 5f) * 5;
    }
}
