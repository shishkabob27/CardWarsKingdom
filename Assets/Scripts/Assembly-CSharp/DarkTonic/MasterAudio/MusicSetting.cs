using System;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class MusicSetting
	{
		public string alias;
		public MasterAudio.AudioLocation audLocation;
		public AudioClip clip;
		public string songName;
		public string resourceFileName;
		public float volume;
		public float pitch;
		public bool isExpanded;
		public bool isLoop;
		public float customStartTime;
		public int lastKnownTimePoint;
		public int songIndex;
		public bool songStartedEventExpanded;
		public string songStartedCustomEvent;
		public bool songChangedEventExpanded;
		public string songChangedCustomEvent;
	}
}
