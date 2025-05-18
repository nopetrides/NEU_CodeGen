# Twine Import for PixelCrusher Dialogue System

This implementation enables the import of Twine stories into the PixelCrusher Dialogue System for Unity. It leverages the existing Twine import functionality in the Dialogue System, which is disabled by default.

## Files Added

1. **TwineImportGuide.cs**: A MonoBehaviour script that provides detailed instructions on how to import Twine stories into the Dialogue System. Add this component to a GameObject in your scene to view the instructions in the Inspector.

2. **SampleTwineStory.json**: A sample Twine story in JSON format (Twison output) that demonstrates various features of Twine stories, including:
   - Multiple passages with links
   - Actor specification
   - Macros
   - Formatting
   - Sequences, conditions, scripts, and descriptions

3. **TwineImportExample.cs**: An Editor script that demonstrates how to programmatically import a Twine story into the Dialogue System. It adds a menu item "Tools/Pixel Crushers/Dialogue System/Examples/Import Sample Twine Story" that imports the sample Twine story.

## How to Use

### Manual Import

1. In Unity, select Tools > Pixel Crushers > Dialogue System > Import > Twine 2 (Twison)
2. The first time you select this menu item, the Dialogue System will ask if you want to enable Twine import capability
3. Click Enable
4. After the Dialogue System has recompiled with Twine import capability enabled, select the menu item again
5. In the Twine Import window, assign a dialogue database to the Database field
6. Add each JSON file to the JSON Files list
7. Select the story's primary actor and conversant from the dropdown menus
8. Check 'Split Pipes Into Entries' if you want pipe characters (|) to split passages into multiple dialogue entry nodes
9. Click the Import button

### Programmatic Import

You can also import Twine stories programmatically using the TwineImporter class. See the TwineImportExample.cs script for a complete example.

```csharp
// Get or create a dialogue database
DialogueDatabase database = GetOrCreateDialogueDatabase();

// Load the Twine story JSON
string json = File.ReadAllText(jsonPath);

// Parse the JSON into a TwineStory object
TwineStory story = JsonUtility.FromJson<TwineStory>(json);

// Get or create actor and conversant
Actor player = database.GetActor("Player");
Actor npc = database.GetActor("NPC");

// Import the Twine story
TwineImporter importer = new TwineImporter();
importer.ConvertStoryToConversation(database, template, story, player.id, npc.id, true, true);

// Save the database
EditorUtility.SetDirty(database);
AssetDatabase.SaveAssets();
```

## Twine Format for the Dialogue System

### Actors
By default, passages are imported as dialogue entries assigned to the conversant. Links are imported as entries assigned to the actor.

To specify an actor, include the actor's name at the beginning of the text. Example: "Narrator: And they lived happily ever after."

### Links
To link one passage directly to another without an intermediate link entry, enclose the link text in parentheses. Example: [[(Enter Cave)]]

The importer handles these link formats:
- [[link]]
- [[text -> link]]
- [[link <- text]]

### Formatting Codes
The importer translates //italic//, ''bold'', italic, and bold to rich text codes.

### Cutscene Sequences, Conditions, Scripts, and Description
To specify a Sequence, Conditions, Script, or Description for a passage, add the following at the bottom of the passage:

```
Hello, world!
Sequence:
AudioWait(hello);
AnimatorPlay(wave)
Conditions:
Variable["saidHello"] == false
Script:
Variable["saidHello"] = true
Description:
Generic greeting if NPC doesn't have a quest for the player.
```

By default, Conditions are set to Block when false. To make Conditions passthrough, add "(passthrough)" to the Conditions: line.

### Macros
The importer handles (if:) and (set:) macros.