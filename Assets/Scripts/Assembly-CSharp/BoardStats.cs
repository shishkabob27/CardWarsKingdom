public struct BoardStats
{
	public int mCreatureCount;

	public int mDeadCreatureCount;

	public int[] mFactionCount;

	public int[] mTypeCount;

	public int mCardsPlayed;

	public int mHeroCardsPlayed;

	public int mCardsDrawn;

	public int mHeroCardsDrawn;

	public int mCreaturesKilledThisTurn;

	public int mCreaturesDeployedThisTurn;

	public void Reset()
	{
		mCreatureCount = 0;
		mDeadCreatureCount = 0;
		for (int i = 0; i < mFactionCount.Length; i++)
		{
			mFactionCount[i] = 0;
		}
		for (int j = 0; j < mTypeCount.Length; j++)
		{
			mTypeCount[j] = 0;
		}
	}
}
