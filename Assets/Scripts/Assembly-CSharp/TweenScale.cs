using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Scale")]
public class TweenScale : UITweener
{
	public Vector3 from = Vector3.one;

	public Vector3 to = Vector3.one;

	public bool updateTable;

	private Vector3 mRestoreValue;

	private bool mRestoreValueSet;

	private Transform mTrans;

	private UITable mTable;

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

	public Vector3 value
	{
		get
		{
			return cachedTransform.localScale;
		}
		set
		{
			cachedTransform.localScale = value;
		}
	}

	[Obsolete("Use 'value' instead")]
	public Vector3 scale
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

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = from * (1f - factor) + to * factor;
		if (!updateTable)
		{
			return;
		}
		if (mTable == null)
		{
			mTable = NGUITools.FindInParents<UITable>(base.gameObject);
			if (mTable == null)
			{
				updateTable = false;
				return;
			}
		}
		mTable.repositionNow = true;
	}

	public static TweenScale Begin(GameObject go, float duration, Vector3 scale)
	{
		TweenScale tweenScale = UITweener.Begin<TweenScale>(go, duration);
		tweenScale.from = tweenScale.value;
		tweenScale.to = scale;
		if (duration <= 0f)
		{
			tweenScale.Sample(1f, true);
			tweenScale.enabled = false;
		}
		return tweenScale;
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
