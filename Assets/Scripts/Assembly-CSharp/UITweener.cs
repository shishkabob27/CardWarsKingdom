using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public abstract class UITweener : MonoBehaviour
{
	public enum Method
	{
		Linear,
		EaseIn,
		EaseOut,
		EaseInOut,
		BounceIn,
		BounceOut
	}

	public enum Style
	{
		Once,
		Loop,
		PingPong
	}

	private const float MaxDelta = 1f / 15f;

	public static UITweener current;

	[HideInInspector]
	public UIWidget target;

	[HideInInspector]
	public Method method;

	[HideInInspector]
	public Style style;

	[HideInInspector]
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

	[HideInInspector]
	public bool ShouldIgnoreTimeScale;

	[HideInInspector]
	public float delay;

	[HideInInspector]
	public float duration = 1f;

	[HideInInspector]
	public bool steeperCurves;

	[HideInInspector]
	public int tweenGroup;

	[HideInInspector]
	public bool disableWhenFinished;

	[HideInInspector]
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	[HideInInspector]
	public GameObject eventReceiver;

	[HideInInspector]
	public string callWhenFinished;

	private bool mStarted;

	private float mStartTime;

	private float mDuration;

	private float mAmountPerDelta = 1000f;

	private float mFactor;

	private List<EventDelegate> mTemp;

	public float amountPerDelta
	{
		get
		{
			if (mDuration != duration)
			{
				mDuration = duration;
				mAmountPerDelta = Mathf.Abs((!(duration > 0f)) ? 1000f : (1f / duration)) * Mathf.Sign(mAmountPerDelta);
			}
			return mAmountPerDelta;
		}
	}

	public float tweenFactor
	{
		get
		{
			return mFactor;
		}
		set
		{
			mFactor = Mathf.Clamp01(value);
		}
	}

	public Direction direction
	{
		get
		{
			return (!(amountPerDelta < 0f)) ? Direction.Forward : Direction.Reverse;
		}
	}

	[ContextMenu("Clear Delegates")]
	public void ClearDelegates()
	{
		onFinished.Clear();
	}

	private void Reset()
	{
		if (!mStarted)
		{
			SetStartToCurrentValue();
			SetEndToCurrentValue();
		}
	}

	protected virtual void Start()
	{
		Update();
	}

	private void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		float num = ((!ShouldIgnoreTimeScale) ? Time.deltaTime : RealTime.deltaTime);
		float num2 = ((!ShouldIgnoreTimeScale) ? Time.time : RealTime.time);
		if (num > 1f / 15f)
		{
			num = 1f / 15f;
		}
		if (!mStarted)
		{
			mStarted = true;
			mStartTime = num2 + delay;
		}
		if (num2 < mStartTime)
		{
			return;
		}
		mFactor += amountPerDelta * num;
		if (style == Style.Loop)
		{
			if (mFactor > 1f)
			{
				mFactor -= Mathf.Floor(mFactor);
			}
		}
		else if (style == Style.PingPong)
		{
			if (mFactor > 1f)
			{
				mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
				mAmountPerDelta = 0f - mAmountPerDelta;
			}
			else if (mFactor < 0f)
			{
				mFactor = 0f - mFactor;
				mFactor -= Mathf.Floor(mFactor);
				mAmountPerDelta = 0f - mAmountPerDelta;
			}
		}
		if (style == Style.Once && (duration == 0f || mFactor > 1f || mFactor < 0f))
		{
			mFactor = Mathf.Clamp01(mFactor);
			Sample(mFactor, true);
			if (duration == 0f || (mFactor == 1f && mAmountPerDelta > 0f) || (mFactor == 0f && mAmountPerDelta < 0f))
			{
				base.enabled = false;
			}
			if (!(current == null))
			{
				return;
			}
			current = this;
			if (onFinished != null)
			{
				mTemp = onFinished;
				onFinished = new List<EventDelegate>();
				EventDelegate.Execute(mTemp);
				for (int i = 0; i < mTemp.Count; i++)
				{
					EventDelegate eventDelegate = mTemp[i];
					if (eventDelegate != null)
					{
						EventDelegate.Add(onFinished, eventDelegate, eventDelegate.oneShot);
					}
				}
				mTemp = null;
			}
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
			}
			current = null;
		}
		else
		{
			Sample(mFactor, false);
		}
	}

	public void SetOnFinished(EventDelegate.Callback del)
	{
		EventDelegate.Set(onFinished, del);
	}

	public void SetOnFinished(EventDelegate del)
	{
		EventDelegate.Set(onFinished, del);
	}

	public void AddOnFinished(EventDelegate.Callback del)
	{
		EventDelegate.Add(onFinished, del);
	}

	public void AddOnFinished(EventDelegate del)
	{
		EventDelegate.Add(onFinished, del);
	}

	public void RemoveOnFinished(EventDelegate del)
	{
		if (onFinished != null)
		{
			onFinished.Remove(del);
		}
		if (mTemp != null)
		{
			mTemp.Remove(del);
		}
	}

	private void OnDisable()
	{
		mStarted = false;
	}

	public void Sample(float factor, bool isFinished)
	{
		float num = Mathf.Clamp01(factor);
		if (method == Method.EaseIn)
		{
			num = 1f - Mathf.Sin((float)Math.PI / 2f * (1f - num));
			if (steeperCurves)
			{
				num *= num;
			}
		}
		else if (method == Method.EaseOut)
		{
			num = Mathf.Sin((float)Math.PI / 2f * num);
			if (steeperCurves)
			{
				num = 1f - num;
				num = 1f - num * num;
			}
		}
		else if (method == Method.EaseInOut)
		{
			num -= Mathf.Sin(num * ((float)Math.PI * 2f)) / ((float)Math.PI * 2f);
			if (steeperCurves)
			{
				num = num * 2f - 1f;
				float num2 = Mathf.Sign(num);
				num = 1f - Mathf.Abs(num);
				num = 1f - num * num;
				num = num2 * num * 0.5f + 0.5f;
			}
		}
		else if (method == Method.BounceIn)
		{
			num = BounceLogic(num);
		}
		else if (method == Method.BounceOut)
		{
			num = 1f - BounceLogic(1f - num);
		}
		OnUpdate((animationCurve == null) ? num : animationCurve.Evaluate(num), isFinished);
	}

	private float BounceLogic(float val)
	{
		val = ((val < 0.363636f) ? (7.5685f * val * val) : ((val < 0.727272f) ? (7.5625f * (val -= 0.545454f) * val + 0.75f) : ((!(val < 0.90909f)) ? (7.5625f * (val -= 0.9545454f) * val + 63f / 64f) : (7.5625f * (val -= 0.818181f) * val + 0.9375f))));
		return val;
	}

	[Obsolete("Use PlayForward() instead")]
	public void Play()
	{
		Play(true);
	}

	public void PlayForward()
	{
		Play(true);
	}

	public void PlayReverse()
	{
		Play(false);
	}

	public void Play(bool forward)
	{
		mAmountPerDelta = Mathf.Abs(amountPerDelta);
		if (!forward)
		{
			mAmountPerDelta = 0f - mAmountPerDelta;
		}
		base.enabled = true;
		Update();
	}

	public void ResetToBeginning()
	{
		mStarted = false;
		mFactor = ((!(amountPerDelta < 0f)) ? 0f : 1f);
		Sample(mFactor, false);
	}

	public void ResetToZero()
	{
		mStarted = false;
		mFactor = 0f;
		Sample(mFactor, false);
	}

	public void End()
	{
		if (mStarted && base.enabled)
		{
			mFactor = ((!(amountPerDelta < 0f)) ? 1f : 0f);
			mStartTime = ((!ShouldIgnoreTimeScale) ? Time.time : RealTime.time);
			Update();
		}
	}

	public void Toggle()
	{
		if (mFactor > 0f)
		{
			mAmountPerDelta = 0f - amountPerDelta;
		}
		else
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
		}
		base.enabled = true;
	}

	protected abstract void OnUpdate(float factor, bool isFinished);

	public static T Begin<T>(GameObject go, float duration) where T : UITweener
	{
		T val = go.GetComponent<T>();
		if ((UnityEngine.Object)val != (UnityEngine.Object)null && val.tweenGroup != 0)
		{
			val = (T)null;
			T[] components = go.GetComponents<T>();
			int i = 0;
			for (int num = components.Length; i < num; i++)
			{
				val = components[i];
				if ((UnityEngine.Object)val != (UnityEngine.Object)null && val.tweenGroup == 0)
				{
					break;
				}
				val = (T)null;
			}
		}
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<T>();
		}
		val.mStarted = false;
		val.duration = duration;
		val.mFactor = 0f;
		val.mAmountPerDelta = Mathf.Abs(val.amountPerDelta);
		val.style = Style.Once;
		val.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		val.eventReceiver = null;
		val.callWhenFinished = null;
		val.enabled = true;
		if (duration <= 0f)
		{
			val.Sample(1f, true);
			val.enabled = false;
		}
		return val;
	}

	public virtual void SetStartToCurrentValue()
	{
	}

	public virtual void SetEndToCurrentValue()
	{
	}

	public void DisableParentObject()
	{
		if (target != null)
		{
			target.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public virtual void RestorePrevious()
	{
	}
}
