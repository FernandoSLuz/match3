using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.TowerDefense.Configs
{
	[CreateAssetMenu(fileName = "EnemyDef", menuName = "TowerDefense/EnemyDef")]
	public class EnemyDef : ScriptableObject
	{
		public string Id;
		public TileColor Color;
		public Sprite BodySprite;
		[Min(0f)] public float Speed = 1f;
	}
}


