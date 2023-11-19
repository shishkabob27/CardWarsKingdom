using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[SerializeField]
	public class PlaySoundResult
	{
		public bool SoundPlayed { get; set; }

		public bool SoundScheduled { get; set; }

		public SoundGroupVariation ActingVariation { get; set; }

		public PlaySoundResult()
		{
			SoundPlayed = false;
			SoundScheduled = false;
			ActingVariation = null;
		}
	}
}
