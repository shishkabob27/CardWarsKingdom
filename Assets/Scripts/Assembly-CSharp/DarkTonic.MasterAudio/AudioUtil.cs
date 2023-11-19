using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public static class AudioUtil
	{
		private const float SemitonePitchChangeAmt = 1.0594635f;

		public static float FrameTime()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime == 0f)
			{
				return Time.fixedDeltaTime;
			}
			return deltaTime;
		}

		public static float GetSemitonesFromPitch(float pitch)
		{
			if (pitch < 1f && pitch > 0f)
			{
				float f = 1f / pitch;
				return Mathf.Log(f, 1.0594635f) * -1f;
			}
			return Mathf.Log(pitch, 1.0594635f);
		}

		public static float GetPitchFromSemitones(float semitones)
		{
			if (semitones >= 0f)
			{
				return Mathf.Pow(1.0594635f, semitones);
			}
			return 1f / Mathf.Pow(1.0594635f, Mathf.Abs(semitones));
		}

		public static float GetDbFromFloatVolume(float vol)
		{
			return Mathf.Log(vol) * 20f;
		}

		public static float GetFloatVolumeFromDb(float db)
		{
			return Mathf.Exp(db / 20f);
		}

		public static float GetAudioPlayedPercentage(AudioSource source)
		{
			if (source.clip == null || source.time == 0f)
			{
				return 0f;
			}
			return source.time / source.clip.length * 100f;
		}

		public static bool IsAudioPaused(AudioSource source)
		{
			return !source.isPlaying && GetAudioPlayedPercentage(source) > 0f;
		}

		public static void UnloadNonPreloadedAudioData(AudioClip clip)
		{
			if (clip != null && !clip.preloadAudioData)
			{
				clip.UnloadAudioData();
			}
		}

		public static bool IsClipReadyToPlay(AudioClip clip)
		{
			return clip.loadType != AudioClipLoadType.Streaming;
		}
	}
}
