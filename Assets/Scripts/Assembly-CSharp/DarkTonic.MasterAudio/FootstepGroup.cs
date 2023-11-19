using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class FootstepGroup
	{
		public bool isExpanded = true;

		public bool useLayerFilter;

		public bool useTagFilter;

		public List<int> matchingLayers = new List<int> { 0 };

		public List<string> matchingTags = new List<string> { "Default" };

		public string soundType = "[None]";

		public EventSounds.VariationType variationType = EventSounds.VariationType.PlayRandom;

		public string variationName = string.Empty;

		public float volume = 1f;

		public bool useFixedPitch;

		public float pitch = 1f;

		public float delaySound;
	}
}
