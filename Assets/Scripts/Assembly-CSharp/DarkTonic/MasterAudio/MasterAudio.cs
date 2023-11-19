using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	public class MasterAudio : MonoBehaviour
	{
		[Serializable]
		public class Playlist
		{
			public enum CrossfadeTimeMode
			{
				UseMasterSetting = 0,
				Override = 1,
			}

			public bool isExpanded;
			public string playlistName;
			public MasterAudio.SongFadeInPosition songTransitionType;
			public List<MusicSetting> MusicSettings;
			public MasterAudio.AudioLocation bulkLocationMode;
			public CrossfadeTimeMode crossfadeMode;
			public float crossFadeTime;
			public bool fadeInFirstSong;
			public bool fadeOutLastSong;
			public bool resourceClipsAllLoadAsync;
		}

		public enum AudioLocation
		{
			Clip = 0,
			ResourceFile = 1,
			FileOnInternet = 2,
		}

		public enum DragGroupMode
		{
			OneGroupPerClip = 0,
			OneGroupWithVariations = 1,
		}

		public enum LanguageMode
		{
			UseDeviceSetting = 0,
			SpecificLanguage = 1,
			DynamicallySet = 2,
		}

		public enum AllMusicSpatialBlendType
		{
			ForceAllTo2D = 0,
			ForceAllTo3D = 1,
			ForceAllToCustom = 2,
			AllowDifferentPerController = 3,
		}

		public enum AllMixerSpatialBlendType
		{
			ForceAllTo2D = 0,
			ForceAllTo3D = 1,
			ForceAllToCustom = 2,
			AllowDifferentPerGroup = 3,
		}

		public enum ItemSpatialBlendType
		{
			ForceTo2D = 0,
			ForceTo3D = 1,
			ForceToCustom = 2,
			UseCurveFromAudioSource = 3,
		}

		public enum SongFadeInPosition
		{
			NewClipFromBeginning = 1,
			NewClipFromLastKnownPosition = 3,
			SynchronizeClips = 5,
		}

		public enum CustomEventReceiveMode
		{
			Always = 0,
			WhenDistanceLessThan = 1,
			WhenDistanceMoreThan = 2,
			Never = 3,
		}

		public enum EventSoundFunctionType
		{
			PlaySound = 0,
			GroupControl = 1,
			BusControl = 2,
			PlaylistControl = 3,
			CustomEventControl = 4,
			GlobalControl = 5,
			UnityMixerControl = 6,
			PersistentSettingsControl = 7,
		}

		public enum PlaylistCommand
		{
			None = 0,
			ChangePlaylist = 1,
			FadeToVolume = 2,
			PlayClip = 3,
			PlayRandomSong = 4,
			PlayNextSong = 5,
			Pause = 6,
			Resume = 7,
			Stop = 8,
			Mute = 9,
			Unmute = 10,
			ToggleMute = 11,
			Restart = 12,
			Start = 13,
		}

		public enum SoundGroupCommand
		{
			None = 0,
			FadeToVolume = 1,
			FadeOutAllOfSound = 2,
			Mute = 3,
			Pause = 4,
			Solo = 5,
			StopAllOfSound = 6,
			Unmute = 7,
			Unpause = 8,
			Unsolo = 9,
			StopAllSoundsOfTransform = 10,
			PauseAllSoundsOfTransform = 11,
			UnpauseAllSoundsOfTransform = 12,
			StopSoundGroupOfTransform = 13,
			PauseSoundGroupOfTransform = 14,
			UnpauseSoundGroupOfTransform = 15,
			FadeOutSoundGroupOfTransform = 16,
			RefillSoundGroupPool = 17,
		}

		public enum BusCommand
		{
			None = 0,
			FadeToVolume = 1,
			Mute = 2,
			Pause = 3,
			Solo = 4,
			Unmute = 5,
			Unpause = 6,
			Unsolo = 7,
			Stop = 8,
			ChangeBusPitch = 9,
			ToggleMute = 10,
		}

		public enum CustomEventCommand
		{
			None = 0,
			FireEvent = 1,
		}

		public enum GlobalCommand
		{
			None = 0,
			PauseMixer = 1,
			UnpauseMixer = 2,
			StopMixer = 3,
			StopEverything = 4,
			PauseEverything = 5,
			UnpauseEverything = 6,
			MuteEverything = 7,
			UnmuteEverything = 8,
			SetMasterMixerVolume = 9,
			SetMasterPlaylistVolume = 10,
		}

		public enum UnityMixerCommand
		{
			None = 0,
			TransitionToSnapshot = 1,
			TransitionToSnapshotBlend = 2,
		}

		public enum PersistentSettingsCommand
		{
			None = 0,
			SetBusVolume = 1,
			SetGroupVolume = 2,
			SetMixerVolume = 3,
			SetMusicVolume = 4,
			MixerMuteToggle = 5,
			MusicMuteToggle = 6,
		}

		public enum SoundSpawnLocationMode
		{
			MasterAudioLocation = 0,
			CallerLocation = 1,
			AttachToCaller = 2,
		}

		public enum InternetFileLoadStatus
		{
			Loading = 0,
			Loaded = 1,
			Failed = 2,
		}

		public AudioLocation bulkLocationMode;
		public string groupTemplateName;
		public string audioSourceTemplateName;
		public bool showGroupCreation;
		public bool useGroupTemplates;
		public DragGroupMode curDragGroupMode;
		public List<GameObject> groupTemplates;
		public List<GameObject> audioSourceTemplates;
		public bool mixerMuted;
		public bool playlistsMuted;
		public LanguageMode langMode;
		public SystemLanguage testLanguage;
		public SystemLanguage defaultLanguage;
		public List<SystemLanguage> supportedLanguages;
		public string busFilter;
		public bool useTextGroupFilter;
		public string textGroupFilter;
		public bool resourceClipsPauseDoNotUnload;
		public bool resourceClipsAllLoadAsync;
		public Transform playlistControllerPrefab;
		public bool persistBetweenScenes;
		public bool areGroupsExpanded;
		public Transform soundGroupTemplate;
		public Transform soundGroupVariationTemplate;
		public List<GroupBus> groupBuses;
		public bool busesShownInNarrow;
		public bool groupByBus;
		public bool showGizmos;
		public bool showAdvancedSettings;
		public bool showLocalization;
		public bool playListExpanded;
		public bool playlistsExpanded;
		public AllMusicSpatialBlendType musicSpatialBlendType;
		public float musicSpatialBlend;
		public AllMixerSpatialBlendType mixerSpatialBlendType;
		public float mixerSpatialBlend;
		public ItemSpatialBlendType newGroupSpatialType;
		public float newGroupSpatialBlend;
		public List<MasterAudio.Playlist> musicPlaylists;
		public float _masterAudioVolume;
		public bool ignoreTimeScale;
		public bool useGaplessPlaylists;
		public bool saveRuntimeChanges;
		public bool prioritizeOnDistance;
		public int rePrioritizeEverySecIndex;
		public bool visualAdvancedExpanded;
		public bool logAdvancedExpanded;
		public bool showFadingSettings;
		public bool stopZeroVolumeVariations;
		public bool stopZeroVolumeGroups;
		public bool stopZeroVolumeBuses;
		public bool stopZeroVolumePlaylists;
		public bool resourceAdvancedExpanded;
		public bool useClipAgePriority;
		public bool LogSounds;
		public bool logCustomEvents;
		public bool disableLogging;
		public bool showMusicDucking;
		public bool enableMusicDucking;
		public List<DuckGroupInfo> musicDuckingSounds;
		public float defaultRiseVolStart;
		public float duckedVolumeMultiplier;
		public float crossFadeTime;
		public float _masterPlaylistVolume;
		public bool showGroupSelect;
		public string newEventName;
		public bool showCustomEvents;
		public List<CustomEvent> customEvents;
		public int frames;
		public bool showUnityMixerGroupAssignment;
	}
}
