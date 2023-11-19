using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;

	public Vector3 to;

	[HideInInspector]
	public bool worldSpace;

	private Transform mTrans;

	private UIRect mRect;

	private Vector3 mRestoreValue;

	private bool mRestoreValueSet;

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				if (target == null)
				{
					mTrans = base.transform;
				}
				else
				{
					mTrans = target.transform;
				}
			}
			return mTrans;
		}
	}

	[Obsolete("Use 'value' instead")]
	public Vector3 position
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

	public Vector3 value
	{
		get
		{
			return (!worldSpace) ? cachedTransform.localPosition : cachedTransform.position;
		}
		set
		{
			if (mRect == null || !mRect.isAnchored || worldSpace)
			{
				if (worldSpace)
				{
					cachedTransform.position = value;
				}
				else
				{
					cachedTransform.localPosition = value;
				}
			}
			else
			{
				value -= cachedTransform.localPosition;
				NGUIMath.MoveRect(mRect, value.x, value.y);
			}
		}
	}

	private void Awake()
	{
		mRect = GetComponent<UIRect>();
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = from * (1f - factor) + to * factor;
	}

	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}

	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.worldSpace = worldSpace;
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
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

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		to = value;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		value = from;
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		value = to;
	}
}
