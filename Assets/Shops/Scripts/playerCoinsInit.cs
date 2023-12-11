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
        playerCoinsTxt.text = "Coins: " + player.coins.ToString();
    }

    void Update()
    {
        playerCoinsTxt.text = "Coins: " + player.coins.ToString();
    }

}
