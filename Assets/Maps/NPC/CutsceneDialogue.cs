using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables; 
public class CutsceneDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string[] dialogue;

    private int index;
    public GameObject button;
    public float wordSpeed;

    public PlayableDirector director;
    private bool isDialogueStarted = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        button.SetActive(false);
    }

    void Update()
    {
        if (isDialogueStarted)
        {
            if (dialogueText.text == dialogue[index])
            {
                button.SetActive(true);
            }
        }
    }

    public void startTalking()
    {
        if (!isDialogueStarted)
        {
            isDialogueStarted = true;
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());

            if (director != null && director.state == PlayState.Playing)
            {
                director.Pause(); // Pause the Timeline
            }
        }
    }

    public void NextLine()
    {
        button.SetActive(false);
        if (index < dialogue.Length - 1)
        {
            index++;
            StartCoroutine(Typing());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void EndDialogue()
    {
        isDialogueStarted = false;
        dialoguePanel.SetActive(false);

        if (director != null && director.state == PlayState.Paused)
        {
            director.Resume(); // Resume the Timeline
        }
    }
}
