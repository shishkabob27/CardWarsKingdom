using UnityEngine;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	public class SoundGroupOrganizer : MonoBehaviour
	{
		public enum TransferMode
		{
			None = 0,
			Import = 1,
			Export = 2,
		}

		public enum MAItemType
		{
			SoundGroups = 0,
			CustomEvents = 1,
		}

		public GameObject dynGroupTemplate;
		public GameObject dynVariationTemplate;
		public GameObject maGroupTemplate;
		public GameObject maVariationTemplate;
		public MasterAudio.DragGroupMode curDragGroupMode;
		public MasterAudio.AudioLocation bulkVariationMode;
		public SystemLanguage previewLanguage;
		public bool useTextGroupFilter;
		public string textGroupFilter;
		public TransferMode transMode;
		public GameObject sourceObject;
		public GameObject destObject;
		public MAItemType itemType;
		public List<CustomEvent> customEvents;
		public string newEventName;
	}
}
