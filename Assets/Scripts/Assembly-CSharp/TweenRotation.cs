using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Rotation")]
public class TweenRotation : UITweener
{
	public Vector3 from;

	public Vector3 to;

	private Transform mTrans;

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
	public Quaternion rotation
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

	public Quaternion value
	{
		get
		{
			return cachedTransform.localRotation;
		}
		set
		{
			cachedTransform.localRotation = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = Quaternion.Euler(new Vector3(Mathf.Lerp(from.x, to.x, factor), Mathf.Lerp(from.y, to.y, factor), Mathf.Lerp(from.z, to.z, factor)));
	}

	public static TweenRotation Begin(GameObject go, float duration, Quaternion rot)
	{
		TweenRotation tweenRotation = UITweener.Begin<TweenRotation>(go, duration);
		tweenRotation.from = tweenRotation.value.eulerAngles;
		tweenRotation.to = rot.eulerAngles;
		if (duration <= 0f)
		{
			tweenRotation.Sample(1f, true);
			tweenRotation.enabled = false;
		}
		return tweenRotation;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		from = value.eulerAngles;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		to = value.eulerAngles;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		value = Quaternion.Euler(from);
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		value = Quaternion.Euler(to);
	}
}
