using UnityEngine;

public class BattleHistoryController : MonoBehaviour
{
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UILabel playerNameLabel;
	public UILabel NoMatchesLabel;
	public UIStreamingGrid currentSeasonGrid;
	public UIStreamingGrid previousSeasonsGrid;
	public GameObject historyEntryTemplate;
	public UIToggle globalToggle;
	public LeagueBadge PlayerBadge;
	public UITexture playerFlag;
}
