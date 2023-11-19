using System.Collections.Generic;

public class PVPTier
{
	public int StartIndex;

	public bool IsPlayersPVPLevel;

	public List<PvpRankData> PvpRanks;

	public PVPTier(int inStartIndex, PvpRankData inPvpRankData, bool inIsPlayersPVPLevel)
	{
		StartIndex = inStartIndex;
		IsPlayersPVPLevel = inIsPlayersPVPLevel;
		PvpRanks = new List<PvpRankData>();
		PvpRanks.Add(inPvpRankData);
	}
}
