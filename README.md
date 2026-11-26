<div align="center">

# 🎮 Pad Aware UI

**Your player picked up the gamepad. The button under the mouse never noticed.**

Drop-in uGUI `Button`, `Toggle`, `Slider` and `Scrollbar` that hide the stale hover highlight while the gamepad is in use, so only one item looks focused.

![Unity 6000.3+](https://img.shields.io/badge/Unity-6000.3%2B-black?logo=unity)
![Input System](https://img.shields.io/badge/Input%20System-1.19-2b6cb0)
![uGUI](https://img.shields.io/badge/uGUI-2.0-6b46c1)
![Install](https://img.shields.io/badge/install-git%20URL-0f766e)
![License MIT](https://img.shields.io/badge/license-MIT-4c9a2a)

[Quick start](#-quick-start) · [The problem](#-the-problem) · [How it works](#-how-it-works) · [Your own input detection](#-your-own-input-detection)

</div>

---

![Before: with stock uGUI, Quit stays highlighted under the resting mouse cursor while the gamepad selects Options, so two buttons look focused. After: with Pad Aware UI, the hover drops as soon as the gamepad is used and comes back when the mouse moves.](Documentation~/before-after.gif)

## 🐛 The problem

1. The mouse cursor rests on a button, so the button is **Highlighted**.
2. The player picks up the gamepad and moves the **Selected** state to another button.
3. The cursor never moves, so no pointer exit arrives. Both buttons now look focused.

Unity has closed reports of this as *By Design* or *Won't Fix*. The common workarounds break something else. Setting **Navigation** to *None* breaks gamepad navigation, and making Highlighted look like Normal removes mouse hover feedback.

## 🚀 Quick start

**1. Install.** In **Package Manager**, choose **Install package from git URL** and enter:

```text
https://github.com/schyro/unity-pad-aware-ui.git#v0.1.0
```

**2. Enable the Input System.** Set **Project Settings > Player > Active Input Handling** to *Input System Package (New)* or *Both*.

**3. Use the Pad Aware components.**

- **New controls:** in **Add Component**, pick **UI > Pad Aware > Button** (or Toggle, Slider, Scrollbar).
- **Existing controls:** switch the Inspector to **Debug** mode, drag `PadAwareButton` onto the component's **Script** field, then switch back to **Normal**. Each Pad Aware type is a subclass of the stock one, so every setting and event is kept.

That's it. Navigation, events and Inspectors work the same as before.

## 🔧 How it works

Each component overrides one method of the stock control:

```csharp
protected override void DoStateTransition(SelectionState state, bool instant)
{
    if (state == SelectionState.Highlighted && PadInputMode.UsingGamepad) state = SelectionState.Normal;
    base.DoStateTransition(state, instant);
}
```

When the input mode changes, the component re-applies its current state. The hover look disappears the moment the gamepad is used and returns when the mouse moves.

`PadInputMode` tracks the last device the player used:

| Switches to gamepad | Switches back to keyboard and mouse |
| --- | --- |
| Stick past 0.5, d-pad or any button | Any key, mouse movement over 3 px, click or scroll |

## 🧩 Your own input detection

If your project already tracks the active device, turn off the built-in detection and set the mode yourself:

```csharp
using Schyro.PadAwareUI;

PadInputMode.AutoDetect = false;
PadInputMode.SetUsingGamepad(playerInput.currentControlScheme == "Gamepad");
```

You can also read `PadInputMode.UsingGamepad` and listen to `PadInputMode.Changed` in your own scripts, for example to hide the cursor or swap button prompts.

## 📝 Notes

- `Dropdown`, `InputField`, `TMP_Dropdown` and `TMP_InputField` are not included. The same override works for any `Selectable`: copy `PadAwareButton.cs` and change the base type.
- Keyboard input counts as keyboard-and-mouse mode, so the hover highlight stays during keyboard navigation.

<div align="center">

[MIT License](LICENSE.md) · made by [schyro](https://github.com/schyro)

</div>
