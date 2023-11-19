using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class DynamicSoundGroup : MonoBehaviour
	{
		public GameObject variationTemplate;

		public bool alwaysHighestPriority;

		public float groupMasterVolume = 1f;

		public int retriggerPercentage = 50;

		public MasterAudioGroup.VariationSequence curVariationSequence;

		public bool useInactivePeriodPoolRefill;

		public float inactivePeriodSeconds = 5f;

		public MasterAudioGroup.VariationMode curVariationMode;

		public MasterAudio.AudioLocation bulkVariationMode;

		public float chainLoopDelayMin;

		public float chainLoopDelayMax;

		public MasterAudioGroup.ChainedLoopLoopMode chainLoopMode;

		public int chainLoopNumLoops;

		public bool useDialogFadeOut;

		public float dialogFadeOutTime = 0.5f;

		public bool resourceClipsAllLoadAsync = true;

		public bool logSound;

		public bool soundPlayedEventActive;

		public string soundPlayedCustomEvent = string.Empty;

		public int busIndex = -1;

		public MasterAudio.ItemSpatialBlendType spatialBlendType = MasterAudio.ItemSpatialBlendType.ForceTo3D;

		public float spatialBlend = 1f;

		public string busName = string.Empty;

		public MasterAudioGroup.LimitMode limitMode;

		public int limitPerXFrames = 1;

		public float minimumTimeBetween = 0.1f;

		public bool limitPolyphony;

		public int voiceLimitCount = 1;

		public MasterAudioGroup.TargetDespawnedBehavior targetDespawnedBehavior;

		public float despawnFadeTime = 1f;

		public bool copySettingsExpanded;

		public int selectedVariationIndex;

		public MasterAudioGroup.ChildGroupMode childGroupMode;

		public List<string> childSoundGroups = new List<string>();

		public List<DynamicGroupVariation> groupVariations = new List<DynamicGroupVariation>();
	}
}
