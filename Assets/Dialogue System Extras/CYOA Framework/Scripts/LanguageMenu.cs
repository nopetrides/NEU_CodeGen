using UnityEngine;

namespace PixelCrushers.DialogueSystem.CYOA {

	/// <summary>
	/// This script handles the Language Menu.
	/// </summary>
	public class LanguageMenu : MonoBehaviour {

		public void Close() {
			gameObject.SetActive(false);
		}

		public void SetLanguage(string language) {
			Debug.Log ("Set language: " + language);
            DialogueManager.SetLanguage(language);
			Close();
		}
		
	}

}