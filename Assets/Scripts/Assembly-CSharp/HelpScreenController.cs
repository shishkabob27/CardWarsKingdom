using UnityEngine;

public class HelpScreenController : Singleton<HelpScreenController>
{
	public GameObject HelpEntryPrefab;
	public UITweenController HelpPopupTween;
	public UIStreamingGrid HelpList;
	public UILabel HelpTitle;
	public UILabel HelpBody;
}
