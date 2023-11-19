using System;
using System.Collections.Generic;

public class PlayerRankData : ILoadableData
{
	private string _ID;

	private int _Level;

	private int _Stamina;

	private UnlockTypeEnum _UnlockType;

	private string _UnlockId;

	private int _TeamCost;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public int Level
	{
		get
		{
			return _Level;
		}
	}

	public int Stamina
	{
		get
		{
			return _Stamina;
		}
	}

	public UnlockTypeEnum UnlockType
	{
		get
		{
			return _UnlockType;
		}
	}

	public string UnlockId
	{
		get
		{
			return _UnlockId;
		}
	}

	public int TeamCost
	{
		get
		{
			return _TeamCost;
		}
	}

	public int InventorySpace { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_Level = TFUtils.LoadInt(dict, "Rank", 0);
		_Stamina = TFUtils.LoadInt(dict, "Stamina", 1);
		_TeamCost = TFUtils.LoadInt(dict, "TeamCost", 1);
		InventorySpace = TFUtils.LoadInt(dict, "InventorySpace", 0);
		string text = TFUtils.LoadString(dict, "Unlock", string.Empty);
		if (text != string.Empty)
		{
			_UnlockType = (UnlockTypeEnum)(int)Enum.Parse(typeof(UnlockTypeEnum), text, true);
			_UnlockId = TFUtils.LoadString(dict, "UnlockId", string.Empty);
		}
		else
		{
			_UnlockType = UnlockTypeEnum.None;
		}
	}
}
