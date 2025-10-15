using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;
using Lighthouse.Shared.EventBus;

namespace Lighthouse.TowerDefense.Application.Events
{
	public struct EnemySpawnedEvent : IGameEvent
	{
		public string EnemyId;
		public int InstanceId;
		public int SpawnId; // unique per spawn/lifetime
		public TileColor Color;
		public Vector3 Position;
	}

	public struct EnemyDamagedEvent : IGameEvent
	{
		public int InstanceId;
		public int SpawnId; // unique per spawn/lifetime
		public int DamageAmount;
		public int RemainingHp;
		public TileColor Color;
	}

	public struct EnemyDiedEvent : IGameEvent
	{
		public int InstanceId;
		public int SpawnId; // unique per spawn/lifetime
		public TileColor Color;
		public Vector3 DeathPosition;
	}

	public struct EnemyReachedGoalEvent : IGameEvent
	{
		public int InstanceId;
		public int SpawnId; // unique per spawn/lifetime
		public TileColor Color;
	}
}


