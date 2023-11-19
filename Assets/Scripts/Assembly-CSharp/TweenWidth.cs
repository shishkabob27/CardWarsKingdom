using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Width")]
public class TweenWidth : UITweener
{
	public int from = 100;

	public int to = 100;

	public bool updateTable;

	private UIWidget mWidget;

	private UITable mTable;

	private int mRestoreValue;

	private bool mRestoreValueSet;

	public UIWidget cachedWidget
	{
		get
		{
			if (mWidget == null)
			{
				if (target == null)
				{
					mWidget = GetComponent<UIWidget>();
				}
				else
				{
					mWidget = target;
				}
			}
			return mWidget;
		}
	}

	[Obsolete("Use 'value' instead")]
	public int width
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

	public int value
	{
		get
		{
			return cachedWidget.width;
		}
		set
		{
			cachedWidget.width = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = Mathf.RoundToInt((float)from * (1f - factor) + (float)to * factor);
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

	public static TweenWidth Begin(UIWidget widget, float duration, int width)
	{
		TweenWidth tweenWidth = UITweener.Begin<TweenWidth>(widget.gameObject, duration);
		tweenWidth.from = widget.width;
		tweenWidth.to = width;
		if (duration <= 0f)
		{
			tweenWidth.Sample(1f, true);
			tweenWidth.enabled = false;
		}
		return tweenWidth;
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
