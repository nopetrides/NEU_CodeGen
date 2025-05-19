using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Core
{
    public class SettingsMenu : UIComponent
    {
        private Slider _volumeSlider;
        private Label _volumeValue;
        private Toggle _muteToggle;
        private Button _returnButton;

        // Audio settings
        private float _masterVolume = 0.75f;
        private bool _isMuted = false;

        public override void Initialize()
        {
            base.Initialize();

            // Get references to UI elements
            _volumeSlider = Root.Q<Slider>("volume-slider");
            _volumeValue = Root.Q<Label>("volume-value");
            _muteToggle = Root.Q<Toggle>("mute-toggle");
            _returnButton = Root.Q<Button>("return-button");

            // Load saved audio settings
            LoadAudioSettings();

            // Initialize UI with current settings
            _volumeSlider.value = _masterVolume;
            _volumeValue.text = $"{Mathf.RoundToInt(_masterVolume * 100)}%";
            _muteToggle.value = _isMuted;

            // Register callbacks
            _volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            _muteToggle.RegisterValueChangedCallback(OnMuteToggled);
            _returnButton.RegisterCallback<ClickEvent>(OnReturnClicked);

            // Button hover sound handlers
            _returnButton.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
        }

        private void OnVolumeChanged(ChangeEvent<float> evt)
        {
			var normalizedVolume = evt.newValue;
			
            _masterVolume = Mathf.Log10(normalizedVolume) * 40;
            _volumeValue.text = $"{Mathf.RoundToInt(normalizedVolume * 100)}%";
            
            // Apply volume change
            ApplyAudioSettings();
            
            // Play a sound to demonstrate volume level
            Game.Audio.PlayUISound("ButtonHover");
        }

        private void OnMuteToggled(ChangeEvent<bool> evt)
        {
            _isMuted = evt.newValue;
            
            // Apply mute setting
            ApplyAudioSettings();
            
            // Play a sound if unmuted
            if (!_isMuted)
            {
                Game.Audio.PlayUISound("ButtonPress");
            }
        }

        private void OnReturnClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");
            
            // Save settings before returning
            SaveAudioSettings();
            
            // Hide settings menu and show main menu
            Hide();
            Game.UI.ShowMainMenu();
        }

        private void LoadAudioSettings()
        {
            // Load saved settings from PlayerPrefs
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            _isMuted = PlayerPrefs.GetInt("MuteAudio", 0) == 1;
        }

        private void SaveAudioSettings()
        {
            // Save settings to PlayerPrefs
            PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
            PlayerPrefs.SetInt("MuteAudio", _isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void ApplyAudioSettings()
        {
            // Apply audio settings to the game's audio system
            float effectiveVolume = _isMuted ? 0f : _masterVolume;
            
            // Set the master volume in the game's audio system
            AudioListener.volume = Mathf.Pow(10.0f, effectiveVolume/40.0f);
            
            // Save settings
            SaveAudioSettings();
        }

        public override void Show()
        {
			base.Show();
            // Load latest settings before showing
            LoadAudioSettings();
            
            // Update UI with current settings
            _volumeSlider.value = _masterVolume;
            _volumeValue.text = $"{Mathf.RoundToInt(_masterVolume * 100)}%";
            _muteToggle.value = _isMuted;
            
            // Show the settings menu
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            // Hide the settings menu
            Root.style.display = DisplayStyle.None;
        }
    }
}