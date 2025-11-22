# Dialogue System

A flexible, event-driven dialogue manager for branching conversations with arrow-key-based choice navigation (Undertale-style). Supports linear and branching dialogue trees with async/await patterns for smooth state integration.

**Status:** 🚧 **In Progress** — Core features implemented; UI polish and edge cases pending.

## Overview

This dialogue system enables:
- **Linear Dialogues** — Single path conversations that auto-advance
- **Branching Dialogues** — Player-selected choices with multi-path storytelling
- **Arrow Key Navigation** — 2-column grid choice selection with Enter to confirm
- **Async Flow** — Integrates seamlessly with the HFSM for state transitions
- **Event-Driven Input** — Decoupled from gameplay input via polling
- **UI Integration** — Dialogue and choice text rendered via TextMeshPro

## Architecture

### Core Components

#### `DialogueManager` (orchestrator)
Main controller for dialogue flow. Manages dialogue sequences, choice navigation, and state transitions.

```csharp
public class DialogueManager : MonoBehaviour
{
    public DialogueLine[] dialogueLines;      // array of dialogue nodes (scriptable objects)
    public DialogueUiManager dialogueUi;     // UI handler
    
    private ChoiceNavigator choiceNavigator;
    private StateMachine stateMachine;
    private PlayerContext ctx;
    
    public async Task ControlDialogueFlow();
    public void Update();                     // input polling
}
```

**Key methods:**
- `ControlDialogueFlow()` — main loop; iterates through dialogue nodes and awaits choice selection
- `Update()` — polls arrow/Enter keys and delegates to `ChoiceNavigator`
- `ReadDialogue()` — displays dialogue text and waits for auto-advance
- Private methods delegate choice display to `DialogueUiManager`

#### `ChoiceNavigator` (choice input handler)
Encapsulates choice selection logic: navigation, input detection, event firing. Exposes an async method to await player choice.

```csharp
public class ChoiceNavigator
{
    private int currentChoiceIndex;
    private int numChoices;
    private int columnsPerRow;  // hard-coded to 2
    
    public bool IsAwaitingChoice { get; private set; }
    
    public event Action<int> OnChoiceHighlighted;
    public event Action<int> OnChoiceConfirmed;
    
    public async Task<int> GetChoiceAsync(int choiceCount);
    public void HandleUpInput();
    public void HandleDownInput();
    public void HandleLeftInput();
    public void HandleRightInput();
    public void HandleConfirmInput();
}
```

**Key methods:**
- `GetChoiceAsync(choiceCount)` — returns a Task<int> that resolves when player confirms a choice
- `Handle*Input()` — process arrow/confirm keys; update highlight and fire events
- `IsAwaitingChoice` — property indicating whether player input is being polled

**Grid Navigation Logic (2-column):**
```
Choice 0 | Choice 1
Choice 2 | Choice 3
Choice 4 | Choice 5

UP/DOWN:    move between rows
LEFT/RIGHT: toggle between columns
ENTER:      confirm current choice
```

#### `DialogueLine` (base, abstract)
Abstract base for all dialogue node types. Serialized as ScriptableObjects.

```csharp
public abstract class DialogueLine : ScriptableObject
{
    [TextArea(3, 5)]
    public string dialogue;
    
    public abstract void Execute();
}
```

**Subclasses:**

##### `OneWayDialogue`
Linear dialogue node with automatic advance to next node.

```csharp
public class OneWayDialogue : DialogueLine
{
    public DialogueLine nextDialogue;
    
    public override void Execute()
    {
        // In ControlDialogueFlow: display text, wait, advance to nextDialogue
    }
}
```

##### `MultiWayDialogue`
Branching dialogue node with player-selectable choices.

```csharp
public class MultiWayDialogue : DialogueLine
{
    [System.Serializable]
    public class DialogueChoice
    {
        [TextArea(2, 3)]
        public string choiceText;
        public DialogueLine resultingDialogue;
    }
    
    public DialogueChoice[] choices;
    
    public string[] GetChoicesText();
    
    public override void Execute()
    {
        // In ControlDialogueFlow: display choices, await ChoiceNavigator, advance to resultingDialogue
    }
}
```

#### `DialogueUiManager` (UI renderer)
Manages on-screen dialogue and choice display. Updates when `ChoiceNavigator` fires highlight/confirm events.

