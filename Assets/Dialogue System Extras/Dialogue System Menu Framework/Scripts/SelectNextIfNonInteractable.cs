using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    // Moves to the next selectable if this selectable is selected but isn't interactable.
    // Used Horizontal and Vertical axis definitions to make a best guess which direction
    // to select.
    // Adapted from: http://forum.unity3d.com/threads/non-interactable-ui-element-e-g-button-not-skipped-by-navigation.285500/#post-2522528
    public class SelectNextIfNonInteractable : MonoBehaviour, UnityEngine.EventSystems.ISelectHandler
    {

        private UnityEngine.UI.Selectable _mSelectable;

        public void Awake()
        {
            _mSelectable = GetComponent<UnityEngine.UI.Selectable>();
        }

        public void OnSelect(UnityEngine.EventSystems.BaseEventData evData)
        {
            // Don't apply skipping unless we are not interactable.
            if (_mSelectable.interactable) return;

            // Check if the user navigated to this selectable.
            if (InputDeviceManager.GetAxis("Horizontal") < 0)
            {
                var select = _mSelectable.FindSelectableOnLeft();
                if (select == null || !select.gameObject.activeInHierarchy) select = _mSelectable.FindSelectableOnRight();
                StartCoroutine(DelaySelect(select));
            }
            else if (InputDeviceManager.GetAxis("Horizontal") > 0)
            {
                var select = _mSelectable.FindSelectableOnRight();
                if (select == null || !select.gameObject.activeInHierarchy) select = _mSelectable.FindSelectableOnLeft();
                StartCoroutine(DelaySelect(select));
            }
            else if (InputDeviceManager.GetAxis("Vertical") < 0)
            {
                var select = _mSelectable.FindSelectableOnDown();
                if (select == null || !select.gameObject.activeInHierarchy) select = _mSelectable.FindSelectableOnUp();
                StartCoroutine(DelaySelect(select));
            }
            else if (InputDeviceManager.GetAxis("Vertical") > 0)
            {
                var select = _mSelectable.FindSelectableOnUp();
                if (select == null || !select.gameObject.activeInHierarchy) select = _mSelectable.FindSelectableOnDown();
                StartCoroutine(DelaySelect(select));
            }
        }

        // Delay the select until the end of the frame.
        // If we do not, the current object will be selected instead.
        private IEnumerator DelaySelect(UnityEngine.UI.Selectable select)
        {
            yield return new WaitForEndOfFrame();
            if (select != null || !select.gameObject.activeInHierarchy) select.Select();
            else Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
        }
    }
}