using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private int playerCoins = 1000;
    public TextMeshProUGUI playerCoinsTxt;
    public CreateItem_SO[] SO_itemList;
    public ItemInfo[] itemInfoList;

    // Start is called before the first frame update
    void Start()
    {
        playerCoinsTxt.text = "Coins: " + playerCoins.ToString();
        LoadItems();
    }


    private void LoadItems()
    {
        for (int i = 0; i < SO_itemList.Length; i++)
        {
            ItemInfo currentItem = itemInfoList[i];

            currentItem.itemImg.sprite = SO_itemList[i].itemImg;
            currentItem.itemNameTxt.text = SO_itemList[i].itemName;
            currentItem.descriptionTxt.text = SO_itemList[i].description;
            currentItem.costTxt.text = SO_itemList[i].cost.ToString() + " Coins";
        }
    }
}