```csharp
public class DialogueUiManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text[] choiceTexts;  // typically 4–6 choices
    public Image[] choiceButtons;
    
    public void DisplayDialogue(string text);
    public void DisplayChoices(string[] texts);
    public void HideChoices();
    public void HighlightChoice(int index);
}
```

**Key methods:**
- `DisplayDialogue(text)` — show dialogue in main text box
- `DisplayChoices(texts)` — populate choice buttons with text
- `HighlightChoice(index)` — update visual state (color/outline/scale) of chosen option
- Called by `DialogueManager` during flow and by `ChoiceNavigator.OnChoiceHighlighted` event

### Data Flow

```
Dialogue Node (SO)
    ↓
DialogueManager.ControlDialogueFlow()
    ↓
OneWayDialogue: DisplayDialogue() → Auto-advance
MultiWayDialogue: DisplayChoices() → Await ChoiceNavigator
    ↓
ChoiceNavigator.GetChoiceAsync()
    ↓
DialogueManager.Update() polls arrow/Enter keys
    ↓
ChoiceNavigator.Handle*Input() fires OnChoiceHighlighted
    ↓
DialogueUiManager.HighlightChoice() updates UI
    ↓
ChoiceNavigator.HandleConfirmInput() fires OnChoiceConfirmed, resolves Task
    ↓
ControlDialogueFlow() resumes, advances to next node
```

## Runtime Flow

### Dialogue Sequence Example

```csharp
// In DialogueManager
public async Task ControlDialogueFlow()
{
    for (int i = 0; i < dialogueLines.Length; i++)
    {
        DialogueLine currentLine = dialogueLines[i];
        
        // Display text and wait
        await ReadDialogue(currentLine);
        
        // If branching, await choice
        if (currentLine is MultiWayDialogue mwd)
        {
            choiceNavigator.Initialize(mwd.choices.Length);
            int selectedIndex = await choiceNavigator.GetChoiceAsync(mwd.choices.Length);
            
            // Advance to selected path
            dialogueLines = new[] { mwd.choices[selectedIndex].resultingDialogue };
            i = -1;  // reset loop
        }
    }
}
```

### Frame-by-Frame Input Handling

```csharp
public void Update()
{
    if (!choiceNavigator.IsAwaitingChoice)
        return;
    
    if (Input.GetKeyDown(KeyCode.UpArrow))
        choiceNavigator.HandleUpInput();
    else if (Input.GetKeyDown(KeyCode.DownArrow))
        choiceNavigator.HandleDownInput();
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
        choiceNavigator.HandleLeftInput();
    else if (Input.GetKeyDown(KeyCode.RightArrow))
        choiceNavigator.HandleRightInput();
    else if (Input.GetKeyDown(KeyCode.Return))
        choiceNavigator.HandleConfirmInput();
}
```

## Usage

### Creating a Dialogue Tree

1. **Create dialogue nodes as ScriptableObjects:**

   *Right-click in Assets → Create → Dialogue → OneWayDialogue*

2. **Build the tree:**

   ```
   Start Dialogue (OneWayDialogue)
        ↓
   "What would you like?" (MultiWayDialogue)
        ├→ Choice A → Continue Path A (OneWayDialogue)
        └→ Choice B → Continue Path B (OneWayDialogue)
   ```

3. **Assign to DialogueManager:**

   ```csharp
   public DialogueLine[] dialogueLines = new DialogueLine[]
   {
       startDialogue,  // OneWayDialogue
       choicePoint,    // MultiWayDialogue
       // Note: path branching handled in ControlDialogueFlow()
   };
   ```

### Triggering Dialogue

```csharp
// From PlayerStateDriver or any manager
void OnOpenDialogueKeyPressed()
{
    StartCoroutine(DialogueManager.ControlDialogueFlow());
}
```

Or bind to input:

```csharp
public void Start()
{
    InputSubscriber.Instance.SubscribeToInputEvent(
        InputType.Interact,
        () => StartCoroutine(DialogueManager.ControlDialogueFlow())
    );
}
```

### Creating a Custom Dialogue Type

