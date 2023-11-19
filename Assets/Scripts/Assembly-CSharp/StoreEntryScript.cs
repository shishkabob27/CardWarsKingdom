public class StoreEntryScript : UIStreamingGridListItem
{
	public enum ExtrasType
	{
		None = 0,
		RefillStamina = 1,
		ExpandInventory = 2,
		ExpandAllyList = 3,
		SoftCurrency = 4,
	}

	public ExtrasType Extra;
	public UILabel Name;
	public UILabel Cost;
	public UILabel CurrencyAmount;
	public UITexture Image;
}
