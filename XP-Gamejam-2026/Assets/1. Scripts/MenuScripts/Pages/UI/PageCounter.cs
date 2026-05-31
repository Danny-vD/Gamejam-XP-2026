using TMPro;
using UnityEngine;
using VDFramework;
using VDFramework.Utility.DataTypes;

namespace XPGJ2026.MenuScripts.Pages.UI
{
	public class PageCounter : BetterMonoBehaviour
	{
		[SerializeField]
		private OrderedPageManager orderedPageManager;

		[SerializeField]
		private TMP_Text pageCounterLabel;

		private StringVariableWriter counterWriter;

		private void Reset()
		{
			pageCounterLabel = GetComponent<TMP_Text>();
		}

		private void Awake()
		{
			counterWriter = new StringVariableWriter(pageCounterLabel.text);
		}

		private void OnEnable()
		{
			orderedPageManager.OnPageChanged.AddCallback(UpdateText);
			UpdateText(orderedPageManager.CurrentPageNumber, orderedPageManager.MaxPageNumber);
		}

		private void OnDisable()
		{
			orderedPageManager.OnPageChanged.RemoveCallback(UpdateText);
		}

		private void UpdateText(int currentPageNumber, int maxPageNumber)
		{
			pageCounterLabel.text = counterWriter.UpdateText(currentPageNumber, maxPageNumber);
		}
	}
}