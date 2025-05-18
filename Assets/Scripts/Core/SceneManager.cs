using UnityEngine;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Handles scene loading and transitions.
    /// </summary>
    public class SceneManager
    {
        private string _currentScene;
        
        /// <summary>
        /// Loads a scene asynchronously.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns>Task that completes when the scene is loaded</returns>
        public async Task LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name cannot be null or empty");
                return;
            }
            
            _currentScene = sceneName;
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
        
        /// <summary>
        /// Gets the name of the currently loaded scene.
        /// </summary>
        public string CurrentScene => _currentScene;
    }
} 