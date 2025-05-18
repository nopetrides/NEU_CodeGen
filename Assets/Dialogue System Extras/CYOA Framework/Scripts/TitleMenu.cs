using UnityEngine;

namespace PixelCrushers.DialogueSystem.CYOA
{

    /// <summary>
    /// Handles the Title Menu.
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {

        public UnityEngine.UI.Button continueButton;

        private StoryManager _storyManager;

        private void Start()
        {
            _storyManager = FindObjectOfType<StoryManager>();
            if (_storyManager == null)
            {
                Debug.LogError("Can't find StoryManager on the Dialogue Manager GameObject!");
                return;
            }
            if (continueButton == null)
            {
                Debug.LogError("The continue button isn't assigned to TitleMenu!", this);
            }
            else
            {
                continueButton.gameObject.SetActive(_storyManager.HasSavedGame());
            }
        }

        public void Continue()
        {
            _storyManager.Continue();
        }

        public void Restart()
        {
            _storyManager.Restart();
        }

        public void ShowLanguageMenu()
        {
            _storyManager.ShowLanguageMenu();
        }

        public void QuitProgram()
        {
            _storyManager.QuitProgram();
        }

    }

}