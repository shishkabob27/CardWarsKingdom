using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Animation")]
public class UIPlayAnimation : MonoBehaviour
{
	public static UIPlayAnimation current;

	public Animation target;

	public Animator animator;

	public string clipName;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public bool resetOnPlay;

	public bool clearSelection;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string callWhenFinished;

	private bool mStarted;

	private bool mActivated;

	private bool dragHighlight;

	private bool dualState
	{
		get
		{
			return trigger == Trigger.OnPress || trigger == Trigger.OnHover;
		}
	}

	private void Awake()
	{
		UIButton component = GetComponent<UIButton>();
		if (component != null)
		{
			dragHighlight = component.dragHighlight;
		}
		if (eventReceiver != null && EventDelegate.IsValid(onFinished))
		{
			eventReceiver = null;
			callWhenFinished = null;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (target == null && animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}
		if (animator != null)
		{
			if (animator.enabled)
			{
				animator.enabled = false;
			}
			return;
		}
		if (target == null)
		{
			target = GetComponentInChildren<Animation>();
		}
		if (target != null && target.enabled)
		{
			target.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (trigger == Trigger.OnPress || trigger == Trigger.OnPressTrue)
			{
				mActivated = UICamera.currentTouch.pressed == base.gameObject;
			}
			if (trigger == Trigger.OnHover || trigger == Trigger.OnHoverTrue)
			{
				mActivated = UICamera.currentTouch.current == base.gameObject;
			}
		}
		UIToggle component = GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Add(component.onChange, OnToggle);
		}
	}

	private void OnDisable()
	{
		UIToggle component = GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, OnToggle);
		}
	}

	private void OnHover(bool isOver)
	{
		if (base.enabled && (trigger == Trigger.OnHover || (trigger == Trigger.OnHoverTrue && isOver) || (trigger == Trigger.OnHoverFalse && !isOver)))
		{
			Play(isOver, dualState);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled && (trigger == Trigger.OnPress || (trigger == Trigger.OnPressTrue && isPressed) || (trigger == Trigger.OnPressFalse && !isPressed)))
		{
			Play(isPressed, dualState);
		}
	}

	private void OnClick()
	{
		if (base.enabled && trigger == Trigger.OnClick)
		{
			Play(true, false);
		}
	}

	private void OnDoubleClick()
	{
		if (base.enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true, false);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (trigger == Trigger.OnSelect || (trigger == Trigger.OnSelectTrue && isSelected) || (trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			Play(isSelected, dualState);
		}
	}

	private void OnToggle()
	{
		if (base.enabled && !(UIToggle.current == null) && (trigger == Trigger.OnActivate || (trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (trigger == Trigger.OnActivateFalse && !UIToggle.current.value)))
		{
			Play(UIToggle.current.value, dualState);
		}
	}

	private void OnDragOver()
	{
		if (base.enabled && dualState)
		{
			if (UICamera.currentTouch.dragged == base.gameObject)
			{
				Play(true, true);
			}
			else if (dragHighlight && trigger == Trigger.OnPress)
			{
				Play(true, true);
			}
		}
	}

	private void OnDragOut()
	{
		if (base.enabled && dualState && UICamera.hoveredObject != base.gameObject)
		{
			Play(false, true);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (base.enabled && trigger == Trigger.OnPress && UICamera.currentTouch.dragged != base.gameObject)
		{
			Play(false, true);
		}
	}

	public void Play(bool forward)
	{
		Play(forward, true);
	}

	public void Play(bool forward, bool onlyIfDifferent)
	{
		if (!target && !animator)
		{
			return;
		}
		if (onlyIfDifferent)
		{
			if (mActivated == forward)
			{
				return;
			}
			mActivated = forward;
		}
		if (clearSelection && UICamera.selectedObject == base.gameObject)
		{
			UICamera.selectedObject = null;
		}
		int num = 0 - playDirection;
		Direction direction = ((!forward) ? ((Direction)num) : playDirection);
		ActiveAnimation activeAnimation = ((!target) ? ActiveAnimation.Play(animator, clipName, direction, ifDisabledOnPlay, disableWhenFinished) : ActiveAnimation.Play(target, clipName, direction, ifDisabledOnPlay, disableWhenFinished));
		if (activeAnimation != null)
		{
			if (resetOnPlay)
			{
				activeAnimation.Reset();
			}
			for (int i = 0; i < onFinished.Count; i++)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
		}
	}

	private void OnFinished()
	{
		if (current == null)
		{
			current = this;
			EventDelegate.Execute(onFinished);
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			eventReceiver = null;
			current = null;
		}
	}
}
