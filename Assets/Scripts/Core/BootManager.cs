using UnityEngine;

namespace Core
{
    /// <summary>
    /// Handles initial game setup and creates the GameManager.
    /// This class should only exist in the BootScene.
    /// </summary>
    public class BootManager : MonoBehaviour
    {
        [SerializeField] private string _mainMenuSceneName = "MainMenuScene";
        
        private void Awake()
        {
            InitializeGame();
        }

        private async void InitializeGame()
        {
            // Create and initialize GameManager
            GameManager.Initialize();
            
            // Store reference to this GameObject before scene load
            var bootManagerGo = gameObject;
            
            // Load main menu scene
            await Game.Scene.LoadSceneAsync(_mainMenuSceneName);
            
            // Show main menu
            Game.UI.Show<MainMenu>();
            
            // Only destroy if the GameObject still exists
            if (bootManagerGo != null)
            {
                Destroy(bootManagerGo);
            }
        }
    }
} 