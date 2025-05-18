using UnityEngine;
using UnityEditor;
using System.IO;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Twine;

/// <summary>
/// This script demonstrates how to programmatically import a Twine story into the Dialogue System.
/// </summary>
public class TwineImportExample : EditorWindow
{
    [MenuItem("Tools/Pixel Crushers/Dialogue System/Examples/Import Sample Twine Story", false, 100)]
    public static void ImportSampleTwineStory()
    {
        // Make sure Twine import is enabled
        #if !USE_TWINE
        if (EditorUtility.DisplayDialog("Enable Twine Support", "Twine import support isn't enabled yet. Would you like to enable it? After clicking Enable, you'll need to wait for scripts to recompile before trying again.", "Enable", "Cancel"))
        {
            EditorTools.TryAddScriptingDefineSymbols("USE_TWINE");
            return; // Return and let the user try again after recompilation
        }
        else
        {
            return; // User cancelled
        }
        #endif

        // Get or create a dialogue database
        DialogueDatabase database = GetOrCreateDialogueDatabase();
        if (database == null) return;

        // Load the sample Twine story JSON
        string jsonPath = Application.dataPath + "/Resources/Dialogue/SampleTwineStory.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Sample Twine story JSON not found at: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        
        // Parse the JSON into a TwineStory object
        TwineStory story = JsonUtility.FromJson<TwineStory>(json);
        if (story == null)
        {
            Debug.LogError("Failed to parse Twine story JSON");
            return;
        }

        // Get or create actor and conversant
        Actor player = database.GetActor("Player");
        if (player == null)
        {
            player = Template.CreateActor(Template.GetNextActorID(database), "Player", true);
            database.actors.Add(player);
        }

        Actor npc = database.GetActor("NPC");
        if (npc == null)
        {
            npc = Template.CreateActor(Template.GetNextActorID(database), "NPC", false);
            database.actors.Add(npc);
        }

        // Import the Twine story
        TwineImporter importer = new TwineImporter();
        importer.ConvertStoryToConversation(database, Template, story, player.id, npc.id, true, true);

        // Save the database
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log("Sample Twine story imported successfully!");
    }

    private static DialogueDatabase GetOrCreateDialogueDatabase()
    {
        // Try to find an existing dialogue database
        string[] guids = AssetDatabase.FindAssets("t:DialogueDatabase");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<DialogueDatabase>(path);
        }

        // Create a new dialogue database
        string databasePath = "Assets/Dialogue Database.asset";
        DialogueDatabase database = ScriptableObject.CreateInstance<DialogueDatabase>();
        AssetDatabase.CreateAsset(database, databasePath);
        return database;
    }

    private static Template Template
    {
        get { return TemplateTools.LoadFromEditorPrefs(); }
    }
}