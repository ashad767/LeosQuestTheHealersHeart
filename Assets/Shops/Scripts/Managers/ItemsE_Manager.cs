using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsE_Manager : MonoBehaviour
{
    [SerializeField] private Player player;
    public TextMeshProUGUI playerCoinsTxt;

    public CreateItems_SO[] SO_itemList;
    public ItemInfo_Items[] itemInfoList;
    public Button[] costBtns;

    private readonly string key = "IE_";

    // Track if player purchased item
    private int[] purchasedTracker = new int[15];


    // Start is called before the first frame update
    void Start()
    {
        // Uncomment this to reset the shop data
        Reset();

        LoadItems();
    }

    private void Reset()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            PlayerPrefs.SetInt(key + i.ToString(), 0);
        }
    }

    // Used by the onClick() function when pressing the "return" button
    public void SaveData()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            PlayerPrefs.SetInt(key + i.ToString(), purchasedTracker[i]);
        }

        PlayerPrefs.Save();
    }

    private void LoadItems()
    {
        for (int item = 0; item < SO_itemList.Length; item++)
        {
            ItemInfo_Items currentItem = itemInfoList[item];

            currentItem.itemImg.sprite = SO_itemList[item].itemImg;
            currentItem.itemNameTxt.text = SO_itemList[item].itemName;

            // If the player didn't buy the item
            if (PlayerPrefs.GetInt(key + item.ToString()) == 0)
            {
                int cost = SO_itemList[item].itemCost;
                if (player.coins >= cost)
                {
                    currentItem.itemCostTxt.text = cost.ToString();
                }
                else
                {
                    Insufficient(currentItem, item);
                }
            }

            // If the player DID buy the item
            else
            {
                triggerPurchased(currentItem, item);
            }

        }
    }

    private void triggerPurchased(ItemInfo_Items currentItem, int item)
    {
        GameObject costBtn = currentItem.transform.GetChild(2).gameObject;
        Destroy(costBtn.transform.GetChild(0).gameObject);

        ChangeBtnConfigs(2, currentItem.itemCostTxt, 2.98f, 90.88f, 8.6f);

        costBtns[item].interactable = false;
        // Convert the hexadecimal color to a Color object
        Color disabledColor = HexToColor("#FF0006");

        // Get the current colors
        ColorBlock colors = costBtns[item].colors;

        // Set the disabled color
        colors.disabledColor = disabledColor;
        costBtns[item].colors = colors;

        purchasedTracker[item] = 1;
    }

    private void Insufficient(ItemInfo_Items currentItem, int item)
    {
        ChangeBtnConfigs(1, currentItem.itemCostTxt, 14.45f, 78.8236f, 5.3f);

        costBtns[item].interactable = false;
        // Convert the hexadecimal color to a Color object
        Color disabledColor = HexToColor("#FF0006");

        // Get the current colors
        ColorBlock colors = costBtns[item].colors;

        // Set the disabled color
        colors.disabledColor = disabledColor;
        costBtns[item].colors = colors;
    }

    // Convert a hexadecimal color string to a Color object
    Color HexToColor(string hex)
    {
        Color color = Color.black;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    private void ChangeBtnConfigs(int btn, TextMeshProUGUI itemCostTxt, float xPos, float xWidth, float fontSize)
    {
        if (btn == 0)
        {
            itemCostTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(9.95f, 0f);
            itemCostTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(90.6f, 39.1646f);
            itemCostTxt.fontSize = 14f;
        }
        else if (btn == 1)
        {
            itemCostTxt.text = "Insufficient Coins";
            itemCostTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0f);
            itemCostTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(xWidth, 39.1646f);
            itemCostTxt.fontSize = fontSize;
        }

        else if (btn == 2)
        {
            itemCostTxt.text = "Purchased";
            itemCostTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0f);
            itemCostTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(xWidth, 39.1646f);
            itemCostTxt.fontSize = fontSize;
        }
    }

    // Used by the onClick() function in the Inspector window
    public void BuyItem(int item)
    {
        ItemInfo_Items currentItem = itemInfoList[item];
        int itemCost = SO_itemList[item].itemCost;

        if (player.RemoveCoins(itemCost))
        {
            // Decrease the player's coins (top right corner)
            playerCoinsTxt.text = "Coins: " + player.coins.ToString();

            triggerPurchased(currentItem, item);

            checkIfPurchaseable();
        }
    }

    public void checkIfPurchaseable()
    {
        for (int item = 0; item < SO_itemList.Length; item++)
        {
            ItemInfo_Items currentItem = itemInfoList[item];
            int cost = SO_itemList[item].itemCost;

            if (purchasedTracker[item] != 1)
            {
                if (player.coins >= cost)
                {
                    currentItem.itemCostTxt.text = cost.ToString();
                    
                    ChangeBtnConfigs(0, currentItem.itemCostTxt, 9.95f, 90.6f, 14f);

                    costBtns[item].interactable = true;
                }
                else
                {
                    Insufficient(currentItem, item);
                }
            }

        }
    }
}
