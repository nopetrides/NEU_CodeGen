using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Dialogue
{
	[Serializable]
	public class GameUIToolkitResponseMenuElements : AbstractUIResponseMenuControls
	{
		[Tooltip("Container panel for response menu.")]
        [SerializeField] private string _responseMenuPanelName;
        [Tooltip("List of all available response buttons. The dialogue UI will use these to fill out the menu.")]
        [SerializeField] private List<string> _responseButtonNames;

        protected UIDocument Document { get; set; }
        public override AbstractUISubtitleControls subtitleReminderControls => null;
        protected VisualElement ResponseMenuPanel => GameUIToolkitDialogue.GetVisualElement<VisualElement>(Document, _responseMenuPanelName);
		protected ProgressBar TimerProgressBar => null;
		protected Label PortraitLabel => null;
		protected VisualElement PortraitImage => null;
        protected virtual Button GetResponseButton(int index) => GameUIToolkitDialogue.GetVisualElement<Button>(Document, _responseButtonNames[index]);

        protected float TimerSecondsMax { get; set; }
        protected float TimerSecondsLeft { get;set; }
        protected System.Action<object> ClickedResponseAction { get; set; }

        protected Dictionary<int, Response> ResponsesByButtonIndex = new Dictionary<int, Response>();

        public virtual void Initialize(UIDocument document, System.Action<object> clickedResponseAction)
        {
            Document = document;
            ClickedResponseAction = clickedResponseAction;
			GameUIToolkitDialogue.SetDisplay(ResponseMenuPanel, false);
            for (int i = 0; i < _responseButtonNames.Count; i++)
            {
                var index = i;
				var button = GetResponseButton(i);
                button.clicked += () => OnClickResponse(index);
				button.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
            }
        }

        public virtual void DoUpdate()
        {
            UpdateTimer();
        }

        public override void SetActive(bool value)
        {
			GameUIToolkitDialogue.SetDisplay(ResponseMenuPanel, value);
			GameUIToolkitDialogue.SetDisplay(TimerProgressBar, false);
        }

        public override void SetPCPortrait(Sprite sprite, string portraitName)
        {
            if (PortraitLabel != null) PortraitLabel.text = portraitName;
            if (PortraitImage != null)
            {
                var hasSprite = sprite != null;
				GameUIToolkitDialogue.SetDisplay(PortraitImage, hasSprite);
                if (hasSprite) PortraitImage.style.backgroundImage = new StyleBackground(sprite);
            }
        }

        protected override void ClearResponseButtons()
        {
            ResponsesByButtonIndex.Clear();
            for (int i = 0; i < _responseButtonNames.Count; i++)
            {
				GameUIToolkitDialogue.SetDisplay(GetResponseButton(i), false);
            }
        }

        public override void ShowResponses(Subtitle subtitle, Response[] responses, Transform target)
        {
            if ((responses != null) && (responses.Length > 0))
            {
                ClearResponseButtons();
                SetResponseButtons(responses, target);
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected override void SetResponseButtons(Response[] responses, Transform target)
        {
            var maxResponses = Mathf.Min(responses.Length, _responseButtonNames.Count);
            int numUnusedButtons = _responseButtonNames.Count - maxResponses;

            // Fill in buttons using specified positions & alignment:
            for (int i = 0; i < responses.Length; i++)
            {
                var response = responses[i];
                var index = (response.formattedText.position != FormattedText.NoAssignedPosition)
                    ? response.formattedText.position
                    : (buttonAlignment == ResponseButtonAlignment.ToFirst)
                        ? i
                        : numUnusedButtons + i;

                ResponsesByButtonIndex[index] = response;
                var button = GetResponseButton(index);
                if (button == null) continue;
                button.text = response.formattedText.text;
				GameUIToolkitDialogue.SetDisplay(button, true);
            }

            // If specified, show unused buttons with no text:
            if (showUnusedButtons)
            {
                var firstUnusedIndex = (buttonAlignment == ResponseButtonAlignment.ToFirst) ? maxResponses : 0;
                for (int i = firstUnusedIndex; i < (firstUnusedIndex + numUnusedButtons); i++)
                {
                    var button = GetResponseButton(i);
                    if (button == null) continue;
                    button.text = string.Empty;
					GameUIToolkitDialogue.SetDisplay(button, true);
                }
            }
        }

        protected virtual void OnClickResponse(int index)
        {
			Game.Audio.PlayUISound("ButtonPress");
            if (ResponsesByButtonIndex.TryGetValue(index, out var response))
            {
                Hide();
                ClickedResponseAction(response);
            }
        }

        public override void StartTimer(float timeout)
        {
            if (TimerProgressBar == null) return;
			GameUIToolkitDialogue.SetDisplay(TimerProgressBar, true);
            TimerSecondsLeft = TimerSecondsMax = timeout;
            TimerProgressBar.value = 1;
        }

        protected virtual void UpdateTimer()
        {
            if (TimerSecondsMax <= 0) return;
            TimerSecondsLeft -= DialogueTime.deltaTime;
            TimerProgressBar.value = Mathf.Clamp01(TimerSecondsLeft / TimerSecondsMax);
            Debug.Log($"{TimerSecondsLeft} --> {TimerProgressBar.value}");

            if (TimerSecondsLeft <= 0)
            {
                TimerSecondsMax = 0;
                OnTimedOut();
            }
        }

        private void OnTimedOut()
        {
            DialogueManager.instance.SendMessage(DialogueSystemMessages.OnConversationTimeout);
        }
	}
}
