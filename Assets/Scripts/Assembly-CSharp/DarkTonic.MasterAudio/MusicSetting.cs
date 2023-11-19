using System;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class MusicSetting
	{
		public string alias = string.Empty;

		public MasterAudio.AudioLocation audLocation;

		public AudioClip clip;

		public string songName = string.Empty;

		public string resourceFileName = string.Empty;

		public float volume = 1f;

		public float pitch = 1f;

		public bool isExpanded = true;

		public bool isLoop;

		public float customStartTime;

		public int lastKnownTimePoint;

		public int songIndex;

		public bool songStartedEventExpanded;

		public string songStartedCustomEvent = string.Empty;

		public bool songChangedEventExpanded;

		public string songChangedCustomEvent = string.Empty;
	}
}
