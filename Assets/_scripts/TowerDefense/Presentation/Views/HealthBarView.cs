using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lighthouse.Match3.Configs;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.TowerDefense.Presentation.Views
{
	[DisallowMultipleComponent]
	public class HealthBarView : MonoBehaviour
	{
		[SerializeField] RectTransform container;
		[SerializeField] Image pipPrefab;

		private readonly List<Image> healthPips = new List<Image>();

		public void Initialize(int maxHp, TileColor color, LevelDef level)
		{
			Clear();
			if (container == null || pipPrefab == null || level == null) return;
			for (var i = 0; i < maxHp; i++)
			{
				var pip = Instantiate(pipPrefab, container);
				pip.sprite = level.GetSprite(color);
				pip.gameObject.SetActive(true);
				healthPips.Add(pip);
			}
		}

		public void SetHealth(int currentHp)
		{
			for (var i = 0; i < healthPips.Count; i++)
			{
				healthPips[i].gameObject.SetActive(i < currentHp);
			}
		}

		private void Clear()
		{
			for (var i = 0; i < healthPips.Count; i++)
			{
				if (healthPips[i] != null) Destroy(healthPips[i].gameObject);
			}
			healthPips.Clear();
		}
	}
}


