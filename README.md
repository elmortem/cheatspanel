# Cheats Panel

## Install package

```
https://github.com/elmortem/cheatspanel.git?path=Project/Packages/com.elmortem.cheatspanel
```

## Enable

The panel is fully stripped from a build until the `CHEATS_ENABLE` define symbol is added
(Project Settings → Player → Other Settings → Scripting Define Symbols).
Without it every `CheatsPanel` call is removed by the compiler.

## Usage

```csharp
CheatsPanel.Bind("Add 100 gold", () => Wallet.Add(100));
CheatsPanel.Bind("Win level", () => Level.Complete(), "Levels");
CheatsPanel.Unbind("Win level", "Levels");

CheatsPanel.Show();
CheatsPanel.Hide();
CheatsPanel.Toggle();
```

## Input

The panel works with any `Active Input Handling` setting of the project:

| Active Input Handling | Provider |
|---|---|
| Input Manager (Old) | `UnityEngine.Input` |
| Input System Package (New) | `UnityEngine.InputSystem` (assembly `CheatPanel.InputSystem`, compiled only when the `com.unity.inputsystem` package is installed) |
| Both | Input System, the old one is used as a fallback |

Default controls: the backquote key (`` ` ``) or three simultaneous touches.
If the new Input System is selected but its package is missing, the panel still toggles
by the backquote key through IMGUI events.

Projects with their own input layer (input actions, remapped keys, an on-screen button)
can replace the provider:

```csharp
public sealed class MyCheatsInput : ICheatsPanelInput
{
    public bool IsToggleKeyDown => _openCheatsAction.WasPressedThisFrame();
    public int TouchCount => MyInput.Touches.Count;
}

CheatsPanelInput.SetProvider(new MyCheatsInput());
```

Pass `null` to `SetProvider` to return to the automatically selected provider.

## License
See. [LICENSE](LICENSE)

## Author
(c) 2025, Makar Osokin
