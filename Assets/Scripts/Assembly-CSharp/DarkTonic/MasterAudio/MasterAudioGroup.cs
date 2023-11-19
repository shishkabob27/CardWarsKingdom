using UnityEngine;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	public class MasterAudioGroup : MonoBehaviour
	{
		public enum VariationMode
		{
			Normal = 0,
			LoopedChain = 1,
			Dialog = 2,
		}

		public enum ChainedLoopLoopMode
		{
			Endless = 0,
			NumberOfLoops = 1,
		}

		public enum VariationSequence
		{
			Randomized = 0,
			TopToBottom = 1,
		}

		public enum ChildGroupMode
		{
			None = 0,
			TriggerLinkedGroupsWhenRequested = 1,
			TriggerLinkedGroupsWhenPlayed = 2,
		}

		public enum LimitMode
		{
			None = 0,
			FrameBased = 1,
			TimeBased = 2,
		}

		public enum TargetDespawnedBehavior
		{
			None = 0,
			Stop = 1,
			FadeOut = 2,
		}

		public int busIndex;
		public MasterAudio.ItemSpatialBlendType spatialBlendType;
		public float spatialBlend;
		public bool isSelected;
		public bool isExpanded;
		public float groupMasterVolume;
		public int retriggerPercentage;
		public VariationMode curVariationMode;
		public bool alwaysHighestPriority;
		public float chainLoopDelayMin;
		public float chainLoopDelayMax;
		public ChainedLoopLoopMode chainLoopMode;
		public int chainLoopNumLoops;
		public bool useDialogFadeOut;
		public float dialogFadeOutTime;
		public VariationSequence curVariationSequence;
		public bool useInactivePeriodPoolRefill;
		public float inactivePeriodSeconds;
		public List<SoundGroupVariation> groupVariations;
		public MasterAudio.AudioLocation bulkVariationMode;
		public bool resourceClipsAllLoadAsync;
		public bool logSound;
		public bool copySettingsExpanded;
		public int selectedVariationIndex;
		public ChildGroupMode childGroupMode;
		public List<string> childSoundGroups;
		public LimitMode limitMode;
		public int limitPerXFrames;
		public float minimumTimeBetween;
		public bool useClipAgePriority;
		public bool limitPolyphony;
		public int voiceLimitCount;
		public TargetDespawnedBehavior targetDespawnedBehavior;
		public float despawnFadeTime;
		public bool isSoloed;
		public bool isMuted;
		public bool soundPlayedEventActive;
		public string soundPlayedCustomEvent;
	}
}
