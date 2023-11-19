using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenController : MonoBehaviour
{
	public bool DisableInput;

	public bool ResetOnPlay;

	public bool DisableWhenFinished;

	public bool CallCleanupTweensOnPlay;

	public UITweenController CleanupTween;

	public UITweenController TweenToCancel;

	public Collider MenuStackCloseButton;

	[HideInInspector]
	public List<EventDelegate> OnFinished = new List<EventDelegate>();

	private static List<UITweenController> mCurrentCleanupTweens = new List<UITweenController>();

	private static bool mCleaningUp = false;

	private UITweener[] mTweens;

	private UIButtonActivate[] mActivates;

	private UISpawnFXOnClick[] mVFXSpawns;

	private UIPlayAnimation[] mPlayAnims;

	private UIPlaySound[] mSounds;

	private UITweener mLongestTween;

	private EventDelegate.Callback mScriptSetCallback;

	private void Awake()
	{
		mTweens = base.gameObject.GetComponents<UITweener>();
		mActivates = base.gameObject.GetComponents<UIButtonActivate>();
		mVFXSpawns = base.gameObject.GetComponents<UISpawnFXOnClick>();
		mSounds = base.gameObject.GetComponents<UIPlaySound>();
		mPlayAnims = base.gameObject.GetComponents<UIPlayAnimation>();
		UITweener[] array = mTweens;
		foreach (UITweener uITweener in array)
		{
			if (uITweener.target == null)
			{
				continue;
			}
			if (uITweener.style == UITweener.Style.Once)
			{
				if (mLongestTween == null || uITweener.delay + uITweener.duration > mLongestTween.delay + mLongestTween.duration)
				{
					mLongestTween = uITweener;
				}
				if (DisableWhenFinished || uITweener.disableWhenFinished)
				{
					uITweener.AddOnFinished(uITweener.DisableParentObject);
				}
			}
			uITweener.enabled = false;
		}
		if (mLongestTween != null)
		{
			mLongestTween.AddOnFinished(OnLongestTweenFinished);
			return;
		}
		if (DisableInput)
		{
		}
		if (OnFinished.Count <= 0)
		{
		}
	}

	public void Play()
	{
		PlayWithCallback(null);
	}

	public void PlayReverse()
	{
		PlayWithCallback(null, true);
	}

	public void PlayWithCallback(EventDelegate.Callback onFinished, bool reverse = false)
	{
		End();
		mScriptSetCallback = onFinished;
		if (mScriptSetCallback == null || mLongestTween == null)
		{
		}
		if (DisableInput && mLongestTween != null)
		{
			UICamera.LockInput();
		}
		if (TweenToCancel != null)
		{
			TweenToCancel.End();
		}
		if (CallCleanupTweensOnPlay)
		{
			CallCleanupTweens();
		}
		UIButtonActivate[] array = mActivates;
		foreach (UIButtonActivate uIButtonActivate in array)
		{
			uIButtonActivate.Trigger();
		}
		UISpawnFXOnClick[] array2 = mVFXSpawns;
		foreach (UISpawnFXOnClick uISpawnFXOnClick in array2)
		{
			uISpawnFXOnClick.Trigger();
		}
		UIPlaySound[] array3 = mSounds;
		foreach (UIPlaySound uIPlaySound in array3)
		{
			uIPlaySound.Play();
		}
		UITweener[] array4 = mTweens;
		foreach (UITweener uITweener in array4)
		{
			NGUITools.SetActive(uITweener.target.gameObject, true);
			if (ResetOnPlay)
			{
				uITweener.ResetToZero();
			}
			if (reverse)
			{
				uITweener.PlayReverse();
			}
			else
			{
				uITweener.PlayForward();
			}
		}
		UIPlayAnimation[] array5 = mPlayAnims;
		foreach (UIPlayAnimation uIPlayAnimation in array5)
		{
			Animation target = uIPlayAnimation.target;
			uIPlayAnimation.target.gameObject.SetActive(true);
			ActiveAnimation.Play(target, uIPlayAnimation.playDirection);
		}
		if (MenuStackCloseButton != null)
		{
			MenuStackManager.RegisterUIPanel(MenuStackCloseButton);
		}
		if (!mCleaningUp)
		{
			mCurrentCleanupTweens.Remove(this);
			if (CleanupTween != null)
			{
				mCurrentCleanupTweens.Add(CleanupTween);
			}
		}
	}

	public IEnumerator PlayAsCoroutine()
	{
		bool waiting = true;
		PlayWithCallback(delegate
		{
			waiting = false;
		});
		while (waiting)
		{
			yield return null;
		}
	}

	private void OnLongestTweenFinished()
	{
		if (DisableInput)
		{
			UICamera.UnlockInput();
		}
		if (OnFinished.Count > 0)
		{
			EventDelegate.Execute(OnFinished);
		}
		if (mScriptSetCallback != null)
		{
			mScriptSetCallback();
		}
	}

	public void End()
	{
		UITweener[] array = mTweens;
		foreach (UITweener uITweener in array)
		{
			uITweener.End();
			uITweener.enabled = false;
		}
		UISpawnFXOnClick[] array2 = mVFXSpawns;
		foreach (UISpawnFXOnClick uISpawnFXOnClick in array2)
		{
			uISpawnFXOnClick.CleanUp();
		}
	}

	public static void CallCleanupTweens()
	{
		mCleaningUp = true;
		foreach (UITweenController mCurrentCleanupTween in mCurrentCleanupTweens)
		{
			if (mCurrentCleanupTween != null)
			{
				mCurrentCleanupTween.Play();
			}
		}
		mCurrentCleanupTweens.Clear();
		mCleaningUp = false;
	}

	public void StopAndReset()
	{
		UITweener[] array = mTweens;
		foreach (UITweener uITweener in array)
		{
			uITweener.End();
			uITweener.RestorePrevious();
			uITweener.enabled = false;
		}
		UISpawnFXOnClick[] array2 = mVFXSpawns;
		foreach (UISpawnFXOnClick uISpawnFXOnClick in array2)
		{
			uISpawnFXOnClick.CleanUp();
		}
	}

	public bool AnyTweenPlaying()
	{
		UITweener[] array = mTweens;
		foreach (UITweener uITweener in array)
		{
			if (uITweener.enabled)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDisable()
	{
	}

	public float LongestTweenDuration()
	{
		if (mLongestTween != null)
		{
			return mLongestTween.duration + mLongestTween.delay;
		}
		return 0f;
	}

	public void ReattachBackButtonTarget()
	{
		if (MenuStackCloseButton != null)
		{
			MenuStackManager.RegisterUIPanel(MenuStackCloseButton);
		}
	}
}
