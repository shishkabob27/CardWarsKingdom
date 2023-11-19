using UnityEngine;

public class QuestSelectLeague : UIStreamingGridListItem
{
	public UITweenController MoveTween;
	public UITweenController MoveAlphaTween;
	public UITweenController CompleteTween;
	public UITweenController UnlockTween;
	public UITweenController ResetTween;
	public UITweenController FlashBonus;
	public UILabel Name;
	public UILabel Battles;
	public UITexture Background;
	public UITexture BossPortrait;
	public GameObject Lock;
	public GameObject HideWhenLocked;
	public UILabel TimeLabel;
	public GameObject UnavailableOverlay;
	public UIButton Button;
	public UILabel BonusText;
	public GameObject LeagueInfoParent;
	public Vector3 StoredPosition;
	public string EnterText;
	public string ExitText;
	public UILabel EnterExitLabel;
	public float DelayToShowUnlockedElements;
	[SerializeField]
	private Color[] _TitleOutlineColors;
	[SerializeField]
	private Color[] _BonusOutlineColors;
}
