using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneralReward
{
	public enum TypeEnum
	{
		_StartQuantities,
		CustomizationCurrency,
		InventorySpace,
		AllySpace,
		SocialCurrency,
		SoftCurrency,
		QuestTickets,
		PvpTickets,
		RankXP,
		_EndQuantities,
		_StartIDs,
		Creatures,
		GachaTable,
		Cards,
		Runes,
		XPMaterials,
		SpeedUp,
		GachaKey,
		CardBack,
		_EndIDs,
		HardCurrency
	}

	public TypeEnum RewardType;

	public int Quantity;

	public int FreeHardCurrencyQuantity;

	public CreatureData Creature;

	public GachaWeightTable GachaTable;

	public string GachaTableIcon;

	public string GachaTableDesc;

	public InventorySlotItem GrantedInventoryItem;

	public CardData Card;

	public EvoMaterialData Rune;

	public XPMaterialData XPMaterial;

	public SpeedUpData SpeedUp;

	public GachaSlotData GachaKey;

	public CardBackData CardBack;

	public GeneralReward(TypeEnum type, int quantity = 1)
	{
		RewardType = type;
		Quantity = quantity;
	}

	public static List<GeneralReward> ParseFromJson(Dictionary<string, object> dict)
	{
		List<GeneralReward> list = new List<GeneralReward>();
		int num = TFUtils.LoadInt(dict, "PaidHardCurrency", 0);
		int num2 = TFUtils.LoadInt(dict, "FreeHardCurrency", 0) + TFUtils.LoadInt(dict, "HardCurrency", 0);
		if (num > 0 || num2 > 0)
		{
			GeneralReward generalReward = new GeneralReward(TypeEnum.HardCurrency);
			generalReward.Quantity = num;
			generalReward.FreeHardCurrencyQuantity = num2;
			list.Add(generalReward);
		}
		for (int i = 11; i < 19; i++)
		{
			TypeEnum typeEnum = (TypeEnum)i;
			string key = typeEnum.ToString();
			string text = TFUtils.LoadString(dict, key, null);
			if (text == null)
			{
				continue;
			}
			string[] array = text.Split(',');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				GeneralReward generalReward2 = new GeneralReward(typeEnum);
				string[] array3 = text2.Trim().Split(':');
				if (array3.Length > 1)
				{
					generalReward2.Quantity = Convert.ToInt32(array3[1].Trim());
				}
				if (generalReward2.Quantity != 0)
				{
					string iD = array3[0].Trim();
					switch (typeEnum)
					{
					case TypeEnum.Creatures:
						generalReward2.Creature = CreatureDataManager.Instance.GetData(iD);
						break;
					case TypeEnum.GachaTable:
						generalReward2.GachaTable = GachaWeightDataManager.Instance.GetData(iD);
						generalReward2.GachaTableIcon = TFUtils.LoadString(dict, "GachaTableIcon", string.Empty);
						generalReward2.GachaTableDesc = TFUtils.LoadLocalizedString(dict, "GachaTableDesc", string.Empty);
						break;
					case TypeEnum.Cards:
						generalReward2.Card = CardDataManager.Instance.GetData(iD);
						break;
					case TypeEnum.Runes:
						generalReward2.Rune = EvoMaterialDataManager.Instance.GetData(iD);
						break;
					case TypeEnum.XPMaterials:
						generalReward2.XPMaterial = XPMaterialDataManager.Instance.GetData(iD);
						break;
					case TypeEnum.GachaKey:
						generalReward2.GachaKey = GachaSlotDataManager.Instance.GetData(iD);
						break;
					case TypeEnum.CardBack:
						generalReward2.CardBack = CardBackDataManager.Instance.GetData(iD);
						break;
					}
					list.Add(generalReward2);
				}
			}
		}
		for (int k = 1; k < 9; k++)
		{
			string key2 = ((TypeEnum)k).ToString();
			int num3 = TFUtils.LoadInt(dict, key2, 0);
			if (num3 > 0)
			{
				list.Add(new GeneralReward((TypeEnum)k, num3));
			}
		}
		return list;
	}

	public void PopulateUI(UILabel label, UISprite iconSprite, UITexture iconTexture, UISprite iconBGSprite, Transform inventoryTileNode, UILabel quantityLabel = null, int tileDepth = -1)
	{
		int layer = 0;
		if (label != null)
		{
			layer = label.gameObject.layer;
		}
		else if (iconSprite != null)
		{
			layer = iconSprite.gameObject.layer;
		}
		else if (iconBGSprite != null)
		{
			layer = iconBGSprite.gameObject.layer;
		}
		else if (inventoryTileNode != null)
		{
			layer = inventoryTileNode.gameObject.layer;
		}
		iconSprite.gameObject.SetActive(false);
		if (iconBGSprite != null)
		{
			iconBGSprite.gameObject.SetActive(false);
		}
		iconTexture.gameObject.SetActive(false);
		inventoryTileNode.DestroyAllChildren();
		if (RewardType == TypeEnum.SoftCurrency)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!CURRENCY_SOFT");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_Soft";
		}
		else if (RewardType == TypeEnum.SocialCurrency)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!CURRENCY_PVP");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_PVPCurrency";
		}
		else if (RewardType == TypeEnum.CustomizationCurrency)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!CURRENCY_CUSTOMIZATION_TOKENS");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_Customize";
		}
		else if (RewardType == TypeEnum.QuestTickets)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!BATTLE_ENERGY");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_BattleEnergy";
		}
		else if (RewardType == TypeEnum.PvpTickets)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!PVP_ENERGY");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_PVPEnergy";
		}
		else if (RewardType == TypeEnum.HardCurrency)
		{
			if (label != null)
			{
				label.text = Quantity + FreeHardCurrencyQuantity + " " + KFFLocalization.Get("!!CURRENCY_HARD");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "Icon_Currency_Hard";
		}
		else if (RewardType == TypeEnum.InventorySpace)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!INVENTORY_SLOTS");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "UI_Creature_0";
		}
		else if (RewardType == TypeEnum.AllySpace)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!ALLY_LIST_SLOTS");
			}
			iconSprite.gameObject.SetActive(true);
			iconSprite.spriteName = "UI_Creature_0";
		}
		else if (RewardType == TypeEnum.Creatures)
		{
			if (label != null)
			{
				if (Quantity == 1)
				{
					label.text = Creature.Name;
				}
				else
				{
					label.text = Quantity + "x " + Creature.Name;
				}
			}
			InventoryTile component = inventoryTileNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			inventoryTileNode.gameObject.ChangeLayer(layer);
			if (tileDepth != -1)
			{
				component.AdjustDepth(tileDepth);
			}
			component.PopulateAndForceDisplay(Creature);
		}
		else if (RewardType == TypeEnum.GachaTable)
		{
			if (label != null)
			{
				label.text = GachaTableDesc;
			}
			iconTexture.gameObject.SetActive(true);
			iconTexture.ReplaceTexture(GachaTableIcon);
		}
		else if (RewardType == TypeEnum.Cards)
		{
			if (label != null)
			{
				if (Quantity == 1)
				{
					label.text = Card.Name;
				}
				else
				{
					label.text = Quantity + "x " + Card.Name;
				}
			}
			InventoryTile component2 = inventoryTileNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			inventoryTileNode.gameObject.ChangeLayer(layer);
			if (tileDepth != -1)
			{
				component2.AdjustDepth(tileDepth);
			}
			InventorySlotItem dataObj = new InventorySlotItem(Card);
			component2.Populate(dataObj);
		}
		else if (RewardType == TypeEnum.Runes)
		{
			if (label != null)
			{
				if (Quantity == 1)
				{
					label.text = Rune.Name;
				}
				else
				{
					label.text = Quantity + "x " + Rune.Name;
				}
			}
			InventoryTile component3 = inventoryTileNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			inventoryTileNode.gameObject.ChangeLayer(layer);
			if (tileDepth != -1)
			{
				component3.AdjustDepth(tileDepth);
			}
			InventorySlotItem dataObj2 = new InventorySlotItem(Rune);
			component3.Populate(dataObj2);
		}
		else if (RewardType == TypeEnum.XPMaterials)
		{
			if (label != null)
			{
				if (Quantity == 1)
				{
					label.text = XPMaterial.Name;
				}
				else
				{
					label.text = Quantity + "x " + XPMaterial.Name;
				}
			}
			InventoryTile component4 = inventoryTileNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			inventoryTileNode.gameObject.ChangeLayer(layer);
			if (tileDepth != -1)
			{
				component4.AdjustDepth(tileDepth);
			}
			InventorySlotItem dataObj3 = new InventorySlotItem(XPMaterial);
			component4.Populate(dataObj3);
		}
		else if (RewardType == TypeEnum.RankXP)
		{
			if (label != null)
			{
				label.text = Quantity + " " + KFFLocalization.Get("!!RANK_XP");
			}
		}
		else if (RewardType == TypeEnum.SpeedUp)
		{
			if (label != null)
			{
				label.text = PlayerInfoScript.FormatTimeString(SpeedUp.Minutes * 60, true, true) + " " + KFFLocalization.Get("!!SPEED_UP");
			}
		}
		else if (RewardType == TypeEnum.GachaKey)
		{
			if (label != null)
			{
				if (Quantity == 1)
				{
					label.text = GachaKey.KeyName;
				}
				else
				{
					label.text = Quantity + "x " + GachaKey.KeyName;
				}
			}
			InventoryTile component5 = inventoryTileNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			inventoryTileNode.gameObject.ChangeLayer(layer);
			if (tileDepth != -1)
			{
				component5.AdjustDepth(tileDepth);
			}
			component5.Populate(GachaKey);
			component5.GetComponent<Collider>().enabled = false;
		}
		else if (RewardType == TypeEnum.CardBack)
		{
			if (label != null)
			{
				label.text = CardBack.Name;
			}
			if (iconTexture != null)
			{
				iconTexture.gameObject.SetActive(true);
				iconTexture.ReplaceTexture(CardBack.TextureUI);
			}
		}
		if (iconBGSprite != null && iconSprite.gameObject.activeInHierarchy)
		{
			iconBGSprite.gameObject.SetActive(true);
			iconBGSprite.spriteName = iconSprite.spriteName;
		}
		if (quantityLabel != null)
		{
			if (RewardType == TypeEnum.HardCurrency && Quantity + FreeHardCurrencyQuantity > 1)
			{
				quantityLabel.text = Quantity + FreeHardCurrencyQuantity + "  ";
			}
			else if (Quantity > 1)
			{
				quantityLabel.text = Quantity.ToString();
			}
		}
	}

	public void Grant(string hardCurrencyEventName = null, string productCountryCode = null, string productPrice = null)
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		if (RewardType == TypeEnum.SoftCurrency)
		{
			saveData.SoftCurrency += Quantity;
		}
		else if (RewardType == TypeEnum.SocialCurrency)
		{
			saveData.PvPCurrency += Quantity;
		}
		else if (RewardType == TypeEnum.InventorySpace)
		{
			saveData.AddEmptyInventorySlots(Quantity);
		}
		else if (RewardType == TypeEnum.AllySpace)
		{
			saveData.AddEmptyAllySlots(Quantity);
		}
		else if (RewardType == TypeEnum.QuestTickets)
		{
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Quests, Quantity);
		}
		else if (RewardType == TypeEnum.PvpTickets)
		{
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Pvp, Quantity);
		}
		else if (RewardType == TypeEnum.Creatures)
		{
			for (int i = 0; i < Quantity; i++)
			{
				CreatureItem creature = new CreatureItem(Creature);
				saveData.AddCreature(creature);
			}
		}
		else if (RewardType == TypeEnum.GachaTable)
		{
			GachaWeightEntry gachaWeightEntry = GachaTable.Spin();
			CreatureItem creature2 = new CreatureItem(gachaWeightEntry.DropID);
			GrantedInventoryItem = saveData.AddCreature(creature2);
		}
		else if (RewardType == TypeEnum.Cards)
		{
			for (int j = 0; j < Quantity; j++)
			{
				CardItem card = new CardItem(Card);
				GrantedInventoryItem = saveData.AddExCard(card);
			}
		}
		else if (RewardType == TypeEnum.Runes)
		{
			for (int k = 0; k < Quantity; k++)
			{
				GrantedInventoryItem = saveData.AddEvoMaterial(Rune);
			}
		}
		else if (RewardType == TypeEnum.XPMaterials)
		{
			for (int l = 0; l < Quantity; l++)
			{
				GrantedInventoryItem = saveData.AddXPMaterial(XPMaterial);
			}
		}
		else if (RewardType == TypeEnum.HardCurrency)
		{
			Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(Quantity, FreeHardCurrencyQuantity, hardCurrencyEventName, Singleton<PurchaseManager>.Instance.getLastHandle, string.Empty);
		}
		else if (RewardType == TypeEnum.RankXP)
		{
			saveData.RankXP += Quantity;
		}
		else if (RewardType == TypeEnum.SpeedUp)
		{
			Singleton<PlayerInfoScript>.Instance.AddSpeedUp(SpeedUp, Quantity);
		}
		else if (RewardType == TypeEnum.GachaKey)
		{
			Singleton<PlayerInfoScript>.Instance.AddGachaKey(GachaKey, Quantity);
		}
		else if (RewardType == TypeEnum.CardBack && !saveData.UnlockedCardBacks.Contains(CardBack))
		{
			saveData.UnlockedCardBacks.Add(CardBack);
		}
	}
}
