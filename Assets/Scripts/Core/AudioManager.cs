using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	public class AudioManager : MonoBehaviour
	{
		private Dictionary<string, AudioClip> _audioClips = new();
		private Dictionary<string, AudioClip> _dialogueSfx = new();
		private AudioSource _uiAudioSource;
		private AudioSource _dialogueAudioSource;

		private void Awake()
		{
			// Create UI audio source as a child of this GameObject
			var go = new GameObject("UIAudioSource");
			go.transform.SetParent(transform);
			_uiAudioSource = go.AddComponent<AudioSource>();
			_uiAudioSource.playOnAwake = false;

			// Create dialogue audio source
			var dialogueGo = new GameObject("DialogueAudioSource");
			dialogueGo.transform.SetParent(transform);
			_dialogueAudioSource = dialogueGo.AddComponent<AudioSource>();
			_dialogueAudioSource.playOnAwake = false;
			
			// Load saved settings from PlayerPrefs
			var masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
			var isMuted = PlayerPrefs.GetInt("MuteAudio", 0) == 1;
			// Apply audio settings to the game's audio system
			float effectiveVolume = isMuted ? 0f : masterVolume;
			// Set the master volume in the game's audio system
			AudioListener.volume = Mathf.Pow(10.0f, effectiveVolume/40.0f);

			// Load audio clips
			LoadAudioClips();

			// Load dialogue SFX
			LoadDialogueSfx();
		}

		private void LoadAudioClips()
		{
			_audioClips["ButtonHover"] = Resources.Load<AudioClip>("Audio/UI/ButtonHover");
			_audioClips["ButtonPress"] = Resources.Load<AudioClip>("Audio/UI/ButtonPress");

			// Validate loaded clips
			foreach (var clip in _audioClips)
			{
				if (clip.Value == null)
				{
					Debug.LogError($"Failed to load audio clip: {clip.Key}");
				}
			}
		}

		private void LoadDialogueSfx()
		{
			// Weather sounds
			AudioClip thunderClip = Resources.Load<AudioClip>("Audio/Dialogue/Thunder");
			if (thunderClip != null)
			{
				_dialogueSfx["Thunder"] = thunderClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: Thunder");
			}

			AudioClip windClip = Resources.Load<AudioClip>("Audio/Dialogue/Wind");
			if (windClip != null)
			{
				_dialogueSfx["Wind"] = windClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: Wind");
			}

			AudioClip rainClip = Resources.Load<AudioClip>("Audio/Dialogue/Rain");
			if (rainClip != null)
			{
				_dialogueSfx["Rain"] = rainClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: Rain");
			}

			AudioClip stormCalmClip = Resources.Load<AudioClip>("Audio/Dialogue/StormCalm");
			if (stormCalmClip != null)
			{
				_dialogueSfx["StormCalm"] = stormCalmClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: StormCalm");
			}

			// Object sounds
			AudioClip bowlBreakClip = Resources.Load<AudioClip>("Audio/Dialogue/BowlBreak");
			if (bowlBreakClip != null)
			{
				_dialogueSfx["BowlBreak"] = bowlBreakClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: BowlBreak");
			}

			AudioClip placeBerryClip = Resources.Load<AudioClip>("Audio/Dialogue/PlaceBerry");
			if (placeBerryClip != null)
			{
				_dialogueSfx["PlaceBerry"] = placeBerryClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: PlaceBerry");
			}

			AudioClip throwStoneClip = Resources.Load<AudioClip>("Audio/Dialogue/ThrowStone");
			if (throwStoneClip != null)
			{
				_dialogueSfx["ThrowStone"] = throwStoneClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: ThrowStone");
			}

			// Character sounds
			AudioClip goruVoiceClip = Resources.Load<AudioClip>("Audio/Dialogue/GoruVoice");
			if (goruVoiceClip != null)
			{
				_dialogueSfx["GoruVoice"] = goruVoiceClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: GoruVoice");
			}

			AudioClip oriseiVoiceClip = Resources.Load<AudioClip>("Audio/Dialogue/OriseiVoice");
			if (oriseiVoiceClip != null)
			{
				_dialogueSfx["OriseiVoice"] = oriseiVoiceClip;
			}
			else
			{
				Debug.LogWarning($"Failed to load dialogue SFX: OriseiVoice");
			}

			// Load Reveal dialogue audio clips
			LoadRevealAudioClips("BerryDestroy");
			LoadRevealAudioClips("BerryOffer");
			LoadRevealAudioClips("BerryRestraint");
			LoadRevealAudioClips("ShoutDestroy");
			LoadRevealAudioClips("ShoutOffer");
			LoadRevealAudioClips("ShoutRestraint");
			LoadRevealAudioClips("StoneDestroy");
			LoadRevealAudioClips("StoneOffer");
			LoadRevealAudioClips("StoneRestraint");
			LoadRevealAudioClips("Unrecorded");
		}

		private void LoadRevealAudioClips(string clipName)
		{
			AudioClip clip = Resources.Load<AudioClip>($"Audio/Dialogue/Reveal/{clipName}");
			if (clip != null)
			{
				_dialogueSfx[$"Reveal_{clipName}"] = clip;
			}
			else
			{
				Debug.LogWarning($"Failed to load Reveal dialogue SFX: {clipName}");
			}
		}

		public void PlayUISound(string soundName)
		{
			if (_audioClips.TryGetValue(soundName, out var clip) && clip && 
				_uiAudioSource.isPlaying == false || clip != _uiAudioSource.clip) // Don't overlap sounds
			{
				_uiAudioSource.PlayOneShot(clip);
			}
			else
			{
				Debug.LogWarning($"UI sound not found: {soundName}");
			}
		}

		public void PlayDialogueSfx(string soundName, float volume = 1.0f)
		{
			if (_dialogueSfx.TryGetValue(soundName, out var clip) && clip != null)
			{
				_dialogueAudioSource.PlayOneShot(clip, volume);
			}
			else
			{
				Debug.LogWarning($"Dialogue SFX not found: {soundName}");
			}
		}
	}
}
