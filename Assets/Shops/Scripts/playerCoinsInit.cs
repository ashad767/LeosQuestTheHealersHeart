using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerCoinsInit : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI playerCoinsTxt;

    // Start is called before the first frame update
    void Awake()
    {
        player.AddCoins(1500); // delete this later
        playerCoinsTxt.text = "Coins: " + player.coins.ToString();
    }

}
