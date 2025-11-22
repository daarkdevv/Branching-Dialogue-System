using TMPro;
using UnityEngine;

public class DialogueUiManager
{
    public GameObject dialogueCanvas;
    public TMP_Text dialogeTextUi;
    public TMP_Text[] choicesUiText;
    public TMP_Text pastChoiceUiText;

    public DialogueUiManager(GameObject dialgoueCanvas, TMP_Text dialogueUi, TMP_Text[] choices)
    {
        this.dialogueCanvas = dialgoueCanvas;
        this.dialogeTextUi = dialogueUi;
        this.choicesUiText = choices;

        
    }

    public void OnRecievingChar(TextDialogueTypeEnum textDialogueTypeEnum , string next , byte choiceNum)
    {
        if (textDialogueTypeEnum == TextDialogueTypeEnum.ChoiceText)
            UpdateChoiceText(next, choiceNum);
        else
            UpdateDialogueUiText(next);
    }

    public void UpdateDialogueUiText(string next)
    {
        dialogeTextUi.text = next;
    }


    public void UpdateChoiceText(string next, byte choiceNum)
    {
        choicesUiText[choiceNum].text = next;
    }
    
    public void ActivateOrDisableMainDialogue(bool isEnabled)
    {
        dialogueCanvas.SetActive(isEnabled);
    }

    public void HightlightChoice(byte choiceIndex)
    {
        if(pastChoiceUiText != null)
        pastChoiceUiText.color = Color.white;
            
        choicesUiText[choiceIndex].color = Color.yellow;
        pastChoiceUiText  = choicesUiText[choiceIndex];  
    }

}

