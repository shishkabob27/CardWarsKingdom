using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio
{
	[AudioScriptOrder(-80)]
	[RequireComponent(typeof(AudioSource))]
	public class PlaylistController : MonoBehaviour
	{
		public enum AudioPlayType
		{
			PlayNow,
			Schedule,
			AlreadyScheduled
		}

		public enum PlaylistStates
		{
			NotInScene,
			Stopped,
			Playing,
			Paused,
			Crossfading
		}

		public enum FadeMode
		{
			None,
			GradualFade
		}

		public enum AudioDuckingMode
		{
			NotDucking,
			SetToDuck,
			Ducked
		}

		public delegate void SongChangedEventHandler(string newSongName);

		public delegate void SongEndedEventHandler(string songName);

		private const string NotReadyMessage = "Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity).";

		public bool startPlaylistOnAwake = true;

		public bool isShuffle;

		public bool isAutoAdvance = true;

		public bool loopPlaylist = true;

		public float _playlistVolume = 1f;

		public bool isMuted;

		public string startPlaylistName = string.Empty;

		public int syncGroupNum = -1;

		public AudioMixerGroup mixerChannel;

		public MasterAudio.ItemSpatialBlendType spatialBlendType;

		public float spatialBlend;

		public bool songChangedEventExpanded;

		public string songChangedCustomEvent = string.Empty;

		public bool songEndedEventExpanded;

		public string songEndedCustomEvent = string.Empty;

		private AudioSource _activeAudio;

		private AudioSource _transitioningAudio;

		private float _activeAudioEndVolume;

		private float _transitioningAudioStartVolume;

		private float _crossFadeStartTime;

		private readonly List<int> _clipsRemaining = new List<int>(10);

		private int _currentSequentialClipIndex;

		private AudioDuckingMode _duckingMode;

		private float _timeToStartUnducking;

		private float _timeToFinishUnducking;

		private float _originalMusicVolume;

		private float _initialDuckVolume;

		private float _duckRange;

		private MusicSetting _currentSong;

		private GameObject _go;

		private string _name;

		private FadeMode _curFadeMode;

		private float _slowFadeTargetVolume;

		private float _slowFadeVolStep;

		private MasterAudio.Playlist _currentPlaylist;

		private float _lastTimeMissingPlaylistLogged = -5f;

		private Action _fadeCompleteCallback;

		private readonly List<MusicSetting> _queuedSongs = new List<MusicSetting>(5);

		private bool _lostFocus;

		private AudioSource _audioClip;

		private AudioSource _transClip;

		private MusicSetting _newSongSetting;

		private bool _nextSongRequested;

		private bool _nextSongScheduled;

		private int _lastRandomClipIndex = -1;

		private readonly Dictionary<AudioSource, double> _scheduledSongsByAudioSource = new Dictionary<AudioSource, double>(2);

		private static List<PlaylistController> _instances;

		private static int _songsPlayedFromPlaylist;

		private AudioSource _audio1;

		private AudioSource _audio2;

		private Transform _trans;

		private bool SongIsNonAdvancible
		{
			get
			{
				return CurrentPlaylist != null && CurrentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips && CrossFadeTime > 0f;
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
				return CurrentPlaylist.resourceClipsAllLoadAsync;
			}
		}

		public bool ControllerIsReady { get; private set; }

		public PlaylistStates PlaylistState
		{
			get
			{
				if (_activeAudio == null || _transitioningAudio == null)
				{
					return PlaylistStates.NotInScene;
				}
				if (!ActiveAudioSource.isPlaying)
				{
					if (ActiveAudioSource.time != 0f)
					{
						return PlaylistStates.Paused;
					}
					return PlaylistStates.Stopped;
				}
				if (IsCrossFading)
				{
					return PlaylistStates.Crossfading;
				}
				return PlaylistStates.Playing;
			}
		}

		public AudioSource ActiveAudioSource
		{
			get
			{
				if (_activeAudio != null && _activeAudio.clip == null)
				{
					return _transitioningAudio;
				}
				return _activeAudio;
			}
		}

		public static List<PlaylistController> Instances
		{
			get
			{
				if (_instances != null)
				{
					return _instances;
				}
				_instances = new List<PlaylistController>();
				UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(PlaylistController));
				for (int i = 0; i < array.Length; i++)
				{
					_instances.Add(array[i] as PlaylistController);
				}
				return _instances;
			}
			set
			{
				_instances = value;
			}
		}

		public GameObject PlaylistControllerGameObject
		{
			get
			{
				return _go;
			}
		}

		public AudioSource CurrentPlaylistSource
		{
			get
			{
				return _activeAudio;
			}
		}

		public AudioClip CurrentPlaylistClip
		{
			get
			{
				if (_activeAudio == null)
				{
					return null;
				}
				return _activeAudio.clip;
			}
		}

		public AudioClip FadingPlaylistClip
		{
			get
			{
				if (!IsCrossFading)
				{
					return null;
				}
				if (_transitioningAudio == null)
				{
					return null;
				}
				return _transitioningAudio.clip;
			}
		}

		public AudioSource FadingSource
		{
			get
			{
				if (!IsCrossFading)
				{
					return null;
				}
				return _transitioningAudio;
			}
		}

		public bool IsCrossFading { get; private set; }

		public bool IsFading
		{
			get
			{
				return IsCrossFading || _curFadeMode != FadeMode.None;
			}
		}

		public float PlaylistVolume
		{
			get
			{
				return MasterAudio.PlaylistMasterVolume * _playlistVolume;
			}
			set
			{
				_playlistVolume = value;
				UpdateMasterVolume();
			}
		}

		public MasterAudio.Playlist CurrentPlaylist
		{
			get
			{
				if (_currentPlaylist != null || !(Time.realtimeSinceStartup - _lastTimeMissingPlaylistLogged > 2f))
				{
					return _currentPlaylist;
				}
				_lastTimeMissingPlaylistLogged = Time.realtimeSinceStartup;
				return _currentPlaylist;
			}
		}

		public bool HasPlaylist
		{
			get
			{
				return _currentPlaylist != null;
			}
		}

		public string PlaylistName
		{
			get
			{
				if (CurrentPlaylist == null)
				{
					return string.Empty;
				}
				return CurrentPlaylist.playlistName;
			}
		}

		private bool IsMuted
		{
			get
			{
				return isMuted;
			}
		}

		private bool PlaylistIsMuted
		{
			set
			{
				isMuted = value;
				if (Application.isPlaying)
				{
					if (_activeAudio != null)
					{
						_activeAudio.mute = value;
					}
					if (_transitioningAudio != null)
					{
						_transitioningAudio.mute = value;
					}
				}
				else
				{
					AudioSource[] components = GetComponents<AudioSource>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].mute = value;
					}
				}
			}
		}

		private float CrossFadeTime
		{
			get
			{
				if (_currentPlaylist != null)
				{
					return (_currentPlaylist.crossfadeMode != 0) ? _currentPlaylist.crossFadeTime : MasterAudio.Instance.MasterCrossFadeTime;
				}
				return MasterAudio.Instance.MasterCrossFadeTime;
			}
		}

		private bool IsAutoAdvance
		{
			get
			{
				if (SongIsNonAdvancible)
				{
					return false;
				}
				return isAutoAdvance;
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

		public string ControllerName
		{
			get
			{
				if (_name != null)
				{
					return _name;
				}
				_name = GameObj.name;
				return _name;
			}
		}

		public bool CanSchedule
		{
			get
			{
				return MasterAudio.Instance.useGaplessPlaylists && IsAutoAdvance;
			}
		}

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

		public int ClipsRemainingInCurrentPlaylist
		{
			get
			{
				return _clipsRemaining.Count;
			}
		}

		public event SongChangedEventHandler SongChanged;

		public event SongEndedEventHandler SongEnded;

		private void Awake()
		{
			if (ControllerIsReady)
			{
				return;
			}
			ControllerIsReady = false;
			PlaylistController[] array = (PlaylistController[])UnityEngine.Object.FindObjectsOfType(typeof(PlaylistController));
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ControllerName == ControllerName)
				{
					num++;
				}
			}
			if (num > 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			base.useGUILayout = false;
			_duckingMode = AudioDuckingMode.NotDucking;
			_currentSong = null;
			_songsPlayedFromPlaylist = 0;
			AudioSource[] components = GetComponents<AudioSource>();
			if (components.Length >= 2)
			{
				_audio1 = components[0];
				_audio2 = components[1];
				_audio1.clip = null;
				_audio2.clip = null;
				if (_audio1.playOnAwake || _audio2.playOnAwake)
				{
				}
				_activeAudio = _audio1;
				_transitioningAudio = _audio2;
				_audio1.outputAudioMixerGroup = mixerChannel;
				_audio2.outputAudioMixerGroup = mixerChannel;
				SetSpatialBlend();
				_curFadeMode = FadeMode.None;
				_fadeCompleteCallback = null;
				_lostFocus = false;
			}
		}

		public void SetSpatialBlend()
		{
			if (MasterAudio.SafeInstance == null)
			{
				return;
			}
			switch (MasterAudio.Instance.musicSpatialBlendType)
			{
			case MasterAudio.AllMusicSpatialBlendType.ForceAllTo2D:
				SetAudioSpatialBlend(0f);
				break;
			case MasterAudio.AllMusicSpatialBlendType.ForceAllTo3D:
				SetAudioSpatialBlend(1f);
				break;
			case MasterAudio.AllMusicSpatialBlendType.ForceAllToCustom:
				SetAudioSpatialBlend(MasterAudio.Instance.musicSpatialBlend);
				break;
			case MasterAudio.AllMusicSpatialBlendType.AllowDifferentPerController:
				switch (spatialBlendType)
				{
				case MasterAudio.ItemSpatialBlendType.ForceTo2D:
					SetAudioSpatialBlend(0f);
					break;
				case MasterAudio.ItemSpatialBlendType.ForceTo3D:
					SetAudioSpatialBlend(1f);
					break;
				case MasterAudio.ItemSpatialBlendType.ForceToCustom:
					SetAudioSpatialBlend(spatialBlend);
					break;
				case MasterAudio.ItemSpatialBlendType.UseCurveFromAudioSource:
					break;
				}
				break;
			}
		}

		private void SetAudiosIfEmpty()
		{
			AudioSource[] components = GetComponents<AudioSource>();
			_audio1 = components[0];
			_audio2 = components[1];
		}

		private void SetAudioSpatialBlend(float blend)
		{
			if (_audio1 == null)
			{
				SetAudiosIfEmpty();
			}
			_audio1.spatialBlend = blend;
			_audio2.spatialBlend = blend;
		}

		private void Start()
		{
			if (!ControllerIsReady)
			{
				if (IsMuted)
				{
					MutePlaylist();
				}
				if (!string.IsNullOrEmpty(startPlaylistName) && _currentPlaylist == null)
				{
					InitializePlaylist();
				}
				ControllerIsReady = true;
				if (_currentPlaylist != null && startPlaylistOnAwake)
				{
					PlayNextOrRandom(AudioPlayType.PlayNow);
				}
			}
		}

		private void CoUpdate()
		{
			if (CanSchedule && !CurrentPlaylistSource.loop && _scheduledSongsByAudioSource.Count > 0 && _scheduledSongsByAudioSource.ContainsKey(_audioClip))
			{
				double num = CalculateNextTrackStartTime();
				double num2 = _scheduledSongsByAudioSource[_audioClip];
				if (num != num2)
				{
					_audioClip.Stop();
					ScheduleClipPlay(num);
				}
			}
			if (_curFadeMode != FadeMode.GradualFade || _activeAudio == null)
			{
				return;
			}
			float val = _playlistVolume + _slowFadeVolStep;
			val = (_playlistVolume = ((!(_slowFadeVolStep > 0f)) ? Math.Max(val, _slowFadeTargetVolume) : Math.Min(val, _slowFadeTargetVolume)));
			UpdateMasterVolume();
			if (val == _slowFadeTargetVolume)
			{
				if (MasterAudio.Instance.stopZeroVolumePlaylists && _slowFadeTargetVolume == 0f)
				{
					StopPlaylist();
				}
				if (_fadeCompleteCallback != null)
				{
					_fadeCompleteCallback();
					_fadeCompleteCallback = null;
				}
				_curFadeMode = FadeMode.None;
			}
		}

		private void OnEnable()
		{
			_instances = null;
		}

		private void OnDisable()
		{
			_instances = null;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			_lostFocus = pauseStatus;
		}

		private void Update()
		{
			CoUpdate();
			if (_lostFocus)
			{
				return;
			}
			if (IsCrossFading)
			{
				if (_activeAudio.volume >= _activeAudioEndVolume)
				{
					CeaseAudioSource(_transitioningAudio);
					IsCrossFading = false;
					if (CanSchedule && !_nextSongScheduled)
					{
						PlayNextOrRandom(AudioPlayType.Schedule);
					}
					SetDuckProperties();
				}
				float num = Math.Max(CrossFadeTime, 0.001f);
				float num2 = (Time.realtimeSinceStartup - _crossFadeStartTime) / num;
				_activeAudio.volume = num2 * _activeAudioEndVolume;
				_transitioningAudio.volume = _transitioningAudioStartVolume * (1f - num2);
			}
			if (!_activeAudio.loop && _activeAudio.clip != null)
			{
				if (!IsAutoAdvance && !_activeAudio.isPlaying)
				{
					CeaseAudioSource(_activeAudio);
					return;
				}
				if (!AudioUtil.IsAudioPaused(_activeAudio))
				{
					bool flag;
					if (!_activeAudio.isPlaying)
					{
						flag = true;
					}
					else
					{
						float num3 = _activeAudio.clip.length - _activeAudio.time - CrossFadeTime * _activeAudio.pitch;
						float num4 = Time.deltaTime * 2f * _activeAudio.pitch;
						flag = num3 <= num4;
					}
					if (flag)
					{
						if (_currentPlaylist.fadeOutLastSong)
						{
							if (isShuffle)
							{
								if (_clipsRemaining.Count == 0 || !IsAutoAdvance)
								{
									FadeOutPlaylist();
									return;
								}
							}
							else if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count || _currentPlaylist.MusicSettings.Count == 1 || !IsAutoAdvance)
							{
								FadeOutPlaylist();
								return;
							}
						}
						if (IsAutoAdvance && !_nextSongRequested)
						{
							if (CanSchedule)
							{
								FadeInScheduledSong();
							}
							else
							{
								PlayNextOrRandom(AudioPlayType.PlayNow);
							}
						}
					}
				}
			}
			if (!IsCrossFading)
			{
				AudioDucking();
			}
		}

		public static PlaylistController InstanceByName(string playlistControllerName, bool errorIfNotFound = true)
		{
			PlaylistController playlistController = Instances.Find((PlaylistController obj) => obj != null && obj.ControllerName == playlistControllerName);
			if (playlistController != null)
			{
				return playlistController;
			}
			if (errorIfNotFound)
			{
			}
			return null;
		}

		public void ClearQueue()
		{
			if (ControllerIsReady)
			{
				_queuedSongs.Clear();
			}
		}

		public void ToggleMutePlaylist()
		{
			if (!Application.isPlaying || ControllerIsReady)
			{
				if (IsMuted)
				{
					UnmutePlaylist();
				}
				else
				{
					MutePlaylist();
				}
			}
		}

		public void MutePlaylist()
		{
			PlaylistIsMuted = true;
		}

		public void UnmutePlaylist()
		{
			PlaylistIsMuted = false;
		}

		public void UpdateDuckedVolumeMultiplier()
		{
			if (ControllerIsReady && Application.isPlaying)
			{
				SetDuckProperties();
			}
		}

		public void PausePlaylist()
		{
			if (ControllerIsReady && !(_activeAudio == null) && !(_transitioningAudio == null))
			{
				_activeAudio.Pause();
				_transitioningAudio.Pause();
			}
		}

		public bool ResumePlaylist()
		{
			if (!ControllerIsReady)
			{
				return false;
			}
			if (_activeAudio == null || _transitioningAudio == null)
			{
				return false;
			}
			if (_activeAudio.clip == null)
			{
				return false;
			}
			_activeAudio.Play();
			_transitioningAudio.Play();
			return true;
		}

		public void StopPlaylist(bool onlyFadingClip = false)
		{
			if (ControllerIsReady && Application.isPlaying)
			{
				_currentSong = null;
				if (!onlyFadingClip)
				{
					CeaseAudioSource(_activeAudio);
				}
				CeaseAudioSource(_transitioningAudio);
			}
		}

		public void FadeToVolume(float targetVolume, float fadeTime, Action callback = null)
		{
			if (ControllerIsReady)
			{
				if (fadeTime <= 0.1f)
				{
					_playlistVolume = targetVolume;
					UpdateMasterVolume();
					_curFadeMode = FadeMode.None;
				}
				else
				{
					_curFadeMode = FadeMode.GradualFade;
					_slowFadeTargetVolume = targetVolume;
					_slowFadeVolStep = (_slowFadeTargetVolume - _playlistVolume) / (fadeTime / AudioUtil.FrameTime());
					_fadeCompleteCallback = callback;
				}
			}
		}

		public void PlayRandomSong()
		{
			if (ControllerIsReady)
			{
				PlayARandomSong(AudioPlayType.PlayNow, false);
			}
		}

		public void PlayARandomSong(AudioPlayType playType, bool isMidsong)
		{
			if (!ControllerIsReady || _clipsRemaining.Count == 0 || (IsCrossFading && playType == AudioPlayType.Schedule))
			{
				return;
			}
			if (isMidsong)
			{
				_nextSongScheduled = false;
			}
			int num = UnityEngine.Random.Range(0, _clipsRemaining.Count);
			int index = _clipsRemaining[num];
			switch (playType)
			{
			case AudioPlayType.PlayNow:
				RemoveRandomClip(num);
				break;
			case AudioPlayType.Schedule:
				_lastRandomClipIndex = num;
				break;
			case AudioPlayType.AlreadyScheduled:
				if (_lastRandomClipIndex >= 0)
				{
					RemoveRandomClip(_lastRandomClipIndex);
				}
				break;
			}
			PlaySong(_currentPlaylist.MusicSettings[index], playType);
		}

		private void RemoveRandomClip(int randIndex)
		{
			_clipsRemaining.RemoveAt(randIndex);
			if (loopPlaylist && _clipsRemaining.Count == 0)
			{
				FillClips();
			}
		}

		private void PlayFirstQueuedSong(AudioPlayType playType)
		{
			if (_queuedSongs.Count != 0)
			{
				MusicSetting musicSetting = _queuedSongs[0];
				_queuedSongs.RemoveAt(0);
				_currentSequentialClipIndex = musicSetting.songIndex;
				PlaySong(musicSetting, playType);
			}
		}

		public void PlayNextSong()
		{
			if (ControllerIsReady)
			{
				PlayTheNextSong(AudioPlayType.PlayNow, false);
			}
		}

		public void PlayTheNextSong(AudioPlayType playType, bool isMidsong)
		{
			if (_currentPlaylist == null || (IsCrossFading && playType == AudioPlayType.Schedule))
			{
				return;
			}
			if (playType != AudioPlayType.AlreadyScheduled && _songsPlayedFromPlaylist > 0 && !_nextSongScheduled)
			{
				AdvanceSongCounter();
			}
			if (_currentSequentialClipIndex < _currentPlaylist.MusicSettings.Count)
			{
				if (isMidsong)
				{
					_nextSongScheduled = false;
				}
				PlaySong(_currentPlaylist.MusicSettings[_currentSequentialClipIndex], playType);
			}
		}

		private void AdvanceSongCounter()
		{
			_currentSequentialClipIndex++;
			if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count && loopPlaylist)
			{
				_currentSequentialClipIndex = 0;
			}
		}

		public void QueuePlaylistClip(string clipName)
		{
			if (!ControllerIsReady)
			{
				return;
			}
			if (_currentPlaylist == null)
			{
				MasterAudio.LogNoPlaylist(ControllerName, "QueuePlaylistClip");
				return;
			}
			if (!_activeAudio.isPlaying)
			{
				TriggerPlaylistClip(clipName);
				return;
			}
			MusicSetting musicSetting = _currentPlaylist.MusicSettings.Find((MusicSetting obj) => (obj.audLocation == MasterAudio.AudioLocation.Clip) ? (obj.clip != null && obj.clip.name == clipName) : (obj.resourceFileName == clipName));
			if (musicSetting != null)
			{
				_activeAudio.loop = false;
				_queuedSongs.Add(musicSetting);
			}
		}

		public bool TriggerPlaylistClip(string clipName)
		{
			if (!ControllerIsReady)
			{
				return false;
			}
			if (_currentPlaylist == null)
			{
				MasterAudio.LogNoPlaylist(ControllerName, "TriggerPlaylistClip");
				return false;
			}
			MusicSetting musicSetting = _currentPlaylist.MusicSettings.Find((MusicSetting obj) => obj.alias == clipName);
			if (musicSetting == null)
			{
				musicSetting = _currentPlaylist.MusicSettings.Find((MusicSetting obj) => (obj.audLocation == MasterAudio.AudioLocation.Clip) ? (obj.clip != null && obj.clip.name == clipName) : (obj.resourceFileName == clipName || obj.songName == clipName));
			}
			if (musicSetting == null)
			{
				return false;
			}
			_currentSequentialClipIndex = musicSetting.songIndex;
			PlaySong(musicSetting, AudioPlayType.PlayNow);
			return true;
		}

		public void DuckMusicForTime(float duckLength, float pitch, float duckedTimePercentage)
		{
			if (!MasterAudio.IsWarming && ControllerIsReady && !IsCrossFading)
			{
				float num = duckLength / pitch;
				_duckingMode = AudioDuckingMode.SetToDuck;
				_timeToStartUnducking = Time.realtimeSinceStartup + num * duckedTimePercentage;
				_timeToFinishUnducking = Math.Max(Time.realtimeSinceStartup + num, _timeToStartUnducking);
			}
		}

		private void InitControllerIfNot()
		{
			if (!ControllerIsReady)
			{
				Awake();
				Start();
			}
		}

		public void UpdateMasterVolume()
		{
			if (Application.isPlaying)
			{
				InitControllerIfNot();
				if (_activeAudio != null && _currentSong != null && !IsCrossFading)
				{
					_activeAudio.volume = _currentSong.volume * PlaylistVolume;
				}
				if (_currentSong != null)
				{
					_activeAudioEndVolume = _currentSong.volume * PlaylistVolume;
				}
				SetDuckProperties();
			}
		}

		public void StartPlaylist(string playlistName)
		{
			if (ControllerIsReady)
			{
				if (_currentPlaylist != null && _currentPlaylist.playlistName == playlistName)
				{
					RestartPlaylist();
				}
				else
				{
					ChangePlaylist(playlistName);
				}
			}
		}

		public void ChangePlaylist(string playlistName, bool playFirstClip = true)
		{
			InitControllerIfNot();
			if (ControllerIsReady && (_currentPlaylist == null || !(_currentPlaylist.playlistName == playlistName)))
			{
				startPlaylistName = playlistName;
				FinishPlaylistInit(playFirstClip);
			}
		}

		private void FinishPlaylistInit(bool playFirstClip = true)
		{
			if (IsCrossFading)
			{
				StopPlaylist(true);
			}
			InitializePlaylist();
			if (Application.isPlaying)
			{
				_queuedSongs.Clear();
				if (playFirstClip)
				{
					PlayNextOrRandom(AudioPlayType.PlayNow);
				}
			}
		}

		public void RestartPlaylist()
		{
			if (ControllerIsReady)
			{
				FinishPlaylistInit();
			}
		}

		private void FadeOutPlaylist()
		{
			if (_curFadeMode != FadeMode.GradualFade)
			{
				float volumeBeforeFade = _playlistVolume;
				FadeToVolume(0f, CrossFadeTime, delegate
				{
					StopPlaylist();
					_playlistVolume = volumeBeforeFade;
				});
			}
		}

		private void InitializePlaylist()
		{
			FillClips();
			_songsPlayedFromPlaylist = 0;
			_currentSequentialClipIndex = 0;
			_nextSongScheduled = false;
			_lastRandomClipIndex = -1;
		}

		private void PlayNextOrRandom(AudioPlayType playType)
		{
			_nextSongRequested = true;
			if (_queuedSongs.Count > 0)
			{
				PlayFirstQueuedSong(playType);
			}
			else if (!isShuffle)
			{
				PlayTheNextSong(playType, false);
			}
			else
			{
				PlayARandomSong(playType, false);
			}
		}

		private void FillClips()
		{
			_clipsRemaining.Clear();
			if (startPlaylistName == "[No Playlist]")
			{
				return;
			}
			_currentPlaylist = MasterAudio.GrabPlaylist(startPlaylistName);
			if (_currentPlaylist == null)
			{
				return;
			}
			for (int i = 0; i < _currentPlaylist.MusicSettings.Count; i++)
			{
				MusicSetting musicSetting = _currentPlaylist.MusicSettings[i];
				musicSetting.songIndex = i;
				if (musicSetting.audLocation != MasterAudio.AudioLocation.ResourceFile)
				{
					if (musicSetting.clip == null)
					{
						continue;
					}
				}
				else if (string.IsNullOrEmpty(musicSetting.resourceFileName))
				{
					continue;
				}
				_clipsRemaining.Add(i);
			}
		}

		private void PlaySong(MusicSetting setting, AudioPlayType playType)
		{
			_newSongSetting = setting;
			if (_activeAudio == null)
			{
				return;
			}
			AudioClip audioClip = null;
			if ((playType == AudioPlayType.PlayNow || playType == AudioPlayType.AlreadyScheduled) && _currentSong != null && !CanSchedule && _currentSong.songChangedCustomEvent != string.Empty && _currentSong.songChangedCustomEvent != "[None]")
			{
				MasterAudio.FireCustomEvent(_currentSong.songChangedCustomEvent, Trans.position);
			}
			if (playType != AudioPlayType.AlreadyScheduled)
			{
				if (_activeAudio.clip != null)
				{
					string value = string.Empty;
					switch (setting.audLocation)
					{
					case MasterAudio.AudioLocation.Clip:
						if (setting.clip != null)
						{
							value = setting.clip.name;
						}
						break;
					case MasterAudio.AudioLocation.ResourceFile:
						value = setting.resourceFileName;
						break;
					}
					if (string.IsNullOrEmpty(value))
					{
						return;
					}
				}
				if (_activeAudio.clip == null)
				{
					_audioClip = _activeAudio;
					_transClip = _transitioningAudio;
				}
				else if (_transitioningAudio.clip == null)
				{
					_audioClip = _transitioningAudio;
					_transClip = _activeAudio;
				}
				else
				{
					_audioClip = _transitioningAudio;
					_transClip = _activeAudio;
				}
				if (setting.clip != null)
				{
					_audioClip.clip = setting.clip;
					_audioClip.pitch = setting.pitch;
				}
				_audioClip.loop = SongShouldLoop(setting);
				switch (setting.audLocation)
				{
				case MasterAudio.AudioLocation.Clip:
					if (setting.clip == null)
					{
						MasterAudio.LogWarning("MasterAudio will not play empty Playlist clip for PlaylistController '" + ControllerName + "'.");
						return;
					}
					audioClip = setting.clip;
					break;
				case MasterAudio.AudioLocation.ResourceFile:
					if (MasterAudio.HasAsyncResourceLoaderFeature() && ShouldLoadAsync)
					{
						StartCoroutine(AudioResourceOptimizer.PopulateResourceSongToPlaylistControllerAsync(setting.resourceFileName, CurrentPlaylist.playlistName, this, playType));
						break;
					}
					audioClip = AudioResourceOptimizer.PopulateResourceSongToPlaylistController(ControllerName, setting.resourceFileName, CurrentPlaylist.playlistName);
					if (audioClip == null)
					{
						return;
					}
					break;
				}
			}
			else
			{
				FinishLoadingNewSong(null, AudioPlayType.AlreadyScheduled);
			}
			if (audioClip != null)
			{
				FinishLoadingNewSong(audioClip, playType);
			}
		}

		public void FinishLoadingNewSong(AudioClip clipToPlay, AudioPlayType playType)
		{
			_nextSongRequested = false;
			bool flag = playType == AudioPlayType.PlayNow || playType == AudioPlayType.Schedule;
			bool flag2 = playType == AudioPlayType.PlayNow || playType == AudioPlayType.AlreadyScheduled;
			if (flag)
			{
				_audioClip.clip = clipToPlay;
				_audioClip.pitch = _newSongSetting.pitch;
			}
			if (_currentSong != null)
			{
				_currentSong.lastKnownTimePoint = _activeAudio.timeSamples;
			}
			if (flag2)
			{
				if (CrossFadeTime == 0f || _transClip.clip == null)
				{
					CeaseAudioSource(_transClip);
					_audioClip.volume = _newSongSetting.volume * PlaylistVolume;
					if (!ActiveAudioSource.isPlaying && _currentPlaylist != null && _currentPlaylist.fadeInFirstSong && CrossFadeTime > 0f)
					{
						CrossFadeNow(_audioClip);
					}
				}
				else
				{
					CrossFadeNow(_audioClip);
				}
				SetDuckProperties();
			}
			switch (playType)
			{
			case AudioPlayType.AlreadyScheduled:
				_nextSongScheduled = false;
				RemoveScheduledClip();
				break;
			case AudioPlayType.PlayNow:
				_audioClip.Play();
				_songsPlayedFromPlaylist++;
				break;
			case AudioPlayType.Schedule:
			{
				double scheduledPlayTime = CalculateNextTrackStartTime();
				ScheduleClipPlay(scheduledPlayTime);
				_nextSongScheduled = true;
				_songsPlayedFromPlaylist++;
				break;
			}
			}
			bool flag3 = false;
			if (syncGroupNum > 0 && _currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips)
			{
				PlaylistController playlistController = Instances.Find((PlaylistController obj) => obj != this && obj.syncGroupNum == syncGroupNum && obj.ActiveAudioSource != null && obj.ActiveAudioSource.isPlaying);
				if (playlistController != null)
				{
					int timeSamples = playlistController._activeAudio.timeSamples;
					if (_audioClip.clip != null && timeSamples < _audioClip.clip.samples)
					{
						_audioClip.timeSamples = timeSamples;
					}
					flag3 = true;
				}
			}
			if (_currentPlaylist != null)
			{
				if (_songsPlayedFromPlaylist <= 1 && !flag3)
				{
					_audioClip.timeSamples = 0;
				}
				else
				{
					switch (_currentPlaylist.songTransitionType)
					{
					case MasterAudio.SongFadeInPosition.SynchronizeClips:
						if (!flag3)
						{
							_transitioningAudio.timeSamples = _activeAudio.timeSamples;
						}
						break;
					case MasterAudio.SongFadeInPosition.NewClipFromLastKnownPosition:
					{
						MusicSetting musicSetting = _currentPlaylist.MusicSettings.Find((MusicSetting obj) => obj == _newSongSetting);
						if (musicSetting != null)
						{
							_transitioningAudio.timeSamples = musicSetting.lastKnownTimePoint;
						}
						break;
					}
					case MasterAudio.SongFadeInPosition.NewClipFromBeginning:
						_audioClip.timeSamples = 0;
						break;
					}
				}
				if (_currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.NewClipFromBeginning && _newSongSetting.customStartTime > 0f)
				{
					_audioClip.timeSamples = (int)(_newSongSetting.customStartTime * (float)_audioClip.clip.frequency);
				}
			}
			if (flag2)
			{
				_activeAudio = _audioClip;
				_transitioningAudio = _transClip;
				if (songChangedCustomEvent != string.Empty && songChangedCustomEvent != "[None]")
				{
					MasterAudio.FireCustomEvent(songChangedCustomEvent, Trans.position);
				}
				if (this.SongChanged != null)
				{
					string empty = string.Empty;
					if (_audioClip != null)
					{
						empty = _audioClip.clip.name;
					}
					this.SongChanged(empty);
				}
			}
			_activeAudioEndVolume = _newSongSetting.volume * PlaylistVolume;
			float volume = _transitioningAudio.volume;
			if (_currentSong != null)
			{
				volume = _currentSong.volume;
			}
			_transitioningAudioStartVolume = volume * PlaylistVolume;
			_currentSong = _newSongSetting;
			if (flag2 && _currentSong.songStartedCustomEvent != string.Empty && _currentSong.songStartedCustomEvent != "[None]")
			{
				MasterAudio.FireCustomEvent(_currentSong.songStartedCustomEvent, Trans.position);
			}
			if (CanSchedule && playType != AudioPlayType.Schedule)
			{
				ScheduleNextSong();
			}
		}

		private void RemoveScheduledClip()
		{
			if (_audioClip != null)
			{
				_scheduledSongsByAudioSource.Remove(_audioClip);
			}
		}

		private void ScheduleNextSong()
		{
			PlayNextOrRandom(AudioPlayType.Schedule);
		}

		private void FadeInScheduledSong()
		{
			PlayNextOrRandom(AudioPlayType.AlreadyScheduled);
		}

		private double CalculateNextTrackStartTime()
		{
			float num = (_activeAudio.clip.length - _activeAudio.time) / _activeAudio.pitch - CrossFadeTime;
			return AudioSettings.dspTime + (double)num;
		}

		private void ScheduleClipPlay(double scheduledPlayTime)
		{
			_audioClip.PlayScheduled(scheduledPlayTime);
			RemoveScheduledClip();
			_scheduledSongsByAudioSource.Add(_audioClip, scheduledPlayTime);
		}

		private void CrossFadeNow(AudioSource audioClip)
		{
			audioClip.volume = 0f;
			IsCrossFading = true;
			_duckingMode = AudioDuckingMode.NotDucking;
			_crossFadeStartTime = Time.realtimeSinceStartup;
		}

		private void CeaseAudioSource(AudioSource source)
		{
			if (!(source == null))
			{
				string text = ((!(source.clip == null)) ? source.clip.name : string.Empty);
				source.Stop();
				AudioUtil.UnloadNonPreloadedAudioData(source.clip);
				AudioResourceOptimizer.UnloadPlaylistSongIfUnused(ControllerName, source.clip);
				source.clip = null;
				RemoveScheduledClip();
				if (songEndedCustomEvent != string.Empty && songEndedCustomEvent != "[None]")
				{
					MasterAudio.FireCustomEvent(songEndedCustomEvent, Trans.position);
				}
				if (this.SongEnded != null && !string.IsNullOrEmpty(text))
				{
					this.SongEnded(text);
				}
			}
		}

		private void SetDuckProperties()
		{
			_originalMusicVolume = ((!(_activeAudio == null)) ? _activeAudio.volume : 1f);
			if (_currentSong != null)
			{
				_originalMusicVolume = _currentSong.volume * PlaylistVolume;
			}
			_initialDuckVolume = MasterAudio.Instance.DuckedVolumeMultiplier * _originalMusicVolume;
			_duckRange = _originalMusicVolume - MasterAudio.Instance.DuckedVolumeMultiplier;
			_duckingMode = AudioDuckingMode.NotDucking;
		}

		private void AudioDucking()
		{
			switch (_duckingMode)
			{
			case AudioDuckingMode.NotDucking:
				break;
			case AudioDuckingMode.SetToDuck:
				_activeAudio.volume = _initialDuckVolume;
				_duckingMode = AudioDuckingMode.Ducked;
				break;
			case AudioDuckingMode.Ducked:
				if (Time.realtimeSinceStartup >= _timeToFinishUnducking)
				{
					_activeAudio.volume = _originalMusicVolume;
					_duckingMode = AudioDuckingMode.NotDucking;
				}
				else if (Time.realtimeSinceStartup >= _timeToStartUnducking)
				{
					_activeAudio.volume = _initialDuckVolume + (Time.realtimeSinceStartup - _timeToStartUnducking) / (_timeToFinishUnducking - _timeToStartUnducking) * _duckRange;
				}
				break;
			}
		}

		private bool SongShouldLoop(MusicSetting setting)
		{
			if (_queuedSongs.Count > 0)
			{
				return false;
			}
			if (SongIsNonAdvancible)
			{
				return true;
			}
			return setting.isLoop;
		}

		public void RouteToMixerChannel(AudioMixerGroup group)
		{
			_activeAudio.outputAudioMixerGroup = group;
			_transitioningAudio.outputAudioMixerGroup = group;
		}
	}
}
