# Dialogue System

> A flexible, event-driven dialogue manager for branching conversations with arrow-key navigation (Undertale-style).

**Status:** 🚧 **In Progress** — Core features implemented; UI polish and edge cases pending.

## Quick Start

1. **Attach** `DialogueManager` (in `Examples/`) to a GameObject
2. **Create** dialogue nodes as ScriptableObjects (Data folder)
3. **Link** them together to form a conversation tree
4. **Assign** UI elements (text boxes, buttons)
5. **Call** `ControlDialogueFlow()` to start dialogue

```csharp
// Start dialogue
if (Input.GetKeyDown(KeyCode.O))
    StartCoroutine(dialogueManager.ControlDialogueFlow());
```

## Features

✅ **Linear & Branching** — OneWayDialogue (auto-advance) and MultiWayDialogue (choices)  
✅ **Arrow Key Navigation** — 2-column grid selection like Undertale  
✅ **Async/Await** — Integrates seamlessly with HFSM state machine  
✅ **Event System** — Hooks for custom sound/animation on choice selection  
✅ **Clean Architecture** — Organized into Core, Data, UI, Examples folders  

## Folder Structure

| Folder | Purpose | Key Classes |
|--------|---------|-------------|
| **Core/** | Runtime flow management | ChoiceNavigator, DialogueReader |
| **Data/** | Dialogue definitions (SO) | DialogueLine, OneWayDialogue, MultiWayDialogue |
| **UI/** | Visual rendering | DialogueUiManager |
| **Examples/** | Sample implementation | DialogueManager |

See **folder READMEs** for detailed documentation.

## Architecture Overview

```
DialogueManager (Examples/)
    ↓
ControlDialogueFlow() iterates through dialogue nodes
    ↓
OneWayDialogue → DisplayDialogue() → Auto-advance
MultiWayDialogue → DisplayChoices() → ChoiceNavigator.GetChoiceAsync()
    ↓
DialogueManager.Update() polls arrow/Enter keys
    ↓
ChoiceNavigator → OnChoiceHighlighted event
    ↓
DialogueUiManager → HighlightChoice() updates UI
    ↓
Player confirms → OnChoiceConfirmed event → resumes flow
```

## Current Status

### ✅ Completed
- Core dialogue flow engine
- Arrow key navigation (2-column grid)
- Choice selection with async/await
- UI text display and highlighting
- Event system (OnChoiceHighlighted, OnChoiceConfirmed)

### 🚧 In Progress
- UI highlight animations and visual polish
- Typewriter text effect
- Auto-advance timing configuration
- Dialogue box fade animations
- Edge case handling (empty choices, missing nodes)

### 📋 Planned
- Dialogue tags (colors, speaker names, portraits)
- Localization (CSV/JSON support)
- Dialogue history/replay
- Conditional branching based on player state
- Quest integration
- Save/load dialogue progression

## Integration Example

```csharp
// Trigger dialogue from input
InputSubscriber.Instance.SubscribeToInputEvent(
    InputType.Interact,
    () => StartCoroutine(dialogueManager.ControlDialogueFlow())
);

// Hook into choice events for sound effects
choiceNavigator.OnChoiceHighlighted += (index) =>
{
    audioManager.PlaySFX("ui_select");
};
```

## For Detailed Documentation

- **Main README:** `DIALOGUE_SYSTEM_README.md` — Full API docs, debugging, extending
- **Core/** — Choice navigation and text display
- **Data/** — Creating and structuring dialogue nodes
- **UI/** — Customizing visual feedback
- **Examples/** — Sample DialogueManager implementation

## Known Issues

| Issue | Workaround |
|-------|-----------|
| Choice highlight visibility | Adjust button colors in `DialogueUiManager.HighlightChoice()` |
| Rapid key presses skip choices | Add input debounce in `ChoiceNavigator` |
| Multi-line choice text overflow | Adjust TMP_Text layout constraints |

## Testing

- [x] OneWayDialogue auto-advance
- [x] Arrow key navigation (all 4 directions)
- [x] Enter to confirm and branch correctly
- [ ] 6+ choices layout (needs testing)
- [ ] UI highlight visibility refinement
- [ ] Rapid input handling

---

**For full documentation, see `DIALOGUE_SYSTEM_README.md`**  
**Last Updated:** November 22, 2025
