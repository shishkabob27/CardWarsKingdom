using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Toggle")]
public class UIToggle : UIWidgetContainer
{
	public delegate bool Validate(bool choice);

	public static BetterList<UIToggle> list = new BetterList<UIToggle>();

	public static UIToggle current;

	public int group;

	public UIWidget activeSprite;

	public Animation activeAnimation;

	public bool startsActive;

	public int toggleId;

	public bool instantTween;

	public bool optionCanBeNone;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public List<EventDelegate> onSet = new List<EventDelegate>();

	public Validate validator;

	[SerializeField]
	[HideInInspector]
	private UISprite checkSprite;

	[HideInInspector]
	[SerializeField]
	private Animation checkAnimation;

	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	[SerializeField]
	[HideInInspector]
	private string functionName = "OnActivate";

	[SerializeField]
	[HideInInspector]
	private bool startsChecked;

	private bool mIsActive = true;

	private bool mStarted;

	public bool value
	{
		get
		{
			return (!mStarted) ? startsActive : mIsActive;
		}
		set
		{
			if (!mStarted)
			{
				startsActive = value;
			}
			else if (group == 0 || value || optionCanBeNone || !mStarted)
			{
				Set(value);
			}
		}
	}

	[Obsolete("Use 'value' instead")]
	public bool isChecked
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

	public static UIToggle GetActiveToggle(int group)
	{
		for (int i = 0; i < list.size; i++)
		{
			UIToggle uIToggle = list[i];
			if (uIToggle != null && uIToggle.group == group && uIToggle.isChecked)
			{
				return uIToggle;
			}
		}
		return null;
	}

	private void OnEnable()
	{
		list.Add(this);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void Start()
	{
		if (startsChecked)
		{
			startsChecked = false;
			startsActive = true;
		}
		if (!Application.isPlaying)
		{
			if (checkSprite != null && activeSprite == null)
			{
				activeSprite = checkSprite;
				checkSprite = null;
			}
			if (checkAnimation != null && activeAnimation == null)
			{
				activeAnimation = checkAnimation;
				checkAnimation = null;
			}
			if (Application.isPlaying && activeSprite != null)
			{
				activeSprite.alpha = ((!startsActive) ? 0f : 1f);
			}
			if (EventDelegate.IsValid(onChange))
			{
				eventReceiver = null;
				functionName = null;
			}
		}
		else
		{
			mIsActive = !startsActive;
			mStarted = true;
			bool flag = instantTween;
			instantTween = true;
			Set(startsActive, true);
			instantTween = flag;
		}
	}

	private void OnClick()
	{
		if (base.enabled)
		{
			value = !value;
		}
	}

	public void Set(bool state, bool initial = false)
	{
		if (validator != null && !validator(state))
		{
			return;
		}
		if (!mStarted)
		{
			mIsActive = state;
			startsActive = state;
			if (activeSprite != null)
			{
				activeSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else
		{
			if (mIsActive == state)
			{
				return;
			}
			if (group != 0 && state)
			{
				int num = 0;
				int size = list.size;
				while (num < size)
				{
					UIToggle uIToggle = list[num];
					if (uIToggle != this && uIToggle.group == group)
					{
						uIToggle.Set(false);
					}
					if (list.size != size)
					{
						size = list.size;
						num = 0;
					}
					else
					{
						num++;
					}
				}
			}
			mIsActive = state;
			if (activeSprite != null)
			{
				if (instantTween || !NGUITools.GetActive(this))
				{
					activeSprite.alpha = ((!mIsActive) ? 0f : 1f);
				}
				else
				{
					TweenAlpha.Begin(activeSprite.gameObject, 0.15f, (!mIsActive) ? 0f : 1f);
				}
			}
			if (!initial && current == null)
			{
				UIToggle uIToggle2 = current;
				current = this;
				if (EventDelegate.IsValid(onChange))
				{
					EventDelegate.Execute(onChange);
				}
				else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
				{
					eventReceiver.SendMessage(functionName, mIsActive, SendMessageOptions.DontRequireReceiver);
				}
				if (state && EventDelegate.IsValid(onSet))
				{
					EventDelegate.Execute(onSet);
				}
				current = uIToggle2;
			}
			if (this.activeAnimation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(this.activeAnimation, null, state ? Direction.Forward : Direction.Reverse, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
				if (activeAnimation != null && (instantTween || !NGUITools.GetActive(this)))
				{
					activeAnimation.Finish();
				}
			}
		}
	}
}
