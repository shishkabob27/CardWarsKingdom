using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[AddComponentMenu("Dark Tonic/Master Audio/Footstep Sounds")]
	public class FootstepSounds : MonoBehaviour
	{
		public enum FootstepTriggerMode
		{
			None,
			OnCollision,
			OnTriggerEnter,
			OnCollision2D,
			OnTriggerEnter2D
		}

		public MasterAudio.SoundSpawnLocationMode soundSpawnMode = MasterAudio.SoundSpawnLocationMode.AttachToCaller;

		public FootstepTriggerMode footstepEvent;

		public List<FootstepGroup> footstepGroups = new List<FootstepGroup>();

		public EventSounds.RetriggerLimMode retriggerLimitMode;

		public int limitPerXFrm;

		public float limitPerXSec;

		public int triggeredLastFrame = -100;

		public float triggeredLastTime = -100f;

		private Transform _trans;

		private Transform Trans
		{
			get
			{
				if (_trans != null)
				{
					return _trans;
				}
				_trans = base.transform;
				return _trans;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (footstepEvent == FootstepTriggerMode.OnTriggerEnter)
			{
				PlaySoundsIfMatch(other.gameObject);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (footstepEvent == FootstepTriggerMode.OnCollision)
			{
				PlaySoundsIfMatch(collision.gameObject);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (footstepEvent == FootstepTriggerMode.OnCollision2D)
			{
				PlaySoundsIfMatch(collision.gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (footstepEvent == FootstepTriggerMode.OnTriggerEnter2D)
			{
				PlaySoundsIfMatch(other.gameObject);
			}
		}

		private bool CheckForRetriggerLimit()
		{
			switch (retriggerLimitMode)
			{
			case EventSounds.RetriggerLimMode.FrameBased:
				if (triggeredLastFrame > 0 && Time.frameCount - triggeredLastFrame < limitPerXFrm)
				{
					return false;
				}
				break;
			case EventSounds.RetriggerLimMode.TimeBased:
				if (triggeredLastTime > 0f && Time.time - triggeredLastTime < limitPerXSec)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private void PlaySoundsIfMatch(GameObject go)
		{
			if (!CheckForRetriggerLimit())
			{
				return;
			}
			switch (retriggerLimitMode)
			{
			case EventSounds.RetriggerLimMode.FrameBased:
				triggeredLastFrame = Time.frameCount;
				break;
			case EventSounds.RetriggerLimMode.TimeBased:
				triggeredLastTime = Time.time;
				break;
			}
			for (int i = 0; i < footstepGroups.Count; i++)
			{
				FootstepGroup footstepGroup = footstepGroups[i];
				if ((!footstepGroup.useLayerFilter || footstepGroup.matchingLayers.Contains(go.layer)) && (!footstepGroup.useTagFilter || footstepGroup.matchingTags.Contains(go.tag)))
				{
					float volume = footstepGroup.volume;
					float? pitch = footstepGroup.pitch;
					if (!footstepGroup.useFixedPitch)
					{
						pitch = null;
					}
					string variationName = null;
					if (footstepGroup.variationType == EventSounds.VariationType.PlaySpecific)
					{
						variationName = footstepGroup.variationName;
					}
					switch (soundSpawnMode)
					{
					case MasterAudio.SoundSpawnLocationMode.CallerLocation:
						MasterAudio.PlaySound3DAtTransform(footstepGroup.soundType, Trans, volume, pitch, footstepGroup.delaySound, variationName);
						break;
					case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
						MasterAudio.PlaySound3DFollowTransform(footstepGroup.soundType, Trans, volume, pitch, footstepGroup.delaySound, variationName);
						break;
					case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
						MasterAudio.PlaySound(footstepGroup.soundType, volume, pitch, footstepGroup.delaySound, variationName);
						break;
					}
				}
			}
		}
	}
}
