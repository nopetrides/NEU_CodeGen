using UnityEngine;

public class LoadVolumeToListener : MonoBehaviour
{
	private void OnEnable()
	{
		// Load saved settings from PlayerPrefs
		var masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
		var isMuted = PlayerPrefs.GetInt("MuteAudio", 0) == 1;
			
		// Apply audio settings to the game's audio system
		float effectiveVolume = isMuted ? 0f : masterVolume;
		// Set the master volume in the game's audio system
		AudioListener.volume = effectiveVolume;
	}
}
