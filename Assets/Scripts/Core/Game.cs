namespace Core
{
    /// <summary>
    /// Static helper class for accessing game systems.
    /// </summary>
    public static class Game
    {
        /// <summary>
        /// Access to the main GameManager instance.
        /// </summary>
        public static GameManager Main => GameManager.Instance;
        
        /// <summary>
        /// Access to the UI management system.
        /// </summary>
        public static UIManager UI => GameManager.UI;
        
        /// <summary>
        /// Access to the scene management system.
        /// </summary>
        public static SceneManager Scene => GameManager.Scene;
        
        /// <summary>
        /// Access to the audio management system.
        /// </summary>
        public static AudioManager Audio => GameManager.Audio;
    }
} 