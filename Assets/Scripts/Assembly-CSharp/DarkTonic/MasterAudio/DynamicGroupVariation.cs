using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class DynamicGroupVariation : MonoBehaviour
	{
		public bool useLocalization;
		public bool useRandomPitch;
		public SoundGroupVariation.RandomPitchMode randomPitchMode;
		public float randomPitchMin;
		public float randomPitchMax;
		public bool useRandomVolume;
		public SoundGroupVariation.RandomVolumeMode randomVolumeMode;
		public float randomVolumeMin;
		public float randomVolumeMax;
		public int weight;
		public MasterAudio.AudioLocation audLocation;
		public string resourceFileName;
		public string internetFileUrl;
		public bool isExpanded;
		public bool isChecked;
		public float fxTailTime;
		public bool useFades;
		public float fadeInTime;
		public float fadeOutTime;
		public bool useIntroSilence;
		public float introSilenceMin;
		public float introSilenceMax;
		public bool useRandomStartTime;
		public float randomStartMinPercent;
		public float randomStartMaxPercent;
	}
}
