using UnityEngine;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{

    public class HandleBackgroundFields : MonoBehaviour
    {

        private BackgroundManager _mBackgroundManager;
        private BackgroundManager BackgroundManager
        {
            get
            {
                if (_mBackgroundManager == null) _mBackgroundManager = FindObjectOfType<BackgroundManager>();
                return _mBackgroundManager;
            }
        }

        private void OnConversationLine(Subtitle subtitle)
        {
            if (subtitle == null || BackgroundManager == null) return;
            var background = Field.LookupValue(subtitle.dialogueEntry.fields, "Background");
            if (!string.IsNullOrEmpty(background))
            {
                BackgroundManager.SetBackgroundImage(background);
            }
            else 
            {
                background = DialogueLua.GetActorField(subtitle.speakerInfo.nameInDatabase, "Background").asString;
                if (!string.IsNullOrEmpty(background))
                {
                    BackgroundManager.SetBackgroundImage(background);
                }
            }
        }
    }
}