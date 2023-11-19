using System.Collections.Generic;

public class StatusRemovalData
{
	public bool RandomBuffs;

	public bool RandomDebuffs;

	public List<StatusData> Targets;

	private static StatusRemovalData mRandomBuffs;

	private static StatusRemovalData mRandomDebuffs;

	public static StatusRemovalData RemoveRandomBuffs()
	{
		if (mRandomBuffs == null)
		{
			mRandomBuffs = new StatusRemovalData();
			mRandomBuffs.RandomBuffs = true;
		}
		return mRandomBuffs;
	}

	public static StatusRemovalData RemoveRandomDebuffs()
	{
		if (mRandomDebuffs == null)
		{
			mRandomDebuffs = new StatusRemovalData();
			mRandomDebuffs.RandomDebuffs = true;
		}
		return mRandomDebuffs;
	}
}
