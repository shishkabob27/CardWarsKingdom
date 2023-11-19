using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class FootstepGroup
	{
		public bool isExpanded;
		public bool useLayerFilter;
		public bool useTagFilter;
		public List<int> matchingLayers;
		public List<string> matchingTags;
		public string soundType;
		public EventSounds.VariationType variationType;
		public string variationName;
		public float volume;
		public bool useFixedPitch;
		public float pitch;
		public float delaySound;
	}
}
