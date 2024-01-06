using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomDungeonRewardDataManager : DataManager<RandomDungeonRewardData>
{
	private int mTotalWeight;

	private static RandomDungeonRewardDataManager _instance;

	public static RandomDungeonRewardDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_RandomDungeonRewards.json");
				_instance = new RandomDungeonRewardDataManager(path);
			}
			return _instance;
		}
	}

	public RandomDungeonRewardDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void PostLoad()
	{
		mTotalWeight = 0;
		foreach (RandomDungeonRewardData item in DatabaseArray)
		{
			if (item.Weight > 0)
			{
				mTotalWeight += item.Weight;
			}
		}
	}

	public List<RandomDungeonRewardData> GetRandomRewards(int count)
	{
		List<RandomDungeonRewardData> results = new List<RandomDungeonRewardData>();
		List<RandomDungeonRewardData> list = new List<RandomDungeonRewardData>(DatabaseArray);
		list.RemoveAll(delegate(RandomDungeonRewardData data)
		{
			if (data.Weight < 0)
			{
				results.Add(data);
				count--;
				return true;
			}
			return false;
		});
		int num = mTotalWeight;
		while (count > 0 && list.Count != 0 && num > 0)
		{
			int num2 = Random.Range(0, num);
			foreach (RandomDungeonRewardData item in list)
			{
				num2 -= item.Weight;
				if (num2 < 0)
				{
					results.Add(item);
					list.Remove(item);
					count--;
					break;
				}
			}
		}
		results.Shuffle();
		return results;
	}
}
