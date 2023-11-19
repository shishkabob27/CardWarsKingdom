using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class SoundGroupVariation : MonoBehaviour
	{
		public enum RandomPitchMode
		{
			AddToClipPitch = 0,
			IgnoreClipPitch = 1,
		}

		public enum RandomVolumeMode
		{
			AddToClipVolume = 0,
			IgnoreClipVolume = 1,
		}

		public enum FadeMode
		{
			None = 0,
			FadeInOut = 1,
			FadeOutEarly = 2,
			GradualFade = 3,
		}

		public enum DetectEndMode
		{
			None = 0,
			DetectEnd = 1,
		}

		public int weight;
		public bool useLocalization;
		public bool useRandomPitch;
		public RandomPitchMode randomPitchMode;
		public float randomPitchMin;
		public float randomPitchMax;
		public bool useRandomVolume;
		public RandomVolumeMode randomVolumeMode;
		public float randomVolumeMin;
		public float randomVolumeMax;
		public MasterAudio.AudioLocation audLocation;
		public string resourceFileName;
		public string internetFileUrl;
		public MasterAudio.InternetFileLoadStatus internetFileLoadStatus;
		public float fxTailTime;
		public float original_pitch;
		public bool isExpanded;
		public bool isChecked;
		public bool useFades;
		public float fadeInTime;
		public float fadeOutTime;
		public bool useRandomStartTime;
		public float randomStartMinPercent;
		public float randomStartMaxPercent;
		public bool useIntroSilence;
		public float introSilenceMin;
		public float introSilenceMax;
		public float fadeMaxVolume;
		public FadeMode curFadeMode;
		public DetectEndMode curDetectEndMode;
	}
}
