using System;
using System.Collections.Generic;
using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Shared.Gameplay
{
	[DisallowMultipleComponent]
	public class MagicEnergyService : MonoBehaviour
	{
		private readonly Dictionary<TileColor, int> colorToEnergy = new Dictionary<TileColor, int>();

		public event Action<TileColor, int> EnergyChanged;

		public void Initialize(TileColor[] availableColors)
		{
			colorToEnergy.Clear();
			if (availableColors == null) return;
			for (var i = 0; i < availableColors.Length; i++)
			{
				var color = availableColors[i];
				if (!colorToEnergy.ContainsKey(color))
				{
					colorToEnergy[color] = 0;
					EnergyChanged?.Invoke(color, 0);
				}
			}
		}

		public void Add(TileColor color, int amount)
		{
			if (amount == 0) return;
			if (!colorToEnergy.TryGetValue(color, out var current)) current = 0;
			var next = current + amount;
			colorToEnergy[color] = next;
			EnergyChanged?.Invoke(color, next);
		}

		public bool TrySpend(TileColor color, int amount)
		{
			if (!colorToEnergy.TryGetValue(color, out var current)) current = 0;
			if (current < amount) return false;
			var next = current - amount;
			colorToEnergy[color] = next;
			EnergyChanged?.Invoke(color, next);
			return true;
		}

		public int Get(TileColor color)
		{
			return colorToEnergy.TryGetValue(color, out var v) ? v : 0;
		}
	}
}


