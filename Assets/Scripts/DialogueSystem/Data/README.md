# Data

Dialogue data structures and node definitions. Serialized as ScriptableObjects.

## Contents

- **DialogueLine.cs** — Abstract base class for all dialogue nodes
  - Contains shared dialogue text and properties
  - Subclassed by OneWayDialogue and MultiWayDialogue

- **OneWayDialogue.cs** — Linear dialogue that auto-advances to next node
  - Property: `nextDialogue` (reference to following node)

- **MultiWayDialogue.cs** — Branching dialogue with player-selectable choices
  - Property: `choices` (array of DialogueChoice)
  - Method: `GetChoicesText()` returns array of choice texts

- **DialogueChoice.cs** — Serializable choice data structure
  - Properties: `choiceText`, `resultingDialogue`

- **DialogueLineSO.cs** — ScriptableObject wrapper for dialogue line creation

- **TextDialogueTypeEnum.cs** — Enum for dialogue text types (dialogue vs. choice)

## Usage

Create new dialogue nodes as ScriptableObjects:
```
Right-click in Assets → Create → Dialogue → OneWayDialogue
Right-click in Assets → Create → Dialogue → MultiWayDialogue
```

Link them together in the Inspector to form conversation trees.
