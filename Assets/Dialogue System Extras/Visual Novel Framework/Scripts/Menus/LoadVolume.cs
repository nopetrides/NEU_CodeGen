using UnityEngine;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
	
	public class LoadVolume : MonoBehaviour
    {

        public enum VolumeType { Music, Sfx };

        public VolumeType volumeType = VolumeType.Music;

        public void Start()
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && Menus.Instance != null && Menus.Instance.optionsPanel != null)
            {
                audioSource.volume = (volumeType == VolumeType.Music)
                    ? Menus.Instance.optionsPanel.MusicVolume
                    : Menus.Instance.optionsPanel.SfxVolume;
            }
        }
	}

}