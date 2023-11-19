using System.Collections.Generic;

public class PvpMatchResultDetails
{
	public bool Won;

	public int StartingLevel;

	public int StartingPointsInLevel;

	public int EndingLevel;

	public int EndingPointsInLevel;

	public bool FirstMatchInSeason;

	public List<GeneralReward> Rewards;

	public List<UnlockableData> GrantedUnlocks;

	public List<GeneralReward> WinRewards;
}
