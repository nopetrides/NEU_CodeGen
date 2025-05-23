//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 4.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;

public partial class SettingsMenu
{
    [SerializeField]
    private UIDocument _document;
    public Label VolumeValue { get; private set; }

    public Label VolumeLabel { get; private set; }

    public Slider VolumeSlider { get; private set; }

    public VisualElement VolumeDisplay { get; private set; }

    public Label MuteLabel { get; private set; }

    public Toggle MuteToggle { get; private set; }

    public VisualElement VolumeControl { get; private set; }

    public VisualElement MuteControl { get; private set; }

    public Button ReturnButton { get; private set; }

    public Label SettingsTitle { get; private set; }

    public VisualElement SettingsPanel { get; private set; }

    public VisualElement SettingsMenuContainer { get; private set; }

    public VisualElement Root => _document?.rootVisualElement;
    public void InitializeDocument()
    {
        VolumeValue = Root?.Q<Label>("volume-value");
        VolumeLabel = Root?.Q<Label>("volume-label");
        VolumeSlider = Root?.Q<Slider>("volume-slider");
        VolumeDisplay = Root?.Q<VisualElement>("volume-display");
        MuteLabel = Root?.Q<Label>("mute-label");
        MuteToggle = Root?.Q<Toggle>("mute-toggle");
        VolumeControl = Root?.Q<VisualElement>("volume-control");
        MuteControl = Root?.Q<VisualElement>("mute-control");
        ReturnButton = Root?.Q<Button>("return-button");
        SettingsTitle = Root?.Q<Label>("settings-title");
        SettingsPanel = Root?.Q<VisualElement>("settings-panel");
        SettingsMenuContainer = Root?.Q<VisualElement>("settings-menu-container");
    }
}