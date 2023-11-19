using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private float mRestoreValue;

	private bool mRestoreValueSet;

	private UIRect mRect;

	public UIRect cachedRect
	{
		get
		{
			if (mRect == null)
			{
				GameObject gameObject = ((!(target == null)) ? target.gameObject : base.gameObject);
				mRect = gameObject.GetComponent<UIRect>();
				if (mRect == null)
				{
					mRect = gameObject.GetComponentInChildren<UIRect>();
				}
			}
			return mRect;
		}
	}

	[Obsolete("Use 'value' instead")]
	public float alpha
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
			return cachedRect.alpha;
		}
		set
		{
			cachedRect.alpha = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = Mathf.Lerp(from, to, factor);
	}

	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	public override void SetEndToCurrentValue()
	{
		to = value;
	}

	public override void RestorePrevious()
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = mRestoreValue;
	}
}
