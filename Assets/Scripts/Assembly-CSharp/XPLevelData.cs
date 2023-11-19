public class XPLevelData
{
	public int mCurrentXP;

	public int mCurrentLevel;

	public int mXPToReachCurrentLevel;

	public int mXPToPassCurrentLevel;

	public int mTotalXPInCurrentLevel;

	public int mRemainingXPToLevelUp;

	public int mXPEarnedWithinCurrentLevel;

	public float mPercentThroughCurrentLevel;

	public bool mIsAtMaxLevel;

	public int mMaxLevel;

	public void PopulateUI(bool verbose, UILabel level = null, UILabel xpCount = null, UILabel xpPercent = null, UISprite xpBar = null)
	{
		if (level != null)
		{
			if (verbose)
			{
				level.text = KFFLocalization.Get("!!LEVEL_X").Replace("<val1>", mCurrentLevel.ToString());
			}
			else
			{
				level.text = mCurrentLevel.ToString();
			}
		}
		if (xpCount != null)
		{
			if (mIsAtMaxLevel)
			{
				xpCount.text = string.Empty;
			}
			else
			{
				xpCount.text = mXPEarnedWithinCurrentLevel + "/" + mTotalXPInCurrentLevel;
			}
		}
		if (xpPercent != null)
		{
			if (mIsAtMaxLevel)
			{
				xpPercent.text = "MAX";
			}
			else
			{
				xpPercent.text = (int)(mPercentThroughCurrentLevel * 100f) + "%";
			}
		}
		if (xpBar != null)
		{
			xpBar.fillAmount = mPercentThroughCurrentLevel;
		}
	}
}
