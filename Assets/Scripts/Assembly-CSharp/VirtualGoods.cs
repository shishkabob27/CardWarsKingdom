using System.Collections;
using System.Collections.Generic;

public class VirtualGoods : ILoadableData
{
	private string _ID;

	private int _SoftCurrency;

	private int _SocialCurrency;

	private int _PaidHardCurrency;

	private int _FreeHardCurrency;

	private int _InventorySpace;

	private int _AllySpace;

	private int _RankXp;

	private int _QuestTickets;

	private int _PvpTickets;

	private List<CreatureData> _Creatures = new List<CreatureData>();

	private List<EvoMaterialData> _EvoMaterials = new List<EvoMaterialData>();

	private List<CardData> _Cards = new List<CardData>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ProductID_Android", string.Empty);
		_SoftCurrency = TFUtils.LoadInt(dict, "SoftCurrency", 0);
		_SocialCurrency = TFUtils.LoadInt(dict, "SocialCurrency", 0);
		_PaidHardCurrency = TFUtils.LoadInt(dict, "PaidHardCurrency", 0);
		_FreeHardCurrency = TFUtils.LoadInt(dict, "FreeHardCurrency", 0);
		_InventorySpace = TFUtils.LoadInt(dict, "InventorySpace", 0);
		_AllySpace = TFUtils.LoadInt(dict, "AllySpace", 0);
		_RankXp = TFUtils.LoadInt(dict, "RankExp", 0);
		_QuestTickets = TFUtils.LoadInt(dict, "QuestTickets", 0);
		_PvpTickets = TFUtils.LoadInt(dict, "PvpTickets", 0);
		string[] array = TFUtils.LoadString(dict, "Creatures", string.Empty).Split(',');
		foreach (string text in array)
		{
			if (!(text == string.Empty))
			{
				CreatureData data = CreatureDataManager.Instance.GetData(text.Trim());
				if (data != null)
				{
					_Creatures.Add(data);
				}
			}
		}
		string[] array2 = TFUtils.LoadString(dict, "EvoMaterials", string.Empty).Split(',');
		foreach (string text2 in array2)
		{
			if (!(text2 == string.Empty))
			{
				EvoMaterialData data2 = EvoMaterialDataManager.Instance.GetData(text2.Trim());
				if (data2 != null)
				{
					_EvoMaterials.Add(data2);
				}
			}
		}
		string[] array3 = TFUtils.LoadString(dict, "Cards", string.Empty).Split(',');
		foreach (string text3 in array3)
		{
			if (!(text3 == string.Empty))
			{
				CardData data3 = CardDataManager.Instance.GetData(text3.Trim());
				if (data3 != null)
				{
					_Cards.Add(data3);
				}
			}
		}
	}

	public IEnumerator Redeem()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.SoftCurrency += _SoftCurrency;
		saveData.PvPCurrency += _SocialCurrency;
		Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(_PaidHardCurrency, _FreeHardCurrency, _ID, -1, string.Empty);
		saveData.AddEmptyInventorySlots(_InventorySpace);
		saveData.AddEmptyAllySlots(_AllySpace);
		if (_RankXp > 0)
		{
			saveData.RankXP += _RankXp;
			if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
			{
				Singleton<TownHudController>.Instance.PopulatePlayerInfo();
			}
		}
		DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Quests, _QuestTickets);
		DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Pvp, _PvpTickets);
		Singleton<PlayerInfoScript>.Instance.Save();
		yield break;
	}
}
