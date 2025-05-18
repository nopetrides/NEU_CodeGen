using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This script adds the ability to backtrack conversations. To backtrack, call Backtrack(true).
    /// The bool parameter specifies whether to backtrack to an NPC line, which is what you usually
    /// want to do; otherwise if you're in a response menu you'll keep backtracking to the same 
    /// response menu instead of going back to a previous NPC line.
    /// </summary>
    public class Backtracker : MonoBehaviour
    {

        public bool debug;

        protected Stack<ConversationState> Stack = new Stack<ConversationState>();
        protected bool IsInMenu;

        public virtual void OnConversationStart(Transform actor)
        {
            Stack.Clear();
            if (debug) Debug.Log("Backtracker: Starting a new conversation. Clearing stack.");
        }

        public virtual void OnConversationLine(Subtitle subtitle)
        {
            IsInMenu = false;
            Stack.Push(DialogueManager.CurrentConversationState);
            if (debug) Debug.Log("Backtracker: Recording dialogue entry " + subtitle.dialogueEntry.conversationID + ":" + subtitle.dialogueEntry.id + " on stack: '" + subtitle.formattedText.text + "' (" + subtitle.speakerInfo.characterType + ").");
        }

        public virtual void OnConversationResponseMenu(Response[] responses)
        {
            IsInMenu = true;
        }

        // Call this method to go back:
        public virtual void Backtrack(bool toPreviousNpcLine)
        {
            if (Stack.Count < 2) return;
            if (!IsInMenu)
            {
                Stack.Pop(); // Pop current entry.
            }
            var destination = Stack.Pop(); // Pop previous entry.
            if (toPreviousNpcLine)
            {
                while (!destination.subtitle.speakerInfo.IsNPC && Stack.Count > 0)
                {
                    destination = Stack.Pop(); // Keep popping until we get an NPC line.
                }
                if (!destination.subtitle.speakerInfo.IsNPC) return;
            }
            if (debug) Debug.Log("Backtracker: Backtracking to " + destination.subtitle.dialogueEntry.conversationID + ":" + destination.subtitle.dialogueEntry.id + " on stack: '" + destination.subtitle.formattedText.text + "' (" + destination.subtitle.speakerInfo.characterType + ").");
            DialogueManager.ConversationController.GotoState(destination);
        }
    }
}
