using UnityEngine;
using Lighthouse.TowerDefense.Configs;
using Lighthouse.Match3.Configs;
using Lighthouse.Match3.Domain.TileModel;
using Lighthouse.Shared.EventBus;
using Lighthouse.TowerDefense.Application.Events;

namespace Lighthouse.TowerDefense.Presentation.Views
{
	[DisallowMultipleComponent]
	public class EnemyView : MonoBehaviour
	{
		[SerializeField] SpriteRenderer bodyRenderer;
		[SerializeField] HealthBarView healthBar;

		private EnemyDef def;
		private int maxHp;
		private int currentHp;
		private Vector3 goalPoint;
		private float speed;
		private DepthSortingManager depthManager;
		private EventBus bus;
		private Vector3 lastPos;
		private int sortingOrder;
		private System.Action<EnemyView> onDespawn;
		private int spawnId;

		public int InstanceId { get; private set; }
		public TileColor Color => def != null ? def.Color : TileColor.None;
		public int CurrentHp => currentHp;

		private static class GlobalSpawnCounter
		{
			public static int Value;
		}

		public void Init(EnemyDef enemyDef, int hp, Vector3 goal, DepthSortingManager manager, LevelDef level, EventBus eventBus)
		{
			def = enemyDef;
			maxHp = Mathf.Max(1, hp);
			currentHp = maxHp;
			speed = Mathf.Max(0f, def.Speed);
			goalPoint = goal;
			depthManager = manager;
			bus = eventBus;
			InstanceId = GetInstanceID();
			spawnId = System.Threading.Interlocked.Increment(ref GlobalSpawnCounter.Value);

			if (bodyRenderer != null) bodyRenderer.sprite = def.BodySprite;
			if (healthBar != null) healthBar.Initialize(maxHp, def.Color, level);
			lastPos = transform.position;

			depthManager?.RegisterEnemy(this);
			bus?.Publish(new EnemySpawnedEvent { EnemyId = def.Id, InstanceId = InstanceId, SpawnId = spawnId, Color = def.Color, Position = transform.position });
		}

		void Update()
		{
			UpdateMovement(Time.deltaTime);
		}

		public void UpdateMovement(float deltaTime)
		{
			var dir = (goalPoint - transform.position);
			var dist = dir.magnitude;
			if (dist <= 0.01f)
			{
				bus?.Publish(new EnemyReachedGoalEvent { InstanceId = InstanceId, SpawnId = spawnId, Color = Color });
				Despawn();
				return;
			}
			dir /= dist;
			transform.position += dir * speed * deltaTime;
		}

		public void SetSortingOrder(int order)
		{
			sortingOrder = order;
			if (bodyRenderer != null) bodyRenderer.sortingOrder = order;
		}

		public bool HasMoved(float threshold)
		{
			var moved = (transform.position - lastPos).sqrMagnitude > (threshold * threshold);
			if (moved) lastPos = transform.position;
			return moved;
		}

		public void SetHealth(int hp)
		{
			currentHp = Mathf.Clamp(hp, 0, maxHp);
			healthBar?.SetHealth(currentHp);
		}

		public void TakeDamage(int amount)
		{
			if (amount <= 0) return;
			currentHp = Mathf.Max(0, currentHp - amount);
			bus?.Publish(new EnemyDamagedEvent { InstanceId = InstanceId, SpawnId = spawnId, DamageAmount = amount, RemainingHp = currentHp, Color = Color });
			healthBar?.SetHealth(currentHp);
			if (currentHp == 0)
			{
				bus?.Publish(new EnemyDiedEvent { InstanceId = InstanceId, SpawnId = spawnId, Color = Color, DeathPosition = transform.position });
				Despawn();
			}
		}

		public int GetSpawnId() => spawnId;

		public void SetDespawnCallback(System.Action<EnemyView> callback)
		{
			onDespawn = callback;
		}

		public float GetRemainingDistance()
		{
			// Lanes are horizontal; use X-axis distance to avoid cross-lane bias
			return Mathf.Abs(goalPoint.x - transform.position.x);
		}

		public void Despawn()
		{
			depthManager?.UnregisterEnemy(this);
			onDespawn?.Invoke(this);
		}

		void OnDestroy()
		{
			depthManager?.UnregisterEnemy(this);
		}
	}
}


