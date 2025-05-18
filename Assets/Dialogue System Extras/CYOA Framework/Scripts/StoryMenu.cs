using UnityEngine;

namespace PixelCrushers.DialogueSystem.CYOA
{

    /// <summary>
    /// Handles the Story Menu.
    /// </summary>
    public class StoryMenu : MonoBehaviour
    {

        public GameObject menuPanel;

        private StoryManager _storyManager;
        private StoryManager StoryManager
        {
            get
            {
                if (_storyManager == null) _storyManager = FindObjectOfType<StoryManager>();
                return _storyManager;
            }
        }

        private void Start()
        {
            if (StoryManager == null)
            {
                Debug.LogError("Can't find StoryManager on the Dialogue Manager GameObject!");
            }
            if (menuPanel == null)
            {
                Debug.LogError("The menu panel isn't assigned to StoryMenu!", this);
            }
            else
            {
                menuPanel.SetActive(false);
            }
        }

        public void ToggleMenu()
        {
            menuPanel.SetActive(!menuPanel.activeInHierarchy);
        }

        public void Continue()
        {
            ToggleMenu();
        }

        public void Save()
        {
            StoryManager.Save();
            ToggleMenu();
        }

        public void Restart()
        {
            StoryManager.Restart();
        }

        public void ShowLanguageMenu()
        {
            StoryManager.ShowLanguageMenu();
        }

        public void QuitToTitle()
        {
            Save();
            StoryManager.QuitToTitle();
        }

        public void QuitProgram()
        {
            Save();
            StoryManager.QuitProgram();
        }

    }

}