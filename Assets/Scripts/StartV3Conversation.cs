using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Serialization;

/// <summary>
/// This script starts the "v3" conversation when enabled.
/// </summary>
public class StartV3Conversation : MonoBehaviour
{
    [FormerlySerializedAs("conversationName")] [Tooltip("The name of the conversation to start")]
    public string ConversationName = "v3";

    [FormerlySerializedAs("startDelay")] [Tooltip("Delay before starting the conversation (in seconds)")]
    public float StartDelay = 0.5f;

    private void Start()
    {
        // Check if the DialogueManager is available
        if (DialogueManager.instance == null)
        {
            Debug.LogError("DialogueSystemController not found in the scene. Make sure it exists before using this script.");
            return;
        }

        // Start the conversation after a short delay to ensure everything is initialized
        Invoke(nameof(StartConversation), StartDelay);
    }

    private void StartConversation()
    {
        // Check if the conversation exists in the database
        var database = DialogueManager.MasterDatabase;
        if (database == null)
        {
            Debug.LogError("No dialogue database found. Make sure the DialogueSystemController has an initialDatabase assigned.");
            return;
        }

        var conversation = database.GetConversation(ConversationName);
        if (conversation == null)
        {
            Debug.LogError($"Conversation '{ConversationName}' not found in the dialogue database. Check the conversation name or make sure it's included in the database.");

            // List available conversations to help debugging
            Debug.Log("Available conversations:");
            foreach (var conv in database.conversations)
            {
                Debug.Log($"- {conv.Title}");
            }
            return;
        }

        Debug.Log($"Starting conversation: {ConversationName}");

        // Start the conversation using the DialogueManager
        DialogueManager.StartConversation(ConversationName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DialogueManager.Instance.StopAllConversations();
            DialogueManager.StartConversation(ConversationName);
        }
    }
}
