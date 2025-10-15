using UnityEngine;

namespace Lighthouse.TowerDefense.Configs
{
	[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "TowerDefense/EnemySpawnerConfig")]
	public class EnemySpawnerConfig : ScriptableObject
	{
		[System.Serializable]
		public struct SpawnEntry
		{
			public EnemyDef Enemy;
			public Vector2Int HpRange;
			public float Weight;
		}

		public SpawnEntry[] SpawnEntries;
		public float StartInterval = 2.0f;
		public float EndInterval = 0.5f;
		public float RampDurationSeconds = 60.0f;
		public AnimationCurve IntervalCurve;
	}
}


