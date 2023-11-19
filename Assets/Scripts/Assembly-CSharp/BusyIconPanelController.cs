using UnityEngine;

public class BusyIconPanelController : Singleton<BusyIconPanelController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public GameObject busyIconPanel;

	public Camera busyIconCamera;

	public void Show()
	{
		UICamera.LockInput();
		busyIconCamera.enabled = true;
		busyIconPanel.SetActive(true);
	}

	public void Hide()
	{
		UICamera.UnlockInput();
		busyIconCamera.enabled = false;
		busyIconPanel.SetActive(false);
	}
}
