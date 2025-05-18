using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
	
	public class LoadGamePanel : GeneralPanel
    {

		public UnityEngine.UI.Button[] slots;
		public UnityEngine.UI.Button loadButton;
		public UnityEngine.UI.Button deleteButton;
		public UnityEngine.UI.Text details;
		public GameObject confirmDeletePanel;
		public GameObject loadInProgressPanel;

		[System.Serializable]
		public class StringEvent : UnityEngine.Events.UnityEvent<string> {}

		public StringEvent onSetDetails = new StringEvent();

		public UnityEngine.Events.UnityEvent onLoadGame = new UnityEngine.Events.UnityEvent();

		[HideInInspector] public int currentSlotNum = -1;

		private Menus _menus;
		private SaveHelper _saveHelper;

		public override void Open()
		{
			base.Open();
			if (_menus == null) _menus = FindObjectOfType<Menus>();
			if (_saveHelper == null) _saveHelper = _menus.GetComponent<SaveHelper>();
			StartCoroutine(SetupPanelAtEndOfFrame());
		}

		private IEnumerator SetupPanelAtEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			SetupPanel();
		}

		private void SetupPanel()
		{
			details.gameObject.SetActive(false);
			loadButton.interactable = false;
			deleteButton.interactable = false;
			for (int slotNum = 0; slotNum < slots.Length; slotNum++)
			{
				var slot = slots[slotNum];
				var containsSavedGame = _saveHelper.IsGameSavedInSlot(slotNum);
				var slotLabel = slot.GetComponentInChildren<UnityEngine.UI.Text>();
				if (slotLabel != null) slotLabel.text = _saveHelper.GetSlotSummary(slotNum);
				slot.interactable = containsSavedGame;
			}
            _menus.startPanel.GetComponent<StartOrContinueButton>().Check();
        }

		public void SelectSlot(int slotNum)
		{
			currentSlotNum = slotNum;
			_saveHelper.CurrentSlotNum = slotNum;
			loadButton.interactable = true;
			deleteButton.interactable = true;
			var detailsText = _saveHelper.GetSlotDetails(slotNum);
			details.text = detailsText;
			details.gameObject.SetActive(true);
			onSetDetails.Invoke(detailsText);
		}

		public void LoadCurrentSlot()
		{
			if (loadInProgressPanel != null) loadInProgressPanel.SetActive(true);
			_menus.StartCoroutine(LoadCoroutine());
		}
			
		private IEnumerator LoadCoroutine()
		{
			if (Debug.isDebugBuild) Debug.Log("Dialogue System Menus: Loading game in slot " + currentSlotNum);
			yield return null;
			onLoadGame.Invoke();
			if (loadInProgressPanel != null) loadInProgressPanel.SetActive(false);
            _menus.HideAllPanels();
		}

		public void LoadCurrentSlotNow()
		{
			_saveHelper.LoadGame(currentSlotNum);
		}

		public void AskDeleteCurrentSlot()
		{
			confirmDeletePanel.SetActive(true);
		}

		public void DeleteCurrentSlot()
		{
			confirmDeletePanel.SetActive(false);
			_saveHelper.DeleteSavedGame(currentSlotNum);
			SetupPanel();
		}

	}

}