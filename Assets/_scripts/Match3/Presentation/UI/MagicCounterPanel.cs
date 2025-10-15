using System.Collections.Generic;
using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;
using Lighthouse.Shared.Gameplay;
using Lighthouse.Match3.Configs;
using Lighthouse.TowerDefense.Application.Integration;

namespace Lighthouse.Match3.Presentation.UI
{
	/// Instantiates a counter prefab per TileColor and keeps them updated.
	public class MagicCounterPanel : MonoBehaviour
	{
		[Header("Setup")]
		public Transform Parent; // container where prefabs will be spawned
		public GameObject CounterPrefab; // must contain MagicCounterView
		public Tools.SimulationRunner Runner;
		public MagicEnergyService EnergyService;
		public TowerDefenseIntegration TowerDefense;


		private readonly Dictionary<TileColor, MagicCounterView> colorToView = new Dictionary<TileColor, MagicCounterView>();

		void Start()
		{
			if (Parent == null || CounterPrefab == null || Runner == null || EnergyService == null || TowerDefense == null)
			{
				Debug.LogError("MagicCounterPanel requires Parent, CounterPrefab, Runner, EnergyService, TowerDefense");
				enabled = false;
				return;
			}

			EnergyService.EnergyChanged += OnEnergyChanged;
			StartCoroutine(BindAndBuild());
		}

		private System.Collections.IEnumerator BindAndBuild()
		{
			while (Runner.Board == null) yield return null; // wait for init
			// Build initial set using level colors
			var colors = Runner.Level.Colors;
			Build(colors);
			// Initialize displayed values
			for (var i = 0; i < colors.Length; i++)
			{
				OnEnergyChanged(colors[i], EnergyService.Get(colors[i]));
			}
		}

		private void Build(TileColor[] colors)
		{
			// Destroy previous
			foreach (Transform child in Parent)
			{
				Destroy(child.gameObject);
			}
			colorToView.Clear();

			for (var i = 0; i < colors.Length; i++)
			{
				var color = colors[i];
				var go = Instantiate(CounterPrefab, Parent);
				var view = go.GetComponent<MagicCounterView>();
				view.Color = color;
				view.SetIcon(GetSprite(color));
				view.SetValue(0);
				colorToView[color] = view;
				view.SetOnClick(() => OnCounterClicked(color));
			}
		}

		private void OnCounterClicked(TileColor color)
		{
			// Attempt to cast using all available charges of this color
			var charges = EnergyService.Get(color);
			if (charges <= 0) return; // later we can trigger feedback
			TowerDefense.TryCast(color, charges);
		}

		private void OnDestroy()
		{
			EnergyService.EnergyChanged -= OnEnergyChanged;
		}

		private void OnEnergyChanged(TileColor color, int value)
		{
			if (colorToView.TryGetValue(color, out var view))
			{
				view.SetValue(value);
			}
		}

		private Sprite GetSprite(TileColor color)
		{
			if (Runner.Level != null) return Runner.Level.GetSprite(color);
			return null;
		}
	}
}


