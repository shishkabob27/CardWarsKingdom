using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Orthographic Size")]
[RequireComponent(typeof(Camera))]
public class TweenOrthoSize : UITweener
{
	public float from = 1f;

	public float to = 1f;

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
	public float orthoSize
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
			return cachedCamera.orthographicSize;
		}
		set
		{
			cachedCamera.orthographicSize = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
	}

	public static TweenOrthoSize Begin(GameObject go, float duration, float to)
	{
		TweenOrthoSize tweenOrthoSize = UITweener.Begin<TweenOrthoSize>(go, duration);
		tweenOrthoSize.from = tweenOrthoSize.value;
		tweenOrthoSize.to = to;
		if (duration <= 0f)
		{
			tweenOrthoSize.Sample(1f, true);
			tweenOrthoSize.enabled = false;
		}
		return tweenOrthoSize;
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
