using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;

	public Color to = Color.white;

	private Color mRestoreValue;

	private bool mRestoreValueSet;

	private bool mCached;

	private UIWidget mWidget;

	private Material mMat;

	private Light mLight;

	[Obsolete("Use 'value' instead")]
	public Color color
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

	public Color value
	{
		get
		{
			if (!mCached)
			{
				Cache();
			}
			if (mWidget != null)
			{
				return mWidget.color;
			}
			if (mLight != null)
			{
				return mLight.color;
			}
			if (mMat != null)
			{
				return mMat.color;
			}
			return Color.black;
		}
		set
		{
			if (!mCached)
			{
				Cache();
			}
			if (mWidget != null)
			{
				mWidget.color = value;
			}
			if (mMat != null)
			{
				mMat.color = value;
			}
			if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = value.r + value.g + value.b > 0.01f;
			}
		}
	}

	private void Cache()
	{
		mCached = true;
		GameObject gameObject = ((!(target == null)) ? target.gameObject : base.gameObject);
		mWidget = gameObject.GetComponent<UIWidget>();
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			mMat = component.material;
		}
		mLight = GetComponent<Light>();
		if (mWidget == null && mMat == null && mLight == null)
		{
			mWidget = gameObject.GetComponentInChildren<UIWidget>();
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (!mRestoreValueSet)
		{
			mRestoreValue = value;
			mRestoreValueSet = true;
		}
		value = Color.Lerp(from, to, factor);
	}

	public static TweenColor Begin(GameObject go, float duration, Color color)
	{
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
		tweenColor.from = tweenColor.value;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, true);
			tweenColor.enabled = false;
		}
		return tweenColor;
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
