# Core

Core dialogue system components that manage flow, navigation, and input handling.

## Contents

- **ChoiceNavigator.cs** — Encapsulates choice selection logic with arrow key navigation (2-column grid)
  - Methods: `Initialize()`, `GetChoiceAsync()`, `Handle*Input()`
  - Events: `OnChoiceHighlighted`, `OnChoiceConfirmed`

- **DialogueReader.cs** — Handles text display and dialogue rendering
  - Manages typewriter effects and text updates
  - Integrates with UI manager for display

These classes work together to manage the runtime dialogue flow and player interaction.
