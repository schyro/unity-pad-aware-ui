using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Schyro.PadAwareUI
{
    /// <summary>
    /// Tracks whether the player last used a gamepad or the keyboard and mouse.
    /// </summary>
    /// <remarks>
    /// A hidden runner is installed after the first scene loads and checks the current devices every
    /// frame. Set <see cref="AutoDetect"/> to false and call <see cref="SetUsingGamepad"/> to drive the
    /// state from your own input code instead, for example from <c>PlayerInput.onControlsChanged</c>.
    /// </remarks>
    public static class PadInputMode
    {
        private const float StickThreshold = 0.5f;
        private const float MouseMoveThreshold = 3f;

        /// <summary>True while the gamepad is the last device the player used.</summary>
        public static bool UsingGamepad { get; private set; }

        /// <summary>When false, the built-in detection stops and only <see cref="SetUsingGamepad"/> changes the state.</summary>
        public static bool AutoDetect { get; set; } = true;

        /// <summary>Raised when <see cref="UsingGamepad"/> changes.</summary>
        public static event Action Changed;

        /// <summary>Sets the input mode by hand and raises <see cref="Changed"/> if it changed.</summary>
        public static void SetUsingGamepad(bool usingGamepad)
        {
            if (usingGamepad == UsingGamepad) return;
            UsingGamepad = usingGamepad;
            Changed?.Invoke();
        }

        // Keeps the state correct when domain reload on entering Play Mode is disabled.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            UsingGamepad = false;
            AutoDetect = true;
            Changed = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            // HideInHierarchy only: DontSave objects outlive Play Mode in the editor and would pile up.
            var host = new GameObject("Pad Input Mode") { hideFlags = HideFlags.HideInHierarchy };
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<Runner>();
        }

        private static void Detect()
        {
            Gamepad pad = Gamepad.current;
            if (pad != null && pad.wasUpdatedThisFrame && GamepadActive(pad))
            {
                SetUsingGamepad(true);
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            {
                SetUsingGamepad(false);
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > MouseMoveThreshold * MouseMoveThreshold
                                  || mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame
                                  || mouse.scroll.ReadValue().sqrMagnitude > 0f))
            {
                SetUsingGamepad(false);
            }
        }

        // Stick drift and analog noise also mark the pad as updated, so only deliberate input counts.
        private static bool GamepadActive(Gamepad pad)
        {
            if (pad.leftStick.ReadValue().magnitude > StickThreshold || pad.rightStick.ReadValue().magnitude > StickThreshold
                || pad.dpad.ReadValue().sqrMagnitude > 0f)
                return true;
            foreach (InputControl control in pad.allControls)
                if (control is ButtonControl button && !control.synthetic && button.wasPressedThisFrame)
                    return true;
            return false;
        }

        private sealed class Runner : MonoBehaviour
        {
            private void Update()
            {
                if (AutoDetect) Detect();
            }
        }
    }
}
