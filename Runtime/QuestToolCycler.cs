// SPDX-License-Identifier: MIT
// Base class for "press a button to cycle through a discrete setting" quest tools.
// Subclasses provide the setting's name, options, current value, and how to apply a choice;
// this class handles the input binding and routes feedback to the shared QuestToolFeedbackDisplay.

using UnityEngine;
using UnityEngine.InputSystem;

namespace ArkanumQuestTools
{
    public abstract class QuestToolCycler : MonoBehaviour
    {
        [Tooltip("Input action that cycles this setting. Expand to add bindings, e.g. \"<XRController>{RightHand}/primaryButton\".")]
        public InputAction cycleAction = new InputAction(name: "Cycle", type: InputActionType.Button);

        /// <summary>Display name of the setting being cycled, e.g. "MSAA".</summary>
        protected abstract string SettingName { get; }

        /// <summary>Display labels for each option, in cycle order.</summary>
        protected abstract string[] Options { get; }

        /// <summary>Index into <see cref="Options"/> of the currently active value.</summary>
        protected abstract int CurrentIndex { get; }

        /// <summary>Apply the option at the given index.</summary>
        protected abstract void Apply(int index);

        protected virtual void OnEnable()
        {
            cycleAction.performed += OnCyclePerformed;
            cycleAction.Enable();
        }

        protected virtual void OnDisable()
        {
            cycleAction.performed -= OnCyclePerformed;
            cycleAction.Disable();
        }

        void OnCyclePerformed(InputAction.CallbackContext ctx)
        {
            string[] options = Options;
            if (options == null || options.Length == 0) return;

            int next = (CurrentIndex + 1) % options.Length;
            Apply(next);

            QuestToolFeedbackDisplay.Instance.Show($"{SettingName}: <b>{options[next]}</b>");
        }
    }
}
