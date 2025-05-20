using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// This script starts the "v3" conversation when enabled.
/// </summary>
public class StartV3Conversation : MonoBehaviour
{
    [Tooltip("The name of the conversation to start")]
    public string conversationName = "v3";

    [Tooltip("Delay before starting the conversation (in seconds)")]
    public float startDelay = 0.5f;

    private void Start()
    {
        // Check if the DialogueManager is available
        if (DialogueManager.instance == null)
        {
            Debug.LogError("DialogueSystemController not found in the scene. Make sure it exists before using this script.");
            return;
        }

        // Start the conversation after a short delay to ensure everything is initialized
        Invoke(nameof(StartConversation), startDelay);
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

        var conversation = database.GetConversation(conversationName);
        if (conversation == null)
        {
            Debug.LogError($"Conversation '{conversationName}' not found in the dialogue database. Check the conversation name or make sure it's included in the database.");

            // List available conversations to help debugging
            Debug.Log("Available conversations:");
            foreach (var conv in database.conversations)
            {
                Debug.Log($"- {conv.Title}");
            }
            return;
        }

        Debug.Log($"Starting conversation: {conversationName}");

        // Start the conversation using the DialogueManager
        DialogueManager.StartConversation(conversationName);
    }
}
