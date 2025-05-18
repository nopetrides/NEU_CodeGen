using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// This is a variation of the LoadLevel() sequencer command that disables 
    /// RememberCurrentDialogueEntry.
    /// </summary>
    public class SequencerCommandVnLoadLevel : SequencerCommand
    {

        private RememberCurrentDialogueEntry _mRememberCurrentDialogueEntry;
        private bool _mWasEnabled;

        public void Start()
        {
            string levelName = GetParameter(0);
            string spawnpoint = GetParameter(1);
            if (string.IsNullOrEmpty(levelName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: VNLoadLevel() level name is an empty string", DialogueDebug.Prefix));
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: VNLoadLevel({1})", DialogueDebug.Prefix, levelName));
                DialogueLua.SetActorField("Player", "Spawnpoint", spawnpoint);
                var saveSystem = FindObjectOfType<SaveSystem>();
                if (saveSystem != null)
                {
                    PersistentDataManager.LevelWillBeUnloaded();
                    SaveSystem.LoadScene(string.IsNullOrEmpty(spawnpoint) ? levelName : levelName + "@" + spawnpoint);
                }
                else
                {
                    var levelManager = FindObjectOfType<LevelManager>();
                    if (levelManager != null)
                    {
                        levelManager.LoadLevel(levelName);
                    }
                    else
                    {
                        PersistentDataManager.Record();
                        PersistentDataManager.LevelWillBeUnloaded();
                        SceneManager.LoadScene(levelName);
                        PersistentDataManager.Apply();
                    }
                }
            }
            _mRememberCurrentDialogueEntry = FindObjectOfType<RememberCurrentDialogueEntry>();
            if (_mRememberCurrentDialogueEntry != null)
            {
                _mWasEnabled = _mRememberCurrentDialogueEntry.enabled;
                _mRememberCurrentDialogueEntry.enabled = false;
            }
            Invoke("Stop", 1);
        }

        public void OnDestroy()
        {
            if (_mRememberCurrentDialogueEntry != null && _mWasEnabled) _mRememberCurrentDialogueEntry.enabled = true;
        }
    }
}