```csharp
[CreateAssetMenu(menuName = "Dialogue/ConditionalDialogue")]
public class ConditionalDialogue : DialogueLine
{
    public DialogueLine ifTrue;
    public DialogueLine ifFalse;
    
    public System.Func<bool> condition;
    
    public override void Execute()
    {
        // In ControlDialogueFlow:
        // if (condition()) { nextNode = ifTrue; }
        // else { nextNode = ifFalse; }
    }
}
```

## Project Structure

```
Assets/Scripts/DialogueSystem/
├── Core/
│   ├── ChoiceNavigator.cs       (choice input & 2-column grid navigation)
│   ├── DialogueReader.cs        (text display & typewriter logic)
│   └── README.md
├── Data/
│   ├── DialogueLine.cs          (abstract base class)
│   ├── OneWayDialogue.cs        (linear dialogue nodes)
│   ├── MultiWayDialogue.cs      (branching dialogue nodes)
│   ├── DialogueChoice.cs        (choice data structure)
│   ├── DialogueLineSO.cs        (ScriptableObject wrapper)
│   ├── TextDialogueTypeEnum.cs  (dialogue type enum)
│   └── README.md
├── UI/
│   ├── DialogueUiManager.cs     (UI rendering & highlight)
│   └── README.md
├── Examples/
│   ├── FirstDialogue.cs         (DialogueManager example)
│   └── README.md
└── DIALOGUE_SYSTEM_README.md    (main documentation)
```

