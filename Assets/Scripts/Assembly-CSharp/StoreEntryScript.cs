public class StoreEntryScript : UIStreamingGridListItem
{
	public enum ExtrasType
	{
		None,
		RefillStamina,
		ExpandInventory,
		ExpandAllyList,
		SoftCurrency
	}

	public ExtrasType Extra;

	public UILabel Name;

	public UILabel Cost;

	public UILabel CurrencyAmount;

	public UITexture Image;

	private StoreScreenController.FoundProductData mProductData;

	private void Awake()
	{
		if (Extra == ExtrasType.SoftCurrency)
		{
			Name.text = KFFLocalization.Get("!!BUY_GOLD").Replace("<val1>", MiscParams.BuySoftCurrencyAmount.ToString());
			Cost.text = MiscParams.BuySoftCurrencyCost.ToString();
			CurrencyAmount.text = "+" + MiscParams.BuySoftCurrencyAmount;
		}
	}

	public override void Populate(object dataObj)
	{
		mProductData = dataObj as StoreScreenController.FoundProductData;
		Cost.text = mProductData.ProductData.FormattedPrice;
		Name.text = mProductData.PackageData.DisplayName;
		if (mProductData.PackageData.CustomizationCurrency > 0)
		{
			CurrencyAmount.text = mProductData.PackageData.CustomizationCurrency.ToString();
		}
		else
		{
			CurrencyAmount.text = mProductData.PackageData.TotalHardCurrency.ToString();
		}
		Image.ReplaceTexture(mProductData.PackageData.UITexture);
		if (mProductData.PackageData.VFXIndex != -1)
		{
			base.transform.InstantiateAsChild(Singleton<StoreScreenController>.Instance.StoreEntryVFXPrefabs[mProductData.PackageData.VFXIndex]);
		}
	}

	public void OnEntryClicked()
	{
		if (Extra == ExtrasType.RefillStamina)
		{
			Singleton<StoreScreenController>.Instance.OnClickRefillStamina();
		}
		else if (Extra == ExtrasType.ExpandInventory)
		{
			Singleton<StoreScreenController>.Instance.OnClickExpandInventory();
		}
		else if (Extra == ExtrasType.ExpandAllyList)
		{
			Singleton<StoreScreenController>.Instance.OnClickExpandAllyList();
		}
		else if (Extra == ExtrasType.SoftCurrency)
		{
			Singleton<StoreScreenController>.Instance.OnClickBuySoftCurrency();
		}
		else
		{
			Singleton<StoreScreenController>.Instance.OnEntryClicked(mProductData);
		}
	}
}
