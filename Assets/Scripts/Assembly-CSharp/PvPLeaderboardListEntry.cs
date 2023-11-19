using Multiplayer;

public class PvPLeaderboardListEntry : UIStreamingGridListItem
{
	public UILabel Name;

	public UILabel Placement;

	public UILabel Score;

	public override void Populate(object dataObj)
	{
		LeaderboardData leaderboardData = dataObj as LeaderboardData;
		Name.text = leaderboardData.name;
		Placement.text = leaderboardData.rank + ".";
		Score.text = leaderboardData.trophies.ToString();
	}
}
