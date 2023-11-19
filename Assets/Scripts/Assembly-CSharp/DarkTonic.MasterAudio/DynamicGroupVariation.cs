using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class DynamicGroupVariation : MonoBehaviour
	{
		public bool useLocalization;

		public bool useRandomPitch;

		public SoundGroupVariation.RandomPitchMode randomPitchMode;

		public float randomPitchMin;

		public float randomPitchMax;

		public bool useRandomVolume;

		public SoundGroupVariation.RandomVolumeMode randomVolumeMode;

		public float randomVolumeMin;

		public float randomVolumeMax;

		public int weight = 1;

		public MasterAudio.AudioLocation audLocation;

		public string resourceFileName;

		public string internetFileUrl;

		public bool isExpanded = true;

		public bool isChecked = true;

		public float fxTailTime;

		public bool useFades;

		public float fadeInTime;

		public float fadeOutTime;

		public bool useIntroSilence;

		public float introSilenceMin;

		public float introSilenceMax;

		public bool useRandomStartTime;

		public float randomStartMinPercent;

		public float randomStartMaxPercent;

		private AudioDistortionFilter _distFilter;

		private AudioEchoFilter _echoFilter;

		private AudioHighPassFilter _hpFilter;

		private AudioLowPassFilter _lpFilter;

		private AudioReverbFilter _reverbFilter;

		private AudioChorusFilter _chorusFilter;

		private DynamicSoundGroup _parentGroupScript;

		private Transform _trans;

		private AudioSource _aud;

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

		public DynamicSoundGroup ParentGroup
		{
			get
			{
				if (_parentGroupScript == null)
				{
					_parentGroupScript = Trans.parent.GetComponent<DynamicSoundGroup>();
				}
				if (_parentGroupScript == null)
				{
				}
				return _parentGroupScript;
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

		public AudioSource VarAudio
		{
			get
			{
				if (_aud != null)
				{
					return _aud;
				}
				_aud = GetComponent<AudioSource>();
				return _aud;
			}
		}
	}
}
