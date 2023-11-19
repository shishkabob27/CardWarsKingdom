using UnityEngine;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	public class DynamicSoundGroupCreator : MonoBehaviour
	{
		public enum CreateItemsWhen
		{
			FirstEnableOnly = 0,
			EveryEnable = 1,
		}

		public SystemLanguage previewLanguage;
		public MasterAudio.DragGroupMode curDragGroupMode;
		public GameObject groupTemplate;
		public GameObject variationTemplate;
		public bool errorOnDuplicates;
		public bool createOnAwake;
		public bool soundGroupsAreExpanded;
		public bool removeGroupsOnSceneChange;
		public CreateItemsWhen reUseMode;
		public bool showCustomEvents;
		public MasterAudio.AudioLocation bulkVariationMode;
		public List<CustomEvent> customEventsToCreate;
		public string newEventName;
		public bool showMusicDucking;
		public List<DuckGroupInfo> musicDuckingSounds;
		public List<GroupBus> groupBuses;
		public bool playListExpanded;
		public bool playlistEditorExp;
		public List<MasterAudio.Playlist> musicPlaylists;
		public List<GameObject> audioSourceTemplates;
		public string audioSourceTemplateName;
		public bool itemsCreatedEventExpanded;
		public string itemsCreatedCustomEvent;
		public bool showUnityMixerGroupAssignment;
	}
}
