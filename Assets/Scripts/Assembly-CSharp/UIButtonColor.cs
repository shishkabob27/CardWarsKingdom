using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : UIWidgetContainer
{
	public enum State
	{
		Normal,
		Hover,
		Pressed,
		Disabled
	}

	public GameObject tweenTarget;

	public Color hover = new Color(0.88235295f, 40f / 51f, 0.5882353f, 1f);

	public Color pressed = new Color(61f / 85f, 0.6392157f, 41f / 85f, 1f);

	public Color disabledColor = Color.grey;

	public float duration = 0.2f;

	[NonSerialized]
	protected Color mStartingColor;

	[NonSerialized]
	protected Color mDefaultColor;

	[NonSerialized]
	protected bool mInitDone;

	[NonSerialized]
	protected UIWidget mWidget;

	[NonSerialized]
	protected State mState;

	public State state
	{
		get
		{
			return mState;
		}
		set
		{
			SetState(value, false);
		}
	}

	public Color defaultColor
	{
		get
		{
			if (!mInitDone)
			{
				OnInit();
			}
			return mDefaultColor;
		}
		set
		{
			if (!mInitDone)
			{
				OnInit();
			}
			mDefaultColor = value;
			State state = mState;
			mState = State.Disabled;
			SetState(state, false);
		}
	}

	public virtual bool isEnabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public void ResetDefaultColor()
	{
		defaultColor = mStartingColor;
	}

	private void Awake()
	{
		if (!mInitDone)
		{
			OnInit();
		}
	}

	private void Start()
	{
		if (!isEnabled)
		{
			SetState(State.Disabled, true);
		}
	}

	protected virtual void OnInit()
	{
		mInitDone = true;
		if (tweenTarget == null)
		{
			tweenTarget = base.gameObject;
		}
		mWidget = tweenTarget.GetComponent<UIWidget>();
		if (mWidget != null)
		{
			mDefaultColor = mWidget.color;
			mStartingColor = mDefaultColor;
			return;
		}
		Renderer component = tweenTarget.GetComponent<Renderer>();
		if (component != null)
		{
			mDefaultColor = ((!Application.isPlaying) ? component.sharedMaterial.color : component.material.color);
			mStartingColor = mDefaultColor;
			return;
		}
		Light component2 = tweenTarget.GetComponent<Light>();
		if (component2 != null)
		{
			mDefaultColor = component2.color;
			mStartingColor = mDefaultColor;
		}
		else
		{
			tweenTarget = null;
			mInitDone = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (mInitDone)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (UICamera.currentTouch.pressed == base.gameObject)
			{
				OnPress(true);
			}
			else if (UICamera.currentTouch.current == base.gameObject)
			{
				OnHover(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (mInitDone && tweenTarget != null)
		{
			SetState(State.Normal, true);
			TweenColor component = tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.value = mDefaultColor;
				component.enabled = false;
			}
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			if (!mInitDone)
			{
				OnInit();
			}
			if (tweenTarget != null)
			{
				SetState(isOver ? State.Hover : State.Normal, false);
			}
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (!isEnabled || UICamera.currentTouch == null)
		{
			return;
		}
		if (!mInitDone)
		{
			OnInit();
		}
		if (!(tweenTarget != null))
		{
			return;
		}
		if (isPressed)
		{
			SetState(State.Pressed, false);
		}
		else if (UICamera.currentTouch.current == base.gameObject)
		{
			if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				SetState(State.Hover, false);
			}
			else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == base.gameObject)
			{
				SetState(State.Hover, false);
			}
			else
			{
				SetState(State.Normal, false);
			}
		}
		else
		{
			SetState(State.Normal, false);
		}
	}

	protected virtual void OnDragOver()
	{
		if (isEnabled)
		{
			if (!mInitDone)
			{
				OnInit();
			}
			if (tweenTarget != null)
			{
				SetState(State.Pressed, false);
			}
		}
	}

	protected virtual void OnDragOut()
	{
		if (isEnabled)
		{
			if (!mInitDone)
			{
				OnInit();
			}
			if (tweenTarget != null)
			{
				SetState(State.Normal, false);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (isEnabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller) && tweenTarget != null)
		{
			OnHover(isSelected);
		}
	}

	public virtual void SetState(State state, bool instant)
	{
		if (!mInitDone)
		{
			mInitDone = true;
			OnInit();
		}
		if (mState != state)
		{
			mState = state;
			UpdateColor(instant);
		}
	}

	public void UpdateColor(bool instant)
	{
		TweenColor tweenColor;
		switch (mState)
		{
		case State.Hover:
			tweenColor = TweenColor.Begin(tweenTarget, duration, hover);
			break;
		case State.Pressed:
			tweenColor = TweenColor.Begin(tweenTarget, duration, pressed);
			break;
		case State.Disabled:
			tweenColor = TweenColor.Begin(tweenTarget, duration, disabledColor);
			break;
		default:
			tweenColor = TweenColor.Begin(tweenTarget, duration, mDefaultColor);
			break;
		}
		if (instant && tweenColor != null)
		{
			tweenColor.value = tweenColor.to;
			tweenColor.enabled = false;
		}
	}
}
