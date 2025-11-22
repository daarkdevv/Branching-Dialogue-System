using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "Dialogue/Dialogue Line")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue Info")]
    public string speaker;

    [TextArea(3, 6)]
    public string text;

    [Header("Dialogue Flow")]
    public DialogueLine nextDialogueLine;

}



