using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	public class WallHackDetector : ActDetectorBase
	{
		private WallHackDetector()
		{
		}

		[SerializeField]
		private bool checkRigidbody;
		[SerializeField]
		private bool checkController;
		[SerializeField]
		private bool checkWireframe;
		[SerializeField]
		private bool checkRaycast;
		public int wireframeDelay;
		public int raycastDelay;
		public Vector3 spawnPosition;
		public byte maxFalsePositives;
	}
}
