using System.Collections.Generic;
using System.IO;

public class InviteRewardsManager : DataManager<InviteReward>
{
	private static InviteRewardsManager _instance;

	public Dictionary<string, List<InviteReward>> GiftData = new Dictionary<string, List<InviteReward>>();

	private bool mInitialized;

	public static InviteRewardsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_InviteRewards.json");
				_instance = new InviteRewardsManager(path);
			}
			return _instance;
		}
	}

	public InviteRewardsManager(string path)
	{
		base.FilePath = path;
		mInitialized = false;
	}

	public InviteReward EarnedReward(int count)
	{
		foreach (InviteReward item in DatabaseArray)
		{
			if (count == item.InvitesRequired)
			{
				return item;
			}
		}
		return null;
	}
}
