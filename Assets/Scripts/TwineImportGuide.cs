using UnityEngine;

/// <summary>
/// This script provides a guide for importing Twine stories into the PixelCrusher Dialogue System.
/// </summary>
public class TwineImportGuide : MonoBehaviour
{
    [TextArea(10, 20)]
    [Tooltip("Instructions for importing Twine stories")]
    public string instructions = @"# Importing Twine Stories into Dialogue System

## Step 1: Export Twine Stories to JSON
1. Use Harlowe story format syntax in Twine 2. For example, macros should use this form: (set: $torch to ""lit"")
2. Use Twison story format to publish your story to a JSON text file:
   - In Twine, go to Story > Details
   - Change the story format to Twison
   - Publish your story and save the output as a JSON file

## Step 2: Enable Twine Import in Unity
1. In Unity, select Tools > Pixel Crushers > Dialogue System > Import > Twine 2 (Twison)
2. The first time you select this menu item, the Dialogue System will ask if you want to enable Twine import capability
3. Click Enable
4. After the Dialogue System has recompiled with Twine import capability enabled, select the menu item again

## Step 3: Import Twine JSON Into Unity
1. In the Twine Import window, assign a dialogue database to the Database field
2. Add each JSON file to the JSON Files list
3. Select the story's primary actor and conversant from the dropdown menus
4. Check 'Split Pipes Into Entries' if you want pipe characters (|) to split passages into multiple dialogue entry nodes
5. Click the Import button

## Twine Format for the Dialogue System

### Actors
By default, passages are imported as dialogue entries assigned to the conversant. Links are imported as entries assigned to the actor.

To specify an actor, include the actor's name at the beginning of the text. Example: ""Narrator: And they lived happily ever after.""

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
Variable[""saidHello""] == false
Script:
Variable[""saidHello""] = true
Description:
Generic greeting if NPC doesn't have a quest for the player.
```

By default, Conditions are set to Block when false. To make Conditions passthrough, add ""(passthrough)"" to the Conditions: line.

### Macros
The importer handles (if:) and (set:) macros.";

    void OnValidate()
    {
        // This is just to make sure the instructions are visible in the Inspector
    }
}