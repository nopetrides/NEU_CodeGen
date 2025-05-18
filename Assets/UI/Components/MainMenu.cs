using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controller for the main menu UI.
/// Handles button interactions, scene loading, and UI animations.
/// </summary>
public partial class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName = "GameScene";
    
    [SerializeField]
    private float buttonPressScale = 0.95f;
    
    [SerializeField]
    private float buttonHoverScale = 1.05f;
    
    [SerializeField]
    private float animationDuration = 0.2f;
    
    private bool _isTransitioning;
    
    /// <summary>
    /// Called when the component is enabled.
    /// Initializes the document and sets up event listeners.
    /// </summary>
    private void OnEnable()
    {
        InitializeDocument();
        
        // Add event listeners
        if (PlayButton != null)
        {
            PlayButton.clicked += OnPlayButtonClicked;
            SetupButtonAnimations(PlayButton);
        }
        
        if (SettingsButton != null)
        {
            SettingsButton.clicked += OnSettingsButtonClicked;
            SetupButtonAnimations(SettingsButton);
        }
        
        if (QuitButton != null)
        {
            QuitButton.clicked += OnQuitButtonClicked;
            SetupButtonAnimations(QuitButton);
        }
        
        if (GameTitle != null)
        {
            GameTitle.RegisterCallback<ClickEvent>(OnTitleClicked);
        }
    }
    
    /// <summary>
    /// Called when the component is disabled.
    /// Removes event listeners to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        // Remove event listeners
        if (PlayButton != null)
        {
            PlayButton.clicked -= OnPlayButtonClicked;
        }
        
        if (SettingsButton != null)
        {
            SettingsButton.clicked -= OnSettingsButtonClicked;
        }
        
        if (QuitButton != null)
        {
            QuitButton.clicked -= OnQuitButtonClicked;
        }
        
        if (GameTitle != null)
        {
            GameTitle.UnregisterCallback<ClickEvent>(OnTitleClicked);
        }
    }
    
    /// <summary>
    /// Sets up button animations for hover and press effects.
    /// </summary>
    /// <param name="button">The button to set up animations for.</param>
    private void SetupButtonAnimations(Button button)
    {
        button.RegisterCallback<MouseEnterEvent>(evt => {
            if (!_isTransitioning)
            {
                button.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(animationDuration, TimeUnit.Second) });
                button.style.scale = new StyleScale(new Vector3(buttonHoverScale, buttonHoverScale, 1));
            }
        });
        
        button.RegisterCallback<MouseLeaveEvent>(evt => {
            if (!_isTransitioning)
            {
                button.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(animationDuration, TimeUnit.Second) });
                button.style.scale = new StyleScale(Vector3.one);
            }
        });
        
        button.RegisterCallback<MouseDownEvent>(evt => {
            if (!_isTransitioning)
            {
                button.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(animationDuration * 0.5f, TimeUnit.Second) });
                button.style.scale = new StyleScale(new Vector3(buttonPressScale, buttonPressScale, 1));
            }
        });
        
        button.RegisterCallback<MouseUpEvent>(evt => {
            if (!_isTransitioning)
            {
                button.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(animationDuration, TimeUnit.Second) });
                button.style.scale = new StyleScale(new Vector3(buttonHoverScale, buttonHoverScale, 1));
            }
        });
    }
    
    /// <summary>
    /// Handles the play button click event.
    /// Loads the game scene.
    /// </summary>
    private void OnPlayButtonClicked()
    {
        if (_isTransitioning) return;
        
        StartCoroutine(LoadGameScene());
    }
    
    /// <summary>
    /// Handles the settings button click event.
    /// Shows the settings popup.
    /// </summary>
    private void OnSettingsButtonClicked()
    {
        if (_isTransitioning) return;
        
        // TODO: Show settings popup
        Debug.Log("Settings button clicked - Settings popup not yet implemented");
    }
    
    /// <summary>
    /// Handles the quit button click event.
    /// Exits the application or play mode.
    /// </summary>
    private void OnQuitButtonClicked()
    {
        if (_isTransitioning) return;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Handles the game title click event.
    /// Triggers the bounce animation.
    /// </summary>
    private void OnTitleClicked(ClickEvent evt)
    {
        if (GameTitle != null)
        {
            GameTitle.RemoveFromClassList("bounce");
            GameTitle.schedule.Execute(() => GameTitle.AddToClassList("bounce")).ExecuteLater(10);
        }
    }
    
    /// <summary>
    /// Coroutine to load the game scene with a transition effect.
    /// </summary>
    private IEnumerator LoadGameScene()
    {
        _isTransitioning = true;
        
        // TODO: Add transition effect here
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
        
        yield return null;
    }
}