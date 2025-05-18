using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;

public class FirewatchStyleMenuPanel : StandardUIMenuPanel
{

    // When pressed, this key shows the response menu. 
    // When released, it clicks the selected button.
    public KeyCode actionKey = KeyCode.LeftShift;

    // Remember the response menu info so we can show it when 
    // the player presses the actionKey:
    private Subtitle _subtitle;
    private Response[] _responses;
    private Transform _target;

    // When we need to open, we wait until we're closed to allow 
    // the Hide animation to finish:
    private bool _needToOpen;

    public override void ShowResponses(Subtitle subtitle, Response[] responses, Transform target)
    {
        // Instead of showing the menu, just save the menu info and prepare the
        // menu to show when the player pressed the actionKey:
        this._subtitle = subtitle;
        this._responses = responses;
        this._target = target;
        gameObject.SetActive(true);
        panelState = PanelState.Closed;
    }

    protected override void Update()
    {
        if (Input.GetKey(actionKey))
        {
            if (panelState == PanelState.Closed || panelState == PanelState.Closing || panelState == PanelState.Uninitialized)
            {
                _needToOpen = true;
            }
        }
        else
        {
            if (panelState == PanelState.Open || panelState == PanelState.Opening)
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    var button = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
                    if (button != null) button.onClick.Invoke();
                }
            }
        }
        if (_needToOpen && panelState == PanelState.Closed)
        {
            base.ShowResponses(_subtitle, _responses, _target);
            UITools.Select(instantiatedButtons[0].GetComponent<UnityEngine.UI.Button>());
            _needToOpen = false;
        }
    }

}
