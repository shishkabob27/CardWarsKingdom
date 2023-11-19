using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
public class UISlider : UIProgressBar
{
	private enum Direction
	{
		Horizontal,
		Vertical,
		Upgraded
	}

	[SerializeField]
	[HideInInspector]
	private Transform foreground;

	[SerializeField]
	[HideInInspector]
	private float rawValue = 1f;

	[HideInInspector]
	[SerializeField]
	private Direction direction = Direction.Upgraded;

	[SerializeField]
	[HideInInspector]
	protected bool mInverted;

	[Obsolete("Use 'value' instead")]
	public float sliderValue
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
		}
	}

	[Obsolete("Use 'fillDirection' instead")]
	public bool inverted
	{
		get
		{
			return base.isInverted;
		}
		set
		{
		}
	}

	protected override void Upgrade()
	{
		if (direction != Direction.Upgraded)
		{
			mValue = rawValue;
			if (foreground != null)
			{
				mFG = foreground.GetComponent<UIWidget>();
			}
			if (direction == Direction.Horizontal)
			{
				mFill = (mInverted ? FillDirection.RightToLeft : FillDirection.LeftToRight);
			}
			else
			{
				mFill = ((!mInverted) ? FillDirection.BottomToTop : FillDirection.TopToBottom);
			}
			direction = Direction.Upgraded;
		}
	}

	protected override void OnStart()
	{
		GameObject go = ((!(mBG != null) || (!(mBG.GetComponent<Collider>() != null) && !(mBG.GetComponent<Collider2D>() != null))) ? base.gameObject : mBG.gameObject);
		UIEventListener uIEventListener = UIEventListener.Get(go);
		uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
		uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
		if (thumb != null && (thumb.GetComponent<Collider>() != null || thumb.GetComponent<Collider2D>() != null) && (mFG == null || thumb != mFG.cachedTransform))
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(thumb.gameObject);
			uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
			uIEventListener2.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener2.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
		}
	}

	protected void OnPressBackground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
			if (!isPressed && onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragBackground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnPressForeground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			if (isPressed)
			{
				mOffset = ((!(mFG == null)) ? (base.value - ScreenToValue(UICamera.lastTouchPosition)) : 0f);
			}
			else if (onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragForeground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = mOffset + ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnKey(KeyCode key)
	{
		if (!base.enabled)
		{
			return;
		}
		float num = ((!((float)numberOfSteps > 1f)) ? 0.125f : (1f / (float)(numberOfSteps - 1)));
		if (base.fillDirection == FillDirection.LeftToRight || base.fillDirection == FillDirection.RightToLeft)
		{
			switch (key)
			{
			case KeyCode.LeftArrow:
				base.value = mValue - num;
				break;
			case KeyCode.RightArrow:
				base.value = mValue + num;
				break;
			}
		}
		else
		{
			switch (key)
			{
			case KeyCode.DownArrow:
				base.value = mValue - num;
				break;
			case KeyCode.UpArrow:
				base.value = mValue + num;
				break;
			}
		}
	}
}
