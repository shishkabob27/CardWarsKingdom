using System;
using UnityEngine;

public class LoginController : MonoBehaviour
{
	public PlayerInfoScript PlayerInfo;

	public UITweenController ShowLoginScreenTween;

	public UITweenController HideLoginScreenTween;

	public event Action OnGuestLogin;

	public event Action OnFBLogin;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ActivateLoginDialog()
	{
		ShowLoginScreenTween.Play();
	}

	public void HideLoginDialog()
	{
		HideLoginScreenTween.Play();
	}

	public void GuestLoginSelected()
	{
		if (this.OnGuestLogin != null)
		{
			this.OnGuestLogin();
			HideLoginScreenTween.Play();
		}
	}

	public void FBLoginSelected()
	{
		if (this.OnFBLogin != null)
		{
			this.OnFBLogin();
			HideLoginScreenTween.Play();
		}
	}
}
