public interface IPurchaseListener
{
	void OnEnable();

	void OnDisable();

	void Fetch();

	PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback);

	void GettingProductDataTimedOut();

	void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback);

	void ConsumeProduct(string a_productID);

	KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData a_Transaction, PurchaseManager.VerifyIAPReceiptCallback a_Callback);

	int VerifyReceiptGameServer(Session session, PurchaseManager.TransactionData transaction, string receipt, string productid, string transaction_id, string partial, PurchaseManager.VerifyGMReceiptCallback callback);

	void ProcessOldPurchases();

	void RestorePurchases(PurchaseManager.RestorePurchasesCallback callback = null);

	void ExecSaveCurrentOldPurchases();

	void RequestProcessOldPurchases();

	void ExecProcessOldPurchases();
}
