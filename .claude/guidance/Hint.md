# Hint.md

This file provides guidance for Hint System.

## Hint System (`Assets/Scripts/AI_test/`)

A 5-step pipeline driven by `HintManager` (singleton-like MonoBehaviour on the player). Toggled with the **F** key.

1. **`PlayerState`** — runtime telemetry struct (stay seconds, hint/fail counts, found/missed clues, last actions, repeated inspections). Scene scripts must `Add()` to these lists as the player interacts (e.g., `WineRackLabel`, `StainIntersection` push to `foundClues`; inspectables push to `lastActions` via `HintManager.AddLastAction`). Many fields still have `TODO` markers — populate them as scenes are built.
2. **`PuzzleConfig` / `PuzzleConfigData`** — static dictionary of per-scene puzzle definitions (required clues, ordered `PuzzleStep`s). To add a new scene puzzle, append an `else if` branch in `PuzzleConfigData.GetConfig` keyed by `puzzleId`.
3. **`HintEngine.Calculate`** — weighted score (emotion 35% / request 25% / stagnation 20% / progress 15% / misunderstanding 5%) → `hintLevel` 1–5 + `playerStatus` enum-string + next uncompleted `PuzzleStep`.
4. **`PromptBuilder`** — assembles the English system+user prompt. Two dictionaries (`LevelGuide`, `StatusGuide`, `TypeGuide`) + a `GetSceneContext` switch keyed by `puzzleId` describe scene/exit/entity/puzzle to the LLM. The system prompt forces Korean-only 1–2 sentence replies. When adding a new puzzle scene, **also add** a case to `GetSceneContext`.
5. **`OllamaClient.RequestHint`** — manually-assembled JSON POST (avoids `JsonUtility` quirks), manual response parsing (extracts the `"response":"..."` substring, strips trailing English parentheses). If you change the request schema, mirror it in `ParseResponse`.

`HintManager.OnSceneChanged(newPuzzleId)` resets the hint counter on scene transitions. `MAX_HINTS = 2` per scene.