using UnityEngine;

public class SLOTMusic : Singleton<SLOTMusic>
{
	public AudioSource[] musicAudioSources;
	public float volume;
	public string FrontEndMusicName;
	public string VictoryMusicName;
	public string LoserMusicName;
}
