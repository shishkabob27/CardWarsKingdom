using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio
{
	[AudioScriptOrder(-50)]
	public class MasterAudio : MonoBehaviour
	{
		public enum AllMusicSpatialBlendType
		{
			ForceAllTo2D,
			ForceAllTo3D,
			ForceAllToCustom,
			AllowDifferentPerController
		}

		public enum AllMixerSpatialBlendType
		{
			ForceAllTo2D,
			ForceAllTo3D,
			ForceAllToCustom,
			AllowDifferentPerGroup
		}

		public enum ItemSpatialBlendType
		{
			ForceTo2D,
			ForceTo3D,
			ForceToCustom,
			UseCurveFromAudioSource
		}

		public enum InternetFileLoadStatus
		{
			Loading,
			Loaded,
			Failed
		}

		public enum MixerWidthMode
		{
			Narrow,
			Normal,
			Wide
		}

		public enum CustomEventReceiveMode
		{
			Always,
			WhenDistanceLessThan,
			WhenDistanceMoreThan,
			Never
		}

		public enum AudioLocation
		{
			Clip,
			ResourceFile,
			FileOnInternet
		}

		public enum BusCommand
		{
			None,
			FadeToVolume,
			Mute,
			Pause,
			Solo,
			Unmute,
			Unpause,
			Unsolo,
			Stop,
			ChangeBusPitch,
			ToggleMute
		}

		public enum DragGroupMode
		{
			OneGroupPerClip,
			OneGroupWithVariations
		}

		public enum EventSoundFunctionType
		{
			PlaySound,
			GroupControl,
			BusControl,
			PlaylistControl,
			CustomEventControl,
			GlobalControl,
			UnityMixerControl,
			PersistentSettingsControl
		}

		public enum LanguageMode
		{
			UseDeviceSetting,
			SpecificLanguage,
			DynamicallySet
		}

		public enum UnityMixerCommand
		{
			None,
			TransitionToSnapshot,
			TransitionToSnapshotBlend
		}

		public enum PlaylistCommand
		{
			None,
			ChangePlaylist,
			FadeToVolume,
			PlayClip,
			PlayRandomSong,
			PlayNextSong,
			Pause,
			Resume,
			Stop,
			Mute,
			Unmute,
			ToggleMute,
			Restart,
			Start
		}

		public enum CustomEventCommand
		{
			None,
			FireEvent
		}

		public enum GlobalCommand
		{
			None,
			PauseMixer,
			UnpauseMixer,
			StopMixer,
			StopEverything,
			PauseEverything,
			UnpauseEverything,
			MuteEverything,
			UnmuteEverything,
			SetMasterMixerVolume,
			SetMasterPlaylistVolume
		}

		public enum SoundGroupCommand
		{
			None,
			FadeToVolume,
			FadeOutAllOfSound,
			Mute,
			Pause,
			Solo,
			StopAllOfSound,
			Unmute,
			Unpause,
			Unsolo,
			StopAllSoundsOfTransform,
			PauseAllSoundsOfTransform,
			UnpauseAllSoundsOfTransform,
			StopSoundGroupOfTransform,
			PauseSoundGroupOfTransform,
			UnpauseSoundGroupOfTransform,
			FadeOutSoundGroupOfTransform,
			RefillSoundGroupPool
		}

		public enum PersistentSettingsCommand
		{
			None,
			SetBusVolume,
			SetGroupVolume,
			SetMixerVolume,
			SetMusicVolume,
			MixerMuteToggle,
			MusicMuteToggle
		}

		public enum SongFadeInPosition
		{
			NewClipFromBeginning = 1,
			NewClipFromLastKnownPosition = 3,
			SynchronizeClips = 5
		}

		public enum SoundSpawnLocationMode
		{
			MasterAudioLocation,
			CallerLocation,
			AttachToCaller
		}

		public enum VariationCommand
		{
			None,
			Stop,
			Pause,
			Unpause
		}

		[Serializable]
		public class AudioGroupInfo
		{
			public List<AudioInfo> Sources;

			public int LastFramePlayed;

			public float LastTimePlayed;

			public MasterAudioGroup Group;

			public bool PlayedForWarming;

			public AudioGroupInfo(List<AudioInfo> sources, MasterAudioGroup groupScript)
			{
				Sources = sources;
				LastFramePlayed = -50;
				LastTimePlayed = -50f;
				Group = groupScript;
				PlayedForWarming = false;
			}
		}

		[Serializable]
		public class AudioInfo
		{
			public AudioSource Source;

			public float OriginalVolume;

			public float LastPercentageVolume;

			public float LastRandomVolume;

			public SoundGroupVariation Variation;

			public AudioInfo(SoundGroupVariation variation, AudioSource source, float origVol)
			{
				Variation = variation;
				Source = source;
				OriginalVolume = origVol;
				LastPercentageVolume = 1f;
				LastRandomVolume = 0f;
			}
		}

		[Serializable]
		public class Playlist
		{
			public enum CrossfadeTimeMode
			{
				UseMasterSetting,
				Override
			}

			public bool isExpanded = true;

			public string playlistName = "new playlist";

			public SongFadeInPosition songTransitionType = SongFadeInPosition.NewClipFromBeginning;

			public List<MusicSetting> MusicSettings;

			public AudioLocation bulkLocationMode;

			public CrossfadeTimeMode crossfadeMode;

			public float crossFadeTime = 1f;

			public bool fadeInFirstSong;

			public bool fadeOutLastSong;

			public bool resourceClipsAllLoadAsync = true;

			public Playlist()
			{
				MusicSettings = new List<MusicSetting>();
			}
		}

		[Serializable]
		public class SoundGroupRefillInfo
		{
			public float LastTimePlayed;

			public float InactivePeriodSeconds;

			public SoundGroupRefillInfo(float lastTimePlayed, float inactivePeriodSeconds)
			{
				LastTimePlayed = lastTimePlayed;
				InactivePeriodSeconds = inactivePeriodSeconds;
			}
		}

		public const string MasterAudioDefaultFolder = "Assets/DarkTonic/MasterAudio";

		public const string PreviewText = "Fading & random settings are ignored by preview in edit mode.";

		public const float SemiTonePitchFactor = 1.05946f;

		public const float SpatialBlend_2DValue = 0f;

		public const float SpatialBlend_3DValue = 1f;

		public const int FramesEarlyToTrigger = 2;

		public const string StoredLanguageNameKey = "~MA_Language_Key~";

		public const string UseDbKey = "~MA_UseDbScaleForVolume~";

		public const string UseCentsPitchKey = "~MA_UseCentsForPitch~";

		public const string HideLogoNavKey = "~MA_HideLogoNav~";

		public const string InstallationFolderKey = "~MA_InstallationPath~";

		public const string MixerWidthSettingKey = "~MA_MixerWidth~";

		public const string GizmoFileName = "MasterAudio Icon.png";

		public const int HardCodedBusOptions = 2;

		public const string AllBusesName = "[All]";

		public const string NoGroupName = "[None]";

		public const string DynamicGroupName = "[Type In]";

		public const string NoPlaylistName = "[No Playlist]";

		public const string NoVoiceLimitName = "[NO LMT]";

		public const string OnlyPlaylistControllerName = "~only~";

		public const float InnerLoopCheckInterval = 0.1f;

		public static readonly YieldInstruction EndOfFrameDelay = new WaitForEndOfFrame();

		public AudioLocation bulkLocationMode;

		public string groupTemplateName = "Default Single";

		public string audioSourceTemplateName = "Max Distance 500";

		public bool showGroupCreation = true;

		public bool useGroupTemplates;

		public DragGroupMode curDragGroupMode;

		public List<GameObject> groupTemplates = new List<GameObject>(10);

		public List<GameObject> audioSourceTemplates = new List<GameObject>(10);

		public bool mixerMuted;

		public bool playlistsMuted;

		public LanguageMode langMode;

		public SystemLanguage testLanguage = SystemLanguage.English;

		public SystemLanguage defaultLanguage = SystemLanguage.English;

		public List<SystemLanguage> supportedLanguages = new List<SystemLanguage> { SystemLanguage.English };

		public string busFilter = string.Empty;

		public bool useTextGroupFilter;

		public string textGroupFilter = string.Empty;

		public bool resourceClipsPauseDoNotUnload;

		public bool resourceClipsAllLoadAsync = true;

		public Transform playlistControllerPrefab;

		public bool persistBetweenScenes;

		public bool areGroupsExpanded = true;

		public Transform soundGroupTemplate;

		public Transform soundGroupVariationTemplate;

		public List<GroupBus> groupBuses = new List<GroupBus>();

		public bool busesShownInNarrow = true;

		public bool groupByBus = true;

		public bool showGizmos = true;

		public bool showAdvancedSettings = true;

		public bool showLocalization = true;

		public bool playListExpanded = true;

		public bool playlistsExpanded = true;

		public AllMusicSpatialBlendType musicSpatialBlendType;

		public float musicSpatialBlend;

		public AllMixerSpatialBlendType mixerSpatialBlendType = AllMixerSpatialBlendType.ForceAllTo3D;

		public float mixerSpatialBlend = 1f;

		public ItemSpatialBlendType newGroupSpatialType = ItemSpatialBlendType.ForceTo3D;

		public float newGroupSpatialBlend = 1f;

		public List<Playlist> musicPlaylists = new List<Playlist>
		{
			new Playlist()
		};

		public float _masterAudioVolume = 1f;

		public bool ignoreTimeScale;

		public bool useGaplessPlaylists;

		public bool saveRuntimeChanges;

		public bool prioritizeOnDistance;

		public int rePrioritizeEverySecIndex;

		public bool visualAdvancedExpanded = true;

		public bool logAdvancedExpanded = true;

		public bool showFadingSettings;

		public bool stopZeroVolumeVariations;

		public bool stopZeroVolumeGroups;

		public bool stopZeroVolumeBuses;

		public bool stopZeroVolumePlaylists;

		public bool resourceAdvancedExpanded = true;

		public bool useClipAgePriority;

		public bool LogSounds;

		public bool logCustomEvents;

		public bool disableLogging;

		public bool showMusicDucking;

		public bool enableMusicDucking = true;

		public List<DuckGroupInfo> musicDuckingSounds = new List<DuckGroupInfo>();

		public float defaultRiseVolStart = 0.5f;

		public float duckedVolumeMultiplier = 0.5f;

		public float crossFadeTime = 1f;

		public float _masterPlaylistVolume = 1f;

		public bool showGroupSelect;

		public string newEventName = "my event";

		public bool showCustomEvents = true;

		public List<CustomEvent> customEvents = new List<CustomEvent>();

		public Dictionary<string, DuckGroupInfo> duckingBySoundType = new Dictionary<string, DuckGroupInfo>();

		public int frames;

		public static bool _editMAFolder = false;

		public bool showUnityMixerGroupAssignment = true;

		private Transform _trans;

		private bool _soundsLoaded;

		private bool _warming;

		private static readonly Dictionary<string, AudioGroupInfo> AudioSourcesBySoundType = new Dictionary<string, AudioGroupInfo>();

		private static Dictionary<string, List<int>> _randomizer = new Dictionary<string, List<int>>();

		private static Dictionary<string, List<int>> _randomizerLeftovers = new Dictionary<string, List<int>>();

		private static Dictionary<string, List<int>> _clipsPlayedBySoundTypeOldestFirst = new Dictionary<string, List<int>>();

		private static readonly List<MasterAudioGroup> SoloedGroups = new List<MasterAudioGroup>();

		private static readonly List<BusFadeInfo> BusFades = new List<BusFadeInfo>();

		private static readonly List<GroupFadeInfo> GroupFades = new List<GroupFadeInfo>();

		private static readonly Dictionary<string, Dictionary<ICustomEventReceiver, Transform>> ReceiversByEventName = new Dictionary<string, Dictionary<ICustomEventReceiver, Transform>>();

		private static readonly Dictionary<string, PlaylistController> PlaylistControllersByName = new Dictionary<string, PlaylistController>();

		private static readonly Dictionary<string, SoundGroupRefillInfo> LastTimeSoundGroupPlayed = new Dictionary<string, SoundGroupRefillInfo>();

		private static MasterAudio _instance;

		private static AudioSource _previewerInstance;

		private static Transform _audListenerTrans;

		private static float _repriTime = -1f;

		private static List<string> _groupsToRemove;

		private static string _prospectiveMAFolder = string.Empty;

		private static YieldInstruction _innerLoopDelay;

		public static readonly List<SoundGroupCommand> GroupCommandsWithNoGroupSelector = new List<SoundGroupCommand>
		{
			SoundGroupCommand.None,
			SoundGroupCommand.PauseAllSoundsOfTransform,
			SoundGroupCommand.StopAllSoundsOfTransform,
			SoundGroupCommand.UnpauseAllSoundsOfTransform
		};

		public static readonly List<SoundGroupCommand> GroupCommandsWithNoAllGroupSelector = new List<SoundGroupCommand>
		{
			SoundGroupCommand.None,
			SoundGroupCommand.FadeOutSoundGroupOfTransform,
			SoundGroupCommand.PauseSoundGroupOfTransform,
			SoundGroupCommand.UnpauseSoundGroupOfTransform,
			SoundGroupCommand.StopSoundGroupOfTransform
		};

		private bool initialized;

		public static bool LoadingAudioBundle { get; set; }

		public static float PlaylistMasterVolume
		{
			get
			{
				return Instance._masterPlaylistVolume;
			}
			set
			{
				Instance._masterPlaylistVolume = value;
				List<PlaylistController> instances = PlaylistController.Instances;
				for (int i = 0; i < instances.Count; i++)
				{
					instances[i].UpdateMasterVolume();
				}
			}
		}

		public static bool LogSoundsEnabled
		{
			get
			{
				return Instance.LogSounds;
			}
			set
			{
				Instance.LogSounds = value;
			}
		}

		public static Transform AudioListenerTransform
		{
			get
			{
				if (_audListenerTrans != null)
				{
					return _audListenerTrans;
				}
				AudioListener audioListener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
				_audListenerTrans = ((!(audioListener == null)) ? audioListener.transform : null);
				return _audListenerTrans;
			}
		}

		public static PlaylistController OnlyPlaylistController
		{
			get
			{
				List<PlaylistController> instances = PlaylistController.Instances;
				if (instances.Count != 0)
				{
					return instances[0];
				}
				return null;
			}
		}

		public static bool IsWarming
		{
			get
			{
				return Instance._warming;
			}
		}

		public MixerWidthMode MixerWidth
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_MixerWidth~"))
				{
					PlayerPrefs.SetString("~MA_MixerWidth~", MixerWidthMode.Narrow.ToString());
				}
				MixerWidthMode result = MixerWidthMode.Narrow;
				if (!string.IsNullOrEmpty(PlayerPrefs.GetString("~MA_MixerWidth~")))
				{
					result = (MixerWidthMode)(int)Enum.Parse(typeof(MixerWidthMode), PlayerPrefs.GetString("~MA_MixerWidth~"));
				}
				return result;
			}
			set
			{
				PlayerPrefs.SetString("~MA_MixerWidth~", value.ToString());
			}
		}

		public static bool MixerMuted
		{
			get
			{
				return Instance.mixerMuted;
			}
			set
			{
				Instance.mixerMuted = value;
				if (value)
				{
					foreach (string key in AudioSourcesBySoundType.Keys)
					{
						MuteGroup(AudioSourcesBySoundType[key].Group.GameObjectName);
					}
					return;
				}
				foreach (string key2 in AudioSourcesBySoundType.Keys)
				{
					UnmuteGroup(AudioSourcesBySoundType[key2].Group.GameObjectName);
				}
			}
		}

		public static bool PlaylistsMuted
		{
			get
			{
				return Instance.playlistsMuted;
			}
			set
			{
				Instance.playlistsMuted = value;
				List<PlaylistController> instances = PlaylistController.Instances;
				for (int i = 0; i < instances.Count; i++)
				{
					if (value)
					{
						instances[i].MutePlaylist();
					}
					else
					{
						instances[i].UnmutePlaylist();
					}
				}
			}
		}

		public bool EnableMusicDucking
		{
			get
			{
				return enableMusicDucking;
			}
			set
			{
				enableMusicDucking = value;
			}
		}

		public float DuckedVolumeMultiplier
		{
			get
			{
				return duckedVolumeMultiplier;
			}
			set
			{
				duckedVolumeMultiplier = value;
				List<PlaylistController> instances = PlaylistController.Instances;
				for (int i = 0; i < instances.Count; i++)
				{
					instances[i].UpdateDuckedVolumeMultiplier();
				}
			}
		}

		public float MasterCrossFadeTime
		{
			get
			{
				return crossFadeTime;
			}
		}

		public static List<Playlist> MusicPlaylists
		{
			get
			{
				return Instance.musicPlaylists;
			}
		}

		public static List<GroupBus> GroupBuses
		{
			get
			{
				return Instance.groupBuses;
			}
		}

		public static List<string> RuntimeSoundGroupNames
		{
			get
			{
				if (!Application.isPlaying)
				{
					return new List<string>();
				}
				return new List<string>(AudioSourcesBySoundType.Keys);
			}
		}

		public static List<string> RuntimeBusNames
		{
			get
			{
				if (!Application.isPlaying)
				{
					return new List<string>();
				}
				List<string> list = new List<string>();
				for (int i = 0; i < Instance.groupBuses.Count; i++)
				{
					list.Add(Instance.groupBuses[i].busName);
				}
				return list;
			}
		}

		public static MasterAudio SafeInstance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				_instance = (MasterAudio)UnityEngine.Object.FindObjectOfType(typeof(MasterAudio));
				return _instance;
			}
		}

		public static MasterAudio Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				_instance = (MasterAudio)UnityEngine.Object.FindObjectOfType(typeof(MasterAudio));
				if (!(_instance == null) || Application.isPlaying)
				{
				}
				return _instance;
			}
			set
			{
				_instance = null;
			}
		}

		public static bool SoundsReady
		{
			get
			{
				return Instance != null && Instance._soundsLoaded;
			}
		}

		public static bool AppIsShuttingDown { get; set; }

		public List<string> GroupNames
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("[Type In]");
				list.Add("[None]");
				List<string> list2 = list;
				List<string> list3 = new List<string>(Trans.childCount);
				for (int i = 0; i < Trans.childCount; i++)
				{
					list3.Add(Trans.GetChild(i).name);
				}
				list3.Sort();
				list2.AddRange(list3);
				return list2;
			}
		}

		public List<string> BusNames
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("[Type In]");
				list.Add("[None]");
				List<string> list2 = list;
				for (int i = 0; i < groupBuses.Count; i++)
				{
					list2.Add(groupBuses[i].busName);
				}
				return list2;
			}
		}

		public List<string> PlaylistNames
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("[Type In]");
				list.Add("[None]");
				List<string> list2 = list;
				for (int i = 0; i < musicPlaylists.Count; i++)
				{
					list2.Add(musicPlaylists[i].playlistName);
				}
				return list2;
			}
		}

		public Transform Trans
		{
			get
			{
				if (_trans != null)
				{
					return _trans;
				}
				_trans = GetComponent<Transform>();
				return _trans;
			}
		}

		public bool ShouldShowUnityAudioMixerGroupAssignments
		{
			get
			{
				return showUnityMixerGroupAssignment;
			}
		}

		public List<string> CustomEventNames
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("[Type In]");
				list.Add("[None]");
				List<string> list2 = list;
				List<CustomEvent> list3 = Instance.customEvents;
				for (int i = 0; i < list3.Count; i++)
				{
					list2.Add(list3[i].EventName);
				}
				return list2;
			}
		}

		public static float MasterVolumeLevel
		{
			get
			{
				return Instance._masterAudioVolume;
			}
			set
			{
				Instance._masterAudioVolume = value;
				if (Application.isPlaying)
				{
					Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
					while (enumerator.MoveNext())
					{
						MasterAudioGroup group = enumerator.Current.Value.Group;
						SetGroupVolume(group.GameObjectName, group.groupMasterVolume);
					}
				}
			}
		}

		private static bool SceneHasMasterAudio
		{
			get
			{
				return Instance != null;
			}
		}

		public static bool IgnoreTimeScale
		{
			get
			{
				return Instance.ignoreTimeScale;
			}
		}

		public static YieldInstruction InnerLoopDelay
		{
			get
			{
				return _innerLoopDelay;
			}
		}

		public static SystemLanguage DynamicLanguage
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_Language_Key~"))
				{
					PlayerPrefs.SetString("~MA_Language_Key~", SystemLanguage.Unknown.ToString());
				}
				return (SystemLanguage)(int)Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString("~MA_Language_Key~"));
			}
			set
			{
				PlayerPrefs.SetString("~MA_Language_Key~", value.ToString());
				AudioResourceOptimizer.ClearSupportLanguageFolder();
			}
		}

		public static bool UseDbScaleForVolume
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_UseDbScaleForVolume~"))
				{
					PlayerPrefs.SetInt("~MA_UseDbScaleForVolume~", 0);
				}
				return PlayerPrefs.GetInt("~MA_UseDbScaleForVolume~") > 0;
			}
			set
			{
				PlayerPrefs.SetInt("~MA_UseDbScaleForVolume~", value ? 1 : 0);
			}
		}

		public static bool UseCentsForPitch
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_UseCentsForPitch~"))
				{
					PlayerPrefs.SetInt("~MA_UseCentsForPitch~", 0);
				}
				return PlayerPrefs.GetInt("~MA_UseCentsForPitch~") > 0;
			}
			set
			{
				PlayerPrefs.SetInt("~MA_UseCentsForPitch~", value ? 1 : 0);
			}
		}

		public static bool HideLogoNav
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_HideLogoNav~"))
				{
					PlayerPrefs.SetInt("~MA_HideLogoNav~", 0);
				}
				return PlayerPrefs.GetInt("~MA_HideLogoNav~") > 0;
			}
			set
			{
				PlayerPrefs.SetInt("~MA_HideLogoNav~", value ? 1 : 0);
			}
		}

		public static float ReprioritizeTime
		{
			get
			{
				if (_repriTime < 0f)
				{
					_repriTime = (float)(Instance.rePrioritizeEverySecIndex + 1) * 0.1f;
				}
				return _repriTime;
			}
		}

		public static string ProspectiveMAPath
		{
			get
			{
				return _prospectiveMAFolder;
			}
			set
			{
				_prospectiveMAFolder = value;
			}
		}

		public static string MasterAudioFolderPath
		{
			get
			{
				if (!PlayerPrefs.HasKey("~MA_InstallationPath~"))
				{
					PlayerPrefs.SetString("~MA_InstallationPath~", "Assets/DarkTonic/MasterAudio");
				}
				return PlayerPrefs.GetString("~MA_InstallationPath~");
			}
			set
			{
				PlayerPrefs.SetString("~MA_InstallationPath~", value);
			}
		}

		public static string GroupTemplateFolder
		{
			get
			{
				return MasterAudioFolderPath + "/Sources/Prefabs/GroupTemplates/";
			}
		}

		public static string AudioSourceTemplateFolder
		{
			get
			{
				return MasterAudioFolderPath + "/Sources/Prefabs/AudioSourceTemplates/";
			}
		}

		private void Awake()
		{
			if (UnityEngine.Object.FindObjectsOfType(typeof(MasterAudio)).Length > 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void DelayedAwake()
		{
			base.useGUILayout = false;
			_soundsLoaded = false;
			_innerLoopDelay = new WaitForSeconds(0.1f);
			AudioSource component = GetComponent<AudioSource>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			AudioSourcesBySoundType.Clear();
			PlaylistControllersByName.Clear();
			LastTimeSoundGroupPlayed.Clear();
			List<string> list = new List<string>();
			AudioResourceOptimizer.ClearAudioClips();
			PlaylistController.Instances = null;
			List<PlaylistController> instances = PlaylistController.Instances;
			for (int i = 0; i < instances.Count; i++)
			{
				PlaylistController playlistController = instances[i];
				if (!list.Contains(playlistController.name))
				{
					list.Add(playlistController.name);
					PlaylistControllersByName.Add(playlistController.name, playlistController);
					if (persistBetweenScenes)
					{
						UnityEngine.Object.DontDestroyOnLoad(playlistController);
					}
				}
			}
			if (persistBetweenScenes)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			List<int> list2 = new List<int>();
			_randomizer = new Dictionary<string, List<int>>();
			_randomizerLeftovers = new Dictionary<string, List<int>>();
			_clipsPlayedBySoundTypeOldestFirst = new Dictionary<string, List<int>>();
			string text = string.Empty;
			List<SoundGroupVariation> list3 = new List<SoundGroupVariation>();
			_groupsToRemove = new List<string>(Trans.childCount);
			for (int j = 0; j < Trans.childCount; j++)
			{
				Transform child = Trans.GetChild(j);
				List<AudioInfo> list4 = new List<AudioInfo>();
				MasterAudioGroup component2 = child.GetComponent<MasterAudioGroup>();
				if (component2 == null)
				{
					continue;
				}
				string text2 = child.name;
				if (string.IsNullOrEmpty(text) && text2.StartsWith("SFX"))
				{
					text = text2;
				}
				List<Transform> list5 = new List<Transform>();
				for (int k = 0; k < child.childCount; k++)
				{
					Transform child2 = child.GetChild(k);
					SoundGroupVariation component3 = child2.GetComponent<SoundGroupVariation>();
					AudioSource component4 = child2.GetComponent<AudioSource>();
					int weight = component3.weight;
					for (int l = 0; l < weight; l++)
					{
						if (l > 0)
						{
							GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(child2.gameObject, child.transform.position, Quaternion.identity);
							gameObject.transform.name = child2.gameObject.name;
							SoundGroupVariation component5 = gameObject.GetComponent<SoundGroupVariation>();
							component5.weight = 1;
							list5.Add(gameObject.transform);
							component4 = gameObject.GetComponent<AudioSource>();
							list4.Add(new AudioInfo(component5, component4, component4.volume));
							list3.Add(component5);
							switch (component5.audLocation)
							{
							case AudioLocation.ResourceFile:
								AudioResourceOptimizer.AddTargetForClip(component5.resourceFileName, component4);
								break;
							case AudioLocation.FileOnInternet:
								AudioResourceOptimizer.AddTargetForClip(component5.internetFileUrl, component4);
								break;
							}
						}
						else
						{
							list4.Add(new AudioInfo(component3, component4, component4.volume));
							list3.Add(component3);
							switch (component3.audLocation)
							{
							case AudioLocation.ResourceFile:
							{
								string localizedFileName = AudioResourceOptimizer.GetLocalizedFileName(component3.useLocalization, component3.resourceFileName);
								AudioResourceOptimizer.AddTargetForClip(localizedFileName, component4);
								break;
							}
							case AudioLocation.FileOnInternet:
								AudioResourceOptimizer.AddTargetForClip(component3.internetFileUrl, component4);
								break;
							}
						}
					}
				}
				for (int m = 0; m < list5.Count; m++)
				{
					list5[m].parent = child;
				}
				AudioGroupInfo audioGroupInfo = new AudioGroupInfo(list4, component2);
				if (component2.isSoloed)
				{
					SoloedGroups.Add(component2);
				}
				if (!AudioSourcesBySoundType.ContainsKey(text2))
				{
					float? groupVolume = PersistentAudioSettings.GetGroupVolume(text2);
					if (groupVolume.HasValue)
					{
						audioGroupInfo.Group.groupMasterVolume = groupVolume.Value;
					}
					AudioSourcesBySoundType.Add(text2, audioGroupInfo);
					for (int n = 0; n < list4.Count; n++)
					{
						list2.Add(n);
					}
					if (audioGroupInfo.Group.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
					{
						ArrayListUtil.SortIntArray(ref list2);
					}
					_randomizer.Add(text2, list2);
					_randomizerLeftovers.Add(text2, new List<int>(list2.Count));
					_clipsPlayedBySoundTypeOldestFirst.Add(text2, new List<int>());
					list2 = new List<int>();
				}
			}
			BusFades.Clear();
			GroupFades.Clear();
			for (int num = 0; num < musicPlaylists.Count; num++)
			{
				Playlist playlist = musicPlaylists[num];
				if (playlist.songTransitionType != SongFadeInPosition.SynchronizeClips || playlist.MusicSettings.Count < 2)
				{
					continue;
				}
				AudioClip clip = playlist.MusicSettings[0].clip;
				if (clip == null)
				{
					continue;
				}
				float length = clip.length;
				for (int num2 = 1; num2 < playlist.MusicSettings.Count; num2++)
				{
					AudioClip clip2 = playlist.MusicSettings[num2].clip;
					if (!(clip2 == null) && clip2.length != length)
					{
						break;
					}
				}
			}
			for (int num3 = 0; num3 < groupBuses.Count; num3++)
			{
				string busName = groupBuses[num3].busName;
				float? busVolume = PersistentAudioSettings.GetBusVolume(busName);
				if (busVolume.HasValue)
				{
					SetBusVolumeByName(busName, busVolume.Value);
				}
			}
			duckingBySoundType.Clear();
			for (int num4 = 0; num4 < musicDuckingSounds.Count; num4++)
			{
				DuckGroupInfo duckGroupInfo = musicDuckingSounds[num4];
				if (!duckingBySoundType.ContainsKey(duckGroupInfo.soundType))
				{
					duckingBySoundType.Add(duckGroupInfo.soundType, duckGroupInfo);
				}
			}
			_soundsLoaded = true;
			_warming = true;
			if (!string.IsNullOrEmpty(text))
			{
				PlaySoundResult playSoundResult = PlaySound3DFollowTransform(text, Trans, 0f);
				if (playSoundResult != null && playSoundResult.SoundPlayed)
				{
					playSoundResult.ActingVariation.Stop();
				}
			}
			FireCustomEvent("FakeEvent", _trans.position);
			for (int num5 = 0; num5 < customEvents.Count; num5++)
			{
				customEvents[num5].frameLastFired = -1;
			}
			frames = 0;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(EventSounds));
			if (array.Length > 0)
			{
				EventSounds eventSounds = array[0] as EventSounds;
				eventSounds.PlaySounds(eventSounds.particleCollisionSound, EventSounds.EventType.UserDefinedEvent);
			}
			_warming = false;
			for (int num6 = 0; num6 < list3.Count; num6++)
			{
				list3[num6].DisableUpdater();
			}
			PersistentAudioSettings.RestoreMasterSettings();
		}

		private void DelayedStart()
		{
			if (musicPlaylists.Count > 0 && musicPlaylists[0].MusicSettings != null && musicPlaylists[0].MusicSettings.Count > 0 && musicPlaylists[0].MusicSettings[0].clip != null && PlaylistControllersByName.Count != 0)
			{
			}
		}

		private void Update()
		{
			if (!initialized)
			{
				DelayedAwake();
				DelayedStart();
				initialized = true;
			}
			frames++;
			PerformBusFades();
			PerformGroupFades();
			RefillInactiveGroupPools();
		}

		private static void UpdateRefillTime(string sType, float inactivePeriodSeconds)
		{
			if (!LastTimeSoundGroupPlayed.ContainsKey(sType))
			{
				LastTimeSoundGroupPlayed.Add(sType, new SoundGroupRefillInfo(Time.realtimeSinceStartup, inactivePeriodSeconds));
			}
			else
			{
				LastTimeSoundGroupPlayed[sType].LastTimePlayed = Time.realtimeSinceStartup;
			}
		}

		private static void RefillInactiveGroupPools()
		{
			Dictionary<string, SoundGroupRefillInfo>.Enumerator enumerator = LastTimeSoundGroupPlayed.GetEnumerator();
			if (_groupsToRemove == null)
			{
				_groupsToRemove = new List<string>();
			}
			_groupsToRemove.Clear();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, SoundGroupRefillInfo> current = enumerator.Current;
				if (current.Value.LastTimePlayed + current.Value.InactivePeriodSeconds < Time.realtimeSinceStartup)
				{
					RefillSoundGroupPool(current.Key);
					_groupsToRemove.Add(current.Key);
				}
			}
			for (int i = 0; i < _groupsToRemove.Count; i++)
			{
				LastTimeSoundGroupPlayed.Remove(_groupsToRemove[i]);
			}
		}

		private void PerformBusFades()
		{
			for (int i = 0; i < BusFades.Count; i++)
			{
				BusFadeInfo busFadeInfo = BusFades[i];
				if (!busFadeInfo.IsActive)
				{
					continue;
				}
				GroupBus actingBus = busFadeInfo.ActingBus;
				if (actingBus == null)
				{
					busFadeInfo.IsActive = false;
					continue;
				}
				float val = actingBus.volume + busFadeInfo.VolumeStep;
				val = ((!(busFadeInfo.VolumeStep > 0f)) ? Math.Max(val, busFadeInfo.TargetVolume) : Math.Min(val, busFadeInfo.TargetVolume));
				SetBusVolumeByName(actingBus.busName, val);
				if (val == busFadeInfo.TargetVolume)
				{
					busFadeInfo.IsActive = false;
					if (stopZeroVolumeBuses && busFadeInfo.TargetVolume == 0f)
					{
						StopBus(busFadeInfo.NameOfBus);
					}
					if (busFadeInfo.completionAction != null)
					{
						busFadeInfo.completionAction();
					}
				}
			}
			BusFades.RemoveAll((BusFadeInfo obj) => !obj.IsActive);
		}

		private void PerformGroupFades()
		{
			for (int i = 0; i < GroupFades.Count; i++)
			{
				GroupFadeInfo groupFadeInfo = GroupFades[i];
				if (!groupFadeInfo.IsActive)
				{
					continue;
				}
				MasterAudioGroup actingGroup = groupFadeInfo.ActingGroup;
				if (actingGroup == null)
				{
					groupFadeInfo.IsActive = false;
					continue;
				}
				float val = actingGroup.groupMasterVolume + groupFadeInfo.VolumeStep;
				val = ((!(groupFadeInfo.VolumeStep > 0f)) ? Math.Max(val, groupFadeInfo.TargetVolume) : Math.Min(val, groupFadeInfo.TargetVolume));
				SetGroupVolume(actingGroup.GameObjectName, val);
				if (val == groupFadeInfo.TargetVolume)
				{
					groupFadeInfo.IsActive = false;
					if (stopZeroVolumeGroups && groupFadeInfo.TargetVolume == 0f)
					{
						StopAllOfSound(groupFadeInfo.NameOfGroup);
					}
					if (groupFadeInfo.completionAction != null)
					{
						groupFadeInfo.completionAction();
					}
				}
			}
			GroupFades.RemoveAll((GroupFadeInfo obj) => !obj.IsActive);
		}

		private void OnApplicationQuit()
		{
			AppIsShuttingDown = true;
		}

		public static void PlaySoundAndForget(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (SceneHasMasterAudio && SoundsReady)
			{
				PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, null, variationName, false, delaySoundTime);
			}
		}

		public static PlaySoundResult PlaySound(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (SoundsReady)
			{
				return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, null, variationName, false, delaySoundTime, false, true, isChaining, isSingleSubscribedPlay);
			}
			return null;
		}

		public static bool SoundExists(string sType)
		{
			return AudioSourcesBySoundType.ContainsKey(sType);
		}

		public static void PlaySound3DAtVector3AndForget(string sType, Vector3 sourcePosition, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (SceneHasMasterAudio && SoundsReady)
			{
				PlaySoundAtVolume(sType, volumePercentage, sourcePosition, pitch, null, variationName, false, delaySoundTime, true);
			}
		}

		public static PlaySoundResult PlaySound3DAtVector3(string sType, Vector3 sourcePosition, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (SoundsReady)
			{
				return PlaySoundAtVolume(sType, volumePercentage, sourcePosition, pitch, null, variationName, false, delaySoundTime, true, true);
			}
			return null;
		}

		public static void PlaySound3DAtTransformAndForget(string sType, Transform sourceTrans = null, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (SceneHasMasterAudio && SoundsReady)
			{
				PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, sourceTrans, variationName, false, delaySoundTime);
			}
		}

		public static PlaySoundResult PlaySound3DAtTransform(string sType, Transform sourceTrans = null, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (SoundsReady)
			{
				return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, sourceTrans, variationName, false, delaySoundTime, false, true, isChaining, isSingleSubscribedPlay);
			}
			return null;
		}

		public static void PlaySound3DFollowTransformAndForget(string sType, Transform sourceTrans = null, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (SceneHasMasterAudio && SoundsReady)
			{
				PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, sourceTrans, variationName, true, delaySoundTime);
			}
		}

		public static PlaySoundResult PlaySound3DFollowTransform(string sType, Transform sourceTrans = null, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (SoundsReady)
			{
				return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, sourceTrans, variationName, true, delaySoundTime, false, true, isChaining, isSingleSubscribedPlay);
			}
			return null;
		}

		public static void PlaySound3DAndForget(string sType, Transform sourceTrans = null, bool attachToSource = false, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (SceneHasMasterAudio && SoundsReady)
			{
				PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, pitch, sourceTrans, variationName, attachToSource, delaySoundTime);
			}
		}

		public static IEnumerator PlaySoundAndWaitUntilFinished(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (!SceneHasMasterAudio || !SoundsReady)
			{
				yield break;
			}
			PlaySoundResult sound = PlaySound(sType, volumePercentage, pitch, delaySoundTime, variationName, false, true);
			bool done = false;
			if (sound != null && !(sound.ActingVariation == null))
			{
				sound.ActingVariation.SoundFinished += delegate
				{
					done = true;
				};
				while (!done)
				{
					yield return EndOfFrameDelay;
				}
			}
		}

		public static IEnumerator PlaySound3DAtTransformAndWaitUntilFinished(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (!SceneHasMasterAudio || !SoundsReady)
			{
				yield break;
			}
			PlaySoundResult sound = PlaySound3DAtTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, false, true);
			bool done = false;
			if (sound != null && !(sound.ActingVariation == null))
			{
				sound.ActingVariation.SoundFinished += delegate
				{
					done = true;
				};
				while (!done)
				{
					yield return EndOfFrameDelay;
				}
			}
		}

		public static IEnumerator PlaySound3DFollowTransformAndWaitUntilFinished(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null)
		{
			if (!SceneHasMasterAudio || !SoundsReady)
			{
				yield break;
			}
			PlaySoundResult sound = PlaySound3DFollowTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, false, true);
			bool done = false;
			if (sound != null && !(sound.ActingVariation == null))
			{
				sound.ActingVariation.SoundFinished += delegate
				{
					done = true;
				};
				while (!done)
				{
					yield return EndOfFrameDelay;
				}
			}
		}

		private static PlaySoundResult PlaySoundAtVolume(string sType, float volumePercentage, Vector3 sourcePosition, float? pitch = null, Transform sourceTrans = null, string variationName = null, bool attachToSource = false, float delaySoundTime = 0f, bool useVector3 = false, bool makePlaySoundResult = false, bool isChaining = false, bool isSingleSubscribedPlay = false, bool triggeredAsChildGroup = false)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (!SoundsReady || sType == string.Empty || sType == "[None]")
			{
				return null;
			}
			if (!AudioSourcesBySoundType.ContainsKey(sType))
			{
				string text = "MasterAudio could not find sound: " + sType + ". If your Scene just changed, this could happen when an OnDisable or OnInvisible event sound happened to a per-scene sound, which is expected.";
				if (sourceTrans != null)
				{
					text = text + " Triggered by prefab: " + sourceTrans.name;
				}
				LogWarning(text);
				return null;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			MasterAudioGroup group = audioGroupInfo.Group;
			bool flag = LoggingEnabledForGroup(group);
			if (audioGroupInfo.Group.childGroupMode == MasterAudioGroup.ChildGroupMode.TriggerLinkedGroupsWhenRequested && !triggeredAsChildGroup)
			{
				for (int i = 0; i < audioGroupInfo.Group.childSoundGroups.Count; i++)
				{
					string sType2 = audioGroupInfo.Group.childSoundGroups[i];
					PlaySoundAtVolume(sType2, volumePercentage, sourcePosition, pitch, sourceTrans, null, attachToSource, delaySoundTime, useVector3, false, false, false, true);
				}
			}
			if (Instance.mixerMuted)
			{
				if (flag)
				{
					LogMessage("MasterAudio skipped playing sound: " + sType + " because the Mixer is muted.");
				}
				return null;
			}
			if (group.isMuted)
			{
				if (flag)
				{
					LogMessage("MasterAudio skipped playing sound: " + sType + " because the Group is muted.");
				}
				return null;
			}
			if (SoloedGroups.Count > 0 && !SoloedGroups.Contains(group))
			{
				if (flag)
				{
					LogMessage("MasterAudio skipped playing sound: " + sType + " because there are one or more Groups soloed. This one is not.");
				}
				return null;
			}
			audioGroupInfo.PlayedForWarming = IsWarming;
			if (group.curVariationMode == MasterAudioGroup.VariationMode.Normal)
			{
				switch (group.limitMode)
				{
				case MasterAudioGroup.LimitMode.TimeBased:
					if (!(group.minimumTimeBetween > 0f))
					{
						break;
					}
					if (Time.realtimeSinceStartup < audioGroupInfo.LastTimePlayed + group.minimumTimeBetween)
					{
						if (flag)
						{
							LogMessage("MasterAudio skipped playing sound: " + sType + " due to Group's Min Seconds Between setting.");
						}
						return null;
					}
					audioGroupInfo.LastTimePlayed = Time.realtimeSinceStartup;
					break;
				case MasterAudioGroup.LimitMode.FrameBased:
					if (Time.frameCount - audioGroupInfo.LastFramePlayed < group.limitPerXFrames)
					{
						if (flag)
						{
							LogMessage("Master Audio skipped playing sound: " + sType + " due to Group's Per Frame Limit.");
						}
						return null;
					}
					audioGroupInfo.LastFramePlayed = Time.frameCount;
					break;
				case MasterAudioGroup.LimitMode.None:
					SetLastPlayed(audioGroupInfo);
					break;
				}
			}
			else
			{
				SetLastPlayed(audioGroupInfo);
			}
			List<AudioInfo> sources = audioGroupInfo.Sources;
			bool flag2 = string.IsNullOrEmpty(variationName);
			if (sources.Count == 0)
			{
				if (flag)
				{
					LogMessage("Sound Group {" + sType + "} has no active variations.");
				}
				return null;
			}
			if (group.curVariationMode == MasterAudioGroup.VariationMode.Normal && audioGroupInfo.Group.limitPolyphony)
			{
				int voiceLimitCount = audioGroupInfo.Group.voiceLimitCount;
				int num = 0;
				for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
				{
					if (audioGroupInfo.Sources[j].Source == null || !audioGroupInfo.Sources[j].Source.isPlaying)
					{
						continue;
					}
					num++;
					if (num >= voiceLimitCount)
					{
						if (flag)
						{
							LogMessage("Polyphony limit of group: " + audioGroupInfo.Group.GameObjectName + " exceeded. Will not play this sound for this instance.");
						}
						return null;
					}
				}
			}
			GroupBus busForGroup = audioGroupInfo.Group.BusForGroup;
			if (busForGroup != null && busForGroup.BusVoiceLimitReached)
			{
				if (!busForGroup.stopOldest)
				{
					if (flag)
					{
						LogMessage("Bus voice limit has been reached. Cannot play the sound: " + audioGroupInfo.Group.GameObjectName + " until one voice has stopped playing. You can turn on the 'Stop Oldest' option for the bus to change ");
					}
					return null;
				}
				StopOldestSoundOnBus(busForGroup);
			}
			AudioInfo audioInfo = null;
			if (sources.Count == 1)
			{
				if (flag)
				{
					LogMessage("Cueing only child of " + sType);
				}
				audioInfo = sources[0];
			}
			List<int> list = null;
			int? randomIndex = null;
			List<int> list2 = null;
			int num2 = -1;
			if (audioInfo == null)
			{
				if (!_randomizer.ContainsKey(sType))
				{
					return null;
				}
				if (flag2)
				{
					list = _randomizer[sType];
					randomIndex = 0;
					num2 = list[randomIndex.Value];
					audioInfo = sources[num2];
					list2 = _randomizerLeftovers[sType];
					list2.Remove(num2);
					if (flag)
					{
						LogMessage(string.Format("Cueing child {0} of {1}", list[randomIndex.Value], sType));
					}
				}
				else
				{
					bool flag3 = false;
					int num3 = 0;
					for (int k = 0; k < sources.Count; k++)
					{
						AudioInfo audioInfo2 = sources[k];
						if (!(audioInfo2.Source.name != variationName))
						{
							num3++;
							if (audioInfo2.Variation.IsAvailableToPlay)
							{
								audioInfo = audioInfo2;
								flag3 = true;
								num2 = k;
								break;
							}
						}
					}
					if (!flag3)
					{
						if (num3 == 0)
						{
							if (flag)
							{
								LogMessage("Can't find variation {" + variationName + "} of " + sType);
							}
						}
						else if (flag)
						{
							LogMessage("Can't find non-busy variation {" + variationName + "} of " + sType);
						}
						return null;
					}
					if (flag)
					{
						LogMessage(string.Format("Cueing child named '{0}' of {1}", variationName, sType));
					}
				}
			}
			if (audioInfo.Variation == null)
			{
				return null;
			}
			if (audioInfo.Variation.audLocation == AudioLocation.Clip && audioInfo.Variation.VarAudio.clip == null)
			{
				if (flag)
				{
					LogMessage(string.Format("Child named '{0}' of {1} has no audio assigned to it so nothing will be played.", audioInfo.Variation.name, sType));
				}
				RemoveClipAndRefillIfEmpty(audioGroupInfo, flag2, randomIndex, list, sType, num2, flag);
				return null;
			}
			if (audioGroupInfo.Group.curVariationMode == MasterAudioGroup.VariationMode.Dialog)
			{
				if (audioGroupInfo.Group.useDialogFadeOut)
				{
					FadeOutAllOfSound(audioGroupInfo.Group.GameObjectName, audioGroupInfo.Group.dialogFadeOutTime);
				}
				else
				{
					StopAllOfSound(audioGroupInfo.Group.GameObjectName);
				}
			}
			bool flag4 = false;
			bool forgetSoundPlayed = false;
			PlaySoundResult playSoundResult;
			bool flag7;
			do
			{
				playSoundResult = PlaySoundIfAvailable(audioInfo, sourcePosition, volumePercentage, ref forgetSoundPlayed, pitch, audioGroupInfo, sourceTrans, attachToSource, delaySoundTime, useVector3, makePlaySoundResult, isChaining, isSingleSubscribedPlay);
				bool flag5 = makePlaySoundResult && playSoundResult != null && (playSoundResult.SoundPlayed || playSoundResult.SoundScheduled);
				bool flag6 = !makePlaySoundResult && forgetSoundPlayed;
				flag7 = flag5 || flag6;
				if (flag7)
				{
					flag4 = true;
					if (!IsWarming)
					{
						RemoveClipAndRefillIfEmpty(audioGroupInfo, flag2, randomIndex, list, sType, num2, flag);
					}
				}
				else if (flag2)
				{
					if (list2 != null && list2.Count > 0)
					{
						audioInfo = sources[list2[0]];
						if (flag)
						{
							LogMessage("Child was busy. Cueing child {" + sources[list2[0]].Source.name + "} of " + sType);
						}
						list2.RemoveAt(0);
					}
				}
				else
				{
					if (flag)
					{
						LogMessage("Child was busy. Since you wanted a named Variation, no others to try. Aborting.");
					}
					if (list2 != null)
					{
						list2.Clear();
					}
				}
			}
			while (!flag4 && list2 != null && list2.Count > 0);
			if (!flag7)
			{
				if (flag)
				{
					LogMessage("All children of " + sType + " were busy. Will not play this sound for this instance.");
				}
			}
			else
			{
				if (audioGroupInfo.Group.childGroupMode == MasterAudioGroup.ChildGroupMode.TriggerLinkedGroupsWhenPlayed && !triggeredAsChildGroup && !IsWarming)
				{
					for (int l = 0; l < audioGroupInfo.Group.childSoundGroups.Count; l++)
					{
						string sType3 = audioGroupInfo.Group.childSoundGroups[l];
						PlaySoundAtVolume(sType3, volumePercentage, sourcePosition, pitch, sourceTrans, null, attachToSource, delaySoundTime, useVector3, false, false, false, true);
					}
				}
				if (audioGroupInfo.Group.soundPlayedEventActive)
				{
					FireCustomEvent(audioGroupInfo.Group.soundPlayedCustomEvent, Instance._trans.position);
				}
			}
			return playSoundResult;
		}

		private static void SetLastPlayed(AudioGroupInfo grp)
		{
			grp.LastTimePlayed = Time.realtimeSinceStartup;
			grp.LastFramePlayed = Time.frameCount;
		}

		private static void RemoveClipAndRefillIfEmpty(AudioGroupInfo grp, bool isNonSpecific, int? randomIndex, List<int> choices, string sType, int pickedChoice, bool loggingEnabledForGrp)
		{
			if (isNonSpecific && randomIndex.HasValue)
			{
				choices.RemoveAt(randomIndex.Value);
				_clipsPlayedBySoundTypeOldestFirst[sType].Add(pickedChoice);
				if (choices.Count == 0)
				{
					if (loggingEnabledForGrp)
					{
						LogMessage("Refilling Variation pool: " + sType);
					}
					RefillSoundGroupPool(sType);
				}
			}
			if (grp.Group.curVariationSequence == MasterAudioGroup.VariationSequence.TopToBottom && grp.Group.useInactivePeriodPoolRefill)
			{
				UpdateRefillTime(sType, grp.Group.inactivePeriodSeconds);
			}
		}

		private static PlaySoundResult PlaySoundIfAvailable(AudioInfo info, Vector3 sourcePosition, float volumePercentage, ref bool forgetSoundPlayed, float? pitch = null, AudioGroupInfo audioGroup = null, Transform sourceTrans = null, bool attachToSource = false, float delaySoundTime = 0f, bool useVector3 = false, bool makePlaySoundResult = false, bool isChaining = false, bool isSingleSubscribedPlay = false)
		{
			if (info.Source == null)
			{
				return null;
			}
			MasterAudioGroup group = audioGroup.Group;
			if (group.curVariationMode == MasterAudioGroup.VariationMode.Normal && info.Source.isPlaying)
			{
				float audioPlayedPercentage = AudioUtil.GetAudioPlayedPercentage(info.Source);
				int retriggerPercentage = group.retriggerPercentage;
				if (audioPlayedPercentage < (float)retriggerPercentage)
				{
					return null;
				}
			}
			info.Variation.Stop();
			info.Variation.ObjectToFollow = null;
			bool flag = Instance.prioritizeOnDistance && (Instance.useClipAgePriority || info.Variation.ParentGroup.useClipAgePriority);
			if (useVector3)
			{
				info.Source.transform.position = sourcePosition;
				if (Instance.prioritizeOnDistance)
				{
					AudioPrioritizer.Set3DPriority(info.Source, flag);
				}
			}
			else if (sourceTrans != null)
			{
				if (attachToSource)
				{
					info.Variation.ObjectToFollow = sourceTrans;
				}
				else
				{
					info.Source.transform.position = sourceTrans.position;
					info.Variation.ObjectToTriggerFrom = sourceTrans;
				}
				if (Instance.prioritizeOnDistance)
				{
					AudioPrioritizer.Set3DPriority(info.Source, flag);
				}
			}
			else
			{
				if (Instance.prioritizeOnDistance)
				{
					AudioPrioritizer.Set2DSoundPriority(info.Source);
				}
				info.Source.transform.localPosition = Vector3.zero;
			}
			float groupMasterVolume = group.groupMasterVolume;
			float busVolume = GetBusVolume(group);
			float num = info.OriginalVolume;
			float num2 = 0f;
			if (info.Variation.useRandomVolume)
			{
				num2 = UnityEngine.Random.Range(info.Variation.randomVolumeMin, info.Variation.randomVolumeMax);
				switch (info.Variation.randomVolumeMode)
				{
				case SoundGroupVariation.RandomVolumeMode.AddToClipVolume:
					num += num2;
					break;
				case SoundGroupVariation.RandomVolumeMode.IgnoreClipVolume:
					num = num2;
					break;
				}
			}
			float num3 = num * groupMasterVolume * busVolume * Instance._masterAudioVolume;
			float num4 = num3 * volumePercentage;
			float num5 = num4;
			info.Source.volume = num5;
			info.LastPercentageVolume = volumePercentage;
			info.LastRandomVolume = num2;
			if (!info.Variation.GameObj.activeInHierarchy)
			{
				return null;
			}
			PlaySoundResult playSoundResult = null;
			if (makePlaySoundResult)
			{
				PlaySoundResult playSoundResult2 = new PlaySoundResult();
				playSoundResult2.ActingVariation = info.Variation;
				playSoundResult = playSoundResult2;
				if (delaySoundTime > 0f)
				{
					playSoundResult.SoundScheduled = true;
				}
				else
				{
					playSoundResult.SoundPlayed = true;
				}
			}
			else
			{
				forgetSoundPlayed = true;
			}
			string gameObjectName = group.GameObjectName;
			if (group.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain)
			{
				if (!isChaining)
				{
					group.ChainLoopCount = 0;
				}
				Transform objectToFollow = info.Variation.ObjectToFollow;
				if (group.ActiveVoices > 0 && !isChaining)
				{
					StopAllOfSound(gameObjectName);
				}
				info.Variation.ObjectToFollow = objectToFollow;
			}
			info.Variation.Play(pitch, num5, gameObjectName, volumePercentage, num5, pitch, sourceTrans, attachToSource, delaySoundTime, isChaining, isSingleSubscribedPlay);
			return playSoundResult;
		}

		public static void DuckSoundGroup(string soundGroupName, AudioSource aSource)
		{
			MasterAudio instance = Instance;
			if (instance.EnableMusicDucking && instance.duckingBySoundType.ContainsKey(soundGroupName) && !(aSource.clip == null))
			{
				DuckGroupInfo duckGroupInfo = instance.duckingBySoundType[soundGroupName];
				float length = aSource.clip.length;
				float pitch = aSource.pitch;
				List<PlaylistController> instances = PlaylistController.Instances;
				for (int i = 0; i < instances.Count; i++)
				{
					instances[i].DuckMusicForTime(length, pitch, duckGroupInfo.riseVolStart);
				}
			}
		}

		private static void StopPauseOrUnpauseSoundsOfTransform(Transform trans, List<AudioInfo> varList, VariationCommand varCmd)
		{
			MasterAudioGroup masterAudioGroup = null;
			for (int i = 0; i < varList.Count; i++)
			{
				SoundGroupVariation variation = varList[i].Variation;
				if (!variation.WasTriggeredFromTransform(trans))
				{
					continue;
				}
				if (masterAudioGroup == null)
				{
					string gameObjectName = variation.ParentGroup.GameObjectName;
					masterAudioGroup = GrabGroup(gameObjectName);
				}
				bool stopEndDetection = masterAudioGroup != null && masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain;
				switch (varCmd)
				{
				case VariationCommand.Stop:
					variation.Stop(stopEndDetection);
					break;
				case VariationCommand.Pause:
					variation.Pause();
					break;
				case VariationCommand.Unpause:
					if (AudioUtil.IsAudioPaused(variation.VarAudio))
					{
						variation.VarAudio.Play();
					}
					break;
				}
			}
		}

		public static void StopAllSoundsOfTransform(Transform trans)
		{
			if (!SceneHasMasterAudio)
			{
				return;
			}
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[key].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Stop);
			}
		}

		public static void StopSoundGroupOfTransform(Transform trans, string sType)
		{
			if (SceneHasMasterAudio && AudioSourcesBySoundType.ContainsKey(sType))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Stop);
			}
		}

		public static void PauseAllSoundsOfTransform(Transform trans)
		{
			if (!SceneHasMasterAudio)
			{
				return;
			}
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[key].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Pause);
			}
		}

		public static void PauseSoundGroupOfTransform(Transform trans, string sType)
		{
			if (SceneHasMasterAudio && AudioSourcesBySoundType.ContainsKey(sType))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Pause);
			}
		}

		public static void UnpauseAllSoundsOfTransform(Transform trans)
		{
			if (!SceneHasMasterAudio)
			{
				return;
			}
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[key].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Unpause);
			}
		}

		public static void UnpauseSoundGroupOfTransform(Transform trans, string sType)
		{
			if (SceneHasMasterAudio && AudioSourcesBySoundType.ContainsKey(sType))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				StopPauseOrUnpauseSoundsOfTransform(trans, sources, VariationCommand.Unpause);
			}
		}

		public static void FadeOutSoundGroupOfTransform(Transform trans, string sType, float fadeTime)
		{
			if (!SceneHasMasterAudio || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				SoundGroupVariation variation = sources[i].Variation;
				if (variation.WasTriggeredFromTransform(trans))
				{
					variation.FadeOutNow(fadeTime);
				}
			}
		}

		public static void StopAllOfSound(string sType)
		{
			if (!SceneHasMasterAudio || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			bool stopEndDetection = masterAudioGroup != null && masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain;
			foreach (AudioInfo item in sources)
			{
				item.Variation.Stop(stopEndDetection);
			}
		}

		public static void FadeOutAllOfSound(string sType, float fadeTime)
		{
			if (!SceneHasMasterAudio || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
			foreach (AudioInfo item in sources)
			{
				item.Variation.FadeOutNow(fadeTime);
			}
		}

		public static void TriggerParticleEmission(Transform trans, int particleCount)
		{
			ParticleSystem component = trans.GetComponent<ParticleSystem>();
			if (!(component == null))
			{
				component.Emit(particleCount);
			}
		}

		public static List<SoundGroupVariation> GetAllPlayingVariations()
		{
			List<SoundGroupVariation> list = new List<SoundGroupVariation>(32);
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[key].Sources;
				for (int i = 0; i < sources.Count; i++)
				{
					SoundGroupVariation variation = sources[i].Variation;
					if (variation.IsPlaying)
					{
						list.Add(variation);
					}
				}
			}
			return list;
		}

		public static List<GameObject> GetAllPlayingVariationsInBus(string busName)
		{
			List<GameObject> list = new List<GameObject>(32);
			int busIndex = GetBusIndex(busName, false);
			if (busIndex < 0)
			{
				return list;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex != busIndex)
				{
					continue;
				}
				for (int i = 0; i < value.Sources.Count; i++)
				{
					SoundGroupVariation variation = value.Sources[i].Variation;
					if (variation.IsPlaying)
					{
						list.Add(variation.gameObject);
					}
				}
			}
			return list;
		}

		public static void CreateGroupVariationFromClip(string sType, AudioClip clip, string variationName, float volume = 1f, float pitch = 1f)
		{
			if (!SoundsReady || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			bool flag = false;
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				if (!(audioInfo.Variation.name != variationName))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				LogWarning("You already have a Variation for this Group named '" + variationName + "'. \n\nPlease rename these Variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Instance.soundGroupVariationTemplate.gameObject, audioGroupInfo.Group.transform.position, Quaternion.identity);
			gameObject.transform.name = variationName;
			gameObject.transform.parent = audioGroupInfo.Group.transform;
			AudioSource component = gameObject.GetComponent<AudioSource>();
			component.clip = clip;
			component.pitch = pitch;
			SoundGroupVariation component2 = gameObject.GetComponent<SoundGroupVariation>();
			if (component2.VariationUpdater != null)
			{
				component2.DisableUpdater();
			}
			AudioInfo item = new AudioInfo(component2, component2.VarAudio, volume);
			audioGroupInfo.Sources.Add(item);
			if (_randomizer.ContainsKey(sType))
			{
				_randomizer[sType].Add(audioGroupInfo.Sources.Count - 1);
				_randomizerLeftovers[sType].Add(audioGroupInfo.Sources.Count - 1);
			}
		}

		public static void ChangeVariationPitch(string sType, bool changeAllVariations, string variationName, float pitch)
		{
			if (!SoundsReady || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			int num = 0;
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				if (changeAllVariations || !(audioInfo.Source.transform.name != variationName))
				{
					audioInfo.Variation.original_pitch = pitch;
					AudioSource varAudio = audioInfo.Variation.VarAudio;
					if (varAudio != null)
					{
						varAudio.pitch = pitch;
					}
					num++;
				}
			}
			if (num == 0 && changeAllVariations)
			{
			}
		}

		public static void ChangeVariationVolume(string sType, bool changeAllVariations, string variationName, float volume)
		{
			if (!SoundsReady || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			int num = 0;
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				if (changeAllVariations || !(audioInfo.Source.transform.name != variationName))
				{
					audioInfo.OriginalVolume = volume;
					num++;
				}
			}
			if (num == 0 && changeAllVariations)
			{
			}
		}

		public static void ChangeVariationClipFromResources(string sType, bool changeAllVariations, string variationName, string resourceFileName)
		{
			if (SoundsReady)
			{
				AudioClip audioClip = ((!Application.isPlaying) ? (Resources.Load(resourceFileName) as AudioClip) : (Singleton<SLOTResourceManager>.Instance.LoadResource(resourceFileName) as AudioClip));
				if (audioClip == null)
				{
					LogWarning("Resource file '" + resourceFileName + "' could not be located.");
				}
				else
				{
					ChangeVariationClip(sType, changeAllVariations, variationName, audioClip);
				}
			}
		}

		public static void ChangeVariationClip(string sType, bool changeAllVariations, string variationName, AudioClip clip)
		{
			if (!SoundsReady || !AudioSourcesBySoundType.ContainsKey(sType))
			{
				return;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				if (changeAllVariations || audioInfo.Source.transform.name == variationName)
				{
					audioInfo.Source.clip = clip;
				}
			}
		}

		public static float GetVariationLength(string sType, string variationName)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (masterAudioGroup == null)
			{
				return -1f;
			}
			SoundGroupVariation soundGroupVariation = null;
			foreach (SoundGroupVariation groupVariation in masterAudioGroup.groupVariations)
			{
				if (groupVariation.name != variationName)
				{
					continue;
				}
				soundGroupVariation = groupVariation;
				break;
			}
			if (soundGroupVariation == null)
			{
				LogError("Could not find Variation '" + variationName + "' in Sound Group '" + sType + "'.");
				return -1f;
			}
			if (soundGroupVariation.audLocation == AudioLocation.ResourceFile)
			{
				LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' length cannot be determined because it's a Resource Files.");
				return -1f;
			}
			if (soundGroupVariation.audLocation == AudioLocation.FileOnInternet)
			{
				LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' length cannot be determined because it's an Internet File.");
				return -1f;
			}
			AudioClip clip = soundGroupVariation.VarAudio.clip;
			if (clip == null)
			{
				LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' has no Audio Clip.");
				return -1f;
			}
			if (!(soundGroupVariation.VarAudio.pitch <= 0f))
			{
				return clip.length / soundGroupVariation.VarAudio.pitch;
			}
			LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' has negative or zero pitch. Cannot compute length.");
			return -1f;
		}

		public static void RefillSoundGroupPool(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType, false);
			if (masterAudioGroup == null)
			{
				return;
			}
			List<int> list = _randomizer[sType];
			List<int> list2 = _clipsPlayedBySoundTypeOldestFirst[sType];
			if (list.Count > 0)
			{
				list2.AddRange(list);
				list.Clear();
			}
			int? num = null;
			if (list2.Count > 0)
			{
				num = list2[list2.Count - 1];
			}
			if (masterAudioGroup.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
			{
				ArrayListUtil.SortIntArray(ref list2);
				if (num.HasValue && num.Value == list2[0])
				{
					int item = list2[0];
					list2.RemoveAt(0);
					list2.Insert(UnityEngine.Random.Range(1, list2.Count), item);
				}
			}
			list.AddRange(list2);
			list2.Clear();
			if (masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain)
			{
				masterAudioGroup.ChainLoopCount++;
			}
		}

		public static bool SoundGroupExists(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType, false);
			return masterAudioGroup != null;
		}

		public static void PauseSoundGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				for (int i = 0; i < sources.Count; i++)
				{
					SoundGroupVariation variation = sources[i].Variation;
					variation.Pause();
				}
			}
		}

		public static void SetGroupSpatialBlend(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				for (int i = 0; i < sources.Count; i++)
				{
					SoundGroupVariation variation = sources[i].Variation;
					variation.SetSpatialBlend();
				}
			}
		}

		public void RouteGroupToUnityMixerGroup(string sType, AudioMixerGroup mixerGroup)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
				for (int i = 0; i < sources.Count; i++)
				{
					SoundGroupVariation variation = sources[i].Variation;
					variation.VarAudio.outputAudioMixerGroup = mixerGroup;
				}
			}
		}

		public static void UnpauseSoundGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (masterAudioGroup == null)
			{
				return;
			}
			List<AudioInfo> sources = AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				SoundGroupVariation variation = sources[i].Variation;
				if (AudioUtil.IsAudioPaused(variation.VarAudio))
				{
					variation.VarAudio.Play();
				}
			}
		}

		public static void FadeSoundGroupToVolume(string sType, float newVolume, float fadeTime, Action completionCallback = null)
		{
			if (newVolume < 0f || newVolume > 1f)
			{
				return;
			}
			if (fadeTime <= 0.1f)
			{
				SetGroupVolume(sType, newVolume);
				if (completionCallback != null)
				{
					completionCallback();
				}
				return;
			}
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null) && !(newVolume < 0f) && !(newVolume > 1f))
			{
				GroupFadeInfo groupFadeInfo = GroupFades.Find((GroupFadeInfo obj) => obj.NameOfGroup == sType);
				if (groupFadeInfo != null)
				{
					groupFadeInfo.IsActive = false;
				}
				float volumeStep = (newVolume - masterAudioGroup.groupMasterVolume) / (fadeTime / AudioUtil.FrameTime());
				GroupFadeInfo groupFadeInfo2 = new GroupFadeInfo();
				groupFadeInfo2.NameOfGroup = sType;
				groupFadeInfo2.ActingGroup = masterAudioGroup;
				groupFadeInfo2.VolumeStep = volumeStep;
				groupFadeInfo2.TargetVolume = newVolume;
				GroupFadeInfo groupFadeInfo3 = groupFadeInfo2;
				if (completionCallback != null)
				{
					groupFadeInfo3.completionAction = completionCallback;
				}
				GroupFades.Add(groupFadeInfo3);
			}
		}

		public static void RemoveSoundGroup(Transform groupTrans)
		{
			if (SafeInstance == null || groupTrans == null)
			{
				return;
			}
			string key = groupTrans.name;
			MasterAudio instance = Instance;
			if (instance.duckingBySoundType.ContainsKey(key))
			{
				instance.duckingBySoundType.Remove(key);
			}
			_randomizer.Remove(key);
			_randomizerLeftovers.Remove(key);
			_clipsPlayedBySoundTypeOldestFirst.Remove(key);
			AudioSourcesBySoundType.Remove(key);
			LastTimeSoundGroupPlayed.Remove(key);
			for (int i = 0; i < groupTrans.childCount; i++)
			{
				Transform child = groupTrans.GetChild(i);
				AudioSource component = child.GetComponent<AudioSource>();
				SoundGroupVariation component2 = child.GetComponent<SoundGroupVariation>();
				switch (component2.audLocation)
				{
				case AudioLocation.ResourceFile:
					AudioResourceOptimizer.DeleteAudioSourceFromList(component2.resourceFileName, component);
					break;
				case AudioLocation.FileOnInternet:
					AudioResourceOptimizer.DeleteAudioSourceFromList(component2.internetFileUrl, component);
					break;
				}
			}
			groupTrans.parent = null;
			UnityEngine.Object.Destroy(groupTrans.gameObject);
		}

		public static Transform CreateNewSoundGroup(DynamicSoundGroup aGroup, string creatorObjectName, bool errorOnExisting = true)
		{
			if (!SceneHasMasterAudio)
			{
				return null;
			}
			if (!SoundsReady)
			{
				return null;
			}
			string text = aGroup.transform.name;
			MasterAudio instance = Instance;
			if (Instance.Trans.Find(text) != null)
			{
				if (errorOnExisting)
				{
				}
				return null;
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(instance.soundGroupTemplate.gameObject, instance.Trans.position, Quaternion.identity);
			Transform transform = gameObject.transform;
			transform.name = UtilStrings.TrimSpace(text);
			transform.parent = Instance.Trans;
			for (int i = 0; i < aGroup.groupVariations.Count; i++)
			{
				DynamicGroupVariation dynamicGroupVariation = aGroup.groupVariations[i];
				for (int j = 0; j < dynamicGroupVariation.weight; j++)
				{
					GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(dynamicGroupVariation.gameObject, transform.position, Quaternion.identity);
					gameObject2.transform.parent = transform;
					UnityEngine.Object.Destroy(gameObject2.GetComponent<DynamicGroupVariation>());
					gameObject2.AddComponent<SoundGroupVariation>();
					SoundGroupVariation component = gameObject2.GetComponent<SoundGroupVariation>();
					string text2 = component.name;
					int num = text2.IndexOf("(Clone)");
					if (num >= 0)
					{
						text2 = text2.Substring(0, num);
					}
					AudioSource component2 = dynamicGroupVariation.GetComponent<AudioSource>();
					switch (dynamicGroupVariation.audLocation)
					{
					case AudioLocation.Clip:
					{
						AudioClip clip = component2.clip;
						component.VarAudio.clip = clip;
						break;
					}
					case AudioLocation.ResourceFile:
						AudioResourceOptimizer.AddTargetForClip(dynamicGroupVariation.resourceFileName, component.VarAudio);
						component.resourceFileName = dynamicGroupVariation.resourceFileName;
						component.useLocalization = dynamicGroupVariation.useLocalization;
						break;
					case AudioLocation.FileOnInternet:
						AudioResourceOptimizer.AddTargetForClip(dynamicGroupVariation.internetFileUrl, component.VarAudio);
						component.internetFileUrl = dynamicGroupVariation.internetFileUrl;
						break;
					}
					component.audLocation = dynamicGroupVariation.audLocation;
					component.original_pitch = component2.pitch;
					component.transform.name = text2;
					component.isExpanded = dynamicGroupVariation.isExpanded;
					component.useRandomPitch = dynamicGroupVariation.useRandomPitch;
					component.randomPitchMode = dynamicGroupVariation.randomPitchMode;
					component.randomPitchMin = dynamicGroupVariation.randomPitchMin;
					component.randomPitchMax = dynamicGroupVariation.randomPitchMax;
					component.useRandomVolume = dynamicGroupVariation.useRandomVolume;
					component.randomVolumeMode = dynamicGroupVariation.randomVolumeMode;
					component.randomVolumeMin = dynamicGroupVariation.randomVolumeMin;
					component.randomVolumeMax = dynamicGroupVariation.randomVolumeMax;
					component.useFades = dynamicGroupVariation.useFades;
					component.fadeInTime = dynamicGroupVariation.fadeInTime;
					component.fadeOutTime = dynamicGroupVariation.fadeOutTime;
					component.useIntroSilence = dynamicGroupVariation.useIntroSilence;
					component.introSilenceMin = dynamicGroupVariation.introSilenceMin;
					component.introSilenceMax = dynamicGroupVariation.introSilenceMax;
					component.fxTailTime = dynamicGroupVariation.fxTailTime;
					component.useRandomStartTime = dynamicGroupVariation.useRandomStartTime;
					component.randomStartMinPercent = dynamicGroupVariation.randomStartMinPercent;
					component.randomStartMaxPercent = dynamicGroupVariation.randomStartMaxPercent;
					if (component.LowPassFilter != null && !component.LowPassFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.LowPassFilter);
					}
					if (component.HighPassFilter != null && !component.HighPassFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.HighPassFilter);
					}
					if (component.DistortionFilter != null && !component.DistortionFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.DistortionFilter);
					}
					if (component.ChorusFilter != null && !component.ChorusFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.ChorusFilter);
					}
					if (component.EchoFilter != null && !component.EchoFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.EchoFilter);
					}
					if (component.ReverbFilter != null && !component.ReverbFilter.enabled)
					{
						UnityEngine.Object.Destroy(component.ReverbFilter);
					}
				}
			}
			MasterAudioGroup component3 = gameObject.GetComponent<MasterAudioGroup>();
			component3.retriggerPercentage = aGroup.retriggerPercentage;
			float? groupVolume = PersistentAudioSettings.GetGroupVolume(aGroup.name);
			if (groupVolume.HasValue)
			{
				component3.groupMasterVolume = groupVolume.Value;
			}
			else
			{
				component3.groupMasterVolume = aGroup.groupMasterVolume;
			}
			component3.limitMode = aGroup.limitMode;
			component3.limitPerXFrames = aGroup.limitPerXFrames;
			component3.minimumTimeBetween = aGroup.minimumTimeBetween;
			component3.limitPolyphony = aGroup.limitPolyphony;
			component3.voiceLimitCount = aGroup.voiceLimitCount;
			component3.curVariationSequence = aGroup.curVariationSequence;
			component3.useInactivePeriodPoolRefill = aGroup.useInactivePeriodPoolRefill;
			component3.inactivePeriodSeconds = aGroup.inactivePeriodSeconds;
			component3.curVariationMode = aGroup.curVariationMode;
			component3.useDialogFadeOut = aGroup.useDialogFadeOut;
			component3.dialogFadeOutTime = aGroup.dialogFadeOutTime;
			component3.chainLoopDelayMin = aGroup.chainLoopDelayMin;
			component3.chainLoopDelayMax = aGroup.chainLoopDelayMax;
			component3.chainLoopMode = aGroup.chainLoopMode;
			component3.chainLoopNumLoops = aGroup.chainLoopNumLoops;
			component3.childGroupMode = aGroup.childGroupMode;
			component3.childSoundGroups = aGroup.childSoundGroups;
			component3.soundPlayedEventActive = aGroup.soundPlayedEventActive;
			component3.soundPlayedCustomEvent = aGroup.soundPlayedCustomEvent;
			component3.targetDespawnedBehavior = aGroup.targetDespawnedBehavior;
			component3.despawnFadeTime = aGroup.despawnFadeTime;
			component3.resourceClipsAllLoadAsync = aGroup.resourceClipsAllLoadAsync;
			component3.logSound = aGroup.logSound;
			component3.alwaysHighestPriority = aGroup.alwaysHighestPriority;
			component3.spatialBlendType = aGroup.spatialBlendType;
			component3.spatialBlend = aGroup.spatialBlend;
			List<AudioInfo> list = new List<AudioInfo>();
			List<int> list2 = new List<int>();
			for (int k = 0; k < gameObject.transform.childCount; k++)
			{
				list2.Add(k);
				Transform child = gameObject.transform.GetChild(k);
				AudioSource component4 = child.GetComponent<AudioSource>();
				SoundGroupVariation component = child.GetComponent<SoundGroupVariation>();
				list.Add(new AudioInfo(component, component4, component4.volume));
				if (component.VariationUpdater != null)
				{
					component.DisableUpdater();
				}
			}
			AudioSourcesBySoundType.Add(text, new AudioGroupInfo(list, component3));
			if (component3.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
			{
				ArrayListUtil.SortIntArray(ref list2);
			}
			_randomizer.Add(text, list2);
			_randomizerLeftovers.Add(text, new List<int>(list2.Count));
			_clipsPlayedBySoundTypeOldestFirst.Add(text, new List<int>(list2.Count));
			if (string.IsNullOrEmpty(aGroup.busName))
			{
				return transform;
			}
			component3.busIndex = GetBusIndex(aGroup.busName, true);
			if (component3.BusForGroup != null && component3.BusForGroup.isMuted)
			{
				MuteGroup(component3.name);
			}
			return transform;
		}

		public static float GetGroupVolume(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (masterAudioGroup == null)
			{
				return 0f;
			}
			return masterAudioGroup.groupMasterVolume;
		}

		public static void SetGroupVolume(string sType, float volumeLevel)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType, Application.isPlaying);
			if (masterAudioGroup == null || AppIsShuttingDown)
			{
				return;
			}
			masterAudioGroup.groupMasterVolume = volumeLevel;
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			float busVolume = GetBusVolume(masterAudioGroup);
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				AudioSource source = audioInfo.Source;
				if (!(source == null))
				{
					float num2 = (source.volume = ((audioInfo.Variation.randomVolumeMode != 0) ? (audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * masterAudioGroup.groupMasterVolume * busVolume * Instance._masterAudioVolume) : (audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * masterAudioGroup.groupMasterVolume * busVolume * Instance._masterAudioVolume + audioInfo.LastRandomVolume)));
				}
			}
		}

		public static void MuteGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				SoloedGroups.Remove(masterAudioGroup);
				masterAudioGroup.isSoloed = false;
				SetGroupMuteStatus(masterAudioGroup, sType, true);
			}
		}

		public static void UnmuteGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				SetGroupMuteStatus(masterAudioGroup, sType, false);
			}
		}

		private static void SetGroupMuteStatus(MasterAudioGroup aGroup, string sType, bool isMute)
		{
			aGroup.isMuted = isMute;
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
			{
				AudioInfo audioInfo = audioGroupInfo.Sources[i];
				AudioSource source = audioInfo.Source;
				source.mute = isMute;
			}
		}

		public static void SoloGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				masterAudioGroup.isMuted = false;
				masterAudioGroup.isSoloed = true;
				SoloedGroups.Add(masterAudioGroup);
				SetGroupMuteStatus(masterAudioGroup, sType, false);
			}
		}

		public static void UnsoloGroup(string sType)
		{
			MasterAudioGroup masterAudioGroup = GrabGroup(sType);
			if (!(masterAudioGroup == null))
			{
				masterAudioGroup.isSoloed = false;
				SoloedGroups.Remove(masterAudioGroup);
			}
		}

		public static MasterAudioGroup GrabGroup(string sType, bool logIfMissing = true)
		{
			if (!AudioSourcesBySoundType.ContainsKey(sType))
			{
				if (logIfMissing)
				{
				}
				return null;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			return audioGroupInfo.Group;
		}

		public static List<AudioInfo> GetAllVariationsOfGroup(string sType, bool logIfMissing = true)
		{
			if (!AudioSourcesBySoundType.ContainsKey(sType))
			{
				if (logIfMissing)
				{
				}
				return null;
			}
			AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[sType];
			return audioGroupInfo.Sources;
		}

		public static AudioGroupInfo GetGroupInfo(string sType)
		{
			if (!AudioSourcesBySoundType.ContainsKey(sType))
			{
				return null;
			}
			return AudioSourcesBySoundType[sType];
		}

		public void SetSpatialBlendForMixer()
		{
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				SetGroupSpatialBlend(key);
			}
		}

		public static void PauseMixer()
		{
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				PauseSoundGroup(AudioSourcesBySoundType[key].Group.GameObjectName);
			}
		}

		public static void UnpauseMixer()
		{
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				UnpauseSoundGroup(AudioSourcesBySoundType[key].Group.GameObjectName);
			}
		}

		public static void StopMixer()
		{
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				StopAllOfSound(AudioSourcesBySoundType[key].Group.GameObjectName);
			}
		}

		public static void UnsubscribeFromAllVariations()
		{
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				List<AudioInfo> sources = AudioSourcesBySoundType[key].Sources;
				for (int i = 0; i < sources.Count; i++)
				{
					sources[i].Variation.ClearSubscribers();
				}
			}
		}

		public static void StopEverything()
		{
			StopMixer();
			StopAllPlaylists();
		}

		public static void PauseEverything()
		{
			PauseMixer();
			PauseAllPlaylists();
		}

		public static void UnpauseEverything()
		{
			UnpauseMixer();
			ResumeAllPlaylists();
		}

		public static void MuteEverything()
		{
			MixerMuted = true;
			MuteAllPlaylists();
		}

		public static void UnmuteEverything()
		{
			MixerMuted = false;
			UnmuteAllPlaylists();
		}

		private static int GetBusIndex(string busName, bool alertMissing)
		{
			for (int i = 0; i < GroupBuses.Count; i++)
			{
				if (GroupBuses[i].busName == busName)
				{
					return i + 2;
				}
			}
			if (alertMissing)
			{
				LogWarning("Could not find bus '" + busName + "'.");
			}
			return -1;
		}

		private static GroupBus GetBusByIndex(int busIndex)
		{
			if (busIndex < 2)
			{
				return null;
			}
			return GroupBuses[busIndex - 2];
		}

		public static void ChangeBusPitch(string busName, float pitch)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					ChangeVariationPitch(group.GameObjectName, true, string.Empty, pitch);
				}
			}
		}

		public static void MuteBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			GroupBus groupBus = GrabBusByName(busName);
			groupBus.isMuted = true;
			if (groupBus.isSoloed)
			{
				UnsoloBus(busName);
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					MuteGroup(group.GameObjectName);
				}
			}
		}

		public static void UnmuteBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			GroupBus groupBus = GrabBusByName(busName);
			groupBus.isMuted = false;
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					UnmuteGroup(group.GameObjectName);
				}
			}
		}

		public static void ToggleMuteBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex >= 0)
			{
				GroupBus groupBus = GrabBusByName(busName);
				if (groupBus.isMuted)
				{
					UnmuteBus(busName);
				}
				else
				{
					MuteBus(busName);
				}
			}
		}

		public static void PauseBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					PauseSoundGroup(group.GameObjectName);
				}
			}
		}

		public static void SoloBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			GroupBus groupBus = GrabBusByName(busName);
			groupBus.isSoloed = true;
			if (groupBus.isMuted)
			{
				UnmuteBus(busName);
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					SoloGroup(group.GameObjectName);
				}
			}
		}

		public static void UnsoloBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			GroupBus groupBus = GrabBusByName(busName);
			groupBus.isSoloed = false;
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					UnsoloGroup(group.GameObjectName);
				}
			}
		}

		public void RouteBusToUnityMixerGroup(string busName, AudioMixerGroup mixerGroup)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					RouteGroupToUnityMixerGroup(group.name, mixerGroup);
				}
			}
		}

		private static void StopOldestSoundOnBus(GroupBus bus)
		{
			int busIndex = GetBusIndex(bus.busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			SoundGroupVariation soundGroupVariation = null;
			float num = -1f;
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex != busIndex || group.ActiveVoices == 0)
				{
					continue;
				}
				for (int i = 0; i < value.Sources.Count; i++)
				{
					SoundGroupVariation variation = value.Sources[i].Variation;
					if (variation.PlaySoundParm.IsPlaying)
					{
						if (soundGroupVariation == null)
						{
							soundGroupVariation = variation;
							num = variation.LastTimePlayed;
						}
						else if (variation.LastTimePlayed < num)
						{
							soundGroupVariation = variation;
							num = variation.LastTimePlayed;
						}
					}
				}
			}
			if (soundGroupVariation != null)
			{
				soundGroupVariation.Stop();
			}
		}

		public static void StopBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					StopAllOfSound(group.GameObjectName);
				}
			}
		}

		public static void UnpauseBus(string busName)
		{
			int busIndex = GetBusIndex(busName, true);
			if (busIndex < 0)
			{
				return;
			}
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AudioGroupInfo value = enumerator.Current.Value;
				MasterAudioGroup group = value.Group;
				if (group.busIndex == busIndex)
				{
					UnpauseSoundGroup(group.GameObjectName);
				}
			}
		}

		public static bool CreateBus(string busName, bool errorOnExisting = true)
		{
			List<GroupBus> list = GroupBuses.FindAll((GroupBus obj) => obj.busName == busName);
			if (list.Count > 0)
			{
				if (errorOnExisting)
				{
					LogError("You already have a bus named '" + busName + "'. Not creating a second one.");
				}
				return false;
			}
			GroupBus groupBus = new GroupBus();
			groupBus.busName = busName;
			GroupBus item = groupBus;
			float? busVolume = PersistentAudioSettings.GetBusVolume(busName);
			GroupBuses.Add(item);
			if (busVolume.HasValue)
			{
				SetBusVolumeByName(busName, busVolume.Value);
			}
			return true;
		}

		public static void DeleteBusByName(string busName)
		{
			int busIndex = GetBusIndex(busName, false);
			if (busIndex > 0)
			{
				DeleteBus(busIndex);
			}
		}

		public static void DeleteBus(int busIndex)
		{
			GroupBuses.RemoveAt(busIndex - 2);
			Dictionary<string, AudioGroupInfo>.Enumerator enumerator = AudioSourcesBySoundType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MasterAudioGroup group = enumerator.Current.Value.Group;
				if (group.busIndex != -1)
				{
					if (group.busIndex == busIndex)
					{
						group.busIndex = -1;
						Instance.RouteGroupToUnityMixerGroup(group.name, null);
					}
					else if (group.busIndex > busIndex)
					{
						group.busIndex--;
					}
				}
			}
		}

		public static float GetBusVolume(MasterAudioGroup maGroup)
		{
			float result = 1f;
			if (maGroup.busIndex >= 2)
			{
				result = GroupBuses[maGroup.busIndex - 2].volume;
			}
			return result;
		}

		public static void FadeBusToVolume(string busName, float newVolume, float fadeTime, Action completionCallback = null)
		{
			if (newVolume < 0f || newVolume > 1f)
			{
				return;
			}
			if (fadeTime <= 0.1f)
			{
				SetBusVolumeByName(busName, newVolume);
				if (completionCallback != null)
				{
					completionCallback();
				}
				return;
			}
			GroupBus groupBus = GrabBusByName(busName);
			if (groupBus != null && !(newVolume < 0f) && !(newVolume > 1f))
			{
				BusFadeInfo busFadeInfo = BusFades.Find((BusFadeInfo obj) => obj.NameOfBus == busName);
				if (busFadeInfo != null)
				{
					busFadeInfo.IsActive = false;
				}
				float volumeStep = (newVolume - groupBus.volume) / (fadeTime / AudioUtil.FrameTime());
				BusFadeInfo busFadeInfo2 = new BusFadeInfo();
				busFadeInfo2.NameOfBus = busName;
				busFadeInfo2.ActingBus = groupBus;
				busFadeInfo2.VolumeStep = volumeStep;
				busFadeInfo2.TargetVolume = newVolume;
				BusFadeInfo busFadeInfo3 = busFadeInfo2;
				if (completionCallback != null)
				{
					busFadeInfo3.completionAction = completionCallback;
				}
				BusFades.Add(busFadeInfo3);
			}
		}

		public static void SetBusVolumeByName(string busName, float newVolume)
		{
			GroupBus groupBus = GrabBusByName(busName);
			if (groupBus != null)
			{
				SetBusVolume(groupBus, newVolume);
			}
		}

		private static void SetBusVolume(GroupBus bus, float newVolume)
		{
			try
			{
				bus.volume = newVolume;
				foreach (string key in AudioSourcesBySoundType.Keys)
				{
					AudioGroupInfo audioGroupInfo = AudioSourcesBySoundType[key];
					GroupBus busByIndex = GetBusByIndex(audioGroupInfo.Group.busIndex);
					if (busByIndex == null || busByIndex.busName != bus.busName)
					{
						continue;
					}
					for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
					{
						AudioInfo audioInfo = audioGroupInfo.Sources[i];
						AudioSource source = audioInfo.Source;
						if (source.isPlaying)
						{
							float num = audioGroupInfo.Group.groupMasterVolume * bus.volume * Instance._masterAudioVolume;
							float num3 = (source.volume = audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * num + audioInfo.LastRandomVolume);
							SoundGroupVariation component = source.GetComponent<SoundGroupVariation>();
							component.SetGroupVolume = num;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static GroupBus GrabBusByName(string busName)
		{
			for (int i = 0; i < GroupBuses.Count; i++)
			{
				GroupBus groupBus = GroupBuses[i];
				if (groupBus.busName == busName)
				{
					return groupBus;
				}
			}
			return null;
		}

		public static void AddSoundGroupToDuckList(string sType, float riseVolumeStart)
		{
			MasterAudio instance = Instance;
			if (!instance.duckingBySoundType.ContainsKey(sType))
			{
				instance.duckingBySoundType.Add(sType, new DuckGroupInfo
				{
					soundType = sType,
					riseVolStart = riseVolumeStart
				});
			}
		}

		public static void RemoveSoundGroupFromDuckList(string sType)
		{
			MasterAudio instance = Instance;
			if (instance.duckingBySoundType.ContainsKey(sType))
			{
				instance.duckingBySoundType.Remove(sType);
			}
		}

		public static Playlist GrabPlaylist(string playlistName, bool logErrorIfNotFound = true)
		{
			if (playlistName == "[None]")
			{
				return null;
			}
			for (int i = 0; i < MusicPlaylists.Count; i++)
			{
				Playlist playlist = MusicPlaylists[i];
				if (playlist.playlistName == playlistName)
				{
					return playlist;
				}
			}
			if (logErrorIfNotFound)
			{
			}
			return null;
		}

		public static void ChangePlaylistPitch(string playlistName, float pitch, string songName = null)
		{
			Playlist playlist = GrabPlaylist(playlistName);
			if (playlist == null)
			{
				return;
			}
			for (int i = 0; i < playlist.MusicSettings.Count; i++)
			{
				MusicSetting musicSetting = playlist.MusicSettings[i];
				if (string.IsNullOrEmpty(songName) || !(musicSetting.alias != songName) || !(musicSetting.songName != songName))
				{
					musicSetting.pitch = pitch;
				}
			}
		}

		public static void MutePlaylist()
		{
			MutePlaylist("~only~");
		}

		public static void MutePlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			MutePlaylists(list);
		}

		public static void MuteAllPlaylists()
		{
			MutePlaylists(PlaylistController.Instances);
		}

		private static void MutePlaylists(List<PlaylistController> playlists)
		{
			if (playlists.Count == PlaylistController.Instances.Count)
			{
				PlaylistsMuted = true;
			}
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.MutePlaylist();
			}
		}

		public static void UnmutePlaylist()
		{
			UnmutePlaylist("~only~");
		}

		public static void UnmutePlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			UnmutePlaylists(list);
		}

		public static void UnmuteAllPlaylists()
		{
			UnmutePlaylists(PlaylistController.Instances);
		}

		private static void UnmutePlaylists(List<PlaylistController> playlists)
		{
			if (playlists.Count == PlaylistController.Instances.Count)
			{
				PlaylistsMuted = false;
			}
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.UnmutePlaylist();
			}
		}

		public static void ToggleMutePlaylist()
		{
			ToggleMutePlaylist("~only~");
		}

		public static void ToggleMutePlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			ToggleMutePlaylists(list);
		}

		public static void ToggleMuteAllPlaylists()
		{
			ToggleMutePlaylists(PlaylistController.Instances);
		}

		private static void ToggleMutePlaylists(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.ToggleMutePlaylist();
			}
		}

		public static void PausePlaylist()
		{
			PausePlaylist("~only~");
		}

		public static void PausePlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			PausePlaylists(list);
		}

		public static void PauseAllPlaylists()
		{
			PausePlaylists(PlaylistController.Instances);
		}

		private static void PausePlaylists(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.PausePlaylist();
			}
		}

		public static void ResumePlaylist()
		{
			ResumePlaylist("~only~");
		}

		public static void ResumePlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "ResumePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			ResumePlaylists(list);
		}

		public static void ResumeAllPlaylists()
		{
			ResumePlaylists(PlaylistController.Instances);
		}

		private static void ResumePlaylists(List<PlaylistController> controllers)
		{
			for (int i = 0; i < controllers.Count; i++)
			{
				PlaylistController playlistController = controllers[i];
				playlistController.ResumePlaylist();
			}
		}

		public static void StopPlaylist()
		{
			StopPlaylist("~only~");
		}

		public static void StopPlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "StopPlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			StopPlaylists(list);
		}

		public static void StopAllPlaylists()
		{
			StopPlaylists(PlaylistController.Instances);
		}

		private static void StopPlaylists(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.StopPlaylist();
			}
		}

		public static void TriggerNextPlaylistClip()
		{
			TriggerNextPlaylistClip("~only~");
		}

		public static void TriggerNextPlaylistClip(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerNextPlaylistClip"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			NextPlaylistClips(list);
		}

		public static void TriggerNextClipAllPlaylists()
		{
			NextPlaylistClips(PlaylistController.Instances);
		}

		private static void NextPlaylistClips(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.PlayNextSong();
			}
		}

		public static void TriggerRandomPlaylistClip()
		{
			TriggerRandomPlaylistClip("~only~");
		}

		public static void TriggerRandomPlaylistClip(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerRandomPlaylistClip"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			RandomPlaylistClips(list);
		}

		public static void TriggerRandomClipAllPlaylists()
		{
			RandomPlaylistClips(PlaylistController.Instances);
		}

		private static void RandomPlaylistClips(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.PlayRandomSong();
			}
		}

		public static void RestartPlaylist()
		{
			RestartPlaylist("~only~");
		}

		public static void RestartPlaylist(string playlistControllerName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			PlaylistController playlistController;
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "RestartPlaylist"))
				{
					return;
				}
				playlistController = instances[0];
			}
			else
			{
				PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController2 == null)
				{
					return;
				}
				playlistController = playlistController2;
			}
			if (playlistController != null)
			{
				List<PlaylistController> list = new List<PlaylistController>();
				list.Add(playlistController);
				RestartPlaylists(list);
			}
		}

		public static void RestartAllPlaylists()
		{
			RestartPlaylists(PlaylistController.Instances);
		}

		private static void RestartPlaylists(List<PlaylistController> playlists)
		{
			for (int i = 0; i < playlists.Count; i++)
			{
				PlaylistController playlistController = playlists[i];
				playlistController.RestartPlaylist();
			}
		}

		public static void StartPlaylist(string playlistName)
		{
			StartPlaylist("~only~", playlistName);
		}

		public static void StartPlaylist(string playlistControllerName, string playlistName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].StartPlaylist(playlistName);
			}
		}

		public static void QueuePlaylistClip(string clipName)
		{
			QueuePlaylistClip("~only~", clipName);
		}

		public static void QueuePlaylistClip(string playlistControllerName, string clipName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			PlaylistController playlistController;
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "QueuePlaylistClip"))
				{
					return;
				}
				playlistController = instances[0];
			}
			else
			{
				PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController2 == null)
				{
					return;
				}
				playlistController = playlistController2;
			}
			if (playlistController != null)
			{
				playlistController.QueuePlaylistClip(clipName);
			}
		}

		public static bool TriggerPlaylistClip(string clipName)
		{
			return TriggerPlaylistClip("~only~", clipName);
		}

		public static bool TriggerPlaylistClip(string playlistControllerName, string clipName)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			PlaylistController playlistController;
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerPlaylistClip"))
				{
					return false;
				}
				playlistController = instances[0];
			}
			else
			{
				PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController2 == null)
				{
					return false;
				}
				playlistController = playlistController2;
			}
			if (playlistController == null)
			{
				return false;
			}
			return playlistController.TriggerPlaylistClip(clipName);
		}

		public static void ChangePlaylistByName(string playlistName, bool playFirstClip = true)
		{
			ChangePlaylistByName("~only~", playlistName, playFirstClip);
		}

		public static void ChangePlaylistByName(string playlistControllerName, string playlistName, bool playFirstClip = true)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			PlaylistController playlistController;
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "ChangePlaylistByName"))
				{
					return;
				}
				playlistController = instances[0];
			}
			else
			{
				PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController2 == null)
				{
					return;
				}
				playlistController = playlistController2;
			}
			if (playlistController != null)
			{
				playlistController.ChangePlaylist(playlistName, playFirstClip);
			}
		}

		public static void FadePlaylistToVolume(float targetVolume, float fadeTime)
		{
			FadePlaylistToVolume("~only~", targetVolume, fadeTime);
		}

		public static void FadePlaylistToVolume(string playlistControllerName, float targetVolume, float fadeTime)
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			List<PlaylistController> list = new List<PlaylistController>();
			if (playlistControllerName == "~only~")
			{
				if (!IsOkToCallOnlyPlaylistMethod(instances, "FadePlaylistToVolume"))
				{
					return;
				}
				list.Add(instances[0]);
			}
			else
			{
				PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
				if (playlistController != null)
				{
					list.Add(playlistController);
				}
			}
			FadePlaylists(list, targetVolume, fadeTime);
		}

		public static void FadeAllPlaylistsToVolume(float targetVolume, float fadeTime)
		{
			FadePlaylists(PlaylistController.Instances, targetVolume, fadeTime);
		}

		private static void FadePlaylists(List<PlaylistController> playlists, float targetVolume, float fadeTime)
		{
			if (!(targetVolume < 0f) && !(targetVolume > 1f))
			{
				for (int i = 0; i < playlists.Count; i++)
				{
					PlaylistController playlistController = playlists[i];
					playlistController.FadeToVolume(targetVolume, fadeTime);
				}
			}
		}

		public static void CreatePlaylist(Playlist playlist, bool errorOnDuplicate)
		{
			Playlist playlist2 = GrabPlaylist(playlist.playlistName, false);
			if (playlist2 != null)
			{
				if (!errorOnDuplicate)
				{
				}
			}
			else
			{
				MusicPlaylists.Add(playlist);
			}
		}

		public static void DeletePlaylist(string playlistName)
		{
			if (SafeInstance == null)
			{
				return;
			}
			Playlist playlist = GrabPlaylist(playlistName);
			if (playlist == null)
			{
				return;
			}
			for (int i = 0; i < PlaylistController.Instances.Count; i++)
			{
				PlaylistController playlistController = PlaylistController.Instances[i];
				if (!(playlistController.PlaylistName != playlistName))
				{
					playlistController.StopPlaylist();
					break;
				}
			}
			MusicPlaylists.Remove(playlist);
		}

		public static void AddSongToPlaylist(string playlistName, AudioClip song, bool loopSong = false, float songPitch = 1f, float songVolume = 1f)
		{
			Playlist playlist = GrabPlaylist(playlistName);
			if (playlist != null)
			{
				MusicSetting musicSetting = new MusicSetting();
				musicSetting.clip = song;
				musicSetting.isExpanded = true;
				musicSetting.isLoop = loopSong;
				musicSetting.pitch = songPitch;
				musicSetting.volume = songVolume;
				MusicSetting item = musicSetting;
				playlist.MusicSettings.Add(item);
			}
		}

		public static void ReDownloadAllInternetFiles()
		{
			List<SoundGroupVariation> list = new List<SoundGroupVariation>();
			foreach (string key in AudioSourcesBySoundType.Keys)
			{
				for (int i = 0; i < AudioSourcesBySoundType[key].Sources.Count; i++)
				{
					AudioSource source = AudioSourcesBySoundType[key].Sources[i].Source;
					SoundGroupVariation component = source.GetComponent<SoundGroupVariation>();
					if (!(component == null) && component.audLocation == AudioLocation.FileOnInternet)
					{
						AudioResourceOptimizer.RemoveLoadedInternetClip(component.internetFileUrl);
						component.internetFileLoadStatus = InternetFileLoadStatus.Loading;
						list.Add(component);
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				SoundGroupVariation soundGroupVariation = list[j];
				soundGroupVariation.Stop();
				AudioResourceOptimizer.AddTargetForClip(soundGroupVariation.internetFileUrl, soundGroupVariation.VarAudio);
				soundGroupVariation.LoadInternetFile();
			}
		}

		public static void AddCustomEventReceiver(ICustomEventReceiver receiver, Transform receiverTrans)
		{
			if (AppIsShuttingDown)
			{
				return;
			}
			IList<AudioEventGroup> allEvents = receiver.GetAllEvents();
			for (int i = 0; i < allEvents.Count; i++)
			{
				AudioEventGroup audioEventGroup = allEvents[i];
				if (!receiver.SubscribesToEvent(audioEventGroup.customEventName))
				{
					continue;
				}
				if (!ReceiversByEventName.ContainsKey(audioEventGroup.customEventName))
				{
					ReceiversByEventName.Add(audioEventGroup.customEventName, new Dictionary<ICustomEventReceiver, Transform> { { receiver, receiverTrans } });
					continue;
				}
				Dictionary<ICustomEventReceiver, Transform> dictionary = ReceiversByEventName[audioEventGroup.customEventName];
				if (!dictionary.ContainsKey(receiver))
				{
					dictionary.Add(receiver, receiverTrans);
				}
			}
		}

		public static void RemoveCustomEventReceiver(ICustomEventReceiver receiver)
		{
			if (AppIsShuttingDown || SafeInstance == null)
			{
				return;
			}
			for (int i = 0; i < Instance.customEvents.Count; i++)
			{
				CustomEvent customEvent = Instance.customEvents[i];
				if (receiver.SubscribesToEvent(customEvent.EventName))
				{
					Dictionary<ICustomEventReceiver, Transform> dictionary = ReceiversByEventName[customEvent.EventName];
					dictionary.Remove(receiver);
				}
			}
		}

		public static List<Transform> ReceiversForEvent(string customEventName)
		{
			List<Transform> list = new List<Transform>();
			if (!ReceiversByEventName.ContainsKey(customEventName))
			{
				return list;
			}
			Dictionary<ICustomEventReceiver, Transform> dictionary = ReceiversByEventName[customEventName];
			foreach (ICustomEventReceiver key in dictionary.Keys)
			{
				if (key.SubscribesToEvent(customEventName))
				{
					list.Add(dictionary[key]);
				}
			}
			return list;
		}

		public static void CreateCustomEvent(string customEventName, CustomEventReceiveMode eventReceiveMode, float distanceThreshold, bool errorOnDuplicate = true)
		{
			if (AppIsShuttingDown)
			{
				return;
			}
			if (Instance.customEvents.FindAll((CustomEvent obj) => obj.EventName == customEventName).Count > 0)
			{
				if (!errorOnDuplicate)
				{
				}
			}
			else
			{
				CustomEvent customEvent = new CustomEvent(customEventName);
				customEvent.eventReceiveMode = eventReceiveMode;
				customEvent.distanceThreshold = distanceThreshold;
				CustomEvent item = customEvent;
				Instance.customEvents.Add(item);
			}
		}

		public static void DeleteCustomEvent(string customEventName)
		{
			if (!AppIsShuttingDown && !(SafeInstance == null))
			{
				Instance.customEvents.RemoveAll((CustomEvent obj) => obj.EventName == customEventName);
			}
		}

		private static CustomEvent GetCustomEventByName(string customEventName)
		{
			List<CustomEvent> list = Instance.customEvents.FindAll((CustomEvent obj) => obj.EventName == customEventName);
			return (list.Count <= 0) ? null : list[0];
		}

		public static void FireCustomEvent(string customEventName, Vector3 originPoint)
		{
			if (AppIsShuttingDown || "[None]" == customEventName || (!CustomEventExists(customEventName) && !IsWarming))
			{
				return;
			}
			CustomEvent customEventByName = GetCustomEventByName(customEventName);
			if (customEventByName == null || customEventByName.frameLastFired >= Time.frameCount)
			{
				return;
			}
			customEventByName.frameLastFired = Time.frameCount;
			if (Instance.disableLogging || Instance.logCustomEvents)
			{
			}
			float? num = null;
			switch (customEventByName.eventReceiveMode)
			{
			case CustomEventReceiveMode.Never:
				if (!Instance.LogSounds)
				{
				}
				return;
			case CustomEventReceiveMode.WhenDistanceLessThan:
			case CustomEventReceiveMode.WhenDistanceMoreThan:
				num = customEventByName.distanceThreshold * customEventByName.distanceThreshold;
				break;
			}
			if (!ReceiversByEventName.ContainsKey(customEventName))
			{
				return;
			}
			Dictionary<ICustomEventReceiver, Transform> dictionary = ReceiversByEventName[customEventName];
			foreach (ICustomEventReceiver key in dictionary.Keys)
			{
				switch (customEventByName.eventReceiveMode)
				{
				case CustomEventReceiveMode.WhenDistanceLessThan:
				{
					float sqrMagnitude2 = (dictionary[key].position - originPoint).sqrMagnitude;
					if (num.HasValue && sqrMagnitude2 > num.Value)
					{
						continue;
					}
					break;
				}
				case CustomEventReceiveMode.WhenDistanceMoreThan:
				{
					float sqrMagnitude = (dictionary[key].position - originPoint).sqrMagnitude;
					if (num.HasValue && sqrMagnitude < num.Value)
					{
						continue;
					}
					break;
				}
				}
				key.ReceiveEvent(customEventName, originPoint);
			}
		}

		public static bool CustomEventExists(string customEventName)
		{
			if (AppIsShuttingDown)
			{
				return true;
			}
			return Instance.customEvents.FindAll((CustomEvent obj) => obj.EventName == customEventName).Count > 0;
		}

		private static bool LoggingEnabledForGroup(MasterAudioGroup grp)
		{
			if (IsWarming)
			{
				return false;
			}
			if (Instance.disableLogging)
			{
				return false;
			}
			if (grp != null && grp.logSound)
			{
				return true;
			}
			return Instance.LogSounds;
		}

		private static void LogMessage(string message)
		{
		}

		public static void LogWarning(string msg)
		{
			if (!Instance.disableLogging && !msg.Contains("low_"))
			{
			}
		}

		public static void LogError(string msg)
		{
			if (!Instance.disableLogging)
			{
			}
		}

		public static void LogNoPlaylist(string playlistControllerName, string methodName)
		{
			LogWarning("There is currently no Playlist assigned to Playlist Controller '" + playlistControllerName + "'. Cannot call '" + methodName + "' method.");
		}

		private static bool IsOkToCallOnlyPlaylistMethod(List<PlaylistController> pcs, string methodName)
		{
			if (pcs.Count == 0)
			{
				LogError(string.Format("You have no Playlist Controllers in the Scene. You cannot '{0}'.", methodName));
				return false;
			}
			if (pcs.Count > 1)
			{
				LogError(string.Format("You cannot call '{0}' without specifying a Playlist Controller name when you have more than one Playlist Controller.", methodName));
				return false;
			}
			return true;
		}

		public static bool HasAsyncResourceLoaderFeature()
		{
			return Application.HasProLicense();
		}

		public static GameObject CreateMasterAudio()
		{
			UnityEngine.Object @object = Resources.Load(MasterAudioFolderPath + "/Prefabs/MasterAudio.prefab", typeof(GameObject));
			if (@object == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.name = "MasterAudio";
			return gameObject;
		}

		public static GameObject CreatePlaylistController()
		{
			UnityEngine.Object @object = Resources.Load(MasterAudioFolderPath + "/Prefabs/PlaylistController.prefab", typeof(GameObject));
			if (@object == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.name = "PlaylistController";
			return gameObject;
		}

		public static GameObject CreateDynamicSoundGroupCreator()
		{
			UnityEngine.Object @object = Resources.Load(MasterAudioFolderPath + "/Prefabs/DynamicSoundGroupCreator.prefab", typeof(GameObject));
			if (@object == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.name = "DynamicSoundGroupCreator";
			return gameObject;
		}

		public static GameObject CreateSoundGroupOrganizer()
		{
			UnityEngine.Object @object = Resources.Load(MasterAudioFolderPath + "/Prefabs/SoundGroupOrganizer.prefab", typeof(GameObject));
			if (@object == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.name = "SoundGroupOrganizer";
			return gameObject;
		}
	}
}
