
public class DialogueChoice
{
    public string choiceText;
    public DialogueLine nextLine;

    public DialogueChoice(string choicetext , DialogueLine next)
    {
        this.choiceText = choicetext;
        this.nextLine = next;
    }
   
}



