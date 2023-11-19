using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom
	}

	public AudioClip audioClip;

	public Trigger trigger;

	public float delay;

	private bool mIsOver;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 2f)]
	public float pitch = 1f;

	private bool mDelayStart;

	private float mDelayTimer;

	private float timer;

	private bool canPlay
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			UIButton component = GetComponent<UIButton>();
			return component == null || component.isEnabled;
		}
	}

	private void OnHover(bool isOver)
	{
		if (trigger == Trigger.OnMouseOver)
		{
			if (mIsOver == isOver)
			{
				return;
			}
			mIsOver = isOver;
		}
		if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			Play();
		}
	}

	private void OnPress(bool isPressed)
	{
		if (trigger == Trigger.OnPress)
		{
			if (mIsOver == isPressed)
			{
				return;
			}
			mIsOver = isPressed;
		}
		if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			Play();
		}
	}

	private void OnClick()
	{
		if (canPlay && trigger == Trigger.OnClick)
		{
			Play();
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			OnHover(isSelected);
		}
	}

	public void Play()
	{
		if (delay > 0f)
		{
			mDelayStart = true;
		}
		else
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	private void Update()
	{
		if (mDelayStart)
		{
			timer += Time.deltaTime;
			if (timer >= delay)
			{
				NGUITools.PlaySound(audioClip, volume, pitch);
				timer = 0f;
				mDelayStart = false;
			}
		}
	}
}
