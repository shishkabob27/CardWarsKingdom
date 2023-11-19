using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Tween Field of View")]
public class TweenFOV : UITweener
{
	public float from = 45f;

	public float to = 45f;

	private Camera mCam;

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				if (target == null)
				{
					mCam = GetComponent<Camera>();
				}
				else
				{
					mCam = target.GetComponent<Camera>();
				}
			}
			return mCam;
		}
	}

	[Obsolete("Use 'value' instead")]
	public float fov
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
			return cachedCamera.fieldOfView;
		}
		set
		{
			cachedCamera.fieldOfView = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
	}

	public static TweenFOV Begin(GameObject go, float duration, float to)
	{
		TweenFOV tweenFOV = UITweener.Begin<TweenFOV>(go, duration);
		tweenFOV.from = tweenFOV.value;
		tweenFOV.to = to;
		if (duration <= 0f)
		{
			tweenFOV.Sample(1f, true);
			tweenFOV.enabled = false;
		}
		return tweenFOV;
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
