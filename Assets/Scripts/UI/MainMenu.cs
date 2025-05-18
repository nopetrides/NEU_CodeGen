using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Core
{
    public class MainMenu : UIComponent
    {
        private Button _playButton;
        private Button _settingsButton;
        private Button _quitButton;
        private Label _gameTitle;
        
        public override void Initialize()
        {
            base.Initialize();
            
            // Get references to UI elements
            _playButton = Root.Q<Button>("play-button");
            _settingsButton = Root.Q<Button>("settings-button");
            _quitButton = Root.Q<Button>("quit-button");
            _gameTitle = Root.Q<Label>("game-title");
            
            // Set up button click handlers
            _playButton?.RegisterCallback<ClickEvent>(OnPlayClicked);
            _settingsButton?.RegisterCallback<ClickEvent>(OnSettingsClicked);
            _quitButton?.RegisterCallback<ClickEvent>(OnQuitClicked);
            
            // Set up hover sound handlers
            _playButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            _settingsButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            _quitButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            
            // Set up title click handler for bounce effect
            _gameTitle?.RegisterCallback<ClickEvent>(OnTitleClicked);
        }
        
        private async void OnPlayClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");
            SetButtonsEnabled(false);
            
            try
            {
                await Game.Scene.LoadSceneAsync("GameScene");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game scene: {e.Message}");
                SetButtonsEnabled(true);
            }
        }
        
        private void OnSettingsClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");
            SetButtonsEnabled(false);
            
            try
            {
                // TODO: Implement SettingsMenu
                Debug.Log("Settings clicked - SettingsMenu not implemented yet");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in settings menu: {e.Message}");
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }
        
        private void OnQuitClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");
            SetButtonsEnabled(false);
            
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
                SetButtonsEnabled(true);
            }
        }
        
        private void OnTitleClicked(ClickEvent evt)
        {
            Game.Audio.PlayUISound("ButtonPress");
            // The bounce effect is handled by USS
        }
        
        private void SetButtonsEnabled(bool enabled)
        {
            if (_playButton != null)
            {
                _playButton.SetEnabled(enabled);
                _playButton.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            }
            
            if (_settingsButton != null)
            {
                _settingsButton.SetEnabled(enabled);
                _settingsButton.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            }
            
            if (_quitButton != null)
            {
                _quitButton.SetEnabled(enabled);
                _quitButton.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            }
        }
    }
} 