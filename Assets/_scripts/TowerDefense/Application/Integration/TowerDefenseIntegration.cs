using UnityEngine;
using Lighthouse.Match3.Tools;
using Lighthouse.Shared.EventBus;
using Lighthouse.Shared.Gameplay;
using Lighthouse.Match3.Application.Events;
using Lighthouse.Match3.Domain.TileModel;
using Lighthouse.TowerDefense.Application.Controllers;

namespace Lighthouse.TowerDefense.Application.Integration
{
	[DisallowMultipleComponent]
	public class TowerDefenseIntegration : MonoBehaviour
	{
		public SimulationRunner Simulation;
		public MagicEnergyService Energy;
		public EnemySpawnController Spawner;

		[Header("Combat Settings")]
		public int DamagePerSpend = 1;

		private EventBus bus;

		void Start()
		{
			if (Simulation == null || Energy == null || Spawner == null)
			{
				Debug.LogError("TowerDefenseIntegration: missing references.");
				enabled = false;
				return;
			}
			bus = Simulation.Bus;
		}

		public bool TrySpendEnergyAndDamage(TileColor color, int spendAmount)
		{
			// Peek current energy; only spend what is actually needed based on targets
			var available = Energy.Get(color);
			if (available <= 0 || spendAmount <= 0) return false;
			var toUse = Mathf.Min(available, spendAmount);
			var potentialDamage = toUse * Mathf.Max(1, DamagePerSpend);
			var appliedDamage = Spawner.DamageByColor(color, potentialDamage);
			// Convert applied damage back to energy units consumed (ceil to be safe)
			var consumed = Mathf.CeilToInt((float)appliedDamage / Mathf.Max(1, DamagePerSpend));
			if (consumed <= 0) return false;
			// Spend exactly what we used
			Energy.TrySpend(color, consumed);
			return true;
		}

		public bool TryCast(TileColor color, int charges)
		{
			return TrySpendEnergyAndDamage(color, charges);
		}
	}
}


