﻿// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using PixelCrushers.DialogueSystem.VisualNovelFramework;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Add this to the Menu System if you want to manage music tracks.
    /// </summary>
    public class MusicManager : Saver
    {

        public AudioSource musicAudioSource;

        public int titleScene;
        public AudioClip titleMusic;
        public AudioClip[] gameplayMusic;

        private float _originalVolume = -1;
        private int _currentTrack = -1;

        public override void Awake()
        {
            base.Awake();
            if (musicAudioSource == null) musicAudioSource = GetComponent<AudioSource>();
            if (musicAudioSource != null) _originalVolume = musicAudioSource.volume;
        }

        public override void Start()
        {
            base.Start();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == titleScene)
            {
                PlayTitleMusic();
            }
            else
            {
                var saveHelper = GetComponent<SaveHelper>();
                if (saveHelper != null && scene.name == saveHelper.firstGameplaySceneName)
                {
                    if (gameplayMusic != null && gameplayMusic.Length >= 1)
                    {
                        PlayGameplayMusic(0);
                    }
                    else
                    {
                        FadeOutMusic(1);
                    }
                }
            }
        }

        public void PlayTitleMusic()
        {
            PlayAudioClip(titleMusic);
            _currentTrack = -1;
        }

        public void PlayGameplayMusic(int index)
        {
            if (gameplayMusic == null) return;
            if (0 <= index && index < gameplayMusic.Length)
            {
                PlayAudioClip(gameplayMusic[index]);
                _currentTrack = index;
            }
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            if (audioClip == null || musicAudioSource == null || !musicAudioSource.enabled) return;
            if (musicAudioSource.isPlaying && musicAudioSource.clip == audioClip) return;
            musicAudioSource.Stop();
            musicAudioSource.clip = audioClip;
            musicAudioSource.volume = _originalVolume;
            if (musicAudioSource.enabled) musicAudioSource.Play();
        }

        public void StopMusic()
        {
            if (musicAudioSource == null || !musicAudioSource.isPlaying) return;
            musicAudioSource.Stop();
        }

        public void FadeOutMusic(float duration)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            if (musicAudioSource == null) yield break;
            float startingVolume = musicAudioSource.volume;
            float remaining = duration;
            while (remaining > 0)
            {
                musicAudioSource.volume = (remaining / duration) * startingVolume;
                yield return null;
                remaining -= Time.deltaTime;
            }
            musicAudioSource.Stop();
            musicAudioSource.volume = _originalVolume;
        }

        public override string RecordData()
        {
            return _currentTrack.ToString();
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            if (s == "-1")
            {
                PlayTitleMusic();
            }
            else
            {
                PlayGameplayMusic(SafeConvert.ToInt(s));
            }
        }
    }
}