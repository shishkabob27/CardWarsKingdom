using System;
using UnityEngine;

public class SLOTAudioManager : Singleton<SLOTAudioManager>
{
	[Serializable]
	public struct NamedCooldown
	{
		public VOEvent Event;
		public float Cooldown;
	}

	public AudioSource gui_audiosource;
	public bool useMasterAudio;
	public float soundVolume;
	public float voVolume;
	public float musicVolume;
	public NamedCooldown[] VoEventCooldowns;
}
