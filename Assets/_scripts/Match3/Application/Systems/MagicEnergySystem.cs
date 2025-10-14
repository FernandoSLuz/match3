using UnityEngine;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Application.Events;
using Lighthouse.Shared.Gameplay;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Application.Systems
{
	/// Listens to game events and updates the shared MagicEnergyService.
	public class MagicEnergySystem : MonoBehaviour
	{
		public Tools.SimulationRunner Runner;
		public MagicEnergyService EnergyService;

		private EventBus bus;

		void Start()
		{
			if (Runner == null || EnergyService == null)
			{
				Debug.LogError("MagicEnergySystem requires Runner and EnergyService references");
				enabled = false;
				return;
			}
			StartCoroutine(Bind());
		}

		private System.Collections.IEnumerator Bind()
		{
			while (Runner.Bus == null) yield return null;
			bus = Runner.Bus;
			bus.Subscribe<LevelStartEvent>(OnLevelStart);
			bus.Subscribe<TilesRemovedInfoEvent>(OnTilesRemovedInfo);
		}

		private void OnLevelStart(LevelStartEvent e)
		{
			EnergyService.Initialize(e.Colors);
		}

		private void OnTilesRemovedInfo(TilesRemovedInfoEvent e)
		{
			for (var i = 0; i < e.Infos.Count; i++)
			{
				var info = e.Infos[i];
				var color = info.Tile.Color;
				EnergyService.Add(color, 1);
			}
		}
	}
}


