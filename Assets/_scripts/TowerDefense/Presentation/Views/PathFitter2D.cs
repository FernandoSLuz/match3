using UnityEngine;

namespace Lighthouse.TowerDefense.Presentation.Views
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class PathFitter2D : MonoBehaviour
	{
		[Header("Camera")]
		public Camera TargetCamera;

		[Header("Padding (world units)")]
		[Range(0f, 10f)] public float PaddingLeft = 1f;
		[Range(0f, 10f)] public float PaddingRight = 1f;
		[Range(0f, 10f)] public float PaddingTop = 0.5f;
		[Range(0f, 10f)] public float PaddingBottom = 0.5f;

		[Header("Lanes")]
		[Range(1, 10)] public int LaneCount = 1;
		[Range(0f, 10f)] public float LaneSpacing = 0f;

		[Header("Distances (off-screen)")]
		[Range(0f, 10f)] public float SpawnDistance = 2f; // off-screen left
		[Range(0f, 10f)] public float ExitDistance = 2f; // off-screen right

		[Header("Direction")]
		public bool RightToLeft = true;

		public Vector3[] LaneStarts { get; private set; }
		public Vector3[] LaneGoals { get; private set; }

		void OnEnable()
		{
			CalculatePathPoints();
		}

		void OnValidate()
		{
			CalculatePathPoints();
		}

		public void CalculatePathPoints()
		{
			var cam = TargetCamera != null ? TargetCamera : Camera.main;
			if (cam == null) return;

			if (LaneCount < 1) LaneCount = 1;
			if (LaneSpacing < 0f) LaneSpacing = 0f;

			var zPlane = transform.position.z;
			var worldMin = cam.ViewportToWorldPoint(new Vector3(0f, 0f, Mathf.Abs(zPlane - cam.transform.position.z)));
			var worldMax = cam.ViewportToWorldPoint(new Vector3(1f, 1f, Mathf.Abs(zPlane - cam.transform.position.z)));

			var leftX = worldMin.x + PaddingLeft;
			var rightX = worldMax.x - PaddingRight;
			var bottomY = worldMin.y + PaddingBottom;
			var topY = worldMax.y - PaddingTop;

			var centerY = (bottomY + topY) * 0.5f;
			var totalHeight = topY - bottomY;
			var lanes = Mathf.Max(1, LaneCount);
			if (LaneStarts == null || LaneStarts.Length != lanes)
			{
				LaneStarts = new Vector3[lanes];
				LaneGoals = new Vector3[lanes];
			}

			// Distribute lanes vertically centered
			var laneBandHeight = lanes > 1 ? (lanes - 1) * LaneSpacing : 0f;
			var firstLaneY = centerY - laneBandHeight * 0.5f;

			for (var i = 0; i < lanes; i++)
			{
				var laneY = firstLaneY + i * LaneSpacing;
				if (RightToLeft)
				{
					LaneStarts[i] = new Vector3(rightX + SpawnDistance, laneY, zPlane);
					LaneGoals[i] = new Vector3(leftX - ExitDistance, laneY, zPlane);
				}
				else
				{
					LaneStarts[i] = new Vector3(leftX - SpawnDistance, laneY, zPlane);
					LaneGoals[i] = new Vector3(rightX + ExitDistance, laneY, zPlane);
				}
			}
		}
	}
}


