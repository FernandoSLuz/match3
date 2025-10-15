using System.Collections.Generic;
using UnityEngine;

namespace Lighthouse.TowerDefense.Presentation.Views
{
	[DisallowMultipleComponent]
	public class DepthSortingManager : MonoBehaviour
	{
		private readonly List<EnemyView> activeEnemies = new List<EnemyView>();
		public Vector3 GoalPoint;
		public int BaseSortingOrder = 1000;
		public bool AutoUpdateEveryFrame = true;
		public float SignificantMoveThreshold = 0.01f;

		private bool dirty;

		public void RegisterEnemy(EnemyView enemy)
		{
			if (!activeEnemies.Contains(enemy))
			{
				activeEnemies.Add(enemy);
				dirty = true;
			}
		}

		public void UnregisterEnemy(EnemyView enemy)
		{
			if (activeEnemies.Remove(enemy))
			{
				dirty = true;
			}
		}

		public void MarkDirty()
		{
			dirty = true;
		}

		void Update()
		{
			if (AutoUpdateEveryFrame && (dirty || HaveSignificantMovement()))
			{
				UpdateAllDepths();
				dirty = false;
			}
		}

		private bool HaveSignificantMovement()
		{
			for (var i = 0; i < activeEnemies.Count; i++)
			{
				var e = activeEnemies[i];
				if (e.HasMoved(SignificantMoveThreshold)) return true;
			}
			return false;
		}

		public void UpdateAllDepths()
		{
			activeEnemies.Sort((a, b) =>
			{
				var distA = Vector3.Distance(a.transform.position, GoalPoint);
				var distB = Vector3.Distance(b.transform.position, GoalPoint);
				return distB.CompareTo(distA); // closer to goal => higher order
			});

			for (var i = 0; i < activeEnemies.Count; i++)
			{
				activeEnemies[i].SetSortingOrder(BaseSortingOrder + i);
			}
		}
	}
}


