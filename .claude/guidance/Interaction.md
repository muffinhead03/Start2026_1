# Interaction.md

This file provides guidance for player interaction system.

## Interaction System (raycast-driven)

All player interaction with world objects flows through a single chain. Don't bypass it.

1. `Player_Interaction.cs` casts a forward ray from the camera each frame up to `dist`.
2. The hit collider is queried for an `Event_On_Ray` component (which implements `IRayInteractable` — `Interface_Ray.cs`).
3. `Event_On_Ray` exposes four `UnityEvent`s (`OnEnter`, `OnStay`, `OnExit`, `OnClick`) wired in the Inspector — concrete behaviors (doors, keys, pipes, password locks, inspectables, etc.) live as separate `Object_*` components whose methods are bound to those events.
4. `Interact` action from the new Input System (`InputSystem_Actions.inputactions`) triggers `OnRayClick`.

When adding a new interactable: attach `Event_On_Ray` + your `Object_X` behavior to the prefab, wire the UnityEvents in the Inspector. Do not duplicate the raycast logic.

Inventory is a global static list: `Player_Inventory.AddItem(string)` / `Player_Inventory.hasKey(string)` — `hasKey` consumes the key on success.
