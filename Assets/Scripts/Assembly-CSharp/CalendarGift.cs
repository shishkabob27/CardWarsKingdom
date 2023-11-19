using System;
using System.Collections.Generic;

public class CalendarGift
{
	public CalendarGiftType GiftType { get; private set; }

	public int Quantity { get; private set; }

	public string GiftID { get; private set; }

	public string Icon { get; private set; }

	public string Texture { get; private set; }

	public bool Repeating { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		Quantity = TFUtils.LoadInt(dict, "Quantity", 0);
		GiftID = TFUtils.LoadString(dict, "GiftID", string.Empty);
		Icon = TFUtils.LoadString(dict, "Sprite", null);
		Texture = TFUtils.LoadString(dict, "Texture", null);
		Repeating = TFUtils.LoadBool(dict, "Repeating", false);
		string value = TFUtils.LoadString(dict, "Type", string.Empty);
		GiftType = (CalendarGiftType)(int)Enum.Parse(typeof(CalendarGiftType), value);
	}

	public string Grant(out InventorySlotItem slotItem, out CardBackData cardBack)
	{
		slotItem = null;
		cardBack = null;
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		if (GiftType == CalendarGiftType.StaminaPvE)
		{
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Quests, Quantity);
			return Quantity + " " + KFFLocalization.Get("!!BATTLE_ENERGY");
		}
		if (GiftType == CalendarGiftType.StaminaPvP)
		{
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Pvp, Quantity);
			return Quantity + " " + KFFLocalization.Get("!!PVP_ENERGY");
		}
		if (GiftType == CalendarGiftType.HardCurrency)
		{
			Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(0, Quantity, "calendar reward", -1, string.Empty);
			if (Quantity == 1)
			{
				return "1 " + KFFLocalization.Get("!!CURRENCY_HARD_1");
			}
			return Quantity + " " + KFFLocalization.Get("!!CURRENCY_HARD");
		}
		if (GiftType == CalendarGiftType.SoftCurrency)
		{
			saveData.SoftCurrency += Quantity;
			return Quantity + " " + KFFLocalization.Get("!!CURRENCY_SOFT");
		}
		if (GiftType == CalendarGiftType.SocialCurrency)
		{
			saveData.PvPCurrency += Quantity;
			return Quantity + " " + KFFLocalization.Get("!!CURRENCY_PVP");
		}
		if (GiftType == CalendarGiftType.Runes)
		{
			EvoMaterialData data = EvoMaterialDataManager.Instance.GetData(GiftID);
			for (int i = 0; i < Quantity; i++)
			{
				slotItem = saveData.AddEvoMaterial(data);
			}
			return Quantity + "x " + data.Name;
		}
		if (GiftType == CalendarGiftType.XPMaterials)
		{
			XPMaterialData data2 = XPMaterialDataManager.Instance.GetData(GiftID);
			for (int j = 0; j < Quantity; j++)
			{
				slotItem = saveData.AddXPMaterial(data2);
			}
			return Quantity + "x " + data2.Name;
		}
		if (GiftType == CalendarGiftType.CardBack)
		{
			cardBack = CardBackDataManager.Instance.GetData(GiftID);
			if (!saveData.UnlockedCardBacks.Contains(cardBack))
			{
				saveData.UnlockedCardBacks.Add(cardBack);
			}
			return cardBack.Name;
		}
		if (GiftType == CalendarGiftType.CreatureTable)
		{
			GachaWeightTable data3 = GachaWeightDataManager.Instance.GetData(GiftID);
			GachaWeightEntry gachaWeightEntry = data3.Spin();
			CreatureItem creatureItem = new CreatureItem(gachaWeightEntry.DropID);
			slotItem = Singleton<PlayerInfoScript>.Instance.SaveData.AddCreature(creatureItem);
			return creatureItem.Form.Name;
		}
		return null;
	}
}
