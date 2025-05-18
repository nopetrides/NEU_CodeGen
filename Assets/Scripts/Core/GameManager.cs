using UnityEngine;
using System;

namespace Core
{
    /// <summary>
    /// Main game manager that persists throughout the game lifecycle.
    /// Provides access to core game systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private static bool _isInitialized;
        
        // Core systems
        private static UIManager _uiManager;
        private static SceneManager _sceneManager;
        private static AudioManager _audioManager;
        
        /// <summary>
        /// Static access to the GameManager instance.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException("GameManager has not been initialized. Make sure BootManager exists in the BootScene.");
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Access to the UI management system.
        /// </summary>
        public static UIManager UI => _uiManager;
        
        /// <summary>
        /// Access to the scene management system.
        /// </summary>
        public static SceneManager Scene => _sceneManager;
        
        /// <summary>
        /// Access to the audio management system.
        /// </summary>
        public static AudioManager Audio => _audioManager;
        
        /// <summary>
        /// Initializes the GameManager singleton.
        /// Should only be called by BootManager.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("GameManager is already initialized.");
                return;
            }
            
            var go = new GameObject("GameManager");
            _instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
            
            InitializeSystems();
            _isInitialized = true;
        }
        
        private static void InitializeSystems()
        {
            // Initialize core systems
            _uiManager = new UIManager();
            _sceneManager = new SceneManager();
            _audioManager = _instance.gameObject.AddComponent<AudioManager>();
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _isInitialized = false;
                _instance = null;
            }
        }
    }
} 