using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject E_Key;
    [SerializeField] private GameObject toggleShop;
    
    private bool isPlayerInside = false;

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            toggleShop.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInside = true;
            E_Key.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInside = false;
            E_Key.SetActive(false);
        }
    }

    // Used by the onClick() function when pressing the "return" button
    public void Return()
    {
        toggleShop.SetActive(false);
        Time.timeScale = 1f;
    }
}
