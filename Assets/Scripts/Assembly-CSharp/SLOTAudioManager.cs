using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

public class SLOTAudioManager : Singleton<SLOTAudioManager>
{
	public enum AudioType
	{
		SFX,
		VO,
		Music
	}

	private class MusicFadeOutInfo
	{
		public AudioSource audioSourceToFade;

		public AudioSource audioSourceToPlay;

		public AudioClip audioClip;

		public string clipName;

		public float fadeOutTime;

		public float musicVolume;

		public bool loop;

		public MusicFadeOutInfo(AudioSource a1, AudioSource a2, AudioClip c, float fadeouttime, float musicvolume, bool lp)
		{
			audioSourceToFade = a1;
			audioSourceToPlay = a2;
			audioClip = c;
			clipName = null;
			fadeOutTime = fadeouttime;
			musicVolume = musicvolume;
			loop = lp;
		}

		public MusicFadeOutInfo(AudioSource a1, AudioSource a2, string c, float fadeouttime, float musicvolume, bool lp)
		{
			audioSourceToFade = a1;
			audioSourceToPlay = a2;
			audioClip = null;
			clipName = c;
			fadeOutTime = fadeouttime;
			musicVolume = musicvolume;
			loop = lp;
		}
	}

	[Serializable]
	public struct NamedCooldown
	{
		public VOEvent Event;

		public float Cooldown;
	}

	public AudioSource gui_audiosource;

	private bool useMasterAudio = false;

	private AudioListener listener;

	public float soundVolume = 1f;

	public float voVolume = 1f;

	public float musicVolume = 1f;

	private float prevSoundVolume = -1234f;

	private float prevVOVolume = -1234f;

	private float prevMusicVolume = -1234f;

	private List<MusicFadeOutInfo> musicFadeOutList = new List<MusicFadeOutInfo>();

	public NamedCooldown[] VoEventCooldowns;

	private float[] mVoEventCooldowns = new float[13];

	private float[] mCurrentVoEventCooldowns = new float[13];

	private bool mInitialized;

	private static bool s_isOnPhoneCall;

	private static float s_lastPhoneCallCheckTime;

	public float SoundVolume
	{
		get
		{
			return soundVolume;
		}
		set
		{
			soundVolume = value;
			Serialize();
		}
	}

