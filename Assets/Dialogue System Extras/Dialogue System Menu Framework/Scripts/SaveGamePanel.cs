using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
	
	public class SaveGamePanel : GeneralPanel
    {

		public UnityEngine.UI.Button[] slots;
		public GameObject confirmOverwritePanel;
		public GameObject saveInProgressPanel;

		private Menus _menus;
		private SaveHelper _saveHelper;
		private int _currentSlotNum = -1;

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
			for (int slotNum = 0; slotNum < slots.Length; slotNum++)
			{
				var slot = slots[slotNum];
				var slotLabel = slot.GetComponentInChildren<UnityEngine.UI.Text>();
				if (slotLabel != null) slotLabel.text = _saveHelper.GetSlotSummary(slotNum);
			}
		}

		public void SelectSlot(int slotNum)
		{
			_currentSlotNum = slotNum;
			if (_saveHelper.IsGameSavedInSlot(slotNum))
			{
				confirmOverwritePanel.SetActive(true);
			}
			else
			{
				ConfirmSave();
			}
		}

		public void ConfirmSave()
		{
			StartCoroutine(SaveCoroutine());
		}

		private IEnumerator SaveCoroutine()
		{
			if (Debug.isDebugBuild) Debug.Log("Dialogue System Menus: Saving game in slot " + _currentSlotNum);
			saveInProgressPanel.SetActive(true);
			yield return null;
			_saveHelper.SaveGame(_currentSlotNum);
			saveInProgressPanel.SetActive(false);
			Close();
		}

	}

}