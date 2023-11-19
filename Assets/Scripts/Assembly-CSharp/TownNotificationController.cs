using UnityEngine;

public class TownNotificationController : Singleton<TownNotificationController>
{
	public float InitialTime;
	public float TimeBetweenNotifications;
	public float NotificationTypeCooldown;
	public float BuildingYOffset;
	public float PopupXOffset;
	public UITweenController ShowPopup;
	public UILabel PopupText;
	public Transform PopupObject;
	public GameObject LeftPointingGroup;
	public GameObject RightPointingGroup;
	public UIWidget ContentsParent;
}
