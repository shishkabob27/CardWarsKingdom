using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[AudioScriptOrder(-40)]
	[RequireComponent(typeof(SoundGroupVariationUpdater))]
	public class SoundGroupVariation : MonoBehaviour
	{
		public class PlaySoundParams
		{
			public string SoundType;

			public float VolumePercentage;

			public float? Pitch;

			public Transform SourceTrans;

			public bool AttachToSource;

			public float DelaySoundTime;

			public bool IsChainLoop;

			public bool IsSingleSubscribedPlay;

			public float GroupCalcVolume;

			public bool IsPlaying;

			public PlaySoundParams(string soundType, float volPercent, float groupCalcVolume, float? pitch, Transform sourceTrans, bool attach, float delaySoundTime, bool isChainLoop, bool isSingleSubscribedPlay)
			{
				SoundType = soundType;
				VolumePercentage = volPercent;
				GroupCalcVolume = groupCalcVolume;
				Pitch = pitch;
				SourceTrans = sourceTrans;
				AttachToSource = attach;
				DelaySoundTime = delaySoundTime;
				IsChainLoop = isChainLoop;
				IsSingleSubscribedPlay = isSingleSubscribedPlay;
				IsPlaying = false;
			}
		}

		public enum FadeMode
		{
			None,
			FadeInOut,
			FadeOutEarly,
			GradualFade
		}

		public enum RandomPitchMode
		{
			AddToClipPitch,
			IgnoreClipPitch
		}

		public enum RandomVolumeMode
		{
			AddToClipVolume,
			IgnoreClipVolume
		}

		public enum DetectEndMode
		{
			None,
			DetectEnd
		}

		public delegate void SoundFinishedEventHandler();

		public int weight = 1;

		public bool useLocalization;

		public bool useRandomPitch;

		public RandomPitchMode randomPitchMode;

		public float randomPitchMin;

		public float randomPitchMax;

		public bool useRandomVolume;

		public RandomVolumeMode randomVolumeMode;

		public float randomVolumeMin;

		public float randomVolumeMax;

		public MasterAudio.AudioLocation audLocation;

		public string resourceFileName;

		public string internetFileUrl;

		public MasterAudio.InternetFileLoadStatus internetFileLoadStatus;

		public float fxTailTime;

		public float original_pitch;

		public bool isExpanded = true;

		public bool isChecked = true;

		public bool useFades;

		public float fadeInTime;

		public float fadeOutTime;

		public bool useRandomStartTime;

		public float randomStartMinPercent;

		public float randomStartMaxPercent;

		public bool useIntroSilence;

		public float introSilenceMin;

		public float introSilenceMax;

		public float fadeMaxVolume;

		public FadeMode curFadeMode;

		public DetectEndMode curDetectEndMode;

		private AudioSource _audioSource;

		private readonly PlaySoundParams _playSndParam = new PlaySoundParams(string.Empty, 1f, 1f, 1f, null, false, 0f, false, false);

		private AudioDistortionFilter _distFilter;

		private AudioEchoFilter _echoFilter;

		private AudioHighPassFilter _hpFilter;

		private AudioLowPassFilter _lpFilter;

		private AudioReverbFilter _reverbFilter;

		private AudioChorusFilter _chorusFilter;

		private bool _isWaitingForDelay;

		private float _maxVol = 1f;

		private int _instanceId = -1;

		private bool? _audioLoops;

		private SoundGroupVariationUpdater _varUpdater;

		private int _previousSoundFinishedFrame = -1;

		private Transform _trans;

		private GameObject _go;

		private AudioSource _aud;

		private Transform _objectToFollow;

		private Transform _objectToTriggerFrom;

		private MasterAudioGroup _parentGroupScript;

		private bool _attachToSource;

		private string _resFileName = string.Empty;

		public AudioDistortionFilter DistortionFilter
		{
			get
			{
				if (_distFilter != null)
				{
					return _distFilter;
				}
				_distFilter = GetComponent<AudioDistortionFilter>();
				return _distFilter;
			}
		}

		public AudioReverbFilter ReverbFilter
		{
			get
			{
				if (_reverbFilter != null)
				{
					return _reverbFilter;
				}
				_reverbFilter = GetComponent<AudioReverbFilter>();
				return _reverbFilter;
			}
		}

		public AudioChorusFilter ChorusFilter
		{
			get
			{
				if (_chorusFilter != null)
				{
					return _chorusFilter;
				}
				_chorusFilter = GetComponent<AudioChorusFilter>();
				return _chorusFilter;
			}
		}

		public AudioEchoFilter EchoFilter
		{
			get
			{
				if (_echoFilter != null)
				{
					return _echoFilter;
				}
				_echoFilter = GetComponent<AudioEchoFilter>();
				return _echoFilter;
			}
		}

		public AudioLowPassFilter LowPassFilter
		{
			get
			{
				if (_lpFilter != null)
				{
					return _lpFilter;
				}
				_lpFilter = GetComponent<AudioLowPassFilter>();
				return _lpFilter;
			}
		}

		public AudioHighPassFilter HighPassFilter
		{
			get
			{
				if (_hpFilter != null)
				{
					return _hpFilter;
				}
				_hpFilter = GetComponent<AudioHighPassFilter>();
				return _hpFilter;
			}
		}

		public Transform ObjectToFollow
		{
			get
			{
				return _objectToFollow;
			}
			set
			{
				_objectToFollow = value;
			}
		}

		public Transform ObjectToTriggerFrom
		{
			get
			{
				return _objectToTriggerFrom;
			}
			set
			{
				_objectToTriggerFrom = value;
			}
		}

		public bool HasActiveFXFilter
		{
			get
			{
				if (HighPassFilter != null && HighPassFilter.enabled)
				{
					return true;
				}
				if (LowPassFilter != null && LowPassFilter.enabled)
				{
					return true;
				}
				if (ReverbFilter != null && ReverbFilter.enabled)
				{
					return true;
				}
				if (DistortionFilter != null && DistortionFilter.enabled)
				{
					return true;
				}
				if (EchoFilter != null && EchoFilter.enabled)
				{
					return true;
				}
				if (ChorusFilter != null && ChorusFilter.enabled)
				{
					return true;
				}
				return false;
			}
		}

		public MasterAudioGroup ParentGroup
		{
			get
			{
				if (Trans.parent == null)
				{
					return null;
				}
				if (_parentGroupScript == null)
				{
					_parentGroupScript = Trans.parent.GetComponent<MasterAudioGroup>();
				}
				if (_parentGroupScript == null)
				{
				}
				return _parentGroupScript;
			}
		}

		public float OriginalPitch
		{
			get
			{
				if (original_pitch == 0f)
				{
					original_pitch = VarAudio.pitch;
				}
				return original_pitch;
			}
		}

		public bool IsAvailableToPlay
		{
			get
			{
				if (weight == 0)
				{
					return false;
				}
				if (!_playSndParam.IsPlaying && VarAudio.time == 0f)
				{
					return true;
				}
				return AudioUtil.GetAudioPlayedPercentage(VarAudio) >= (float)ParentGroup.retriggerPercentage;
			}
		}

		public float LastTimePlayed { get; set; }

		private int InstanceId
		{
			get
			{
				if (_instanceId < 0)
				{
					_instanceId = GetInstanceID();
				}
				return _instanceId;
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
				_trans = base.transform;
				return _trans;
			}
		}

		public GameObject GameObj
		{
			get
			{
				if (_go != null)
				{
					return _go;
				}
				_go = base.gameObject;
				return _go;
			}
		}

		public AudioSource VarAudio
		{
			get
			{
				if (_audioSource != null)
				{
					return _audioSource;
				}
				_audioSource = GetComponent<AudioSource>();
				return _audioSource;
			}
		}

		public bool AudioLoops
		{
			get
			{
				if (!_audioLoops.HasValue)
				{
					_audioLoops = VarAudio.loop;
				}
				return _audioLoops.Value;
			}
		}

		public string ResFileName
		{
			get
			{
				if (string.IsNullOrEmpty(_resFileName))
				{
					_resFileName = AudioResourceOptimizer.GetLocalizedFileName(useLocalization, resourceFileName);
				}
				return _resFileName;
			}
		}

		public SoundGroupVariationUpdater VariationUpdater
		{
			get
			{
				if (_varUpdater != null)
				{
					return _varUpdater;
				}
				_varUpdater = GetComponent<SoundGroupVariationUpdater>();
				return _varUpdater;
			}
		}

		public bool IsWaitingForDelay
		{
			get
			{
				return _isWaitingForDelay;
			}
			set
			{
				_isWaitingForDelay = value;
			}
		}

		public PlaySoundParams PlaySoundParm
		{
			get
			{
				return _playSndParam;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return _playSndParam.IsPlaying;
			}
		}

		public float SetGroupVolume
		{
			get
			{
				return _playSndParam.GroupCalcVolume;
			}
			set
			{
				_playSndParam.GroupCalcVolume = value;
			}
		}

		private bool ShouldLoadAsync
		{
			get
			{
				if (MasterAudio.Instance.resourceClipsAllLoadAsync)
				{
					return true;
				}
				return ParentGroup.resourceClipsAllLoadAsync;
			}
		}

		public event SoundFinishedEventHandler SoundFinished;

		private void Awake()
		{
			original_pitch = VarAudio.pitch;
			_audioLoops = VarAudio.loop;
			AudioClip clip = VarAudio.clip;
			GameObject gameObj = GameObj;
			if (clip != null || gameObj != null)
			{
			}
			if (!VarAudio.playOnAwake)
			{
			}
		}

		private void Start()
		{
			MasterAudioGroup parentGroup = ParentGroup;
			if (!(parentGroup == null))
			{
				MasterAudio.AudioLocation audioLocation = audLocation;
				if (audioLocation == MasterAudio.AudioLocation.FileOnInternet && internetFileLoadStatus == MasterAudio.InternetFileLoadStatus.Loading)
				{
					LoadInternetFile();
				}
				GroupBus busForGroup = ParentGroup.BusForGroup;
				if (busForGroup != null)
				{
					VarAudio.outputAudioMixerGroup = busForGroup.mixerChannel;
				}
				SetSpatialBlend();
				SetPriority();
			}
		}

		public void SetSpatialBlend()
		{
			float spatialBlendForGroup = ParentGroup.SpatialBlendForGroup;
			if (spatialBlendForGroup != -99f)
			{
				VarAudio.spatialBlend = spatialBlendForGroup;
			}
		}

		public void LoadInternetFile()
		{
			StartCoroutine(AudioResourceOptimizer.PopulateSourceWithInternetFile(internetFileUrl, this, InternetFileLoaded, InternetFileFailedToLoad));
		}

		private void SetPriority()
		{
			if (MasterAudio.Instance.prioritizeOnDistance)
			{
				if (ParentGroup.alwaysHighestPriority)
				{
					AudioPrioritizer.Set2DSoundPriority(VarAudio);
				}
				else
				{
					AudioPrioritizer.SetSoundGroupInitialPriority(VarAudio);
				}
			}
		}

		public void DisableUpdater()
		{
			if (VariationUpdater != null)
			{
				VariationUpdater.enabled = false;
			}
		}

		private void OnDestroy()
		{
			StopSoundEarly();
		}

		private void OnDisable()
		{
			StopSoundEarly();
		}

		private void StopSoundEarly()
		{
			if (!MasterAudio.AppIsShuttingDown)
			{
				Stop();
			}
		}

		private void OnDrawGizmos()
		{
			if (MasterAudio.Instance.showGizmos)
			{
				Gizmos.DrawIcon(base.transform.position, "MasterAudio Icon.png", true);
			}
		}

		public void Play(float? pitch, float maxVolume, string gameObjectName, float volPercent, float targetVol, float? targetPitch, Transform sourceTrans, bool attach, float delayTime, bool isChaining, bool isSingleSubscribedPlay)
		{
			if (!MasterAudio.IsWarming && audLocation == MasterAudio.AudioLocation.FileOnInternet)
			{
				switch (internetFileLoadStatus)
				{
				case MasterAudio.InternetFileLoadStatus.Loading:
					if (MasterAudio.Instance.LogSounds)
					{
						MasterAudio.LogWarning("Cannot play Variation '" + base.name + "' because its Internet file has not been downloaded yet.");
					}
					return;
				case MasterAudio.InternetFileLoadStatus.Failed:
					if (MasterAudio.Instance.LogSounds)
					{
						MasterAudio.LogWarning("Cannot play Variation '" + base.name + "' because its Internet file failed downloading.");
					}
					return;
				}
			}
			this.SoundFinished = null;
			_isWaitingForDelay = false;
			_playSndParam.SoundType = gameObjectName;
			_playSndParam.VolumePercentage = volPercent;
			_playSndParam.GroupCalcVolume = targetVol;
			_playSndParam.Pitch = targetPitch;
			_playSndParam.SourceTrans = sourceTrans;
			_playSndParam.AttachToSource = attach;
			_playSndParam.DelaySoundTime = delayTime;
			_playSndParam.IsChainLoop = isChaining || ParentGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain;
			_playSndParam.IsSingleSubscribedPlay = isSingleSubscribedPlay;
			_playSndParam.IsPlaying = true;
			SetPriority();
			if (MasterAudio.HasAsyncResourceLoaderFeature() && ShouldLoadAsync)
			{
				StopAllCoroutines();
			}
			if (pitch.HasValue)
			{
				VarAudio.pitch = pitch.Value;
			}
			else if (useRandomPitch)
			{
				float num = Random.Range(randomPitchMin, randomPitchMax);
				if (randomPitchMode == RandomPitchMode.AddToClipPitch)
				{
					num += OriginalPitch;
				}
				VarAudio.pitch = num;
			}
			else
			{
				VarAudio.pitch = OriginalPitch;
			}
			curFadeMode = FadeMode.None;
			curDetectEndMode = DetectEndMode.DetectEnd;
			_maxVol = maxVolume;
			switch (audLocation)
			{
			case MasterAudio.AudioLocation.Clip:
				FinishSetupToPlay();
				break;
			case MasterAudio.AudioLocation.ResourceFile:
				if (MasterAudio.HasAsyncResourceLoaderFeature() && ShouldLoadAsync)
				{
					StartCoroutine(AudioResourceOptimizer.PopulateSourcesWithResourceClipAsync(ResFileName, this, FinishSetupToPlay, ResourceFailedToLoad));
				}
				else if (AudioResourceOptimizer.PopulateSourcesWithResourceClip(ResFileName, this))
				{
					FinishSetupToPlay();
				}
				break;
			case MasterAudio.AudioLocation.FileOnInternet:
				FinishSetupToPlay();
				break;
			}
		}

		private void InternetFileFailedToLoad()
		{
			internetFileLoadStatus = MasterAudio.InternetFileLoadStatus.Failed;
		}

		private void InternetFileLoaded()
		{
			if (MasterAudio.Instance.LogSounds)
			{
				MasterAudio.LogWarning("Internet file: '" + internetFileUrl + "' loaded successfully.");
			}
			internetFileLoadStatus = MasterAudio.InternetFileLoadStatus.Loaded;
		}

		private void ResourceFailedToLoad()
		{
			Stop();
		}

		private void FinishSetupToPlay()
		{
			if ((VarAudio.isPlaying || !(VarAudio.time > 0f)) && useFades && (fadeInTime > 0f || fadeOutTime > 0f))
			{
				fadeMaxVolume = _maxVol;
				if (fadeInTime > 0f)
				{
					VarAudio.volume = 0f;
				}
				if (VariationUpdater != null)
				{
					VariationUpdater.enabled = true;
					VariationUpdater.FadeInOut();
				}
			}
			VarAudio.loop = AudioLoops;
			if (_playSndParam.IsPlaying && (_playSndParam.IsChainLoop || _playSndParam.IsSingleSubscribedPlay))
			{
				VarAudio.loop = false;
			}
			if (!_playSndParam.IsPlaying)
			{
				return;
			}
			ParentGroup.AddActiveAudioSourceId(InstanceId);
			if (VariationUpdater != null)
			{
				VariationUpdater.enabled = true;
				VariationUpdater.WaitForSoundFinish(_playSndParam.DelaySoundTime);
			}
			_attachToSource = false;
			bool flag = MasterAudio.Instance.prioritizeOnDistance && (MasterAudio.Instance.useClipAgePriority || ParentGroup.useClipAgePriority);
			if (_playSndParam.AttachToSource || flag)
			{
				_attachToSource = _playSndParam.AttachToSource;
				if (VariationUpdater != null)
				{
					VariationUpdater.FollowObject(_attachToSource, ObjectToFollow, flag);
				}
			}
		}

		public void JumpToTime(float timeToJumpTo)
		{
			if (_playSndParam.IsPlaying)
			{
				VarAudio.time = timeToJumpTo;
			}
		}

		public void AdjustVolume(float volumePercentage)
		{
			if (VarAudio.isPlaying && _playSndParam.IsPlaying)
			{
				float volume = _playSndParam.GroupCalcVolume * volumePercentage;
				VarAudio.volume = volume;
				_playSndParam.VolumePercentage = volumePercentage;
			}
		}

		public void Pause()
		{
			if (audLocation == MasterAudio.AudioLocation.ResourceFile && !MasterAudio.Instance.resourceClipsPauseDoNotUnload)
			{
				Stop();
				return;
			}
			VarAudio.Pause();
			curFadeMode = FadeMode.None;
			if (VariationUpdater != null)
			{
				VariationUpdater.StopWaitingForFinish();
			}
			MaybeUnloadClip();
		}

		private void MaybeUnloadClip()
		{
			if (audLocation == MasterAudio.AudioLocation.ResourceFile)
			{
				AudioResourceOptimizer.UnloadClipIfUnused(_resFileName);
			}
			AudioUtil.UnloadNonPreloadedAudioData(VarAudio.clip);
		}

		public void Stop(bool stopEndDetection = false)
		{
			bool flag = false;
			if ((stopEndDetection || _isWaitingForDelay) && VariationUpdater != null)
			{
				VariationUpdater.StopWaitingForFinish();
				flag = true;
			}
			_objectToFollow = null;
			_objectToTriggerFrom = null;
			ParentGroup.RemoveActiveAudioSourceId(InstanceId);
			VarAudio.Stop();
			VarAudio.time = 0f;
			if (VariationUpdater != null)
			{
				VariationUpdater.StopFollowing();
				VariationUpdater.StopFading();
			}
			if (!flag && VariationUpdater != null)
			{
				VariationUpdater.StopWaitingForFinish();
			}
			_playSndParam.IsPlaying = false;
			if (this.SoundFinished != null)
			{
				bool flag2 = _previousSoundFinishedFrame == Time.frameCount;
				_previousSoundFinishedFrame = Time.frameCount;
				if (!flag2)
				{
					this.SoundFinished();
				}
				this.SoundFinished = null;
			}
			MaybeUnloadClip();
		}

		public void FadeToVolume(float newVolume, float fadeTime)
		{
			if (newVolume < 0f || newVolume > 1f)
			{
				return;
			}
			if (fadeTime <= 0.1f)
			{
				VarAudio.volume = newVolume;
				if (VarAudio.volume <= 0f)
				{
					Stop();
				}
			}
			else if (VariationUpdater != null)
			{
				VariationUpdater.FadeOverTimeToVolume(newVolume, fadeTime);
			}
		}

		public void FadeOutNow()
		{
			if (!MasterAudio.AppIsShuttingDown && IsPlaying && useFades && VariationUpdater != null)
			{
				VariationUpdater.FadeOutEarly(fadeOutTime);
			}
		}

		public void FadeOutNow(float fadeTime)
		{
			if (!MasterAudio.AppIsShuttingDown && IsPlaying && VariationUpdater != null)
			{
				VariationUpdater.FadeOutEarly(fadeTime);
			}
		}

		public bool WasTriggeredFromTransform(Transform trans)
		{
			if (ObjectToFollow == trans || ObjectToTriggerFrom == trans)
			{
				return true;
			}
			return false;
		}

		public void ClearSubscribers()
		{
			this.SoundFinished = null;
		}
	}
}
