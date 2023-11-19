using UnityEngine;

public class BattleHistoryItemController : UIStreamingGridListItem
{
	public UILabel opponentNameLabel;
	public UILabel opponentMatchSeason;
	public UILabel opponentMatchResult;
	public UILabel opponentMatchDate;
	public GameObject AddFriendButton;
	public GameObject ViewReplayButton;
	public LeagueBadge OpponentBadge;
	public UITexture opponentFlag;
	public Transform[] opponentCreatureNodes;
	[SerializeField]
	private Color _DarkGreen;
	[SerializeField]
	private Color _DarkRed;
}
