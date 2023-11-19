namespace CodeStage.AntiCheat.Detectors
{
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		private ObscuredCheatingDetector()
		{
		}

		public float floatEpsilon;
		public float vector2Epsilon;
		public float vector3Epsilon;
		public float quaternionEpsilon;
	}
}
