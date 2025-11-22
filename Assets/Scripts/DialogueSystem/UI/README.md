# UI

User interface components for displaying dialogue and choices on screen.

## Contents

- **DialogueUiManager.cs** — Manages on-screen dialogue and choice display
  - Methods: `UpdateDialogueUiText()`, `UpdateChoiceText()`, `HighlightChoice()`
  - Properties: `dialogueCanvas`, `dialogueText`, `choicesText[]`
  - Subscribes to `ChoiceNavigator.OnChoiceHighlighted` for visual feedback

## Integration

The UI manager is initialized with references to:
- Main dialogue text box (TMP_Text)
- Choice buttons/text elements (TMP_Text[])
- Canvas containing all dialogue elements

When `ChoiceNavigator` navigates between choices, it fires `OnChoiceHighlighted`, which triggers the UI manager to update the highlighted choice's visual state (color, scale, etc.).

## Customization

Override `HighlightChoice()` to customize visual feedback:
- Change color, alpha, scale, or outline
- Add animation or sound effects
- Update button images or icons
