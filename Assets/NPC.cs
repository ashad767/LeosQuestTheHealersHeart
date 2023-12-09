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

    public string[] randomDialogue;
    public int timesTalked;

    private int index;


    public GameObject button;

    public float wordSpeed;
    public bool isRange;

    public bool isCurrentlyTalking;

    public bool isType;

    void Start()

    {
        isType = false;
        timesTalked = 0;
        isCurrentlyTalking = false;
        dialogueText.text = "";
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.E) && isRange && timesTalked < 1 && !isCurrentlyTalking)
        {
            isType = false;
            isCurrentlyTalking = true;

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
        else if(Input.GetKeyDown(KeyCode.E) && isRange && timesTalked > 0 && !isCurrentlyTalking)
        {
            isCurrentlyTalking = true;
            if (dialoguePanel.activeInHierarchy)
            {
                resetText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(RandomTyping());
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
        timesTalked++;
        isCurrentlyTalking = false;
    }

    public void NextLine()
    {
        isType = false;
        isCurrentlyTalking=true;
        button.SetActive(false);
        if (timesTalked <1)
        {
            if (index < dialogue.Length - 1)
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
        else if(timesTalked > 0)
        {
            resetText();


        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;

            if (isType && timesTalked < 1)
            {
                dialogueText.text = dialogue[index];
                break;
            }
            
            yield return new WaitForSeconds(wordSpeed);
        }
        isType = false;
    }

    IEnumerator RandomTyping()
    {
        int randomPhrase = Random.Range(0, randomDialogue.Length);
        dialogueText.text = "";
        foreach (char letter in randomDialogue[randomPhrase].ToCharArray())
        {
            dialogueText.text += letter;

            if (isType)
            {
                dialogueText.text = randomDialogue[randomPhrase];
                break;
            }
            
            yield return new WaitForSeconds(wordSpeed);
        }
        button.SetActive(true);
        isCurrentlyTalking = false;
        isType = false;
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
