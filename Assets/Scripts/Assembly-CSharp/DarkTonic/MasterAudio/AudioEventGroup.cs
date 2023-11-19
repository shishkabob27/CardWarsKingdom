using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class AudioEventGroup
	{
		public bool isExpanded;
		public bool useLayerFilter;
		public bool useTagFilter;
		public List<int> matchingLayers;
		public List<string> matchingTags;
		public bool customSoundActive;
		public bool isCustomEvent;
		public string customEventName;
		public bool mechanimEventActive;
		public bool isMechanimStateCheckEvent;
		public string mechanimStateName;
		public bool mechEventPlayedForState;
		public List<AudioEvent> SoundEvents;
		public EventSounds.PreviousSoundStopMode mouseDragStopMode;
		public float mouseDragFadeOutTime;
		public EventSounds.RetriggerLimMode retriggerLimitMode;
		public int limitPerXFrm;
		public float limitPerXSec;
		public int triggeredLastFrame;
		public float triggeredLastTime;
		public float sliderValue;
	}
}
