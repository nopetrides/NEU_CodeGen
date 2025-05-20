using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Dialogue
{
	[Serializable]
	public class GameUIToolkitDialogueElements : AbstractDialogueUIControls
	{
		[SerializeField] private UIDocument document;
        [Tooltip("Name of document's root container.")]
        [SerializeField] private string rootContainerName;
        [SerializeField] private string dialoguePanelName;
        [SerializeField] private GameUIToolkitSubtitleElements _subtitlePanelElements;
        [SerializeField] private GameUIToolkitResponseMenuElements responseMenuElements;

        public GameUIToolkitSubtitleElements SubtitlePanelElements => _subtitlePanelElements;

        protected UIDocument Document => document;
        protected VisualElement RootContainer => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, rootContainerName);
        protected VisualElement DialoguePanel => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, dialoguePanelName);
        public override AbstractUISubtitleControls npcSubtitleControls => SubtitlePanelElements;
        public override AbstractUISubtitleControls pcSubtitleControls => SubtitlePanelElements;
        public override AbstractUIResponseMenuControls responseMenuControls => responseMenuElements;
		
		// Hand coded for the moment
		private Button _settingsButton;
		

        public void Initialize(System.Action clickedContinueAction, System.Action<object> clickedResponseAction)
        {
            responseMenuElements.Initialize(Document, clickedResponseAction);
			SubtitlePanelElements.Initialize(Document, clickedContinueAction);
			
			_settingsButton = RootContainer?.Q<Button>("SettingsButton");
			_settingsButton?.RegisterCallback<ClickEvent>(OnSettingsClicked);
			_settingsButton?.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
			
        }

        public override void ShowPanel()
        {
			GameUIToolkitDialogue.SetInteractable(RootContainer, false);
			GameUIToolkitDialogue.SetDisplay(DialoguePanel, true);
        }

        public override void SetActive(bool value)
        {
			GameUIToolkitDialogue.SetInteractable(RootContainer, value);
			GameUIToolkitDialogue.SetDisplay(DialoguePanel, value);
            base.SetActive(value);
        }
		
		private void OnSettingsClicked(ClickEvent evt)
		{
			Game.Audio.PlayUISound("ButtonPress");
			DialogueManager.Pause();

			try
			{
				// Hide main menu and show settings menu
				//RootContainer.style.display = DisplayStyle.None;
				Game.UI.ShowSettingsMenu();
				//Hide();
			}
			catch (Exception e)
			{
				Debug.LogError($"Error in settings menu: {e.Message}");
			}
		}
	}
}
