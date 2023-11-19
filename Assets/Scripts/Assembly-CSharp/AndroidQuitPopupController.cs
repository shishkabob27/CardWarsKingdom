using UnityEngine;

public class AndroidQuitPopupController : Singleton<AndroidQuitPopupController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public Transform YesButton;

	public Transform NoButton;

	public bool Showing { get; private set; }

	private void OnEnable()
	{
		UICamera.AlwaysAllowedColliders.Add(YesButton.gameObject);
		UICamera.AlwaysAllowedColliders.Add(NoButton.gameObject);
	}

	private void OnDisable()
	{
		UICamera.AlwaysAllowedColliders.Remove(YesButton.gameObject);
		UICamera.AlwaysAllowedColliders.Remove(NoButton.gameObject);
	}

	public void Show()
	{
		Showing = true;
		ShowTween.Play();
	}

	public void OnClickYes()
	{
		Application.Quit();
	}

	public void OnClickNo()
	{
		Showing = false;
		HideTween.Play();
	}
}
