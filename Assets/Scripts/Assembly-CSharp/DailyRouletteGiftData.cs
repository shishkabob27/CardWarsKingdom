using System.Collections.Generic;

public class DailyRouletteGiftData : ILoadableData
{
	private string _ID;

	private string _GiftType;

	private string _Name;

	private string _Icon;

	private int _Quantity;

	private int _Weight;

	private int _Index;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string GiftType
	{
		get
		{
			return _GiftType;
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

	public int Quantity
	{
		get
		{
			return _Quantity;
		}
	}

	public int Weight
	{
		get
		{
			return _Weight;
		}
	}

	public int Index
	{
		get
		{
			return _Index;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_GiftType = TFUtils.LoadString(dict, "Type");
		_Name = TFUtils.LoadString(dict, "Name");
		_Icon = TFUtils.LoadString(dict, "Icon");
		_Quantity = TFUtils.LoadInt(dict, "Quantity");
		_Weight = TFUtils.LoadInt(dict, "Weight");
		_Index = TFUtils.LoadInt(dict, "Index");
	}
}
