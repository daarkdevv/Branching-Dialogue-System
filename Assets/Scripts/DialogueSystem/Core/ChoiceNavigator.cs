using System;
using System.Threading.Tasks;
using UnityEngine;

public class ChoiceNavigator
{
    private int choiceColumns = 2; // 2-column grid like Undertale
    private int totalChoices = 0;
    private int currentChoiceIndex = 0;
    
    private TaskCompletionSource<bool> choiceSelectionTCS;
    private byte lastSelectedChoice = 0;
    
    public event Action<byte> OnChoiceHighlighted; // Fires when highlighting changes
    public event Action<byte> OnChoiceConfirmed;   // Fires when choice is confirmed

    /// <summary>
    /// Initialize the choice navigator for a given number of choices.
    /// </summary>
    public void Initialize(int numChoices)
    {
        totalChoices = numChoices;
        currentChoiceIndex = 0;
        choiceSelectionTCS = new TaskCompletionSource<bool>();
        
        // Highlight the first choice
        OnChoiceHighlighted?.Invoke((byte)currentChoiceIndex);
        Debug.Log($"[ChoiceNavigator] Initialized with {totalChoices} choices. Starting at index 0.");
    }

    /// <summary>
    /// Await the player's choice selection.
    /// </summary>
    public async Task<byte> GetChoiceAsync()
    {
        Debug.Log("[ChoiceNavigator] Awaiting player choice...");
        await choiceSelectionTCS.Task;
        Debug.Log($"[ChoiceNavigator] Choice confirmed: {lastSelectedChoice}");
        return lastSelectedChoice;
    }

    /// <summary>
    /// Handle UP arrow input - move up one row.
    /// </summary>
    public void HandleUpInput()
    {
        currentChoiceIndex -= choiceColumns;
        if (currentChoiceIndex < 0)
            currentChoiceIndex += totalChoices;
        OnChoiceHighlighted?.Invoke((byte)currentChoiceIndex);
        Debug.Log($"[ChoiceNavigator] UP: Now at index {currentChoiceIndex}");
    }

    /// <summary>
    /// Handle DOWN arrow input - move down one row.
    /// </summary>
    public void HandleDownInput()
    {
        currentChoiceIndex += choiceColumns;
        if (currentChoiceIndex >= totalChoices)
            currentChoiceIndex -= totalChoices;
        OnChoiceHighlighted?.Invoke((byte)currentChoiceIndex);
        Debug.Log($"[ChoiceNavigator] DOWN: Now at index {currentChoiceIndex}");
    }

    /// <summary>
    /// Handle LEFT arrow input - move left within row.
    /// </summary>
    public void HandleLeftInput()
    {
        int row = currentChoiceIndex / choiceColumns;
        int col = currentChoiceIndex % choiceColumns;
        col = (col - 1 + choiceColumns) % choiceColumns;
        currentChoiceIndex = row * choiceColumns + col;
        if (currentChoiceIndex >= totalChoices)
            currentChoiceIndex = totalChoices - 1;
        OnChoiceHighlighted?.Invoke((byte)currentChoiceIndex);
        Debug.Log($"[ChoiceNavigator] LEFT: Now at index {currentChoiceIndex}");
    }

    /// <summary>
    /// Handle RIGHT arrow input - move right within row.
    /// </summary>
    public void HandleRightInput()
    {
        int row = currentChoiceIndex / choiceColumns;
        int col = currentChoiceIndex % choiceColumns;
        col = (col + 1) % choiceColumns;
        currentChoiceIndex = row * choiceColumns + col;
        if (currentChoiceIndex >= totalChoices)
            currentChoiceIndex = totalChoices - 1;
        OnChoiceHighlighted?.Invoke((byte)currentChoiceIndex);
        Debug.Log($"[ChoiceNavigator] RIGHT: Now at index {currentChoiceIndex}");
    }

    /// <summary>
    /// Handle ENTER/Return input - confirm the current choice.
    /// </summary>
    public void HandleConfirmInput()
    {
        lastSelectedChoice = (byte)currentChoiceIndex;
        choiceSelectionTCS?.TrySetResult(true);
        OnChoiceConfirmed?.Invoke(lastSelectedChoice);
        Debug.Log($"[ChoiceNavigator] ENTER: Confirmed choice {lastSelectedChoice}");
    }

    /// <summary>
    /// Check if currently awaiting choice input.
    /// </summary>
    public bool IsAwaitingChoice => choiceSelectionTCS != null && !choiceSelectionTCS.Task.IsCompleted;

    /// <summary>
    /// Get the currently highlighted choice index.
    /// </summary>
    public byte GetCurrentChoiceIndex() => (byte)currentChoiceIndex;
}
