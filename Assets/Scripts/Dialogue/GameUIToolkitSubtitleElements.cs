using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Dialogue
{
	[Serializable]
	public class GameUIToolkitSubtitleElements : AbstractUISubtitleControls
	{
		[Tooltip("Container panel for subtitle.")]
        [SerializeField] private string _subtitlePanelName;
        [Tooltip("Subtitle text.")]
        [SerializeField] private string _subtitleLabelName;
        [Tooltip("Optional speaker portrait name.")]
        [SerializeField] private string _portraitLabelName;
        [Tooltip("Optional speaker portrait image.")]
        [SerializeField] private string _speakerPortrait;
		[Tooltip("Optional speaker portrait image.")]
		[SerializeField] private string _npcPortrait;
        [Tooltip("Continue button to advance conversation (if mode requires continue button click).")]
        [SerializeField] private string _continueButtonName;
        [Tooltip("Specifies when panel should be visible/hidden.")]
        [SerializeField] private UIVisibility _visibility;

        public bool IsSamePanel(GameUIToolkitSubtitleElements panel) => panel._subtitlePanelName == this._subtitlePanelName;
        public string SubtitlePanelName => _subtitlePanelName;
        public UIVisibility Visibility => _visibility;

        protected UIDocument Document { get; set; }
        protected VisualElement SubtitlePanel => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, _subtitlePanelName);
        protected Label SubtitleLabel => GameUIToolkitDialogue.GetVisualElement<Label>(Document, _subtitleLabelName);
        protected Label PortraitLabel => null;
        protected VisualElement PortraitImage => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, _speakerPortrait);
		protected VisualElement ListenerPortraitImage => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, _npcPortrait);
        protected Button ContinueButton => GameUIToolkitDialogue.GetVisualElement<Button>(Document, _continueButtonName);

        public bool ShouldStayVisible => Visibility == UIVisibility.AlwaysFromStart || Visibility == UIVisibility.AlwaysOnceShown;

        public override bool hasText => !string.IsNullOrEmpty(SubtitleLabel.text);

        public virtual void Initialize(UIDocument document, System.Action clickedContinueAction)
        {
            Document = document;
            if (ContinueButton != null) ContinueButton.clicked += clickedContinueAction;
        }

        public override void SetActive(bool value)
        {
			GameUIToolkitDialogue.SetDisplay(SubtitlePanel, value);
            HideContinueButton();
        }

        public void OpenOnStartConversation()
        {
            SetActive(true);
			
			var conversation = DialogueManager.masterDatabase.GetConversation(DialogueManager.lastConversationStarted);
			if (conversation == null) return;
			
			SetPlayerSprite();
			SetNpcSprite(conversation);
			
            if (SubtitleLabel != null) SubtitleLabel.text = string.Empty;
        }
		
		private void SetPlayerSprite()
		{
			var playerActor = DialogueManager.MasterDatabase.GetActor(DialogueManager.masterDatabase.playerID);
			Sprite actorSpritePortrait = GetActorSprite(playerActor.Name);
			//var localizedName = PixelCrushers.DialogueSystem.CharacterInfo.GetLocalizedDisplayNameInDatabase(playerActor.Name);
			SetPortraitSprite("", actorSpritePortrait);
		}
		
		private void SetNpcSprite(Conversation conversation)
		{
			var npcActorID = conversation.ActorID == DialogueManager.masterDatabase.playerID ? conversation.ConversantID : conversation.ActorID;
			var npcActor = DialogueManager.MasterDatabase.GetActor(npcActorID);
			Sprite actorSpritePortrait = GetActorSprite(npcActor.Name);
			SetNpcPortraitSprite(actorSpritePortrait);
		}
		
		private void SetNpcSprite(Subtitle subtitle)
		{
			var npcActorID = subtitle.speakerInfo.id == DialogueManager.masterDatabase.playerID ? subtitle.listenerInfo.id : subtitle.speakerInfo.id;
			var npcActor = DialogueManager.MasterDatabase.GetActor(npcActorID);
			Sprite actorSpritePortrait = GetActorSprite(npcActor.Name);
			SetNpcPortraitSprite(actorSpritePortrait);
		}
		
		private Sprite GetActorSprite(string actorName)
		{
			var actorTransform = GameUIToolkitDialogue.GetActorTransform(actorName);
			DialogueActor dialogueActor = DialogueActor.GetDialogueActorComponent(actorTransform);
			return dialogueActor?.GetPortraitSprite();
		}

        public override void ClearSubtitle()
        {
            if (SubtitleLabel != null) SubtitleLabel.text = string.Empty;
            HideContinueButton();
        }

        public override void SetSubtitle(Subtitle subtitle)
        {
            SetActive(true);
            if (SubtitleLabel != null) SubtitleLabel.text = subtitle.formattedText.text;
			SetPlayerSprite();
            SetNpcSprite(subtitle);
        }

        public override void SetPortraitSprite(string actorName, Sprite sprite)
        {
            if (PortraitLabel != null) PortraitLabel.text = actorName;
            if (PortraitImage != null)
            {
                var hasSprite = sprite != null;
				GameUIToolkitDialogue.SetDisplay(PortraitImage, hasSprite);
                if (hasSprite) PortraitImage.style.backgroundImage = new StyleBackground(sprite);
            }
        }

		public void SetNpcPortraitSprite(Sprite sprite)
		{
			if (PortraitImage != null)
			{
				var hasSprite = sprite != null;
				GameUIToolkitDialogue.SetDisplay(ListenerPortraitImage, hasSprite);
				if (hasSprite) ListenerPortraitImage.style.backgroundImage = new StyleBackground(sprite);
			}
		}

        public override void ShowContinueButton() => GameUIToolkitDialogue.SetDisplay(ContinueButton, true);
        public override void HideContinueButton() => GameUIToolkitDialogue.SetDisplay(ContinueButton, false);
	}
}
