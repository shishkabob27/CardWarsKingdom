using UnityEngine;

public class CardEquipper
{
	public delegate void ChangeMadeDelegate();

	private static ChangeMadeDelegate mOnChangeMade;

	private static InventorySlotItem mSocketingCard;

	private static int mUnsocketCost;

	private static int mUnsocketingCardSlot;

	private static CreatureItem mSelectedCreature;

	private static InventoryTile[] mEquippedTiles;

	private static Transform[] mCardSpawnNodes;

	private static int mOverrideSlotIndex = -1;

	private static int mBuyingGoldPacks;

	public static bool SetCard(InventoryTile[] equippedTiles, Transform[] cardSpawnNodes, CreatureItem creature, InventorySlotItem item, int slot, ChangeMadeDelegate changeDelegate, int overrideSlotIndex = -1)
	{
		if (creature.AlreadyHasCard(item.Card.Form))
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CARD_DUPLICATE"));
			return false;
		}
		mEquippedTiles = equippedTiles;
		mCardSpawnNodes = cardSpawnNodes;
		mOnChangeMade = changeDelegate;
		mSelectedCreature = creature;
		mOverrideSlotIndex = overrideSlotIndex;
		mSocketingCard = null;
		if (slot >= mSelectedCreature.ExCardSlotsUnlocked)
		{
			int emptyCardSlot = mSelectedCreature.GetEmptyCardSlot();
			if (emptyCardSlot == -1)
			{
				mSocketingCard = item;
				return false;
			}
			slot = emptyCardSlot;
		}
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		bool flag = false;
		bool flag2 = false;
		mUnsocketCost = 0;
		CardItem card = item.Card;
		if (card.CreatureUID != 0)
		{
			flag = true;
			mUnsocketCost += card.Form.CostToUnsocket;
		}
		if (mSelectedCreature.ExCards[slot] != null)
		{
			flag2 = true;
			mUnsocketCost += mSelectedCreature.ExCards[slot].Card.Form.CostToUnsocket;
		}
		if (mUnsocketCost > saveData.SoftCurrency)
		{
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			int totalGold;
			int totalGemCost;
			Singleton<PlayerInfoScript>.Instance.CalculateGoldPacksNeeded(mUnsocketCost, out mBuyingGoldPacks, out totalGold, out totalGemCost);
			string confirmString = string.Empty;
			string insufficientString = string.Empty;
			if (flag && flag2)
			{
				confirmString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_2_BUY"), mUnsocketCost, card.Form.Name, mSelectedCreature.ExCards[slot].Card.Form.Name, totalGold);
				insufficientString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_2_NOBUY"), mUnsocketCost, card.Form.Name, mSelectedCreature.ExCards[slot].Card.Form.Name);
			}
			else if (flag)
			{
				confirmString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_BUY"), mUnsocketCost, card.Form.Name, totalGold);
				insufficientString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_NOBUY"), mUnsocketCost, card.Form.Name);
			}
			else if (flag2)
			{
				confirmString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_BUY"), mUnsocketCost, mSelectedCreature.ExCards[slot].Card.Form.Name, totalGold);
				insufficientString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_NOBUY"), mUnsocketCost, mSelectedCreature.ExCards[slot].Card.Form.Name);
			}
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(confirmString, insufficientString, totalGemCost, OnClickConfirmBuyGold);
			return false;
		}
		mSocketingCard = item;
		mUnsocketingCardSlot = slot;
		if (!flag && !flag2)
		{
			ConfirmReplaceCard();
		}
		else
		{
			string body = string.Empty;
			if (flag && flag2)
			{
				body = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_REPLACE_1"), mUnsocketCost, card.Form.Name, mSelectedCreature.ExCards[slot].Card.Form.Name);
			}
			else if (flag)
			{
				body = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_REPLACE_2"), mUnsocketCost, card.Form.Name);
			}
			else if (flag2)
			{
				body = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_REPLACE_3"), mUnsocketCost, card.Form.Name, mSelectedCreature.ExCards[slot].Card.Form.Name);
			}
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body, ConfirmReplaceCard, DeclineReplaceCard);
		}
		return true;
	}

	private static void OnClickConfirmBuyGold()
	{
		Singleton<PlayerInfoScript>.Instance.BuyGold(mBuyingGoldPacks);
	}

	private static void ConfirmReplaceCard()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.SoftCurrency -= mUnsocketCost;
		if (mSelectedCreature.ExCards[mUnsocketingCardSlot] != null)
		{
			mSelectedCreature.ExCards[mUnsocketingCardSlot].Card.CreatureUID = 0;
			mSelectedCreature.ExCards[mUnsocketingCardSlot].Card.CreatureSlot = 0;
			NGUITools.Destroy(mEquippedTiles[mUnsocketingCardSlot].gameObject);
		}
		if (mSocketingCard.Card.CreatureUID != 0)
		{
			CreatureItem creature = saveData.GetCreatureItem(mSocketingCard.Card.CreatureUID).Creature;
			if (creature != null)
			{
				creature.ExCards[mSocketingCard.Card.CreatureSlot] = null;
			}
		}
		mSelectedCreature.ExCards[mUnsocketingCardSlot] = mSocketingCard;
		mSocketingCard.Card.CreatureUID = mSelectedCreature.UniqueId;
		mSocketingCard.Card.CreatureSlot = mUnsocketingCardSlot;
		InventoryTile component = mCardSpawnNodes[mUnsocketingCardSlot].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
		component.gameObject.ChangeLayer(mCardSpawnNodes[0].gameObject.layer);
		if (mOverrideSlotIndex != -1)
		{
			component.AssignedSlot = mOverrideSlotIndex;
		}
		else
		{
			component.AssignedSlot = mUnsocketingCardSlot;
		}
		component.Populate(mSocketingCard);
		component.SpawnCard();
		mEquippedTiles[mUnsocketingCardSlot] = component;
		mSocketingCard = null;
		mOnChangeMade();
	}

	private static void DeclineReplaceCard()
	{
		mSocketingCard = null;
	}

	public static void ClearCard(InventoryTile[] equippedTiles, CreatureItem creature, int slot, ChangeMadeDelegate changeDelegate)
	{
		mEquippedTiles = equippedTiles;
		mOnChangeMade = changeDelegate;
		mSelectedCreature = creature;
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		CardItem card = mSelectedCreature.ExCards[slot].Card;
		mUnsocketCost = card.Form.CostToUnsocket;
		if (mUnsocketCost > saveData.SoftCurrency)
		{
			mUnsocketingCardSlot = slot;
			int num = mUnsocketCost - saveData.SoftCurrency;
			mBuyingGoldPacks = 1 + (num - 1) / MiscParams.BuySoftCurrencyAmount;
			int coinAmount = mBuyingGoldPacks * MiscParams.BuySoftCurrencyCost;
			int num2 = mBuyingGoldPacks * MiscParams.BuySoftCurrencyAmount;
			string confirmString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_BUY"), mUnsocketCost, card.Form.Name, num2);
			string insufficientString = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CANNOTAFFORD_NOBUY"), mUnsocketCost, card.Form.Name);
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(confirmString, insufficientString, coinAmount, OnClickConfirmBuyGold);
		}
		else
		{
			mUnsocketingCardSlot = slot;
			string body = string.Format(KFFLocalization.Get("!!ERROR_GEMEQUIP_UNSOCKET_CONFIRM"), mUnsocketCost, card.Form.Name, KFFLocalization.Get("!!CURRENCY_SOFT"));
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body, ConfirmUnsocketCard, null);
		}
	}

	private static void ConfirmUnsocketCard()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		CardItem card = mSelectedCreature.ExCards[mUnsocketingCardSlot].Card;
		saveData.SoftCurrency -= mUnsocketCost;
		card.CreatureUID = 0;
		card.CreatureSlot = 0;
		mSelectedCreature.ExCards[mUnsocketingCardSlot] = null;
		if (mEquippedTiles[mUnsocketingCardSlot] != null)
		{
			NGUITools.Destroy(mEquippedTiles[mUnsocketingCardSlot].gameObject);
		}
		mEquippedTiles[mUnsocketingCardSlot] = null;
		mOnChangeMade();
	}
}
