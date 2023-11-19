using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class SoundGroupVariationUpdater : MonoBehaviour
	{
		private enum WaitForSoundFinishMode
		{
			None,
			Delay,
			Play,
			WaitForEnd,
			StopOrRepeat,
			FxTailWait
		}

		private Transform _objectToFollow;

		private GameObject _objectToFollowGo;

		private bool _isFollowing;

		private SoundGroupVariation _variation;

		private float _priorityLastUpdated = -5f;

		private bool _useClipAgePriority;

		private WaitForSoundFinishMode _waitMode;

		private float _soundPlayTime;

		private float _fadeOutStartTime = -5f;

		private bool _fadeInOutWillFadeOut;

		private bool _hasFadeInOutSetMaxVolume;

		private float _fadeInOutInFactor;

		private float _fadeInOutOutFactor;

		private int _fadeOutEarlyTotalFrames;

		private float _fadeOutEarlyFrameVolChange;

		private int _fadeOutEarlyFrameNumber;

		private float _fadeOutEarlyOrigVol;

		private float _fadeToTargetFrameVolChange;

		private int _fadeToTargetFrameNumber;

		private float _fadeToTargetOrigVol;

		private int _fadeToTargetTotalFrames;

		private float _fadeToTargetVolume;

		private bool _fadeOutStarted;

		private float _lastFrameClipTime = -1f;

		private float _fxTailEndTime = -1f;

		private bool _isPlayingBackward;

		private bool _hasStartedNextInChain;

		private Transform Trans
		{
			get
			{
				return GrpVariation.Trans;
			}
		}

		private AudioSource VarAudio
		{
			get
			{
				return GrpVariation.VarAudio;
			}
		}

		private MasterAudioGroup ParentGroup
		{
			get
			{
				return GrpVariation.ParentGroup;
			}
		}

		private SoundGroupVariation GrpVariation
		{
			get
			{
				if (_variation != null)
				{
					return _variation;
				}
				_variation = GetComponent<SoundGroupVariation>();
				return _variation;
			}
		}

		public void FadeOverTimeToVolume(float targetVolume, float fadeTime)
		{
			GrpVariation.curFadeMode = SoundGroupVariation.FadeMode.GradualFade;
			float num = targetVolume - VarAudio.volume;
			if (!VarAudio.loop && VarAudio.clip != null && fadeTime + VarAudio.time > VarAudio.clip.length)
			{
				fadeTime = VarAudio.clip.length - VarAudio.time;
			}
			_fadeToTargetTotalFrames = (int)(fadeTime / Time.deltaTime);
			_fadeToTargetFrameVolChange = num / (float)_fadeToTargetTotalFrames;
			_fadeToTargetFrameNumber = 0;
			_fadeToTargetOrigVol = VarAudio.volume;
			_fadeToTargetVolume = targetVolume;
		}

		public void FadeOutEarly(float fadeTime)
		{
			GrpVariation.curFadeMode = SoundGroupVariation.FadeMode.FadeOutEarly;
			if (!VarAudio.loop && VarAudio.clip != null && VarAudio.time + fadeTime > VarAudio.clip.length)
			{
				fadeTime = VarAudio.clip.length - VarAudio.time;
			}
			float num = Time.deltaTime;
			if (num == 0f)
			{
				num = Time.fixedDeltaTime;
			}
			_fadeOutEarlyTotalFrames = (int)(fadeTime / num);
			_fadeOutEarlyFrameVolChange = (0f - VarAudio.volume) / (float)_fadeOutEarlyTotalFrames;
			_fadeOutEarlyFrameNumber = 0;
			_fadeOutEarlyOrigVol = VarAudio.volume;
		}

		public void FadeInOut()
		{
			GrpVariation.curFadeMode = SoundGroupVariation.FadeMode.FadeInOut;
			_fadeOutStartTime = VarAudio.clip.length - GrpVariation.fadeOutTime * VarAudio.pitch;
			if (GrpVariation.fadeInTime > 0f)
			{
				VarAudio.volume = 0f;
				_fadeInOutInFactor = GrpVariation.fadeMaxVolume / GrpVariation.fadeInTime;
			}
			else
			{
				_fadeInOutInFactor = 0f;
			}
			_fadeInOutWillFadeOut = GrpVariation.fadeOutTime > 0f && !VarAudio.loop;
			if (_fadeInOutWillFadeOut)
			{
				_fadeInOutOutFactor = GrpVariation.fadeMaxVolume / (VarAudio.clip.length - _fadeOutStartTime);
			}
			else
			{
				_fadeInOutOutFactor = 0f;
			}
		}

		public void FollowObject(bool follow, Transform objToFollow, bool clipAgePriority)
		{
			_isFollowing = follow;
			if (objToFollow != null)
			{
				_objectToFollow = objToFollow;
				_objectToFollowGo = objToFollow.gameObject;
			}
			_useClipAgePriority = clipAgePriority;
			UpdateAudioLocationAndPriority(false);
		}

		public void WaitForSoundFinish(float delaySound)
		{
			if (MasterAudio.IsWarming)
			{
				PlaySoundAndWait();
				return;
			}
			_waitMode = WaitForSoundFinishMode.Delay;
			float num = 0f;
			if (GrpVariation.useIntroSilence && GrpVariation.introSilenceMax > 0f)
			{
				float num2 = Random.Range(GrpVariation.introSilenceMin, GrpVariation.introSilenceMax);
				num += num2;
			}
			if (delaySound > 0f)
			{
				num += delaySound;
			}
			if (num == 0f)
			{
				_waitMode = WaitForSoundFinishMode.Play;
				return;
			}
			_soundPlayTime = Time.realtimeSinceStartup + num;
			GrpVariation.IsWaitingForDelay = true;
		}

		public void StopFading()
		{
			GrpVariation.curFadeMode = SoundGroupVariation.FadeMode.None;
			DisableIfFinished();
		}

		public void StopWaitingForFinish()
		{
			_waitMode = WaitForSoundFinishMode.None;
			GrpVariation.curDetectEndMode = SoundGroupVariation.DetectEndMode.None;
			DisableIfFinished();
		}

		public void StopFollowing()
		{
			_isFollowing = false;
			_useClipAgePriority = false;
			_objectToFollow = null;
			_objectToFollowGo = null;
			DisableIfFinished();
		}

		private void DisableIfFinished()
		{
			if (!_isFollowing && GrpVariation.curDetectEndMode != SoundGroupVariation.DetectEndMode.DetectEnd && GrpVariation.curFadeMode == SoundGroupVariation.FadeMode.None)
			{
				base.enabled = false;
			}
		}

		private void UpdateAudioLocationAndPriority(bool rePrioritize)
		{
			if (_isFollowing && _objectToFollow != null)
			{
				Trans.position = _objectToFollow.position;
			}
			if (MasterAudio.Instance.prioritizeOnDistance && rePrioritize && !ParentGroup.alwaysHighestPriority && Time.realtimeSinceStartup - _priorityLastUpdated > MasterAudio.ReprioritizeTime)
			{
				AudioPrioritizer.Set3DPriority(VarAudio, _useClipAgePriority);
				_priorityLastUpdated = Time.realtimeSinceStartup;
			}
		}

		private void PlaySoundAndWait()
		{
			GrpVariation.IsWaitingForDelay = false;
			if (!(VarAudio.clip == null))
			{
				VarAudio.Play();
				if (GrpVariation.useRandomStartTime)
				{
					float time = Random.Range(GrpVariation.randomStartMinPercent, GrpVariation.randomStartMaxPercent) * 0.01f * VarAudio.clip.length;
					VarAudio.time = time;
				}
				GrpVariation.LastTimePlayed = Time.time;
				MasterAudio.DuckSoundGroup(ParentGroup.GameObjectName, VarAudio);
				_isPlayingBackward = GrpVariation.OriginalPitch < 0f;
				_lastFrameClipTime = ((!_isPlayingBackward) ? (-1f) : (VarAudio.clip.length + 1f));
				_waitMode = WaitForSoundFinishMode.WaitForEnd;
			}
		}

		private void StopOrChain()
		{
			SoundGroupVariation.PlaySoundParams playSoundParm = GrpVariation.PlaySoundParm;
			bool flag = playSoundParm.IsPlaying && playSoundParm.IsChainLoop;
			if (!VarAudio.loop || flag)
			{
				GrpVariation.Stop();
			}
			if (flag)
			{
				StopWaitingForFinish();
				MaybeChain();
			}
		}

		private void MaybeChain()
		{
			if (_hasStartedNextInChain)
			{
				return;
			}
			_hasStartedNextInChain = true;
			SoundGroupVariation.PlaySoundParams playSoundParm = GrpVariation.PlaySoundParm;
			if (ParentGroup.chainLoopMode == MasterAudioGroup.ChainedLoopLoopMode.NumberOfLoops && ParentGroup.ChainLoopCount >= ParentGroup.chainLoopNumLoops)
			{
				return;
			}
			float delaySoundTime = playSoundParm.DelaySoundTime;
			if (ParentGroup.chainLoopDelayMin > 0f || ParentGroup.chainLoopDelayMax > 0f)
			{
				delaySoundTime = Random.Range(ParentGroup.chainLoopDelayMin, ParentGroup.chainLoopDelayMax);
			}
			if (playSoundParm.AttachToSource || playSoundParm.SourceTrans != null)
			{
				if (playSoundParm.AttachToSource)
				{
					MasterAudio.PlaySound3DFollowTransform(playSoundParm.SoundType, playSoundParm.SourceTrans, playSoundParm.VolumePercentage, playSoundParm.Pitch, delaySoundTime, null, true);
				}
				else
				{
					MasterAudio.PlaySound3DAtTransform(playSoundParm.SoundType, playSoundParm.SourceTrans, playSoundParm.VolumePercentage, playSoundParm.Pitch, delaySoundTime, null, true);
				}
			}
			else
			{
				MasterAudio.PlaySound(playSoundParm.SoundType, playSoundParm.VolumePercentage, playSoundParm.Pitch, delaySoundTime, null, true);
			}
		}

		private void PerformFading()
		{
			switch (GrpVariation.curFadeMode)
			{
			case SoundGroupVariation.FadeMode.None:
				break;
			case SoundGroupVariation.FadeMode.FadeInOut:
			{
				if (!VarAudio.isPlaying)
				{
					break;
				}
				float time = VarAudio.time;
				if (GrpVariation.fadeInTime > 0f && time < GrpVariation.fadeInTime)
				{
					VarAudio.volume = time * _fadeInOutInFactor;
				}
				else if (time >= GrpVariation.fadeInTime && !_hasFadeInOutSetMaxVolume)
				{
					VarAudio.volume = GrpVariation.fadeMaxVolume;
					_hasFadeInOutSetMaxVolume = true;
					if (!_fadeInOutWillFadeOut)
					{
						StopFading();
					}
				}
				else if (_fadeInOutWillFadeOut && time >= _fadeOutStartTime)
				{
					if (GrpVariation.PlaySoundParm.IsChainLoop && !_fadeOutStarted)
					{
						MaybeChain();
						_fadeOutStarted = true;
					}
					VarAudio.volume = (VarAudio.clip.length - time) * _fadeInOutOutFactor;
				}
				break;
			}
			case SoundGroupVariation.FadeMode.FadeOutEarly:
				if (!VarAudio.isPlaying)
				{
					break;
				}
				_fadeOutEarlyFrameNumber++;
				VarAudio.volume = (float)_fadeOutEarlyFrameNumber * _fadeOutEarlyFrameVolChange + _fadeOutEarlyOrigVol;
				if (_fadeOutEarlyFrameNumber >= _fadeOutEarlyTotalFrames)
				{
					GrpVariation.curFadeMode = SoundGroupVariation.FadeMode.None;
					if (MasterAudio.Instance.stopZeroVolumeVariations)
					{
						GrpVariation.Stop();
					}
				}
				break;
			case SoundGroupVariation.FadeMode.GradualFade:
				if (VarAudio.isPlaying)
				{
					_fadeToTargetFrameNumber++;
					if (_fadeToTargetFrameNumber >= _fadeToTargetTotalFrames)
					{
						VarAudio.volume = _fadeToTargetVolume;
						StopFading();
					}
					else
					{
						VarAudio.volume = (float)_fadeToTargetFrameNumber * _fadeToTargetFrameVolChange + _fadeToTargetOrigVol;
					}
				}
				break;
			}
		}

		private void OnEnable()
		{
			_fadeInOutWillFadeOut = false;
			_hasFadeInOutSetMaxVolume = false;
			_fadeOutStarted = false;
			_hasStartedNextInChain = false;
		}

		private void LateUpdate()
		{
			if (_isFollowing && ParentGroup.targetDespawnedBehavior != 0 && (_objectToFollowGo == null || !DTMonoHelper.IsActive(_objectToFollowGo)))
			{
				switch (ParentGroup.targetDespawnedBehavior)
				{
				case MasterAudioGroup.TargetDespawnedBehavior.Stop:
					GrpVariation.Stop();
					break;
				case MasterAudioGroup.TargetDespawnedBehavior.FadeOut:
					GrpVariation.FadeOutNow(ParentGroup.despawnFadeTime);
					break;
				}
				StopFollowing();
			}
			PerformFading();
			UpdateAudioLocationAndPriority(true);
			switch (_waitMode)
			{
			case WaitForSoundFinishMode.None:
				break;
			case WaitForSoundFinishMode.Delay:
				if (Time.realtimeSinceStartup >= _soundPlayTime)
				{
					_waitMode = WaitForSoundFinishMode.Play;
				}
				break;
			case WaitForSoundFinishMode.Play:
				PlaySoundAndWait();
				break;
			case WaitForSoundFinishMode.WaitForEnd:
			{
				bool flag = false;
				if (_isPlayingBackward)
				{
					if (VarAudio.time > _lastFrameClipTime)
					{
						flag = true;
					}
				}
				else if (VarAudio.time < _lastFrameClipTime)
				{
					flag = true;
				}
				_lastFrameClipTime = VarAudio.time;
				if (flag)
				{
					if (GrpVariation.fxTailTime > 0f)
					{
						_waitMode = WaitForSoundFinishMode.FxTailWait;
						_fxTailEndTime = Time.realtimeSinceStartup + GrpVariation.fxTailTime;
					}
					else
					{
						_waitMode = WaitForSoundFinishMode.StopOrRepeat;
					}
				}
				break;
			}
			case WaitForSoundFinishMode.FxTailWait:
				if (Time.realtimeSinceStartup >= _fxTailEndTime)
				{
					_waitMode = WaitForSoundFinishMode.StopOrRepeat;
				}
				break;
			case WaitForSoundFinishMode.StopOrRepeat:
				StopOrChain();
				break;
			}
		}
	}
}
