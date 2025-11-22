using System;

public class OneWayDialogue : DialogueLine
{
    private DialogueLine nextDialogue;

    public OneWayDialogue(string text) : base(text)
    {
    }

    public OneWayDialogue(string text, DialogueLine next) : base(text)
    {
        this.nextDialogue = next;
    }

    public override DialogueLine GetNext(byte choiceNum = 0)
    {
        return nextDialogue;
    }
}
