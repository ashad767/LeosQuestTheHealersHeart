using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;


[System.Serializable]
public class DialogueSet
{
    public string[] dialogues; 
}

public class CutsceneDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public DialogueSet[] dialogue;

    public MenuController menuController;
    private int dialogueSetIndex = 0;
    private int dialogueLineIndex = 0;
    private int totalDialogueSets;
    private int totalLinesInCurrentSet;

    public GameObject button;
    public float wordSpeed;

    public PlayableDirector director;
    private bool isDialogueStarted = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        button.SetActive(false);

        dialogueSetIndex = 0;
        dialogueLineIndex = 0;

        totalDialogueSets = dialogue.Length;
        totalLinesInCurrentSet = dialogue[dialogueSetIndex].dialogues.Length;
    }

    public void StopTimeline()
    {
        if (director != null)
        {
            director.Stop();
            EndDialogue();
            menuController.enablePlayerForCutscene();


        }
    }

    void Update()
    {
        
        //Debug.Log("totalDialogueSets: " + totalDialogueSets + " " + "totalLinesInCurrentSet: " + totalLinesInCurrentSet + this.gameObject.name);

        if (isDialogueStarted && dialogueText.text == dialogue[dialogueSetIndex].dialogues[dialogueLineIndex])
        {
            button.SetActive(true);
        }
    }

    public void StartTalking()
    {
        if (!isDialogueStarted)
        {
            isDialogueStarted = true;
            dialoguePanel.SetActive(true);
            dialogueLineIndex = 0;

            

            if (director != null && director.state == PlayState.Playing)
            {
                director.Pause();
            }
            StartCoroutine(Typing());
        }
    }

    public void NextLine()
    {
        button.SetActive(false);
        if (dialogueLineIndex < totalLinesInCurrentSet - 1)
        {
            dialogueLineIndex++;
            StartCoroutine(Typing());
        }
        else
        {
            Debug.Log("ending dialogue");
            EndDialogue();
        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[dialogueSetIndex].dialogues[dialogueLineIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void EndDialogue()
    {
        isDialogueStarted = false;
        dialoguePanel.SetActive(false);

        if (totalDialogueSets > 1)//more than 1 dialogues
        {
            dialogueSetIndex++;//prepare for next dialogue
            if (dialogueSetIndex >= totalDialogueSets)//if at the end of the dialogues in the set
            {
                dialogueSetIndex = 0;
            }
            totalLinesInCurrentSet = dialogue[dialogueSetIndex].dialogues.Length;//get the lines in new current set
        }

        if (director != null && director.state == PlayState.Paused)
        {
            director.Resume();
        }
    }
}
