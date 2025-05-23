// Recompile at 5/15/2025 5:13:56 PM
#if USE_TIMELINE
#if UNITY_2017_1_OR_NEWER
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PixelCrushers.DialogueSystem
{

    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(ContinueConversationClip))]
    [TrackBindingType(typeof(GameObject))]
    public class ContinueConversationTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ContinueConversationMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
#endif
#endif
