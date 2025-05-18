// Copyright © Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem.VisualNovelFramework;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Handles the title menu.
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {

        [Tooltip("Index of title scene in build settings.")]
        public int titleSceneIndex;

        [Tooltip("Index of credits scene in build settings.")]
        public int creditsSceneIndex = 2;

        public UIPanel titleMenuPanel;
        public UnityEngine.UI.Button startButton;
        public UnityEngine.UI.Button continueButton;
        public UnityEngine.UI.Button restartButton;
        public UnityEngine.UI.Button loadGameButton;

        public bool actAsSingleton = true;

        public bool neverSleep;

        private SaveHelper _mSaveHelper;
        private MusicManager _mMusicManager;

        private static TitleMenu _mInstance;

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            _mInstance = null;
        }
#endif

        private void Awake()
        {
            if (actAsSingleton)
            {
                if (_mInstance != null)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    _mInstance = this;
                    if (transform.root != null) transform.SetParent(null, false);
                    DontDestroyOnLoad(gameObject);
                }
            }
            _mSaveHelper = GetComponent<SaveHelper>();
            _mMusicManager = GetComponent<MusicManager>();
        }

        private void Start()
        {
            UpdateAvailableButtons();
            if (_mMusicManager != null) _mMusicManager.PlayTitleMusic();
            if (neverSleep) Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void OnSceneLoaded(int index)
        {
            if (index == titleSceneIndex)
            {
                titleMenuPanel.Open();
                if (InputDeviceManager.deviceUsesCursor) Tools.SetCursorActive(true);
            }
            else
            {
                titleMenuPanel.Close();
            }
        }

        public void UpdateAvailableButtons()
        {
            UpdateAvailableButtonsNow();
            Invoke("UpdateAvailableButtonsNow", 0.5f);
        }

        private void UpdateAvailableButtonsNow()
        {
            var hasSavedGame = (_mSaveHelper != null) ? _mSaveHelper.HasLastSavedGame() : false;
            if (startButton != null) startButton.gameObject.SetActive(!hasSavedGame);
            if (continueButton != null) continueButton.gameObject.SetActive(hasSavedGame);
            if (restartButton != null) restartButton.gameObject.SetActive(hasSavedGame);
            if (loadGameButton != null) loadGameButton.gameObject.SetActive(hasSavedGame);
            var selectableToFocus = hasSavedGame ? ((continueButton != null) ? continueButton.gameObject : null) 
                : ((startButton != null) ? startButton.gameObject : null);
            titleMenuPanel.firstSelected = selectableToFocus;
        }

        public void ShowCreditsScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(creditsSceneIndex);
        }

    }

}