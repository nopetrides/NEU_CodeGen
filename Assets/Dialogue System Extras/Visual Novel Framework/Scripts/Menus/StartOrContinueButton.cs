using UnityEngine;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
	
    /// <summary>
    /// Enabled the Start, Continue, and/or Restart buttons as appropriate
    /// for the player's current saved games.
    /// </summary>
	public class StartOrContinueButton : MonoBehaviour {

		public UnityEngine.UI.Button startButton;
		public UnityEngine.UI.Button continueButton;
		public UnityEngine.UI.Button restartButton;

		private bool _mStarted;

		public void Start()
		{
            _mStarted = true;
			Check();
		}

		public void OnEnable()
		{
			if (_mStarted) Check();
		}

		public void Check()
		{
			var saveHelper = FindObjectOfType<SaveHelper>();
			if (saveHelper)
			{
				var hasSavedGame = saveHelper.HasLastSavedGame();
				startButton.gameObject.SetActive(!hasSavedGame);
				continueButton.gameObject.SetActive(hasSavedGame);
				restartButton.gameObject.SetActive(hasSavedGame);
			}
		}

	}

}