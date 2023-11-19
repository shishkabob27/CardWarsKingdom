using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Height")]
public class TweenHeight : UITweener
{
	public int from = 100;

	public int to = 100;

	public bool updateTable;

	private UIWidget mWidget;

	private UITable mTable;

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
	public int height
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
			return cachedWidget.height;
		}
		set
		{
			cachedWidget.height = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
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

	public static TweenHeight Begin(UIWidget widget, float duration, int height)
	{
		TweenHeight tweenHeight = UITweener.Begin<TweenHeight>(widget.gameObject, duration);
		tweenHeight.from = widget.height;
		tweenHeight.to = height;
		if (duration <= 0f)
		{
			tweenHeight.Sample(1f, true);
			tweenHeight.enabled = false;
		}
		return tweenHeight;
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
