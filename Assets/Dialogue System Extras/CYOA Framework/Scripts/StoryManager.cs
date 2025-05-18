using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.CYOA
{

    /// <summary>
    /// This is the main coordination script for the CYOA framework. It records
    /// the current dialogue entry, reconnects the dialogue UI when switching to
    /// the Story scene, and provides the guts of menu item functionality.
    /// </summary>
    public class StoryManager : MonoBehaviour
    {

        public string startScene = "Start";
        public string storyScene = "Story";
        public string storyConversation = "Story";

        public bool autoSaveEveryLine;

        public int saveSlot;

        public string gameSavedMessage = "Game saved.";

        public string restartMessage = "Start from the beginning?";

        public TextTable textTable;

        public LanguageMenu languageMenu;

        public GameObject confirmationPanel;

        public UnityEngine.UI.Text confirmationText;

        private System.Action _confirmationAction;

        public void OnConversationLine(Subtitle subtitle)
        {
            DialogueLua.SetVariable("conversationID", subtitle.dialogueEntry.conversationID);
            DialogueLua.SetVariable("dialogueEntryID", subtitle.dialogueEntry.id);
            if (autoSaveEveryLine) SaveGame();
        }

        private void SaveGame()
        {
            SaveSystem.SaveToSlot(saveSlot);
        }

        public void Save()
        {
            SaveGame();
            var message = DialogueManager.GetLocalizedText(gameSavedMessage);
            DialogueManager.ShowAlert(message);
        }

        public bool HasSavedGame()
        {
            return SaveSystem.HasSavedGameInSlot(saveSlot);
        }

        public void ClearSavedGame()
        {
            SaveSystem.DeleteSavedGameInSlot(saveSlot);
        }

        public void Continue()
        {
            StartCoroutine(ContinueCoroutine());
        }

        public void Restart()
        {
            Confirm(restartMessage, ConfirmRestart);
        }

        public void ConfirmRestart()
        {
            StartCoroutine(RestartCoroutine());
        }

        private IEnumerator ContinueCoroutine()
        {
            DialogueManager.StopConversation();
            Tools.LoadLevel(storyScene);
            yield return null;
            SaveSystem.saveDataApplied += OnSaveDataApplied;
            SaveSystem.LoadFromSlot(saveSlot);
        }

        private void OnSaveDataApplied()
        {
            SaveSystem.saveDataApplied -= OnSaveDataApplied;
            var conversationID = DialogueLua.GetVariable("conversationID").AsInt;
            var dialogueEntryID = DialogueLua.GetVariable("dialogueEntryID").AsInt;
            var conversation = DialogueManager.MasterDatabase.GetConversation(conversationID);
            if (conversation == null)
            {
                Debug.LogError("Can't find a conversation with ID " + conversationID);
                DialogueManager.ShowAlert("Sorry, can't load the story conversation!");
            }
            else
            {
                DialogueManager.StartConversation(conversation.Title, null, null, dialogueEntryID);
            }
        }

        private IEnumerator RestartCoroutine()
        {
            DialogueManager.StopConversation();
            ClearSavedGame();
            Tools.LoadLevel(storyScene);
            yield return null;
            DialogueManager.StartConversation(storyConversation);
            if (!DialogueManager.IsConversationActive)
            {
                Debug.Log("Wasn't able to start conversation " + storyConversation);
                DialogueManager.ShowAlert("Sorry, can't start the story conversation!");
            }
        }

        public void QuitToTitle()
        {
            DialogueManager.StopConversation();
            Tools.LoadLevel(startScene);
        }

        public void QuitProgram()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void ShowLanguageMenu()
        {
            languageMenu.gameObject.SetActive(true);
        }

        public void Confirm(string question, System.Action confirmationAction)
        {
            this._confirmationAction = confirmationAction;
            var message = DialogueManager.GetLocalizedText(question);
            if (confirmationPanel == null || confirmationText == null)
            {
                Debug.LogError("The confirmation panel isn't assigned to StoryManager!", this);
                OkConfirm();
            }
            else
            {
                confirmationPanel.gameObject.SetActive(true);
                confirmationText.text = message;
            }
        }

        public void OkConfirm()
        {
            if (confirmationPanel != null) confirmationPanel.gameObject.SetActive(false);
            if (_confirmationAction != null) _confirmationAction();
        }

        public void CancelConfirm()
        {
            if (confirmationPanel != null) confirmationPanel.gameObject.SetActive(false);
        }

    }

}