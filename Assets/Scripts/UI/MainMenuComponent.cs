using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Core
{
    public class MainMenuComponent : UIComponent
    {
        private Button _playButton;
        private Button _settingsButton;
        private Button _quitButton;
        private Label _gameTitle;
		private VisualElement _bgParallax;
		private VisualElement _bgParallaxDark;
		private VisualElement _oriseiParallax;

		private float _amplitudeX = 50f;    // How far left/right
		private float _amplitudeY = 25f;     // How far down
		private float _frequency = 1f;      // Speed of motion

        public override void Initialize()
        {
            base.Initialize();

            // Get references to UI elements
            _playButton = Root.Q<Button>("play-button");
            _settingsButton = Root.Q<Button>("settings-button");
            _quitButton = Root.Q<Button>("quit-button");
            _gameTitle = Root.Q<Label>("game-title");
			_bgParallax = Root.Q<VisualElement>("mountain-parallax");
			_bgParallaxDark = Root.Q<VisualElement>("mountain-parallax-dark");
			_oriseiParallax = Root.Q<VisualElement>("orisei-parallax");

            // Button click handlers
            _playButton?.RegisterCallback<ClickEvent>(OnPlayClicked);
            _settingsButton?.RegisterCallback<ClickEvent>(OnSettingsClicked);
            _quitButton?.RegisterCallback<ClickEvent>(OnQuitClicked);

            // Button hover sound handlers
            _playButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            _settingsButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            _quitButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));

            // Title click bounce animation handler
            _gameTitle?.RegisterCallback<ClickEvent>(OnTitleClicked);

			AnimateStormTitle();
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
                // Hide main menu and show settings menu
                Root.style.display = DisplayStyle.None;
                Game.UI.ShowSettingsMenu();
				Hide();
				SetButtonsEnabled(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in settings menu: {e.Message}");
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
            Game.Audio.PlayUISound("Thunder");
            AnimateTitleBounce();
        }

        private void SetButtonsEnabled(bool enabled)
        {
            var pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;

            _playButton?.SetEnabled(enabled);
            _playButton.pickingMode = pickingMode;

            _settingsButton?.SetEnabled(enabled);
            _settingsButton.pickingMode = pickingMode;

            _quitButton?.SetEnabled(enabled);
            _quitButton.pickingMode = pickingMode;
        }

        // Visual Enhancements via scripting

        private void AnimateTitleBounce()
        {
            _gameTitle.AddToClassList("bounce");
            Root.schedule.Execute(() => _gameTitle.RemoveFromClassList("bounce")).StartingIn(500);
        }

        private void AnimateStormTitle()
		{
			// Flash effect for dark parallax layer
			Root.schedule.Execute(() =>
			{
				StyleColor color = _bgParallaxDark.style.unityBackgroundImageTintColor;
				Color c = color.value;
        
				// Make transparent
				c.a = 0f;
				color.value = c;
				_bgParallaxDark.style.unityBackgroundImageTintColor = color;

				void Unflash()
				{
					c.a = .5f;
					color.value = c;
					_bgParallaxDark.style.unityBackgroundImageTintColor = color;
				}
				// Schedule return to opaque after 100ms and 300ms
				Root.schedule.Execute(Unflash).StartingIn(100);
				Root.schedule.Execute(Unflash).StartingIn(300);

				void Flash()
				{
					c.a = .5f;
					color.value = c;
					_bgParallaxDark.style.unityBackgroundImageTintColor = color;
				}
				// Schedule return to opaque after 200ms
				Root.schedule.Execute(Flash).StartingIn(200);

			}).Every(5000); // Repeat every 5 seconds

			Root.schedule.Execute(() =>
			{
				float time = Time.time * _frequency;
				float x = Mathf.Sin(time) * _amplitudeX * 0.25f; // Reduced amplitude for background
				// Normalize to range [0, 1], then apply amplitude to Y (no upward movement)
				float y = ((Mathf.Sin(time + Mathf.PI / 2f) + 1f) / 2f) * _amplitudeY * 0.25f; // Reduced amplitude for background
				_bgParallax.style.translate = new Translate(x, y);
			}).Every(30);

            Root.schedule.Execute(() =>
            {
				float time = Time.time * _frequency;
				float x = Mathf.Sin(time) * _amplitudeX;
				// Normalize to range [0, 1], then apply amplitude to Y (no upward movement)
				float y = ((Mathf.Sin(time + Mathf.PI / 2f) + 1f) / 2f) * _amplitudeY;
				_oriseiParallax.style.translate = new Translate(x, y);
            }).Every(20);
        }
    }
}

/* Unity Editor Setup Instructions:

1. Assign USS Classes:
   - Ensure your USS defines a ".bounce" class to scale up the title temporarily.

2. Assets:
   - Ensure you have UI sound effects named "ButtonHover", "ButtonPress", and "Thunder".
   - Ensure background images (storm_clouds.png, rain_overlay.png) are set correctly in USS.

3. UI Elements:
   - Set correct USS class names and UI element names ("play-button", "settings-button", "quit-button", "game-title").

This setup creates a stormy animated effect for your main menu, enhancing interactivity and thematic coherence. */
