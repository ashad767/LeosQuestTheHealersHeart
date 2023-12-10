using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsW_Manager : MonoBehaviour
{
    [SerializeField] private Player player;
    public TextMeshProUGUI playerCoinsTxt;

    public CreateItems_SO[] SO_itemList;
    public ItemInfo_Items[] itemInfoList;
    public Button[] costBtns;

    private readonly string key = "IW_";

    // Track if player purchased item
    private int[] purchasedTracker = new int[15];


    // Start is called before the first frame update
    void Start()
    {
        // Uncomment this to reset the shop data
        //Reset();

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
                if(player.coins >= cost)
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
        
        currentItem.itemCostTxt.text = "Purchased";
        currentItem.itemCostTxt.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // moves the "Purchased" text to the middle after removing coins img
        currentItem.itemCostTxt.fontSize = 20.4f;

        purchasedTracker[item] = 1;
        costBtns[item].interactable = false;
    }

    private void Insufficient(ItemInfo_Items currentItem, int item)
    {
        currentItem.itemCostTxt.text = "Insufficient Funds";
        currentItem.itemCostTxt.fontSize = 16;
        currentItem.itemCostTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(15f, 0f);
        costBtns[item].interactable = false;
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
            
            if(purchasedTracker[item] != 1)
            {
                if (player.coins >= cost)
                {
                    currentItem.itemCostTxt.text = cost.ToString();
                    currentItem.itemCostTxt.fontSize = 20.4f;
                    currentItem.itemCostTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(4.4349f, 0f);

                }
                else
                {
                    Insufficient(currentItem, item);
                }
            }
            
        }
    }

}
