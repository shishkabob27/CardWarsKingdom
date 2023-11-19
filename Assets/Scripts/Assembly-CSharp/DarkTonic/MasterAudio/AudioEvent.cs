using System;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	[Serializable]
	public class AudioEvent
	{
		[Serializable]
		public class MA_SnapshotInfo
		{
			public MA_SnapshotInfo(AudioMixerSnapshot snap, float wt)
			{
			}

			public AudioMixerSnapshot snapshot;
			public float weight;
		}

		public enum TargetVolumeMode
		{
			UseSliderValue = 0,
			UseSpecificValue = 1,
		}

		public string actionName;
		public bool isExpanded;
		public string soundType;
		public bool allPlaylistControllersForGroupCmd;
		public bool allSoundTypesForGroupCmd;
		public bool allSoundTypesForBusCmd;
		public float volume;
		public bool useFixedPitch;
		public float pitch;
		public bool emitParticles;
		public int particleCountToEmit;
		public float delaySound;
		public MasterAudio.EventSoundFunctionType currentSoundFunctionType;
		public MasterAudio.PlaylistCommand currentPlaylistCommand;
		public MasterAudio.SoundGroupCommand currentSoundGroupCommand;
		public MasterAudio.BusCommand currentBusCommand;
		public MasterAudio.CustomEventCommand currentCustomEventCommand;
		public MasterAudio.GlobalCommand currentGlobalCommand;
		public MasterAudio.UnityMixerCommand currentMixerCommand;
		public AudioMixerSnapshot snapshotToTransitionTo;
		public float snapshotTransitionTime;
		public List<AudioEvent.MA_SnapshotInfo> snapshotsToBlend;
		public MasterAudio.PersistentSettingsCommand currentPersistentSettingsCommand;
		public string busName;
		public string playlistName;
		public string playlistControllerName;
		public bool startPlaylist;
		public float fadeVolume;
		public float fadeTime;
		public TargetVolumeMode targetVolMode;
		public string clipName;
		public EventSounds.VariationType variationType;
		public string variationName;
		public string theCustomEventName;
	}
}
