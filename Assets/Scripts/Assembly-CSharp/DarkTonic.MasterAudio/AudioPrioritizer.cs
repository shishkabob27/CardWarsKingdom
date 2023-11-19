using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public static class AudioPrioritizer
	{
		private const int MaxPriority = 0;

		private const int HighestPriority = 16;

		private const int LowestPriority = 128;

		public static void Set2DSoundPriority(AudioSource audio)
		{
			audio.priority = 16;
		}

		public static void SetSoundGroupInitialPriority(AudioSource audio)
		{
			audio.priority = 128;
		}

		public static void SetPreviewPriority(AudioSource audio)
		{
			audio.priority = 0;
		}

		public static void Set3DPriority(AudioSource audio, bool useClipAgePriority)
		{
			if (!(MasterAudio.AudioListenerTransform == null))
			{
				float num = Vector3.Distance(audio.transform.position, MasterAudio.AudioListenerTransform.position);
				float num2;
				switch (audio.rolloffMode)
				{
				case AudioRolloffMode.Logarithmic:
					num2 = audio.volume / Mathf.Max(audio.minDistance, num - audio.minDistance);
					break;
				case AudioRolloffMode.Linear:
					num2 = Mathf.Lerp(audio.volume, 0f, Mathf.Max(0f, num - audio.minDistance) / (audio.maxDistance - audio.minDistance));
					break;
				default:
					num2 = Mathf.Lerp(audio.volume, 0f, Mathf.Max(0f, num - audio.minDistance) / (audio.maxDistance - audio.minDistance));
					break;
				}
				if (useClipAgePriority && !audio.loop)
				{
					num2 = Mathf.Lerp(num2, num2 * 0.1f, AudioUtil.GetAudioPlayedPercentage(audio) * 0.01f);
				}
				audio.priority = (int)Mathf.Lerp(16f, 128f, Mathf.InverseLerp(1f, 0f, num2));
			}
		}
	}
}
