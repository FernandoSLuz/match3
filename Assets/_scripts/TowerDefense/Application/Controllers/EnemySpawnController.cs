using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lighthouse.TowerDefense.Configs;
using Lighthouse.TowerDefense.Presentation.Views;
using Lighthouse.Match3.Configs;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Tools;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.TowerDefense.Application.Controllers
{
	[DisallowMultipleComponent]
	public class EnemySpawnController : MonoBehaviour
	{
		[Header("Dependencies")]
		public SimulationRunner Simulation;
		public EnemySpawnerConfig SpawnerConfig;
		public PathFitter2D PathFitter;
		public DepthSortingManager DepthManager;
		public Transform SpawnParent;
		public EnemyView EnemyPrefab;

		private EventBus bus;
		private float startTime;
		private readonly System.Random rng = new System.Random();

		private readonly List<(EnemyView view, EnemyDef def)> active = new List<(EnemyView, EnemyDef)>();
		private readonly Stack<EnemyView> pool = new Stack<EnemyView>();

		void Start()
		{
			if (Simulation == null || SpawnerConfig == null || PathFitter == null || DepthManager == null || EnemyPrefab == null)
			{
				Debug.LogError("EnemySpawnController: missing references.");
				enabled = false;
				return;
			}
			bus = Simulation.Bus;
			startTime = Time.time;
			StartCoroutine(SpawnLoop());
		}

		private IEnumerator SpawnLoop()
		{
			var elapsed = 0f;
			while (enabled)
			{
				elapsed = Time.time - startTime;
				var wait = CalculateCurrentInterval(elapsed);
				SpawnOne();
				yield return new WaitForSeconds(wait);
			}
		}

		private float CalculateCurrentInterval(float elapsed)
		{
			var start = Mathf.Max(0.01f, SpawnerConfig.StartInterval);
			var end = Mathf.Max(0.01f, SpawnerConfig.EndInterval);
			var dur = Mathf.Max(0.01f, SpawnerConfig.RampDurationSeconds);
			var t = Mathf.Clamp01(elapsed / dur);
			if (SpawnerConfig.IntervalCurve != null && SpawnerConfig.IntervalCurve.keys != null && SpawnerConfig.IntervalCurve.keys.Length > 0)
			{
				t = Mathf.Clamp01(SpawnerConfig.IntervalCurve.Evaluate(t));
			}
			return Mathf.Lerp(start, end, t);
		}

		private EnemySpawnerConfig.SpawnEntry SelectEnemyEntry()
		{
			var entries = SpawnerConfig.SpawnEntries;
			if (entries == null || entries.Length == 0) return default;
			var total = 0f;
			for (var i = 0; i < entries.Length; i++) total += Mathf.Max(0.0001f, entries[i].Weight);
			var pick = (float)rng.NextDouble() * total;
			var acc = 0f;
			for (var i = 0; i < entries.Length; i++)
			{
				var w = Mathf.Max(0.0001f, entries[i].Weight);
				if (pick <= acc + w) return entries[i];
				acc += w;
			}
			return entries[entries.Length - 1];
		}

		private void SpawnOne()
		{
			if (PathFitter.LaneStarts == null || PathFitter.LaneStarts.Length == 0) PathFitter.CalculatePathPoints();
			var laneIndex = rng.Next(0, Mathf.Max(1, PathFitter.LaneStarts.Length));
			var start = PathFitter.LaneStarts[laneIndex];
			var goal = PathFitter.LaneGoals[laneIndex];
			var entry = SelectEnemyEntry();
			if (entry.Enemy == null)
			{
				return;
			}
			var hp = Mathf.Clamp(rng.Next(entry.HpRange.x, entry.HpRange.y + 1), 1, 9999);
			var view = Acquire(start);
			view.Init(entry.Enemy, hp, goal, DepthManager, Simulation.Level, bus);
			view.SetDespawnCallback(OnDespawn);
			active.Add((view, entry.Enemy));
		}

		private EnemyView Acquire(Vector3 position)
		{
			EnemyView view;
			if (pool.Count > 0)
			{
				view = pool.Pop();
				view.transform.SetParent(SpawnParent != null ? SpawnParent : transform, false);
				view.transform.position = position;
				view.gameObject.SetActive(true);
			}
			else
			{
				view = Instantiate(EnemyPrefab, position, Quaternion.identity, SpawnParent != null ? SpawnParent : transform);
			}
			return view;
		}

		private void OnDespawn(EnemyView view)
		{
			for (var i = 0; i < active.Count; i++)
			{
				if (active[i].view == view)
				{
					active.RemoveAt(i);
					break;
				}
			}
			view.gameObject.SetActive(false);
			pool.Push(view);
		}

		public int DamageByColor(TileColor color, int totalDamage)
		{
			// Sort enemies by their own remaining distance to goal (closest first)
			var snapshot = new List<EnemyView>(active.Count);
			for (var i = 0; i < active.Count; i++) snapshot.Add(active[i].view);
			snapshot.Sort((a, b) =>
			{
				var distA = a.GetRemainingDistance();
				var distB = b.GetRemainingDistance();
				return distA.CompareTo(distB);
			});

			var remaining = totalDamage;
			for (var i = 0; i < snapshot.Count && remaining > 0; i++)
			{
				var view = snapshot[i];
				// Find its def color (from active list)
				EnemyDef def = null;
				for (var j = 0; j < active.Count; j++)
				{
					if (active[j].view == view) { def = active[j].def; break; }
				}
				if (def == null || def.Color != color) continue;

				var hpBefore = view.CurrentHp;
				if (remaining < hpBefore)
				{
					view.TakeDamage(remaining);
					remaining = 0;
					break;
				}
				else if (remaining == hpBefore)
				{
					view.TakeDamage(hpBefore); // will kill and stop
					remaining = 0;
					break;
				}
				else
				{
					view.TakeDamage(hpBefore); // kill and continue to next enemy
					remaining -= hpBefore;
				}
			}
			return totalDamage - remaining;
		}
	}
}


