using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Core
{
    public class SettingsMenuComponent : UIComponent
    {
        private Slider _volumeSlider;
        private Label _volumeValue;
        private Toggle _muteToggle;
        private Button _returnButton;
        private Button _quitButton;
        

        // Audio settings
		/// <summary>
		/// 0-1 that gets saved
		/// </summary>
		private float _masterSliderNormalized = 0.75f;
		/// <summary>
		/// log value applied to listeners
		/// </summary>
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
            _quitButton = Root.Q<Button>("quit-button");

            // Load saved audio settings
            LoadAudioSettings();

            // Initialize UI with current settings
            _volumeSlider.value = _masterSliderNormalized;
            _volumeValue.text = $"{Mathf.RoundToInt(_masterVolume * 100)}%";
            _muteToggle.value = _isMuted;

            // Register callbacks
            _volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            _muteToggle.RegisterValueChangedCallback(OnMuteToggled);
            _returnButton.RegisterCallback<ClickEvent>(OnReturnClicked);
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);

            // Button hover sound handlers
            _returnButton.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            _quitButton.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));

            
        }

        private void OnVolumeChanged(ChangeEvent<float> evt)
        {
			_masterSliderNormalized = evt.newValue;
			
			// Convert 0-1 range to decibels (-80dB to 0dB is a common range)
			_masterVolume = Mathf.Log10(Mathf.Max(0.0001f, _masterSliderNormalized)) * 20f;
            _volumeValue.text = $"{Mathf.RoundToInt(_masterSliderNormalized * 100)}%";
            
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
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			if (scene.name == "MainMenuScene")
				Game.UI.ShowMainMenu();
        }

        private void OnQuitClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");

            try
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Error quitting game: {e.Message}");
            }
        }

        private void LoadAudioSettings()
        {
            // Load saved settings from PlayerPrefs
			
			_masterSliderNormalized = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
			// Convert 0-1 range to decibels (-80dB to 0dB is a common range)
			_masterVolume = Mathf.Log10(Mathf.Max(0.0001f, _masterSliderNormalized)) * 20f;
            _isMuted = PlayerPrefs.GetInt("MuteAudio", 0) == 1;
        }

        private void SaveAudioSettings()
        {
            // Save settings to PlayerPrefs
            PlayerPrefs.SetFloat("MasterVolume", _masterSliderNormalized);
            PlayerPrefs.SetInt("MuteAudio", _isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void ApplyAudioSettings()
        {
            // Apply audio settings to the game's audio system
            float effectiveVolume = _isMuted ? 0f : _masterVolume;
            
            // Set the master volume in the game's audio system
            // Convert decibels back to linear (0-1) for Unity's AudioListener
            AudioListener.volume = Mathf.Pow(10.0f, effectiveVolume / 20.0f);
            
            // Save settings
            SaveAudioSettings();
        }

        public override void Show()
        {
			base.Show();
            // Load latest settings before showing
            LoadAudioSettings();
            
            // Update UI with current settings
            _volumeSlider.value = _masterSliderNormalized;
            _volumeValue.text = $"{Mathf.RoundToInt(_masterSliderNormalized * 100)}%";
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