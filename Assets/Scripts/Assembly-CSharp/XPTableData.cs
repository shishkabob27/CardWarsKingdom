using System.Collections.Generic;

public class XPTableData : ILoadableData
{
	private string _ID;

	private List<int> _XPToReachLevel = new List<int>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public XPTableData(string tableId)
	{
		_ID = tableId;
	}

	public int GetXpToReachLevel(int level)
	{
		if (level < 1 || level > _XPToReachLevel.Count)
		{
			return -1;
		}
		return _XPToReachLevel[level - 1];
	}

	public void AddLevel(int xpToReach)
	{
		_XPToReachLevel.Add(xpToReach);
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}

	public int GetCurrentLevel(int currentXP, int levelCap)
	{
		if (currentXP < 0)
		{
			return 1;
		}
		for (int i = 1; i <= levelCap; i++)
		{
			if (i > _XPToReachLevel.Count || currentXP < GetXpToReachLevel(i))
			{
				return i - 1;
			}
		}
		return levelCap;
	}

	public XPLevelData GetLevelData(int currentXP, int levelCap = -1)
	{
		if (levelCap == -1)
		{
			levelCap = _XPToReachLevel.Count;
		}
		if (levelCap > _XPToReachLevel.Count)
		{
			levelCap = _XPToReachLevel.Count;
		}
		XPLevelData xPLevelData = new XPLevelData();
		xPLevelData.mCurrentXP = currentXP;
		xPLevelData.mCurrentLevel = GetCurrentLevel(currentXP, levelCap);
		xPLevelData.mXPToReachCurrentLevel = GetXpToReachLevel(xPLevelData.mCurrentLevel);
		xPLevelData.mMaxLevel = levelCap;
		xPLevelData.mIsAtMaxLevel = xPLevelData.mCurrentLevel >= levelCap;
		if (xPLevelData.mIsAtMaxLevel)
		{
			xPLevelData.mXPToPassCurrentLevel = int.MaxValue;
			xPLevelData.mRemainingXPToLevelUp = int.MaxValue;
			xPLevelData.mTotalXPInCurrentLevel = xPLevelData.mXPToReachCurrentLevel - GetXpToReachLevel(xPLevelData.mCurrentLevel - 1);
			xPLevelData.mXPEarnedWithinCurrentLevel = xPLevelData.mTotalXPInCurrentLevel;
			xPLevelData.mPercentThroughCurrentLevel = 1f;
		}
		else
		{
			xPLevelData.mXPToPassCurrentLevel = GetXpToReachLevel(xPLevelData.mCurrentLevel + 1);
			xPLevelData.mTotalXPInCurrentLevel = xPLevelData.mXPToPassCurrentLevel - xPLevelData.mXPToReachCurrentLevel;
			xPLevelData.mRemainingXPToLevelUp = xPLevelData.mXPToPassCurrentLevel - currentXP;
			xPLevelData.mXPEarnedWithinCurrentLevel = currentXP - xPLevelData.mXPToReachCurrentLevel;
			xPLevelData.mPercentThroughCurrentLevel = (float)(currentXP - xPLevelData.mXPToReachCurrentLevel) / (float)xPLevelData.mTotalXPInCurrentLevel;
			if (xPLevelData.mPercentThroughCurrentLevel < 0f)
			{
				xPLevelData.mPercentThroughCurrentLevel = 0f;
			}
			else if (xPLevelData.mPercentThroughCurrentLevel > 1f)
			{
				xPLevelData.mPercentThroughCurrentLevel = 1f;
			}
		}
		return xPLevelData;
	}
}
