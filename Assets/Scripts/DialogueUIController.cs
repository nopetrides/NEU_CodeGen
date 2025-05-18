using UnityEngine;
using UnityEngine.UIElements;
using PixelCrushers.DialogueSystem;

public class DialogueUIController : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _dialogueContainer;
    private Label _speakerNameLabel;
    private Label _dialogueTextLabel;
    private VisualElement _responseContainer;
    
    private void OnEnable()
    {
        // Get UI Document component
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }
    
        // Get UI elements
        var root = _document.rootVisualElement;
        _dialogueContainer = root.Q("dialogue-container");
        _speakerNameLabel = root.Q<Label>("speaker-name");
        _dialogueTextLabel = root.Q<Label>("dialogue-text");
        _responseContainer = root.Q("response-container");
    
        // Subscribe to Dialogue System events
        DialogueManager.instance.conversationStarted += OnConversationStarted;
        DialogueManager.instance.conversationEnded += OnConversationEnded;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from Dialogue System events
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationStarted -= OnConversationStarted;
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
        }
    }
    
    private void OnConversationStarted(Transform actor)
    {
        _dialogueContainer.style.display = DisplayStyle.Flex;
    }
    
    private void OnConversationEnded(Transform actor)
    {
        _dialogueContainer.style.display = DisplayStyle.None;
        _responseContainer.Clear();
    }
    
    private void OnSubtitle(Subtitle subtitle)
    {
        _speakerNameLabel.text = subtitle.speakerInfo.Name;
        _dialogueTextLabel.text = subtitle.formattedText.text;
    }
    
    private void OnResponseMenu(Response[] responses)
    {
        _responseContainer.Clear();
    
        foreach (var response in responses)
        {
            var button = new Button(() => {
                DialogueManager.instance.SendMessage("OnConversationResponse", response);
                _responseContainer.Clear();
            })
            {
                text = response.formattedText.text,
                name = "response-button"
            };
            button.AddToClassList("response-button");
            _responseContainer.Add(button);
        }
    }
} 