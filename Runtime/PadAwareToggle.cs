using UnityEngine;
using UnityEngine.UI;

namespace Schyro.PadAwareUI
{
    /// <summary>
    /// A <see cref="Toggle"/> that shows its Normal state instead of Highlighted while the gamepad is in use.
    /// </summary>
    /// <remarks>
    /// A plain Toggle stays highlighted under a resting mouse cursor after the player picks up the gamepad,
    /// because no pointer exit ever arrives, so two items look focused at once. This subclass maps
    /// Highlighted to Normal while <see cref="PadInputMode.UsingGamepad"/> is true and re-applies its
    /// state whenever the input mode changes.
    /// </remarks>
    [AddComponentMenu("UI/Pad Aware/Toggle", 31)]
    public class PadAwareToggle : Toggle
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying) PadInputMode.Changed += Reapply;
        }

        protected override void OnDisable()
        {
            PadInputMode.Changed -= Reapply;
            base.OnDisable();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (state == SelectionState.Highlighted && PadInputMode.UsingGamepad) state = SelectionState.Normal;
            base.DoStateTransition(state, instant);
        }

        private void Reapply() => DoStateTransition(currentSelectionState, false);
    }
}
