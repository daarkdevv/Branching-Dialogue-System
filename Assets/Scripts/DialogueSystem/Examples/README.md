# Examples

Example implementations and sample dialogue setups.

## Contents

- **FirstDialogue.cs** — Main DialogueManager MonoBehaviour
  - Entry point for dialogue system
  - Manages dialogue flow and state
  - Integrates with ChoiceNavigator and DialogueUiManager

## Usage

Attach `FirstDialogue.cs` (DialogueManager) to a GameObject in your scene:

1. Assign dialogue nodes in the Inspector (`dialogueLines` array)
2. Assign UI components (dialogue text, choice texts, canvas)
3. Call `ControlDialogueFlow()` to start a conversation
4. Input is handled automatically via Update()

## Example Setup

```csharp
// In your scene
DialogueManager dialogueManager = gameObject.GetComponent<DialogueManager>();

// Start dialogue
StartCoroutine(dialogueManager.ControlDialogueFlow());

// Or bind to input
if (Input.GetKeyDown(KeyCode.O))
    StartCoroutine(dialogueManager.ControlDialogueFlow());
```

## Creating Your Own Example

1. Create a new prefab with the dialogue UI
2. Attach `DialogueManager` script
3. Create dialogue nodes (OneWayDialogue, MultiWayDialogue)
4. Link them in the Inspector
5. Test in Play mode
