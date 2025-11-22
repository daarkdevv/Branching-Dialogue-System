using System;

public class MultiWayDialogue : DialogueLine
{
    public DialogueChoice[] choices;

    public MultiWayDialogue(string text) : base(text)
    {     
    }
    
    public MultiWayDialogue(string text, string[] textChoices, params DialogueLine[] next) : base(text)
    {
        if (textChoices.Length != next.Length)
            throw new ArgumentException("Choices text and next dialogue lines count must match.");
        
        choices = new DialogueChoice[textChoices.Length];

        for (int i = 0; i < textChoices.Length; i++)
        {
            choices[i] = new DialogueChoice(textChoices[i], next[i]);
        }
    }

    public override DialogueLine GetNext(byte choiceNum = 0)
    {
        // Validate the array itself
        if (choices == null || choiceNum >= choices.Length)
            return null;

        // Validate the specific choice element
        if (choices[choiceNum].nextLine == null) 
            return null;

        return choices[choiceNum].nextLine;
    }

    public string[] GetChoicesText()
    {
        if (choices == null)
            return null;

        string[] strings = new string[choices.Length];

        for (int i = 0; i < choices.Length; i++)
        {
            strings[i] = choices[i].choiceText;
        }

        return strings; 
    }
}
