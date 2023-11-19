using System.Collections.Generic;

public class DailyRandomDungeonManager : DetachedSingleton<DailyRandomDungeonManager>
{
	public void BuildDailyRandomBattles()
	{
		RandomDungeonFloorData currentData = RandomDungeonFloorDataManager.Instance.GetCurrentData();
		LeagueData data = LeagueDataManager.Instance.GetData("DailyRandom");
		if (data == null)
		{
			return;
		}
		data.Quests.Clear();
		List<RandomDungeonRewardData> randomRewards = RandomDungeonRewardDataManager.Instance.GetRandomRewards(currentData.Paths);
		int num = 0;
		foreach (RandomDungeonRewardData item in randomRewards)
		{
			QuestData questData = new QuestData();
			questData.CreateRandomQuest(0, item, num);
			data.Quests.Add(questData);
			num++;
		}
	}
}