	public float VOVolume
	{
		get
		{
			return voVolume;
		}
		set
		{
			voVolume = value;
			Serialize();
		}
	}

	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
			Serialize();
		}
	}

	public float GetSoundVolume()
	{
		return SoundVolume;
	}

	public float GetVOVolume()
	{
		return VOVolume;
	}

	public float GetMusicVolume()
	{
		return MusicVolume;
	}

	public void SetSoundVolume(float v)
	{
		SoundVolume = v;
		UpdateAudioVolumes();
	}

	public void SetVOVolume(float v)
	{
		VOVolume = v;
		UpdateAudioVolumes();
	}

	public void SetMusicVolume(float v)
	{
		MusicVolume = v;
		UpdateAudioVolumes();
	}

	private void Start()
	{
		Deserialize();
		KFFSoundPlayer.SetPlayOneShotFunction(PlayOneShot);
		base.transform.position = new Vector3(0f, 0f, 0f);
		NamedCooldown[] voEventCooldowns = VoEventCooldowns;
		for (int i = 0; i < voEventCooldowns.Length; i++)
		{
			NamedCooldown namedCooldown = voEventCooldowns[i];
			mVoEventCooldowns[(int)namedCooldown.Event] = namedCooldown.Cooldown;
		}
		SetSoundVolumeMasterAudio(soundVolume);
		SetMusicVolumeMasterAudio(musicVolume);
		mInitialized = true;
	}

	public AudioSource PlaySound(AudioSource audiosource)
	{
		if (audiosource != null)
		{
			return PlaySound(audiosource, audiosource.clip);
		}
		return null;
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip)
	{
		if (audiosource != null)
		{
			return PlaySound(audiosource, audioclip, true, false, AudioType.SFX);
		}
		return null;
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip)
	{
		return PlaySound(obj, audioclip, true);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot)
	{
		return PlaySound(obj, audioclip, oneshot, false);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot, bool createnewaudiosource)
	{
		return PlaySound(obj, audioclip, oneshot, createnewaudiosource, false);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot, bool createnewaudiosource, bool NoStacking)
	{
		AudioSource audioSource = obj.GetComponent(typeof(AudioSource)) as AudioSource;
		if (audioSource == null || createnewaudiosource)
		{
			audioSource = obj.AddComponent(typeof(AudioSource)) as AudioSource;
		}
		return PlaySound(audioSource, audioclip, oneshot, NoStacking);
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip, bool oneshot, bool NoStacking)
	{
		return PlaySound(audiosource, audioclip, oneshot, NoStacking, AudioType.SFX);
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip, bool oneshot, bool NoStacking, AudioType audioType)
	{
		if (useMasterAudio)
		{
			if (audioclip != null)
			{
				if (oneshot)
				{
					MasterAudio.PlaySoundAndForget(audioclip.name);
				}
				else
				{
					MasterAudio.PlaySound(audioclip.name);
				}
			}
		}
		else
		{
			if (audiosource == null)
			{
				return null;
			}
			switch (audioType)
			{
			case AudioType.SFX:
				audiosource.volume = soundVolume;
				break;
			case AudioType.VO:
				audiosource.volume = voVolume;
				break;
			case AudioType.Music:
				audiosource.volume = musicVolume;
				break;
			}
			audiosource.enabled = true;
			if (oneshot)
			{
				audiosource.PlayOneShot(audioclip);
			}
			else if (!NoStacking || !audiosource.isPlaying)
			{
				audiosource.clip = audioclip;
				audiosource.Play();
			}
		}
		return audiosource;
	}

	public void PlaySound(string audioClipName)
	{
		if (useMasterAudio)
		{
			if (audioClipName.StartsWith("low_"))
			{
				audioClipName = audioClipName.Substring("low_".Length);
			}
			MasterAudio.PlaySound(audioClipName);
		}
	}

	public void StopSound(string audioClipName)
	{
		if (!useMasterAudio)
		{
			return;
		}
		MasterAudio.AudioGroupInfo groupInfo = MasterAudio.GetGroupInfo(audioClipName);
		if (groupInfo == null || !(groupInfo.Group != null) || groupInfo.Sources == null)
		{
			return;
		}
		foreach (MasterAudio.AudioInfo source in groupInfo.Sources)
		{
			if (source != null && source.Source != null)
			{
				source.Source.Stop();
			}
		}
	}

	public void PlayRandomSound(GameObject obj, AudioClip[] clips)
	{
		if (clips != null && clips.Length > 0)
		{
			PlaySound(obj, clips[UnityEngine.Random.Range(0, clips.Length)], true, false, false);
		}
	}

	public void StopMusic(string musicName)
	{
		MasterAudio.FadeOutAllOfSound(musicName, 1f);
	}

	public AudioSource PlayVO(AudioSource audiosource, AudioClip audioclip)
	{
		return PlaySound(audiosource, audioclip, false, false, AudioType.VO);
	}

	public AudioSource PlayOneShot(AudioClip audioclip)
	{
		return PlayOneShot(audioclip, 1f);
	}

	public AudioSource PlayOneShot(AudioClip audioclip, float volume)
	{
		return PlayOneShot(audioclip, volume, 1f);
	}

	public AudioSource PlayOneShot(AudioClip audioclip, float volume, float pitch)
	{
		AudioSource audioSource = PlayGUISound(audioclip, true);
		if (audioSource != null)
		{
			audioSource.volume = soundVolume * volume;
			audioSource.pitch = pitch;
		}
		return audioSource;
	}

	public AudioSource PlayOneShot(AudioSource audiosource, AudioClip audioclip)
	{
		return PlaySound(audiosource, audioclip, true, false, AudioType.SFX);
	}

	public AudioSource PlayGUISound(AudioClip audioclip)
	{
		return PlayGUISound(audioclip, true);
	}

	public AudioSource PlayGUISound(AudioClip audioclip, bool oneshot)
	{
		CreateListener();
		if ((bool)gui_audiosource)
		{
			return PlaySound(gui_audiosource, audioclip, oneshot, true);
		}
		gui_audiosource = PlaySound(base.gameObject, audioclip, oneshot, true, false);
		return gui_audiosource;
	}

	public void Serialize()
	{
		PlayerPrefs.SetFloat("Options_Sound_Volume", soundVolume);
		PlayerPrefs.SetFloat("Options_VO_Volume", voVolume);
		PlayerPrefs.SetFloat("Options_Music_Volume", musicVolume);
	}

	public void Deserialize()
	{
		soundVolume = PlayerPrefs.GetFloat("Options_Sound_Volume", 1f);
		voVolume = PlayerPrefs.GetFloat("Options_VO_Volume", 1f);
		musicVolume = PlayerPrefs.GetFloat("Options_Music_Volume", 1f);
	}

	private void Update()
	{
		UpdateVolumes();
		for (int i = 0; i < mCurrentVoEventCooldowns.Length; i++)
		{
			if (mCurrentVoEventCooldowns[i] > 0f)
			{
				mCurrentVoEventCooldowns[i] -= Time.deltaTime;
			}
		}
	}

	private void UpdateVolumes()
	{
		if (s_isOnPhoneCall && Time.time - s_lastPhoneCallCheckTime > 1f)
		{
			s_isOnPhoneCall = KFFCSUtils.IsOnPhoneCall();
			s_lastPhoneCallCheckTime = Time.time;
		}
		float num = ((!s_isOnPhoneCall) ? soundVolume : 0f);
		float num2 = ((!s_isOnPhoneCall) ? voVolume : 0f);
		float num3 = ((!s_isOnPhoneCall) ? musicVolume : 0f);
		if (num != prevSoundVolume || num2 != prevVOVolume || num3 != prevMusicVolume)
		{
			UpdateAudioVolumes(s_isOnPhoneCall ? 1 : 0);
			prevSoundVolume = num;
			prevVOVolume = num2;
			prevMusicVolume = num3;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		s_isOnPhoneCall = KFFCSUtils.IsOnPhoneCall();
		s_lastPhoneCallCheckTime = Time.time;
		UpdateVolumes();
	}

	public void UpdateAudioVolumes(int isonphonecall = -1)
	{
		UpdateAudioVolumes(null, isonphonecall);
	}

	public void UpdateAudioVolumes(GameObject root, int isonphonecall = -1)
	{
		bool flag = ((isonphonecall < 0) ? KFFCSUtils.IsOnPhoneCall() : (isonphonecall != 0));
		float num = ((!flag) ? soundVolume : 0f);
		float num2 = ((!flag) ? voVolume : 0f);
		float num3 = ((!flag) ? musicVolume : 0f);
		CreateListener();
		UnityEngine.Object[] array = null;
		array = ((!(root != null)) ? UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) : root.GetComponentsInChildren(typeof(AudioSource)));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Component component = (Component)array2[i];
			AudioSource audioSource = component as AudioSource;
			if (!(audioSource == null))
			{
				SLOTMusic sLOTMusic = audioSource.gameObject.GetComponent(typeof(SLOTMusic)) as SLOTMusic;
				if (sLOTMusic != null && sLOTMusic.FindAudioSource(audioSource))
				{
					audioSource.volume = num3 * sLOTMusic.volume;
					continue;
				}
				bool flag2 = false;
				audioSource.volume = ((!flag2) ? num : num2);
			}
		}
	}

	public void CreateListener()
	{
		if (!(listener == null))
		{
			return;
		}
		listener = UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
		if (listener == null)
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				camera = UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera;
			}
			if (camera != null)
			{
				listener = camera.gameObject.AddComponent(typeof(AudioListener)) as AudioListener;
			}
		}
	}

	public void SetMusicVolumeMasterAudio()
	{
		musicVolume = 1f - musicVolume;
		MasterAudio.SetBusVolumeByName("Music", musicVolume);
		prevMusicVolume = musicVolume;
		Serialize();
	}

	public void SetSoundVolumeMasterAudio()
	{
		soundVolume = 1f - soundVolume;
		MasterAudio.SetBusVolumeByName("Sound", soundVolume);
		prevSoundVolume = soundVolume;
		Serialize();
	}

	public void SetMusicVolumeMasterAudio(float volume)
	{
		musicVolume = volume;
		MasterAudio.SetBusVolumeByName("Music", musicVolume);
		prevMusicVolume = musicVolume;
		Serialize();
	}

	public void SetSoundVolumeMasterAudio(float volume)
	{
		soundVolume = volume;
		MasterAudio.SetBusVolumeByName("Sound", soundVolume);
		prevSoundVolume = soundVolume;
		Serialize();
	}

	public void OnInstantiate(UnityEngine.Object clone)
	{
		if (!(clone != null))
		{
			return;
		}
		GameObject gameObject = clone as GameObject;
		if (gameObject == null)
		{
			Transform transform = clone as Transform;
			if (transform != null)
			{
				gameObject = transform.gameObject;
			}
			else
			{
				MonoBehaviour monoBehaviour = clone as MonoBehaviour;
				if (monoBehaviour != null)
				{
					gameObject = monoBehaviour.gameObject;
				}
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		if (useMasterAudio)
		{
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(AudioSource));
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				AudioSource audioSource = component as AudioSource;
				if (audioSource != null && audioSource.clip != null && audioSource.playOnAwake)
				{
					audioSource.Stop();
					PlaySound(audioSource.clip.name);
					UnityEngine.Object.Destroy(audioSource);
				}
			}
		}
		else
		{
			UpdateAudioVolumes(gameObject);
		}
	}

	public bool IsInitialized()
	{
		return mInitialized;
	}

	public void PlayErrorSound()
	{
		PlaySound("UI_ErrorSound");
	}

	public void TriggerVOEvent(LeaderData leader, VOEvent voEvent, CreatureFaction faction = CreatureFaction.Count)
	{
		if (mCurrentVoEventCooldowns[(int)voEvent] > 0f || Singleton<TutorialController>.Instance.IsShowingText() || leader.VO == null)
		{
			return;
		}
		string text = "VO_" + leader.VO + "_" + voEvent;
		if (faction != CreatureFaction.Count)
		{
			string sType = text + "_" + faction.CDubsName();
			if (MasterAudio.SoundExists(sType))
			{
				MasterAudio.PlaySound(sType);
			}
			else
			{
				MasterAudio.PlaySound(text);
			}
		}
		else
		{
			MasterAudio.PlaySound(text);
		}
		mCurrentVoEventCooldowns[(int)voEvent] = mVoEventCooldowns[(int)voEvent];
	}

	public void SetVOEventCooldown(VOEvent voEvent)
	{
		mCurrentVoEventCooldowns[(int)voEvent] = mVoEventCooldowns[(int)voEvent];
	}
}
