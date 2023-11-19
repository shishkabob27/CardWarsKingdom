using System.Collections.Generic;

public class InviteReward : ILoadableData
{
	private string _ID;

	private string _RewardType;

	private string _Name;

	private string _Icon;

	private int _InvitesRequired;

	private int _RewardQuantity;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string RewardType
	{
		get
		{
			return _RewardType;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public string Icon
	{
		get
		{
			return _Icon;
		}
	}

	public int InvitesRequired
	{
		get
		{
			return _InvitesRequired;
		}
	}

	public int RewardQuantity
	{
		get
		{
			return _RewardQuantity;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_RewardType = TFUtils.LoadString(dict, "RewardType");
		_Name = TFUtils.LoadString(dict, "Name");
		_Icon = TFUtils.LoadString(dict, "Icon");
		_RewardQuantity = TFUtils.LoadInt(dict, "RewardQuantity");
		_InvitesRequired = TFUtils.LoadInt(dict, "InvitesRequired");
	}
}
