using UnityEngine;

public class LeagueBadge : MonoBehaviour
{
	public UITexture PortraitTexture;
	public UITexture CurrentLeagueTexture;
	public UILabel NameLabel;
	public UILabel RankNumberLabel;
	public UILabel RankNameLabel;
	public UITexture FlagTexture;
	[SerializeField]
	private Color[] _RankLabelColors;
	[SerializeField]
	private Color[] _RankLabelOutlineColors;
	[SerializeField]
	private Color[] _RankLabelShadowColors;
	[SerializeField]
	private Color[] _RankLabelShadowOutlineColors;
}
