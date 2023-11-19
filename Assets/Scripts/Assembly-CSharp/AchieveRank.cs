public class AchieveRank : Mission
{
	public override int ProgressValue
	{
		get
		{
			return Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		}
	}
}
