namespace CodeStage.AntiCheat.Detectors
{
	public class SpeedHackDetector : ActDetectorBase
	{
		private SpeedHackDetector()
		{
		}

		public float interval;
		public byte maxFalsePositives;
		public int coolDown;
	}
}
