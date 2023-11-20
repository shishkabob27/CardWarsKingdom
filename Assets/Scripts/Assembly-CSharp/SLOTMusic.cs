using System;
using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine;

public class SLOTMusic : Singleton<SLOTMusic>
{
	public AudioSource[] musicAudioSources;

	public float volume = 1f;

	public string FrontEndMusicName;

	public string VictoryMusicName;

	public string LoserMusicName;

	public static string CURRENT_MUSIC = string.Empty;

	private float mPrevMusicVolume;

	public void PlayVictoryMusic()
	{
		StopMusic(0f);
		StartCoroutine(StartMusic(VictoryMusicName));
	}

	public void PlayLoserMusic()
	{
		StopMusic(0f);
		StartCoroutine(StartMusic(LoserMusicName));
	}

	public IEnumerator PlayBattleMusic()
	{
		QuestData qd = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (qd == null)
		{
			qd = QuestDataManager.Instance.GetData(0);
		}
		yield return StartCoroutine(StartMusic(qd.BGM));
	}

	public IEnumerator PlayFrontEndMusic()
	{
		yield return StartCoroutine(StartMusic(FrontEndMusicName));
	}

	private IEnumerator StartMusic(string clipName)
	{
		if (clipName != string.Empty)
		{
			MasterAudio.PlaySound(clipName);
			while (MasterAudio.LoadingAudioBundle)
			{
				yield return null;
			}
			CURRENT_MUSIC = clipName;
		}
	}

	public void StopMusic(float fadeTime)
	{
		if (CURRENT_MUSIC != string.Empty)
		{
			StartCoroutine(StopMusicCo(CURRENT_MUSIC, fadeTime));
			CURRENT_MUSIC = string.Empty;
		}
	}

	private IEnumerator StopMusicCo(string music, float fadeTime)
	{
		if (fadeTime > 0f)
		{
			MasterAudio.FadeOutAllOfSound(music, fadeTime);
			yield return new WaitForSeconds(fadeTime);
		}
		MasterAudio.StopAllOfSound(music);
	}

	public bool FindAudioSource(AudioSource source)
	{
		return Array.Find(musicAudioSources, (AudioSource m) => m == source) != null;
	}

	public void LowerMusicVolume(bool lower, float time = 2f)
	{
		float num = 1f;
		if (lower)
		{
			//GroupBus groupBus = MasterAudio.GrabBusByName("Music");
			//mPrevMusicVolume = groupBus.volume;
			//num = Mathf.Min(0.1f, mPrevMusicVolume);
		}
		else
		{
			num = mPrevMusicVolume;
		}
		MasterAudio.FadeBusToVolume("Music", num, time);
	}
}
