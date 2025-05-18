using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Reference to the UIDocument in the scene
    [SerializeField] private UIDocument uiDocument;

    private void OnEnable()
    {
        // Get root element of the UI
        VisualElement root = uiDocument.rootVisualElement;

        // Find buttons by their names
        Button playButton = root.Q<Button>("play-button");
        Button quitButton = root.Q<Button>("quit-button");

        // Assign callbacks
        playButton.clicked += OnPlayClicked;
        quitButton.clicked += OnQuitClicked;
    }

    /// <summary>
    /// Called when the Play button is clicked.
    /// </summary>
    private void OnPlayClicked()
    {
        // Load another scene — replace "GameScene" with your actual scene name
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Called when the Quit button is clicked.
    /// </summary>
    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