**Folder Overview:**
- **Core/** — Runtime dialogue flow management (ChoiceNavigator, DialogueReader)
- **Data/** — Dialogue node definitions and data structures (OneWayDialogue, MultiWayDialogue, DialogueChoice)
- **UI/** — User interface rendering and visual feedback (DialogueUiManager)
- **Examples/** — Sample implementations and usage patterns (DialogueManager)

## Current Implementation Status

### ✅ Completed

- **Core Flow** — `DialogueManager.ControlDialogueFlow()` iterates through nodes and branches
- **Choice Navigator** — Arrow key navigation with 2-column grid, Enter to confirm
- **OneWay Nodes** — Auto-advance to next dialogue
- **MultiWay Nodes** — Display choices, await selection, advance to choice result
- **Input Polling** — `Update()` frame-by-frame polling of arrow/Enter keys
- **Event System** — `OnChoiceHighlighted` and `OnChoiceConfirmed` events
- **Basic UI** — Text display and choice button management

### 🚧 In Progress

- **UI Polish** — Choice highlight animation and visual feedback refinement
- **Auto-Advance Timing** — Configurable delay or manual "next" key
- **Dialogue Box Animations** — Fade in/out, text scroll effects
- **Sound Integration** — Audio cues for choice selection
- **Typewriter Effect** — Text appearing character-by-character
- **Choice Button Scaling** — Visual emphasis on current highlight
- **Edge Cases** — Empty dialogue handling, missing node references

### 📋 Planned

- **Dialogue Tags** — Color text, embed speaker names, render portraits
- **Localization** — Multi-language support via CSV/JSON
- **Dialogue History** — Replay or reference previous conversations
- **Conditional Branches** — `ConditionalDialogue` with player state checks
- **Quest Integration** — Mark quests as started/completed via dialogue
- **Persistence** — Save/load dialogue progression and choices
- **Visualization Tool** — Unity Editor window to preview dialogue tree structure

## Known Issues & Workarounds

| Issue | Status | Workaround |
|-------|--------|-----------|
| Choice highlight not always visible | 🚧 In Progress | Manually adjust choice button color in `DialogueUiManager.HighlightChoice()` |
| No typewriter effect | ⏳ Planned | Use `DialogueReader.DisplayTextWithDelay()` method (half-implemented) |
| Multi-line choice text overflows | 🚧 In Progress | Adjust `TMP_Text` layout group and button size constraints |
| Rapid key presses skip choices | ⚠️ Minor | Add input debounce delay in `ChoiceNavigator.Handle*Input()` |
| Dialogue persists after state change | 🚧 In Progress | Call `DialogueManager.StopDialogue()` on state exit |

## Performance & Optimization

- **Memory** — Dialogue nodes are ScriptableObjects (pooled in memory); no per-frame allocations during display
- **Input** — Single `Input.GetKeyDown()` per key per frame; efficient polling
- **UI** — Text and button updates only when choices highlight; no constant redraw
- **Async** — `TaskCompletionSource` used for choice await; no coroutines in main flow

**Optimization opportunities:**
- Object pool choice buttons if > 6 choices
- Cache `TMP_Text` component references
- Use `OnGUI()` instead of `Input.GetKeyDown()` for faster input detection

## Extending the System

### Add a New Dialogue Type

1. Create a new class inheriting from `DialogueLine`
2. Implement `Execute()` (can be empty; logic lives in `ControlDialogueFlow`)
3. Add `[CreateAssetMenu]` to generate in Inspector
4. Handle in `ControlDialogueFlow()` with `if (currentLine is NewDialogueType)`

### Customize Choice Navigation

Edit `ChoiceNavigator.cs`:
- Change `columnsPerRow` to support 3-column grids
- Adjust wrapping logic in `HandleUpInput()`, `HandleDownInput()`
- Add diagonal movement (up-left, up-right, etc.)

### Hook into Dialogue Events

```csharp
void Start()
{
    choiceNavigator.OnChoiceHighlighted += (index) =>
    {
        Debug.Log($"Highlighted choice {index}");
        PlaySoundEffect("UI_Select");
    };
    
    choiceNavigator.OnChoiceConfirmed += (index) =>
    {
        Debug.Log($"Confirmed choice {index}");
        PlaySoundEffect("UI_Confirm");
    };
}
```

## Debugging

### Enable Debug Logs

Check `DialogueManager.Update()` console output:
- `"UP/DOWN/LEFT/RIGHT"` — arrow key press detected
- `"[CHOICE] Highlighted choice X"` — selection changed
- `"[CHOICE] Confirmed choice X"` — player confirmed

### Common Issues

**Dialogue doesn't display:**
- Ensure `DialogueUiManager` is assigned in inspector
- Check that `DialogueLine[]` array is not empty
- Verify `TMP_Text` components are populated

**Choices don't respond to input:**
- Confirm `ChoiceNavigator.IsAwaitingChoice` is true
- Check that `Update()` is called every frame
- Verify `Input.GetKeyDown()` works in standalone (vs. editor)

**UI doesn't highlight:**
- Ensure `HighlightChoice()` is called (check logs)
- Verify choice button color is different from default (contrast issue)
- Check that `OnChoiceHighlighted` event fires

## Integration with HFSM

The dialogue system can be triggered during state transitions via activities:

```csharp
public class DialogueActivity : IActivity
{
    private DialogueManager dialogueManager;
    
    public override async Task PerformActivity()
    {
        await dialogueManager.ControlDialogueFlow();
    }
}
```

Or triggered via input during gameplay:

```csharp
// In StateInputManager or PlayerStateDriver
public void Update()
{
    if (Input.GetKeyDown(KeyCode.O))
        StartCoroutine(dialogueManager.ControlDialogueFlow());
}
```

## Testing Checklist

- [ ] Create test OneWayDialogue and verify auto-advance
- [ ] Create test MultiWayDialogue with 2–4 choices
- [ ] Test arrow key navigation (all 4 directions)
- [ ] Test Enter to confirm and verify correct branch taken
- [ ] Test rapid key presses (ensure no double-selection)
- [ ] Test UI highlight visibility
- [ ] Test with 6+ choices (layout issues?)
- [ ] Test closing dialogue mid-conversation
- [ ] Test re-opening dialogue (state preserved?)

## Related Systems

- **HFSM** — Player state machine; dialogue can be triggered from states or activities
- **InputSystem** — Unity modern input; could integrate `InputActions` for cleaner key binding
- **DialogueUiManager** — Separate component for UI logic; decoupled from flow
- **PlayerContext** — Shared state; could reference player choices for conditional branches

## Contributing & Future Work

Areas for improvement:
- [ ] Typewriter text effect (character-by-character reveal)
- [ ] Dialogue box slide-in animation
- [ ] Sound effects for choice selection
- [ ] Speaker name and portrait display
- [ ] Save/load dialogue state
- [ ] Editor window for dialogue tree visualization
- [ ] Localization support

## License

Part of a challenge/portfolio project. Feel free to adapt and extend.

---

**Last Updated:** November 22, 2025  
**Status:** 🚧 In Progress — Core functionality working; UI and edge cases pending refinement.

For detailed API documentation, see inline code comments in each class.
