using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class DynamicSoundGroupCreator : MonoBehaviour
	{
		public enum CreateItemsWhen
		{
			FirstEnableOnly,
			EveryEnable
		}

		public const int ExtraHardCodedBusOptions = 1;

		public SystemLanguage previewLanguage = SystemLanguage.English;

		public MasterAudio.DragGroupMode curDragGroupMode;

		public GameObject groupTemplate;

		public GameObject variationTemplate;

		public bool errorOnDuplicates;

		public bool createOnAwake = true;

		public bool soundGroupsAreExpanded = true;

		public bool removeGroupsOnSceneChange = true;

		public CreateItemsWhen reUseMode;

		public bool showCustomEvents = true;

		public MasterAudio.AudioLocation bulkVariationMode;

		public List<CustomEvent> customEventsToCreate = new List<CustomEvent>();

		public string newEventName = "my event";

		public bool showMusicDucking = true;

		public List<DuckGroupInfo> musicDuckingSounds = new List<DuckGroupInfo>();

		public List<GroupBus> groupBuses = new List<GroupBus>();

		public bool playListExpanded;

		public bool playlistEditorExp = true;

		public List<MasterAudio.Playlist> musicPlaylists = new List<MasterAudio.Playlist>();

		public List<GameObject> audioSourceTemplates = new List<GameObject>(10);

		public string audioSourceTemplateName = "Max Distance 500";

		public bool itemsCreatedEventExpanded;

		public string itemsCreatedCustomEvent = string.Empty;

		public bool showUnityMixerGroupAssignment = true;

		private bool _hasCreated;

		private readonly List<Transform> _groupsToRemove = new List<Transform>();

		private Transform _trans;

		private readonly List<DynamicSoundGroup> _groupsToCreate = new List<DynamicSoundGroup>();

		public static int HardCodedBusOptions
		{
			get
			{
				return 3;
			}
		}

		public List<DynamicSoundGroup> GroupsToCreate
		{
			get
			{
				return _groupsToCreate;
			}
		}

		public bool ShouldShowUnityAudioMixerGroupAssignments
		{
			get
			{
				return showUnityMixerGroupAssignment;
			}
		}

		private void Awake()
		{
			_trans = base.transform;
			_hasCreated = false;
			AudioSource component = GetComponent<AudioSource>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}

		private void OnEnable()
		{
			CreateItemsIfReady();
		}

		private void Start()
		{
			CreateItemsIfReady();
		}

		private void OnDisable()
		{
			if (!MasterAudio.AppIsShuttingDown && removeGroupsOnSceneChange && MasterAudio.SafeInstance != null)
			{
				RemoveItems();
			}
		}

		private void CreateItemsIfReady()
		{
			if (createOnAwake && MasterAudio.SoundsReady && !_hasCreated)
			{
				CreateItems();
			}
		}

		public void RemoveItems()
		{
			for (int i = 0; i < groupBuses.Count; i++)
			{
				GroupBus groupBus = groupBuses[i];
				if (!groupBus.isExisting)
				{
					MasterAudio.DeleteBusByName(groupBus.busName);
				}
			}
			for (int j = 0; j < _groupsToRemove.Count; j++)
			{
				MasterAudio.RemoveSoundGroup(_groupsToRemove[j]);
			}
			_groupsToRemove.Clear();
			for (int k = 0; k < customEventsToCreate.Count; k++)
			{
				CustomEvent customEvent = customEventsToCreate[k];
				MasterAudio.DeleteCustomEvent(customEvent.EventName);
			}
			for (int l = 0; l < musicPlaylists.Count; l++)
			{
				MasterAudio.Playlist playlist = musicPlaylists[l];
				MasterAudio.DeletePlaylist(playlist.playlistName);
			}
			if (reUseMode == CreateItemsWhen.EveryEnable)
			{
				_hasCreated = false;
			}
		}

		public void CreateItems()
		{
			if (_hasCreated)
			{
				return;
			}
			MasterAudio instance = MasterAudio.Instance;
			if (instance == null)
			{
				return;
			}
			PopulateGroupData();
			for (int i = 0; i < groupBuses.Count; i++)
			{
				GroupBus groupBus = groupBuses[i];
				if (groupBus.isExisting)
				{
					GroupBus groupBus2 = MasterAudio.GrabBusByName(groupBus.busName);
					if (groupBus2 == null)
					{
						MasterAudio.LogWarning("Existing bus '" + groupBus.busName + "' was not found, specified in prefab '" + base.name + "'.");
					}
				}
				else
				{
					if (!MasterAudio.CreateBus(groupBus.busName, errorOnDuplicates))
					{
						continue;
					}
					GroupBus groupBus3 = MasterAudio.GrabBusByName(groupBus.busName);
					if (groupBus3 != null)
					{
						if (!PersistentAudioSettings.GetBusVolume(groupBus.busName).HasValue)
						{
							groupBus3.volume = groupBus.volume;
						}
						groupBus3.voiceLimit = groupBus.voiceLimit;
						groupBus3.stopOldest = groupBus.stopOldest;
						groupBus3.mixerChannel = groupBus.mixerChannel;
					}
				}
			}
			for (int j = 0; j < _groupsToCreate.Count; j++)
			{
				DynamicSoundGroup dynamicSoundGroup = _groupsToCreate[j];
				string busName = string.Empty;
				int num = ((dynamicSoundGroup.busIndex != -1) ? dynamicSoundGroup.busIndex : 0);
				if (num >= HardCodedBusOptions)
				{
					GroupBus groupBus4 = groupBuses[num - HardCodedBusOptions];
					busName = groupBus4.busName;
				}
				dynamicSoundGroup.busName = busName;
				Transform transform = MasterAudio.CreateNewSoundGroup(dynamicSoundGroup, _trans.name, errorOnDuplicates);
				for (int k = 0; k < dynamicSoundGroup.groupVariations.Count; k++)
				{
					DynamicGroupVariation dynamicGroupVariation = dynamicSoundGroup.groupVariations[k];
					if (dynamicGroupVariation.LowPassFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.LowPassFilter);
					}
					if (dynamicGroupVariation.HighPassFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.HighPassFilter);
					}
					if (dynamicGroupVariation.DistortionFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.DistortionFilter);
					}
					if (dynamicGroupVariation.ChorusFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.ChorusFilter);
					}
					if (dynamicGroupVariation.EchoFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.EchoFilter);
					}
					if (dynamicGroupVariation.ReverbFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.ReverbFilter);
					}
				}
				if (!(transform == null))
				{
					_groupsToRemove.Add(transform);
				}
			}
			for (int l = 0; l < musicDuckingSounds.Count; l++)
			{
				DuckGroupInfo duckGroupInfo = musicDuckingSounds[l];
				if (!(duckGroupInfo.soundType == "[None]"))
				{
					MasterAudio.AddSoundGroupToDuckList(duckGroupInfo.soundType, duckGroupInfo.riseVolStart);
				}
			}
			for (int m = 0; m < customEventsToCreate.Count; m++)
			{
				CustomEvent customEvent = customEventsToCreate[m];
				MasterAudio.CreateCustomEvent(customEvent.EventName, customEvent.eventReceiveMode, customEvent.distanceThreshold, errorOnDuplicates);
			}
			for (int n = 0; n < musicPlaylists.Count; n++)
			{
				MasterAudio.Playlist playlist = musicPlaylists[n];
				MasterAudio.CreatePlaylist(playlist, errorOnDuplicates);
			}
			_hasCreated = true;
			if (itemsCreatedEventExpanded)
			{
				MasterAudio.FireCustomEvent(itemsCreatedCustomEvent, _trans.position);
			}
		}

		private void PopulateGroupData()
		{
			_groupsToCreate.Clear();
			for (int i = 0; i < _trans.childCount; i++)
			{
				DynamicSoundGroup component = _trans.GetChild(i).GetComponent<DynamicSoundGroup>();
				if (component == null)
				{
					continue;
				}
				component.groupVariations.Clear();
				for (int j = 0; j < component.transform.childCount; j++)
				{
					DynamicGroupVariation component2 = component.transform.GetChild(j).GetComponent<DynamicGroupVariation>();
					if (!(component2 == null))
					{
						component.groupVariations.Add(component2);
					}
				}
				_groupsToCreate.Add(component);
			}
		}
	}
}
