using UnityEngine;

public class BusyIconPanelController : Singleton<BusyIconPanelController>
{
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public GameObject busyIconPanel;
	public Camera busyIconCamera;
}
