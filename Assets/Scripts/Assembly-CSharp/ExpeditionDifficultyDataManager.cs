using System.IO;
using UnityEngine;

public class ExpeditionDifficultyDataManager : DataManager<ExpeditionDifficultyData>
{
	private static ExpeditionDifficultyDataManager _instance;

	private float mTotalChance;

	public static ExpeditionDifficultyDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_ExpeditionDifficulty.json");
				_instance = new ExpeditionDifficultyDataManager(path);
			}
			return _instance;
		}
	}

	public ExpeditionDifficultyDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void PostLoad()
	{
		mTotalChance = 0f;
		foreach (ExpeditionDifficultyData item in DatabaseArray)
		{
			mTotalChance += item.Chance;
		}
	}

	public ExpeditionDifficultyData GetDataByDifficulty(int difficulty)
	{
		return DatabaseArray[difficulty - 1];
	}

	public ExpeditionDifficultyData RollDifficulty()
	{
		if (mTotalChance == 0f)
		{
			return null;
		}
		float num;
		for (num = mTotalChance; num == mTotalChance; num = Random.Range(0f, mTotalChance))
		{
		}
		foreach (ExpeditionDifficultyData item in DatabaseArray)
		{
			num -= item.Chance;
			if (num < 0f)
			{
				return item;
			}
		}
		return null;
	}

	public int GetTierFromStars(int stars)
	{
		for (int i = 0; i < DatabaseArray.Count; i++)
		{
			if (stars < DatabaseArray[i].StarRequirement)
			{
				return i;
			}
		}
		return DatabaseArray.Count;
	}
}
