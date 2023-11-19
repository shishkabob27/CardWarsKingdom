using System;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class AudioEvent
	{
		public enum TargetVolumeMode
		{
			UseSliderValue,
			UseSpecificValue
		}

		[Serializable]
		public class MA_SnapshotInfo
		{
			public AudioMixerSnapshot snapshot;

			public float weight;

			public MA_SnapshotInfo(AudioMixerSnapshot snap, float wt)
			{
				snapshot = snap;
				weight = wt;
			}
		}

		public string actionName = "Your action name";

		public bool isExpanded = true;

		public string soundType = string.Empty;

		public bool allPlaylistControllersForGroupCmd;

		public bool allSoundTypesForGroupCmd;

		public bool allSoundTypesForBusCmd;

		public float volume = 1f;

		public bool useFixedPitch;

		public float pitch = 1f;

		public bool emitParticles;

		public int particleCountToEmit = 1;

		public float delaySound;

		public MasterAudio.EventSoundFunctionType currentSoundFunctionType;

		public MasterAudio.PlaylistCommand currentPlaylistCommand;

		public MasterAudio.SoundGroupCommand currentSoundGroupCommand;

		public MasterAudio.BusCommand currentBusCommand;

		public MasterAudio.CustomEventCommand currentCustomEventCommand;

		public MasterAudio.GlobalCommand currentGlobalCommand;

		public MasterAudio.UnityMixerCommand currentMixerCommand;

		public AudioMixerSnapshot snapshotToTransitionTo;

		public float snapshotTransitionTime = 1f;

		public List<MA_SnapshotInfo> snapshotsToBlend = new List<MA_SnapshotInfo>
		{
			new MA_SnapshotInfo(null, 1f)
		};

		public MasterAudio.PersistentSettingsCommand currentPersistentSettingsCommand;

		public string busName = string.Empty;

		public string playlistName = string.Empty;

		public string playlistControllerName = string.Empty;

		public bool startPlaylist = true;

		public float fadeVolume;

		public float fadeTime = 1f;

		public TargetVolumeMode targetVolMode;

		public string clipName = "[None]";

		public EventSounds.VariationType variationType = EventSounds.VariationType.PlayRandom;

		public string variationName = string.Empty;

		public string theCustomEventName = string.Empty;

		public bool IsFadeCommand
		{
			get
			{
				if (currentSoundFunctionType == MasterAudio.EventSoundFunctionType.PlaylistControl && currentPlaylistCommand == MasterAudio.PlaylistCommand.FadeToVolume)
				{
					return true;
				}
				if (currentSoundFunctionType == MasterAudio.EventSoundFunctionType.BusControl && currentBusCommand == MasterAudio.BusCommand.FadeToVolume)
				{
					return true;
				}
				if (currentSoundFunctionType == MasterAudio.EventSoundFunctionType.GroupControl && (currentSoundGroupCommand == MasterAudio.SoundGroupCommand.FadeToVolume || currentSoundGroupCommand == MasterAudio.SoundGroupCommand.FadeOutAllOfSound || currentSoundGroupCommand == MasterAudio.SoundGroupCommand.FadeOutSoundGroupOfTransform))
				{
					return true;
				}
				return false;
			}
		}
	}
}
