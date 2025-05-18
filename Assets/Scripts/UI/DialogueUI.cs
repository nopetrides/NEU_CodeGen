using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core;

public partial class DialogueUI : UIComponent
{
    // UI Elements
    private VisualElement _leftCharacter;
    private VisualElement _rightCharacter;
    private Label _speakerName;
    private Label _dialogueText;
    private VisualElement _optionsContainer;
    private List<Button> _optionButtons = new List<Button>();

    // Typewriter effect
    private bool _typewriterActive;
    private string _fullText = "";
    private string _currentText = "";
    private float _typewriterSpeed = 0.03f; // Seconds per character
    private float _lastCharTime;
    private int _currentCharIndex;
    private bool _inTag;


    // Dialogue state
    private string _currentNode;
    private Dictionary<string, DialogueNode> _dialogueNodes;

    // Event handlers
    public event Action<string> OnNodeSelected;

    public override void Initialize()
    {
        base.Initialize();

        // Set the _document field in the generated class to the Document property from the UIComponent base class
        _document = Document;

        // Initialize document using Rosalina bindings
        InitializeDocument();

        // Get references to UI elements using the properties from Rosalina bindings
        _leftCharacter = LeftCharacter;
        _rightCharacter = RightCharacter;
        _speakerName = SpeakerName;
        _dialogueText = DialogueText;
        _optionsContainer = OptionsContainer;

        // Get option buttons
        _optionButtons.Add(Option1);
        _optionButtons.Add(Option2);
        _optionButtons.Add(Option3);

        // Hide all option buttons initially
        foreach (var button in _optionButtons)
        {
            if (button != null)
            {
                button.style.display = DisplayStyle.None;
            }
        }
    }

    public void SetCharacterImage(bool isLeft, Texture2D image)
    {
        var character = isLeft ? _leftCharacter : _rightCharacter;
        if (character == null)
        {
            Debug.LogWarning($"Character visual element is null. isLeft: {isLeft}");
            return;
        }
        character.style.backgroundImage = image;
    }

    public void SetDialogue(string speaker, string text)
    {
        if (_speakerName == null)
        {
            Debug.LogWarning("Speaker name label is null");
        }
        else
        {
            _speakerName.text = speaker;
        }

        if (_dialogueText == null)
        {
            Debug.LogWarning("Dialogue text label is null");
        }
        else
        {
            // Apply keyword highlighting
            string highlightedText = HighlightKeywords(text);

            // Reset typewriter effect
            _fullText = highlightedText;
            _currentText = "";
            _currentCharIndex = 0;
            _lastCharTime = Time.time;
            _typewriterActive = true;
            _inTag = false;

            // Enable rich text support
            _dialogueText.enableRichText = true;

            // Clear the text initially
            _dialogueText.text = "";
        }
    }

    // Call this method from your Update loop
    public void UpdateTypewriter()
    {
        if (!_typewriterActive || _dialogueText == null) return;

        if (_currentCharIndex >= _fullText.Length)
        {
            _typewriterActive = false;
            return;
        }

        if (Time.time - _lastCharTime >= _typewriterSpeed)
        {
            char c = _fullText[_currentCharIndex];

            if (c == '<')
            {
                _inTag = true;
            }

            _currentText += c;
            _currentCharIndex++;

            if (c == '>')
            {
                _inTag = false;
            }

            if (!_inTag)
            {
                _lastCharTime = Time.time;
            }

            _dialogueText.text = _currentText;
        }
    }

    private string HighlightKeywords(string text)
    {
        // Define keywords to highlight with their colors
        Dictionary<string, string> keywords = new Dictionary<string, string>
        {
            { "storm", "<color=#00a0ff>storm</color>" },
            { "lightning", "<color=#ffff00>lightning</color>" },
            { "thunder", "<color=#ffff00>thunder</color>" },
            { "wind", "<color=#80c0ff>wind</color>" },
            { "relic", "<color=#ff8000>relic</color>" },
            { "Goru", "<color=#00ffff>Goru</color>" },
            { "clouds", "<color=#c0c0ff>clouds</color>" },
            { "blessing", "<color=#00ff00>blessing</color>" },
            { "wrath", "<color=#ff0000>wrath</color>" }
        };

        // Replace keywords with highlighted versions (case-insensitive)
        string result = text;
        foreach (var keyword in keywords)
        {
            result = Regex.Replace(
                result,
                $"\\b{keyword.Key}\\b",
                keyword.Value,
                RegexOptions.IgnoreCase
            );
        }

        return result;
    }

    // Method to skip the typewriter effect and show the full text immediately
    public void SkipTypewriter()
    {
        if (_typewriterActive && _dialogueText != null)
        {
            _dialogueText.text = _fullText;
            _typewriterActive = false;
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateTypewriter();
    }

    private void AnimateCharacterImage(VisualElement character, string animationType)
    {
        if (character == null) return;

        switch (animationType)
        {
            case "Shake":
                character.experimental.animation.Start(
                    0f, 1f,
                    1000,
                    (element, progress) => {
                        element.style.translate = new Translate(
                            UnityEngine.Random.Range(-5f, 5f),
                            UnityEngine.Random.Range(-5f, 5f),
                            0f
                        );
                    }
                ).OnCompleted(() => {
                    character.style.translate = new Translate(0, 0, 0);
                });
                break;

            case "Fade":
                character.experimental.animation.Start(
                    0f, 1f,
                    500,
                    (element, progress) => {
                        element.style.opacity = progress;
                    }
                );
                break;

            case "Pulse":
				character.experimental.animation.Start(
					0.9f, 1.1f,
					500,
					(element, progress) =>
					{
						element.style.scale = new StyleScale(new Vector2(progress, progress));
					}
				);
                break;
        }
    }

    public void SetOptions(List<DialogueOption> options)
    {
        if (options == null)
        {
            Debug.LogWarning("SetOptions called with null options list");
            return;
        }

        // Hide all option buttons first
        foreach (var button in _optionButtons)
        {
            if (button == null) continue;

            button.style.display = DisplayStyle.None;
            // Clear previous event handlers by recreating the clickable
            button.clickable = new Clickable(() => {});
        }

        // Show and set up option buttons based on available options
        for (int i = 0; i < options.Count && i < _optionButtons.Count; i++)
        {
            var button = _optionButtons[i];
            if (button == null) continue;

            var option = options[i];

            button.text = option.Text;
            button.style.display = DisplayStyle.Flex;

            // Store the target node in the button's userData
            button.userData = option.TargetNode;

            // Set up click handler
            button.clickable.clicked += () => OnOptionClicked(button);

            // Add hover sound
            button.RegisterCallback<MouseEnterEvent>(evt => Game.Audio.PlayUISound("ButtonHover"));
        }
    }

    private void OnOptionClicked(Button button)
    {
        if (button == null)
        {
            Debug.LogWarning("OnOptionClicked called with null button");
            return;
        }

        var targetNode = (string)button.userData;
        if (targetNode == null)
        {
            Debug.LogWarning("Button userData is null");
            return;
        }

        OnNodeSelected?.Invoke(targetNode);
    }

    public void ClearOptions()
    {
        foreach (var button in _optionButtons)
        {
            if (button == null) continue;

            button.style.display = DisplayStyle.None;
            // Clear all event handlers by recreating the clickable
            button.clickable = new Clickable(() => {});
        }
    }
}

// Data structures for dialogue system
public class DialogueNode
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Speaker { get; set; }
    public List<DialogueOption> Options { get; set; } = new List<DialogueOption>();
}

public class DialogueOption
{
    public string Text { get; set; }
    public string TargetNode { get; set; }
}
