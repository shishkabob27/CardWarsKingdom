public class UpsightPurchase : UpsightReward
{
	public static UpsightPurchase purchaseFromJson(string json)
	{
		UpsightPurchase upsightPurchase = new UpsightPurchase();
		upsightPurchase.populateFromJson(json);
		return upsightPurchase;
	}

	public override string ToString()
	{
		return string.Format("[UpsightPurchase] productIdentifier: {0}, quantity: {1}, billboardScope: {2}", base.productIdentifier, base.quantity, base.billboardScope);
	}
}
