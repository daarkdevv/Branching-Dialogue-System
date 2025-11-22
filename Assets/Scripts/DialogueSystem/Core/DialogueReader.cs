
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueReader
{
    public StringBuilder reader;
    float readingSpeed;

    public event Action<TextDialogueTypeEnum, string , byte> OnReadingChar; 

    public DialogueReader(float readingSpeed)
    {
        reader = new();
        this.readingSpeed = readingSpeed;
    }

    public async Task ReadDialogue(DialogueLine currentDialogueLine , CancellationToken token = default)
    {

        string dialogueTextSequence = currentDialogueLine.text;

        await ReadTask(TextDialogueTypeEnum.DialogueText,dialogueTextSequence, token);

    }

    public async Task ReadChoice(float coolDown , string choice , byte choiceNum , CancellationToken token = default)
    {
        await ReadTask(TextDialogueTypeEnum.ChoiceText,choice ,token ,choiceNum);
        await Awaitable.WaitForSecondsAsync(coolDown);
    }
    
   public async Task ReadTask(TextDialogueTypeEnum textDialogueType, string ToRead, CancellationToken token, byte choiceNum = 0)
{
    int index = reader.Length; // Start from where the buffer left off
    
    // 1. Loop through remaining characters
    while (index < ToRead.Length)
    {
        
        try
        {
            reader.Append(ToRead[index]);
            OnReadingChar?.Invoke(textDialogueType, reader.ToString(), choiceNum);
            
            index++;
            await Awaitable.WaitForSecondsAsync(readingSpeed, token);
        }
        catch (OperationCanceledException)
        {
            // If the wait itself was cancelled, run the skip logic outside the loop.
            // In the provided solution above, we handle it inside the loop for simplicity.
            Reset();
            OnReadingChar?.Invoke(textDialogueType, ToRead, choiceNum); 
            break; 
        }
    }
    
    Reset();
     
  }

  
    void Reset()
    {
        reader.Clear();
    }
}



