using System;
using System.Diagnostics;

public abstract class DialogueLine
{
    public string speaker { get; private set; }
    public string text { get; private set; }

    public DialogueLine(string text)
    {
        this.text = text;
    }

    public virtual DialogueLine GetNext(byte choiceNum = 0)
    {
        return null;
    }
}


