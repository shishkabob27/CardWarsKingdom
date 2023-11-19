public class iOSPurchaseListener : IPurchaseListener
{
	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}

	public void Fetch()
	{
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void GettingProductDataTimedOut()
	{
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
	}

	public void ConsumeProduct(string a_productID)
	{
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		return null;
	}

	public int VerifyReceiptGameServer(Session session, PurchaseManager.TransactionData transaction, string receipt, string productid, string transaction_id, string partial, PurchaseManager.VerifyGMReceiptCallback callback)
	{
		return -1;
	}

	public void ProcessOldPurchases()
	{
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback restorePurchaseCB = null)
	{
	}

	public void ExecSaveCurrentOldPurchases()
	{
	}

	public void RequestProcessOldPurchases()
	{
	}

	public void ExecProcessOldPurchases()
	{
	}
}
