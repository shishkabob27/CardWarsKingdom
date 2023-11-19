using System.Collections.Generic;
using System.Text;

public class CardItem
{
	private const int TEMP_STARTING_XP = 0;

	private static int mMaxUniqueId;

	private CardData _Form;

	private int _UniqueId;

	public CardData Form
	{
		get
		{
			return _Form;
		}
	}

	public int UniqueId
	{
		get
		{
			return _UniqueId;
		}
	}

	public bool Favorite { get; set; }

	public int CreatureUID { get; set; }

	public int CreatureSlot { get; set; }

	public CreatureFaction Faction
	{
		get
		{
			return _Form.Faction;
		}
	}

	public bool GivenForFusionTutorial { get; set; }

	public CardItem(CardData Data)
	{
		_Form = Data;
	}

	public CardItem(string CardID)
	{
		_Form = CardDataManager.Instance.GetData(CardID);
	}

	public CardItem(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public static void ResetMaxUniqueId()
	{
		mMaxUniqueId = 0;
	}

	public void AssignUniqueId()
	{
		mMaxUniqueId++;
		_UniqueId = mMaxUniqueId;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("_T", "CA") + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("ID", Form.ID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("UniqueID", _UniqueId) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Favorite", Favorite) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreatureUID", CreatureUID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreatureSlot", CreatureSlot) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		_Form = CardDataManager.Instance.GetData(TFUtils.LoadString(dict, "ID", string.Empty));
		_UniqueId = TFUtils.LoadInt(dict, "UniqueID", 0);
		Favorite = TFUtils.LoadBool(dict, "Favorite", false);
		CreatureUID = TFUtils.LoadInt(dict, "CreatureUID", 0);
		CreatureSlot = TFUtils.LoadInt(dict, "CreatureSlot", 0);
		if (_UniqueId > mMaxUniqueId)
		{
			mMaxUniqueId = _UniqueId;
		}
	}

	public int GetSellPrice()
	{
		return 10;
	}
}
