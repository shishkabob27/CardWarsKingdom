using UnityEngine;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio
{
	public class EventSounds : MonoBehaviour
	{
		public enum UnityUIVersion
		{
			Legacy = 0,
			uGUI = 1,
		}

		public enum VariationType
		{
			PlaySpecific = 0,
			PlayRandom = 1,
		}

		public enum PreviousSoundStopMode
		{
			None = 0,
			Stop = 1,
			FadeOut = 2,
		}

		public enum RetriggerLimMode
		{
			None = 0,
			FrameBased = 1,
			TimeBased = 2,
		}

		public bool showGizmo;
		public MasterAudio.SoundSpawnLocationMode soundSpawnMode;
		public bool disableSounds;
		public bool showPoolManager;
		public bool showNGUI;
		public UnityUIVersion unityUIMode;
		public bool logMissingEvents;
		public AudioEventGroup startSound;
		public AudioEventGroup visibleSound;
		public AudioEventGroup invisibleSound;
		public AudioEventGroup collisionSound;
		public AudioEventGroup collisionExitSound;
		public AudioEventGroup triggerSound;
		public AudioEventGroup triggerExitSound;
		public AudioEventGroup mouseEnterSound;
		public AudioEventGroup mouseExitSound;
		public AudioEventGroup mouseClickSound;
		public AudioEventGroup mouseUpSound;
		public AudioEventGroup mouseDragSound;
		public AudioEventGroup spawnedSound;
		public AudioEventGroup despawnedSound;
		public AudioEventGroup enableSound;
		public AudioEventGroup disableSound;
		public AudioEventGroup collision2dSound;
		public AudioEventGroup collisionExit2dSound;
		public AudioEventGroup triggerEnter2dSound;
		public AudioEventGroup triggerExit2dSound;
		public AudioEventGroup particleCollisionSound;
		public AudioEventGroup nguiOnClickSound;
		public AudioEventGroup nguiMouseDownSound;
		public AudioEventGroup nguiMouseUpSound;
		public AudioEventGroup nguiMouseEnterSound;
		public AudioEventGroup nguiMouseExitSound;
		public AudioEventGroup unitySliderChangedSound;
		public AudioEventGroup unityButtonClickedSound;
		public AudioEventGroup unityPointerDownSound;
		public AudioEventGroup unityDragSound;
		public AudioEventGroup unityPointerUpSound;
		public AudioEventGroup unityPointerEnterSound;
		public AudioEventGroup unityPointerExitSound;
		public AudioEventGroup unityDropSound;
		public AudioEventGroup unityScrollSound;
		public AudioEventGroup unityUpdateSelectedSound;
		public AudioEventGroup unitySelectSound;
		public AudioEventGroup unityDeselectSound;
		public AudioEventGroup unityMoveSound;
		public AudioEventGroup unityInitializePotentialDragSound;
		public AudioEventGroup unityBeginDragSound;
		public AudioEventGroup unityEndDragSound;
		public AudioEventGroup unitySubmitSound;
		public AudioEventGroup unityCancelSound;
		public AudioEventGroup unityToggleSound;
		public List<AudioEventGroup> userDefinedSounds;
		public List<AudioEventGroup> mechanimStateChangedSounds;
		public bool useStartSound;
		public bool useVisibleSound;
		public bool useInvisibleSound;
		public bool useCollisionSound;
		public bool useCollisionExitSound;
		public bool useTriggerEnterSound;
		public bool useTriggerExitSound;
		public bool useMouseEnterSound;
		public bool useMouseExitSound;
		public bool useMouseClickSound;
		public bool useMouseUpSound;
		public bool useMouseDragSound;
		public bool useSpawnedSound;
		public bool useDespawnedSound;
		public bool useEnableSound;
		public bool useDisableSound;
		public bool useCollision2dSound;
		public bool useCollisionExit2dSound;
		public bool useTriggerEnter2dSound;
		public bool useTriggerExit2dSound;
		public bool useParticleCollisionSound;
		public bool useNguiOnClickSound;
		public bool useNguiMouseDownSound;
		public bool useNguiMouseUpSound;
		public bool useNguiMouseEnterSound;
		public bool useNguiMouseExitSound;
		public bool useUnitySliderChangedSound;
		public bool useUnityButtonClickedSound;
		public bool useUnityPointerDownSound;
		public bool useUnityDragSound;
		public bool useUnityPointerUpSound;
		public bool useUnityPointerEnterSound;
		public bool useUnityPointerExitSound;
		public bool useUnityDropSound;
		public bool useUnityScrollSound;
		public bool useUnityUpdateSelectedSound;
		public bool useUnitySelectSound;
		public bool useUnityDeselectSound;
		public bool useUnityMoveSound;
		public bool useUnityInitializePotentialDragSound;
		public bool useUnityBeginDragSound;
		public bool useUnityEndDragSound;
		public bool useUnitySubmitSound;
		public bool useUnityCancelSound;
		public bool useUnityToggleSound;
	}
}
