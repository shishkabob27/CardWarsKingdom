using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("NGUI/Tween/Tween Volume")]
public class TweenVolume : UITweener
{
	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private AudioSource mSource;

	public AudioSource audioSource
	{
		get
		{
			if (mSource == null)
			{
				mSource = GetComponent<AudioSource>();
				if (mSource == null)
				{
					mSource = GetComponent<AudioSource>();
					if (mSource == null)
					{
						base.enabled = false;
					}
				}
			}
			return mSource;
		}
	}

	[Obsolete("Use 'value' instead")]
	public float volume
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public float value
	{
		get
		{
			return (!(audioSource != null)) ? 0f : mSource.volume;
		}
		set
		{
			if (audioSource != null)
			{
				mSource.volume = value;
			}
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
		mSource.enabled = mSource.volume > 0.01f;
	}

	public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
	{
		TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration);
		tweenVolume.from = tweenVolume.value;
		tweenVolume.to = targetVolume;
		return tweenVolume;
	}

	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	public override void SetEndToCurrentValue()
	{
		to = value;
	}
}
