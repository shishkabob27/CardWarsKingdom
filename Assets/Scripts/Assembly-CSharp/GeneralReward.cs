public class GeneralReward
{
	public enum TypeEnum
	{
		_StartQuantities = 0,
		CustomizationCurrency = 1,
		InventorySpace = 2,
		AllySpace = 3,
		SocialCurrency = 4,
		SoftCurrency = 5,
		QuestTickets = 6,
		PvpTickets = 7,
		RankXP = 8,
		_EndQuantities = 9,
		_StartIDs = 10,
		Creatures = 11,
		GachaTable = 12,
		Cards = 13,
		Runes = 14,
		XPMaterials = 15,
		SpeedUp = 16,
		GachaKey = 17,
		CardBack = 18,
		_EndIDs = 19,
		HardCurrency = 20,
	}

	public GeneralReward(GeneralReward.TypeEnum type, int quantity)
	{
	}

	public TypeEnum RewardType;
	public int Quantity;
	public int FreeHardCurrencyQuantity;
	public string GachaTableIcon;
	public string GachaTableDesc;
}
