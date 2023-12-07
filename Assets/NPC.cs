using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPC : MonoBehaviour
{

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string[] dialogue;
    private int index;


    public GameObject button;

    public float wordSpeed;
    public bool isRange;

    public bool isType;

    void Start()

    {
        isType = false;
        dialogueText.text = "";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isRange)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                resetText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        if(dialogueText.text == dialogue[index])
        {
            button.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Return) && dialoguePanel.activeInHierarchy)
        {
            isType = true;
        }
    }
    public void resetText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    public void NextLine()
    {
        isType = false;
        button.SetActive(false);
        if(index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            resetText();
        }
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;

            if (isType)
            {
                dialogueText.text = dialogue[index];
                break;
            }
            isType = false;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isRange = false;
            resetText();
        }
    }
}
