// Copyright (c) Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;
using PixelCrushers.DialogueSystem.UIToolkit;
using CharacterInfo = PixelCrushers.DialogueSystem.CharacterInfo;

namespace Core.Dialogue
{
    /// <summary>
    /// Dialogue UI implementation for UI Toolkit.
    /// </summary>
    public class GameUIToolkitDialogue : AbstractDialogueUI, IDialogueUI
    {
        [SerializeField] private UIToolkitRootElements rootElements;
        [SerializeField] private GameUIToolkitDialogueElements dialogueElements;

        public override AbstractUIRoot uiRootControls => rootElements;
        public override AbstractDialogueUIControls dialogueControls => dialogueElements;
        public override AbstractUIQTEControls qteControls => null;
        public override AbstractUIAlertControls alertControls => null;

        public override void Awake()
        {
            base.Awake();
            dialogueElements.Initialize(OnContinueConversation, OnClick);
        }

        public override void Update()
        {
            base.Update();
            (dialogueElements.responseMenuControls as UIToolkitResponseMenuElements)?.DoUpdate();
        }

        public override void Open()
        {
            base.Open();
            OpenSubtitlePanelsOnStart();
        }

        public override void ShowSubtitle(Subtitle subtitle)
        {
            var panel = GetSubtitlePanel(subtitle);
            if (panel != null)
            {
                HideOtherApplicablePanels(panel);
                panel.ShowSubtitle(subtitle);
            }
        }

        public override void HideSubtitle(Subtitle subtitle)
        {
            var panel = GetSubtitlePanel(subtitle);
            if (panel != null && !panel.ShouldStayVisible) panel.Hide();
        }

        protected virtual GameUIToolkitSubtitleElements GetSubtitlePanel()
		{
			return dialogueElements.SubtitlePanelElements;
        }

        protected virtual GameUIToolkitSubtitleElements GetSubtitlePanel(Subtitle subtitle)
        {
            if (subtitle == null) return null;

            // Check for override via [panel=#] tag:
            var panel = GetSubtitlePanel();
            if (panel != null) return panel;

            // Check for override on speaker's DialogueActor component:
            var dialogueActor = DialogueActor.GetDialogueActorComponent(subtitle.speakerInfo.transform);
            panel = GetDialogueActorSubtitlePanel(dialogueActor);
            if (panel != null) return panel;

            // Otherwise choose standard panel:
            return subtitle.speakerInfo.isNPC
                ? dialogueElements.npcSubtitleControls as GameUIToolkitSubtitleElements
                : dialogueElements.pcSubtitleControls as GameUIToolkitSubtitleElements;
        }

        protected virtual GameUIToolkitSubtitleElements GetDialogueActorSubtitlePanel(DialogueActor dialogueActor)
        {
            if (dialogueActor != null && dialogueActor.standardDialogueUISettings.subtitlePanelNumber != SubtitlePanelNumber.Default)
            {
                var index = PanelNumberUtility.GetSubtitlePanelIndex(dialogueActor.standardDialogueUISettings.subtitlePanelNumber);
                return GetSubtitlePanel();
            }
            return null;
        }

        private void OpenSubtitlePanelsOnStart()
        {
			var panel = GetSubtitlePanel();
			panel.OpenOnStartConversation();
        }
		
        public virtual GameUIToolkitSubtitleElements GetActorTransformPanel(Transform speakerTransform, GameUIToolkitSubtitleElements defaultPanel, 
            out DialogueActor dialogueActor)
        {
            dialogueActor = null;
            if (speakerTransform == null) return defaultPanel;
            dialogueActor = DialogueActor.GetDialogueActorComponent(speakerTransform);
            if (dialogueActor != null && dialogueActor.standardDialogueUISettings.subtitlePanelNumber != SubtitlePanelNumber.Default)
            {
                var panel = GetDialogueActorSubtitlePanel(dialogueActor);
                if (panel != null) return panel;
            }
            return defaultPanel;
        }

        protected virtual void HideOtherApplicablePanels(GameUIToolkitSubtitleElements panel)
        {
        }

        public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
        {
            // Don't hide the subtitle panel when showing responses
            // This allows the previous subtitle to remain visible while showing responses
            if (dialogueElements.responseMenuControls != null)
            {
                dialogueElements.responseMenuControls.ShowResponses(subtitle, responses, this.transform);
                if (timeout > 0) dialogueElements.responseMenuControls.StartTimer(timeout);
            }
        }

        #region Static Utility Methods

        public static void SetDisplay(VisualElement visualElement, bool value)
        {
            if (visualElement == null) return;
            visualElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static bool IsVisible(VisualElement visualElement)
        {
            if (visualElement == null) return false;
            return visualElement.style.display != DisplayStyle.None;

        }

        public static T GetVisualElement<T>(UIDocument document, string visualElementName) where T : VisualElement
        {
            if (document == null || document.rootVisualElement == null) return null;
            return document.rootVisualElement.Q<T>(visualElementName);
        }

        public static void SetInteractable(VisualElement rootVisualElement, bool value)
        {
            if (rootVisualElement == null) return;
            rootVisualElement.pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
        }

		public static Transform GetActorTransform(string actorName)
		{
			var actorTransform = CharacterInfo.GetRegisteredActorTransform(actorName);
			if (actorTransform == null)
			{
				var go = GameObject.Find(actorName);
				if (go != null) actorTransform = go.transform;
			}
			return actorTransform;
		}
		
        #endregion

    }

}
