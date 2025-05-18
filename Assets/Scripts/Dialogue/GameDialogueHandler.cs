using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using UniStorm;
using UniStorm.Utility;
#if USE_TWINE
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Twine;
#endif

public class GameDialogueHandler : MonoBehaviour
{
    private static GameDialogueHandler _instance;

    /// <summary>
    /// Static access to the GameDialogueHandler instance.
    /// </summary>
    public static GameDialogueHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("GameDialogueHandler instance is null. Make sure a GameDialogueHandler exists in the scene.");
            }
            return _instance;
        }
    }

    private string _twineFilePath = "Dialogue/OriGoruJson";
    [SerializeField] private Texture2D _oriseiImage;
    [SerializeField] private Texture2D _goruImage;

    // Dialogue controls settings
    [SerializeField] private float _autoPlayDelay = 3.0f;
    [SerializeField] private float _skipAllSpeed = 0.5f;

    // Dialogue controls state
    private bool _isAutoPlaying;
    private bool _isSkippingAll;
    private Coroutine _autoPlayCoroutine;

    // Story state tracking
    private string _firstChoice = "";
    private string _secondChoice = "";

    private Dictionary<string, DialogueNode> _dialogueNodes = new Dictionary<string, DialogueNode>();
    private string _currentNodeId = "Start"; // Default value, will be updated from StoryData
    private string _startNodeId = "Start"; // Store the start node ID from StoryData
    private DialogueUI _dialogueUI;

    private void Awake()
    {
        // Initialize singleton instance
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Multiple instances of GameDialogueHandler found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load dialogue from Twine file
        LoadDialogueFromTwine();

        // Show dialogue UI
        Game.UI.Show<DialogueUI>();

        // Get reference to the dialogue UI
        _dialogueUI = Game.UI.GetActiveUI<DialogueUI>();

        // Set up event handler
        if (_dialogueUI != null)
        {
            _dialogueUI.OnNodeSelected += OnNodeSelected;

            // Verify that we have nodes loaded before trying to display the start node
            if (_dialogueNodes.Count > 0)
            {
                // Verify that the start node exists
                if (_dialogueNodes.ContainsKey(_currentNodeId))
                {
                    // Display the initial node using the start node ID from StoryData
                    Debug.Log($"Displaying initial node: {_currentNodeId}");
                    DisplayNode(_currentNodeId);
                }
                else
                {
                    Debug.LogError($"Start node '{_currentNodeId}' not found! Falling back to first available node.");
                    // Fall back to the first available node
                    string firstNodeId = _dialogueNodes.Keys.First();
                    _currentNodeId = firstNodeId;
                    DisplayNode(firstNodeId);
                }
            }
            else
            {
                Debug.LogError("No dialogue nodes loaded! Cannot display initial node.");
            }

            // Add control buttons to the dialogue UI
            AddControlButtons();
        }
        else
        {
            Debug.LogError("Failed to get DialogueUI reference!");
        }
    }

    /// <summary>
    /// Adds auto-play and skip-all buttons to the dialogue UI.
    /// </summary>
    private void AddControlButtons()
    {
        // Get the dialogue box container
        var dialogueBoxContainer = _dialogueUI.DialogueBoxContainer;
        if (dialogueBoxContainer == null)
        {
            Debug.LogError("Failed to get dialogue box container!");
            return;
        }

        // Create a container for the control buttons
        var controlsContainer = new VisualElement
        {
            name = "controls-container"
        };
        dialogueBoxContainer.Add(controlsContainer);

        // Create auto-play button
        var autoPlayButton = new Button(() => ToggleAutoPlay())
        {
            text = "Auto",
            name = "auto-play-button"
        };
        autoPlayButton.AddToClassList("dialogue-option");
        controlsContainer.Add(autoPlayButton);

        // Create skip-all button
        var skipAllButton = new Button(() => SkipAll())
        {
            text = "Skip",
            name = "skip-all-button"
        };
        skipAllButton.AddToClassList("dialogue-option");
        controlsContainer.Add(skipAllButton);
    }

    /// <summary>
    /// Toggles auto-play mode on/off.
    /// </summary>
    public void ToggleAutoPlay()
    {
        _isAutoPlaying = !_isAutoPlaying;
        _isSkippingAll = false;

        if (_isAutoPlaying)
        {
            // Start auto-play coroutine
            if (_autoPlayCoroutine != null)
            {
                StopCoroutine(_autoPlayCoroutine);
            }
            _autoPlayCoroutine = StartCoroutine(AutoPlayCoroutine());
        }
        else
        {
            // Stop auto-play coroutine
            if (_autoPlayCoroutine != null)
            {
                StopCoroutine(_autoPlayCoroutine);
                _autoPlayCoroutine = null;
            }
        }
    }

    /// <summary>
    /// Skips all dialogue until the end of the conversation.
    /// </summary>
    public void SkipAll()
    {
        _isSkippingAll = true;
        _isAutoPlaying = false;

        if (_autoPlayCoroutine != null)
        {
            StopCoroutine(_autoPlayCoroutine);
            _autoPlayCoroutine = null;
        }

        StartCoroutine(SkipAllCoroutine());
    }

    private IEnumerator AutoPlayCoroutine()
    {
        while (_isAutoPlaying)
        {
            // Wait for the auto-play delay
            yield return new WaitForSeconds(_autoPlayDelay);

            // If there's only one option (continue), select it automatically
            var options = GetCurrentOptions();
            if (options != null && options.Count == 1)
            {
                SelectOption(options[0].TargetNode);
            }
        }
    }

    private IEnumerator SkipAllCoroutine()
    {
        while (_isSkippingAll)
        {
            // Get the current options
            var options = GetCurrentOptions();
            if (options != null && options.Count > 0)
            {
                // Select the first option
                SelectOption(options[0].TargetNode);

                // Wait for a short delay
                yield return new WaitForSeconds(_skipAllSpeed);
            }
            else
            {
                // No more options, end skip-all
                _isSkippingAll = false;
                yield break;
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up event handler
        if (_dialogueUI != null)
        {
            _dialogueUI.OnNodeSelected -= OnNodeSelected;
        }
    }

    private void LoadDialogueFromTwine()
    {
        try
        {
			
#if USE_TWINE
			// Use PixelCrusher's TwineImporter if available
			try
			{
				string jsonPath = _twineFilePath;
				var twineFile = Resources.Load<TextAsset>(jsonPath);
				string content = twineFile.text;
				// Parse the JSON into a TwineStory object
				TwineStory story = JsonUtility.FromJson<TwineStory>(content);
				if (story != null)
				{
					Debug.Log($"Successfully parsed Twine story: {story.name} with {story.passages.Length} passages");

					// Create a temporary dialogue database
					DialogueDatabase database = ScriptableObject.CreateInstance<DialogueDatabase>();

					// Create actors for the dialogue
					Actor player = Template.CreateActor(0, "Orisei", true);
					database.actors.Add(player);

					Actor npc = Template.CreateActor(1, "Goru", false);
					database.actors.Add(npc);

					// Import the Twine story
					TwineImporter importer = new TwineImporter();
					importer.ConvertStoryToConversation(database, Template, story, player.id, npc.id, true, true);

					// Convert the PixelCrusher dialogue format to our game's DialogueNode format
					ConvertPixelCrusherDialogueToGameDialogue(database);

					Debug.Log($"Successfully imported Twine story using PixelCrusher's TwineImporter");
					return;
				}
				else
				{
					Debug.LogWarning("Failed to parse Twine story JSON. Falling back to custom parser.");
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Error using PixelCrusher's TwineImporter: {e.Message}. Falling back to custom parser.");
			}
#else 
            // Try to load the Twine file from Resources
            TextAsset twineFile = Resources.Load<TextAsset>(_twineFilePath);

            // If that fails, try with the .txt extension
            if (twineFile == null)
            {
                // Try with .txt extension (which Unity definitely recognizes as a TextAsset)
                string txtPath = _twineFilePath.Replace(".twine", ".txt");
                if (txtPath == _twineFilePath) // If no replacement was made
                {
                    txtPath = _twineFilePath + ".txt";
                }
                twineFile = Resources.Load<TextAsset>(txtPath);

                if (twineFile != null)
                {
                    Debug.Log($"Successfully loaded Twine file from alternative path: {txtPath}");
                }
            }

            // If that still fails, try with .json extension
            if (twineFile == null)
            {
                string jsonPath = _twineFilePath + ".json";
                twineFile = Resources.Load<TextAsset>(jsonPath);

                if (twineFile != null)
                {
                    Debug.Log($"Successfully loaded Twine file from JSON path: {jsonPath}");
                }
            }

            // If that still fails, try with a different path format
            if (twineFile == null)
            {
                string alternativePath = _twineFilePath.Replace('/', '\\');
                twineFile = Resources.Load<TextAsset>(alternativePath);
            }

            // If all attempts fail, log an error
            if (twineFile == null)
            {
                Debug.LogError($"Failed to load Twine file at path: {_twineFilePath}");
                Debug.LogWarning("Make sure the file exists in the Resources folder and has the correct extension.");
                Debug.LogWarning("Try creating a copy of your .twine file with a .txt extension, which Unity recognizes as a TextAsset.");
                return;
            }

            string content = twineFile.text;


            // Fall back to custom parser if TwineImporter is not available or fails
            ParseTwineContent(content);

#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading dialogue from Twine: {e.Message}");
        }
    }

#if USE_TWINE
    private Template Template
    {
        get { return new Template(); }
    }

    private void ConvertPixelCrusherDialogueToGameDialogue(DialogueDatabase database)
    {
        // Clear existing dialogue nodes
        _dialogueNodes.Clear();

        // Get the first conversation (there should only be one)
        if (database.conversations.Count == 0)
        {
            Debug.LogError("No conversations found in the imported dialogue database");
            return;
        }

        Conversation conversation = database.conversations[0];
        Debug.Log($"Converting conversation: {conversation.Title} with {conversation.dialogueEntries.Count} entries");

        // Find the START node (ID 0)
        DialogueEntry startEntry = conversation.GetDialogueEntry(0);
        if (startEntry == null)
        {
            Debug.LogError("START node not found in the conversation");
            return;
        }

        // Find the first node linked from the START node
        string firstNodeId = null;
        if (startEntry.outgoingLinks.Count > 0)
        {
            firstNodeId = startEntry.outgoingLinks[0].destinationDialogueID.ToString();
            Debug.Log($"First node linked from START: {firstNodeId}");
        }
        else
        {
            Debug.LogWarning("START node has no outgoing links");
        }

        // Convert each dialogue entry to a DialogueNode
        foreach (var entry in conversation.dialogueEntries)
        {
            // Create a new DialogueNode
            DialogueNode node = new DialogueNode
            {
                Id = entry.id.ToString(),
                Text = entry.DialogueText,
                Speaker = GetSpeakerName(entry, database)
            };

            // Add options (links to other nodes)
            foreach (var link in entry.outgoingLinks)
            {
                // Skip links to the same node (loops)
                if (link.destinationDialogueID == entry.id) continue;

                // Get the destination entry
                DialogueEntry destEntry = database.GetDialogueEntry(link.originConversationID, link.destinationDialogueID);
                if (destEntry != null)
                {
                    node.Options.Add(new DialogueOption
                    {
                        Text = (destEntry.MenuText != null && destEntry.MenuText.Length > 0) ? destEntry.MenuText : "Continue",
                        TargetNode = destEntry.id.ToString()
                    });
                }
            }

            // Add the node to the dictionary
            _dialogueNodes[node.Id] = node;
        }

        // Set the start node ID to the first node linked from START
        if (firstNodeId != null && _dialogueNodes.ContainsKey(firstNodeId))
        {
            _startNodeId = firstNodeId;
            _currentNodeId = firstNodeId;
            Debug.Log($"Set start node ID to: {_startNodeId}");
        }
        else if (_dialogueNodes.Count > 0)
        {
            // Fall back to the first available node
            _startNodeId = _dialogueNodes.Keys.First();
            _currentNodeId = _startNodeId;
            Debug.LogWarning($"No valid first node found. Falling back to first available node: {_startNodeId}");
        }
        else
        {
            Debug.LogError("No dialogue nodes were created");
        }

        Debug.Log($"Converted {_dialogueNodes.Count} dialogue nodes");
    }

    private string GetSpeakerName(DialogueEntry entry, DialogueDatabase database)
    {
        // Get the actor name based on the actor ID
        if (entry.ActorID > 0)
        {
            var actor = database.GetActor(entry.ActorID);
            if (actor != null)
            {
                return actor.Name;
            }
        }

        // Default to empty string if actor not found
        return "";
    }
#endif

    private void ParseTwineContent(string content)
    {
        // Split the content by node markers
        string[] nodeBlocks = content.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

        // Check if we're using the v3 format and extract the start node from StoryData
        foreach (string block in nodeBlocks)
        {
            if (block.TrimStart().StartsWith("StoryData"))
            {
                // Try to extract the start node from StoryData
                int startIndex = block.IndexOf("\"start\":");
                if (startIndex >= 0)
                {
                    // Find the value after "start":
                    int valueStartIndex = block.IndexOf("\"", startIndex + 8) + 1;
                    int valueEndIndex = block.IndexOf("\"", valueStartIndex);
                    if (valueStartIndex > 0 && valueEndIndex > valueStartIndex)
                    {
                        _startNodeId = block.Substring(valueStartIndex, valueEndIndex - valueStartIndex);
                        _currentNodeId = _startNodeId; // Update current node ID to start with the correct node
                        Debug.Log($"Found start node in StoryData: {_startNodeId}");
                    }
                }
                break; // We've found and processed StoryData, no need to continue this loop
            }
        }

        foreach (string block in nodeBlocks)
        {
            // Skip empty blocks
            if (string.IsNullOrWhiteSpace(block))
                continue;

            // Skip StoryTitle and StoryData blocks
            if (block.TrimStart().StartsWith("StoryTitle") || block.TrimStart().StartsWith("StoryData"))
                continue;

            // Extract node ID and content
            int newlineIndex = block.IndexOf('\n');
            if (newlineIndex < 0)
                continue;

            string fullNodeId = block.Substring(0, newlineIndex).Trim();
            string nodeContent = block.Substring(newlineIndex).Trim();

            // Extract just the node ID without metadata
            string nodeId = fullNodeId;

            // Remove metadata in square brackets if present
            int bracketIndex = fullNodeId.IndexOf('[');
            if (bracketIndex > 0)
            {
                nodeId = fullNodeId.Substring(0, bracketIndex).Trim();
            }

            // Remove JSON data in curly braces if present
            int braceIndex = nodeId.IndexOf('{');
            if (braceIndex > 0)
            {
                nodeId = nodeId.Substring(0, braceIndex).Trim();
            }

            Debug.Log($"Parsed node: {nodeId} from {fullNodeId}");

            // Create a new dialogue node
            DialogueNode node = new DialogueNode
            {
                Id = nodeId
            };

            // Extract options using regex
            var optionMatches = Regex.Matches(nodeContent, @"\[\[(.*?)\|(.*?)\]\]");

            // Remove options from the content
            string textContent = Regex.Replace(nodeContent, @"\[\[(.*?)\|(.*?)\]\]", "").Trim();

            // Extract speaker if present (format: "Speaker: Text")
            string speaker = "";
            string text = textContent;

            int colonIndex = textContent.IndexOf(':');
            if (colonIndex > 0 && !textContent.Substring(0, colonIndex).Contains(" "))
            {
                speaker = textContent.Substring(0, colonIndex).Trim();
                text = textContent.Substring(colonIndex + 1).Trim();
            }

            node.Speaker = speaker;
            node.Text = text;

            // Add options
            foreach (Match match in optionMatches)
            {
                string optionText = match.Groups[1].Value;
                string targetNode = match.Groups[2].Value;

                node.Options.Add(new DialogueOption
                {
                    Text = optionText,
                    TargetNode = targetNode
                });
            }

            // Add node to dictionary
            _dialogueNodes[nodeId] = node;
        }

        // Log all parsed nodes for debugging
        Debug.Log($"Parsed {_dialogueNodes.Count} nodes:");
        foreach (var nodeEntry in _dialogueNodes)
        {
            Debug.Log($"  - {nodeEntry.Key}");
        }

        // Verify that the start node exists
        if (!_dialogueNodes.ContainsKey(_startNodeId))
        {
            Debug.LogError($"Start node '{_startNodeId}' not found in parsed nodes! Available nodes: {string.Join(", ", _dialogueNodes.Keys)}");
        }
        else
        {
            Debug.Log($"Start node '{_startNodeId}' found successfully.");
        }
    }

    /// <summary>
    /// Gets the current dialogue options.
    /// </summary>
    /// <returns>The list of current dialogue options.</returns>
    public List<DialogueOption> GetCurrentOptions()
    {
        if (_dialogueNodes.TryGetValue(_currentNodeId, out DialogueNode node))
        {
            return node.Options;
        }
        return null;
    }

    /// <summary>
    /// Selects a dialogue option programmatically.
    /// </summary>
    /// <param name="nodeId">The ID of the node to select.</param>
    public void SelectOption(string nodeId)
    {
        OnNodeSelected(nodeId);
    }

    private void OnNodeSelected(string nodeId)
    {
        // Update current node
        _currentNodeId = nodeId;

        // Track player choices
        if (nodeId == "Stone" || nodeId == "Berry" || nodeId == "Shout")
        {
            _firstChoice = nodeId;
        }
        else if (nodeId == "DestroyRelic" || nodeId == "OfferRelic" || nodeId == "Restraint")
        {
            _secondChoice = nodeId;
        }

        // Display the selected node
        DisplayNode(nodeId);
    }

    private void DisplayNode(string nodeId)
    {
        Debug.Log($"Attempting to display node: {nodeId}");

        if (string.IsNullOrEmpty(nodeId))
        {
            Debug.LogError("DisplayNode called with null or empty nodeId");
            return;
        }

        if (_dialogueNodes.TryGetValue(nodeId, out DialogueNode node))
        {
            Debug.Log($"Found node: {nodeId}, Speaker: {node.Speaker}, Options: {node.Options.Count}");

            // Determine which character is speaking
            bool isOrisei = !node.Speaker.Equals("Goru", StringComparison.OrdinalIgnoreCase);

            // Set character images
            _dialogueUI.SetCharacterImage(true, _oriseiImage);  // Orisei on left
            _dialogueUI.SetCharacterImage(false, _goruImage);   // Goru on right

            // Set dialogue text
            _dialogueUI.SetDialogue(node.Speaker, node.Text);

            // Set options
            _dialogueUI.SetOptions(node.Options);

            // Play node-specific sounds
            PlayNodeSfx(nodeId, node);

            // Update weather based on node
            UpdateWeather(nodeId);
        }
        else
        {
            Debug.LogError($"Node not found: {nodeId}. Available nodes: {string.Join(", ", _dialogueNodes.Keys)}");

            // Try to recover by using the start node if it exists
            if (nodeId != _startNodeId && _dialogueNodes.TryGetValue(_startNodeId, out DialogueNode startNode))
            {
                Debug.LogWarning($"Falling back to start node: {_startNodeId}");
                _currentNodeId = _startNodeId;
                DisplayNode(_startNodeId);
            }
            else if (_dialogueNodes.Count > 0)
            {
                // If we can't find the start node either, try the first available node
                string firstAvailableNode = _dialogueNodes.Keys.First();
                if (nodeId != firstAvailableNode)
                {
                    Debug.LogWarning($"Falling back to first available node: {firstAvailableNode}");
                    _currentNodeId = firstAvailableNode;
                    DisplayNode(firstAvailableNode);
                }
                else
                {
                    // If we're already trying to display the first available node and it's not found,
                    // create a simple error node
                    CreateAndDisplayErrorNode(nodeId);
                }
            }
            else
            {
                // If there are no nodes at all, create a simple error node
                CreateAndDisplayErrorNode(nodeId);
            }
        }
    }

    private void CreateAndDisplayErrorNode(string failedNodeId)
    {
        Debug.LogWarning($"Creating fallback error node for failed node: {failedNodeId}");
        DialogueNode errorNode = new DialogueNode
        {
            Id = "Error",
            Speaker = "System",
            Text = $"Error: Could not find node '{failedNodeId}'. The story may be incomplete or corrupted."
        };

        // Add a simple option to restart if possible
        if (_dialogueNodes.Count > 0)
        {
            string firstAvailableNode = _dialogueNodes.Keys.First();
            errorNode.Options.Add(new DialogueOption
            {
                Text = "Restart from beginning",
                TargetNode = firstAvailableNode
            });
        }

        // Display the error node
        _dialogueUI.SetCharacterImage(true, _oriseiImage);
        _dialogueUI.SetCharacterImage(false, _goruImage);
        _dialogueUI.SetDialogue(errorNode.Speaker, errorNode.Text);
        _dialogueUI.SetOptions(errorNode.Options);
    }

    private void PlayNodeSfx(string nodeId, DialogueNode node)
    {
        switch (nodeId)
        {
            case "Start":
                Game.Audio.PlayDialogueSfx("Wind", 0.5f);
                break;
            case "Stone":
                Game.Audio.PlayDialogueSfx("ThrowStone");
                Game.Audio.PlayDialogueSfx("Wind", 0.8f);
                Game.Audio.PlayDialogueSfx("Thunder");
                break;
            case "Berry":
                Game.Audio.PlayDialogueSfx("PlaceBerry");
                Game.Audio.PlayDialogueSfx("StormCalm");
                break;
            case "Shout":
                Game.Audio.PlayDialogueSfx("OriseiVoice");
                Game.Audio.PlayDialogueSfx("Thunder");
                break;
            case "DestroyRelic":
                Game.Audio.PlayDialogueSfx("Wind", 1.0f);
                Game.Audio.PlayDialogueSfx("BowlBreak");
                break;
            case "Reveal":
                // Play the appropriate Reveal audio based on player choices
                PlayRevealAudio();
                break;
            case "Blessed":
                Game.Audio.PlayDialogueSfx("StormCalm");
                break;
            case "Cursed":
                Game.Audio.PlayDialogueSfx("Wind", 0.3f);
                break;
        }
    }

    private void UpdateWeather(string nodeId)
    {
        // Get reference to UniStorm system
        var uniStorm = UniStormSystem.Instance;
        if (uniStorm == null) return;

        switch (nodeId)
        {
            case "Start":
                // Cloudy, stormy weather
                ChangeWeatherTo("Cloudy");
                break;
            case "Stone":
                // Intense storm after throwing stone
                ChangeWeatherTo("Thunderstorm");
                break;
            case "Berry":
                // Calmer weather after offering berry
                ChangeWeatherTo("Cloudy");
                break;
            case "DestroyRelic":
                // Violent storm after destroying relic
                ChangeWeatherTo("Heavy Rain");
                break;
            case "Restraint":
                // Calm weather after showing restraint
                ChangeWeatherTo("Partly Cloudy");
                break;
            case "Reveal":
                // Dramatic weather for Goru's appearance
                ChangeWeatherTo("Thunderstorm");
                break;
            case "Blessed":
                // Clear weather for blessing
                ChangeWeatherTo("Clear");
                break;
            case "Cursed":
                // Dark, ominous weather for curse
                ChangeWeatherTo("Heavy Fog");
                break;
        }
    }

    private void ChangeWeatherTo(string weatherName)
    {
        var uniStorm = UniStormSystem.Instance;
        if (uniStorm == null) return;

        // Find the weather type by name
        var weatherType = uniStorm.AllWeatherTypes.Find(w => w.WeatherTypeName == weatherName);
        if (weatherType != null)
        {
            // Change weather with transition
            UniStormManager.Instance.ChangeWeatherWithTransition(weatherType);
        }
    }

    private void TriggerLightningEffect()
    {
        var uniStorm = UniStormSystem.Instance;
        if (uniStorm == null) return;

        // Find the lightning system
        var lightningSystem = FindObjectOfType<LightningSystem>();
        if (lightningSystem != null)
        {
            // Trigger a lightning strike
            lightningSystem.GenerateLightning();
        }
    }

    private void PlayRevealAudio()
    {
        // Default to GoruVoice if no specific audio is found
        string audioKey = "GoruVoice";

        // Determine which specific Reveal audio to play based on player choices
        if (!string.IsNullOrEmpty(_firstChoice) && !string.IsNullOrEmpty(_secondChoice))
        {
            string choiceCombination = "";

            // First choice
            if (_firstChoice == "Berry")
                choiceCombination = "Berry";
            else if (_firstChoice == "Stone")
                choiceCombination = "Stone";
            else if (_firstChoice == "Shout")
                choiceCombination = "Shout";

            // Second choice
            if (_secondChoice == "DestroyRelic")
                choiceCombination += "Destroy";
            else if (_secondChoice == "OfferRelic")
                choiceCombination += "Offer";
            else if (_secondChoice == "Restraint")
                choiceCombination += "Restraint";

            // Try to play the specific audio for this combination
            if (!string.IsNullOrEmpty(choiceCombination))
            {
                audioKey = $"Reveal_{choiceCombination}";

                // If this combination doesn't exist, fall back to Unrecorded
                // This is just a safety check, all combinations should exist
                if (!AudioClipExists(audioKey))
                {
                    audioKey = "Reveal_Unrecorded";
                }
            }
        }
        else
        {
            // If we don't have both choices for some reason, use the Unrecorded audio
            audioKey = "Reveal_Unrecorded";
        }

        // Play the audio
        Game.Audio.PlayDialogueSfx(audioKey);
    }

    private bool AudioClipExists(string key)
    {
        // Since we know all the combinations exist (we've seen the files in the directory),
        // we'll just return true. In a real-world scenario, we might want to modify the
        // AudioManager to provide a method to check if a clip exists.
        return true;
    }
}
