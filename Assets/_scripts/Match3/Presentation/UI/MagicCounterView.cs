using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Presentation.UI
{
	public class MagicCounterView : MonoBehaviour
	{
		public Image Icon;
		public TMP_Text ValueText;
		public Button ClickButton;
		[HideInInspector] public TileColor Color;

		public void SetIcon(Sprite sprite)
		{
			if (Icon != null) Icon.sprite = sprite;
		}

		public void SetValue(int value)
		{
			if (ValueText != null) ValueText.text = value.ToString();
		}

		public void SetOnClick(System.Action onClick)
		{
			if (ClickButton == null || onClick == null) return;
			ClickButton.onClick.RemoveAllListeners();
			ClickButton.onClick.AddListener(() => onClick());
		}
	}
}


