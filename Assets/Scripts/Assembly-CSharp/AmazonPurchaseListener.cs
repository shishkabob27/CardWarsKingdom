using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt;
using UnityEngine;

public class AmazonPurchaseListener : IPurchaseListener
{
	private static string amazonUserID;

	private static string IAP_RECEIPT_VERIFICATION_URL = "verify_purchase_amazon.php";

	private bool BillingSupported = true;

	private bool isInitialProductDataUpdate = true;

	private int processPurchaseUpdates;

	private static int uniqhandle;

	private PurchaseManager.ReceivedProductDataCallback receivedProductDataCallback;

	private PurchaseManager.ProductPurchaseCallback productPurchaseCallback;

	public static PurchaseManager.VerifyIAPReceiptCallback verifyIAPReceiptCallback;

	private PurchaseManager.VerifyGMReceiptCallback m_receiptCallback;

	private List<PurchaseReceipt> purchases = new List<PurchaseReceipt>();

	private bool awaitingProductData;

	private static string GetAmazonUserID()
	{
		return amazonUserID;
	}

	public void OnEnable()
	{
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		instance.AddGetProductDataResponseListener(AmazonProductDataRequestCallback);
		instance.AddGetPurchaseUpdatesResponseListener(AmazonPurchaseUpdatesResponseCallback);
		instance.AddGetUserDataResponseListener(AmazonUserDataResponseCallback);
		instance.AddPurchaseResponseListener(AmazonPurchaseResponseCallback);
	}

	public void OnDisable()
	{
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		instance.RemoveGetProductDataResponseListener(AmazonProductDataRequestCallback);
		instance.RemoveGetPurchaseUpdatesResponseListener(AmazonPurchaseUpdatesResponseCallback);
		instance.RemoveGetUserDataResponseListener(AmazonUserDataResponseCallback);
		instance.RemovePurchaseResponseListener(AmazonPurchaseResponseCallback);
	}

	public void Fetch()
	{
	}

	protected void SendRecieptVerification(string userID, PurchaseReceipt receipt)
	{
		string receiptId = receipt.ReceiptId;
		string text = PurchaseManager.IAP_VERIFICATION_SERVER_URL + "/" + IAP_RECEIPT_VERIFICATION_URL + "?";
		if (PurchaseManager.IAP_SANDBOX)
		{
			text += "sandbox=1&";
		}
		text = text + "language=" + KFFCSUtils.GetLanguageCode();
		text += "&subdirectory=data_1.01";
		text += "&json=1";
		text = text + "&userid=" + userID;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("purchaseToken", WWW.EscapeURL(receiptId));
		//KFFNetwork.GetInstance().SendWWWRequestWithForm(wWWForm, text, WWWVerifyReceiptCallback, receipt);
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		if (BillingSupported)
		{
			awaitingProductData = true;
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;
			receivedProductDataCallback = a_Callback;
			GetProductDataResponseDelegate productDataCallback = null;
			productDataCallback = delegate(GetProductDataResponse response)
			{
				iapService.RemoveGetProductDataResponseListener(productDataCallback);
				if (!(response.Status != "SUCCESSFUL"))
				{
					processPurchaseUpdates++;
					iapService.GetPurchaseUpdates(new ResetInput
					{
						Reset = isInitialProductDataUpdate
					});
					isInitialProductDataUpdate = false;
				}
			};
			iapService.GetUserData();
			iapService.AddGetProductDataResponseListener(productDataCallback);
			SkusInput skusInput = new SkusInput();
			skusInput.Skus = new List<string>(a_ProductIDs);
			iapService.GetProductData(skusInput);
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void GettingProductDataTimedOut()
	{
		if (awaitingProductData)
		{
			if (receivedProductDataCallback != null)
			{
				receivedProductDataCallback(false, null, "Timed out getting product data from Amazon");
			}
			awaitingProductData = false;
		}
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
		TFUtils.DebugLog("Calling Amazon PurchaseListener.PurchaseProduct for product: " + a_ProductID);
		productPurchaseCallback = a_Callback;
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		SkuInput skuInput = new SkuInput();
		skuInput.Sku = a_ProductID;
		SkuInput skuInput2 = skuInput;
		instance.Purchase(skuInput2);
	}

	public void ProcessOldPurchases()
	{
	}

	public void ConsumeProduct(string a_productID)
	{
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback restorePurchaseCB = null)
	{
		if (restorePurchaseCB != null)
		{
			restorePurchaseCB(true);
		}
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		PurchaseResponse purchaseResponse = (PurchaseResponse)transaction.NativeTransaction;
		verifyIAPReceiptCallback = callback;
		SendRecieptVerification(purchaseResponse.AmazonUserData.UserId, purchaseResponse.PurchaseReceipt);
		return null;
	}

	public void VerifyOldReceiptGameServer(Dictionary<string, string> a_TransactionDict)
	{
		Session theSession = SessionManager.Instance.theSession;
		string partial = Singleton<PurchaseManager>.Instance.m_partial;
		VerifyReceiptGameServer(theSession, null, a_TransactionDict["base64EncodedTransactionReceipt"], a_TransactionDict["productIdentifier"], a_TransactionDict["transactionIdentifier"], partial, null);
	}

	public int VerifyReceiptGameServer(Session session, PurchaseManager.TransactionData transaction, string receipt, string product_id, string transaction_id, string partial, PurchaseManager.VerifyGMReceiptCallback callback)
	{
		string playerCode = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		int num = -1;
		m_receiptCallback = callback;
		uniqhandle++;
		m_receiptCallback(transaction, num, true);
		NotifyFulfillment(((PurchaseResponse)transaction.NativeTransaction).PurchaseReceipt);
		return num;
	}

	private void WWWVerifyReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		//Discarded unreachable code: IL_001e, IL_0097
		KFFNetwork.WWWRequestResult wWWRequestResult;
		try
		{
			wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to convert incoming result -> " + ex);
		}
		if (wWWRequestResult == null)
		{
		}
		if (wWWRequestResult != null && wWWRequestResult.IsValid())
		{
			string key = "RZrb({~IXRD{Oen[UXZA(a5my-5bZe17JeJMyR-DcuOI6";
			string text = "azTn-kDeI=niczdj8BIw{oNG=6|]{{EY%[Ym";
			string valueAsString = wWWRequestResult.GetValueAsString(key);
			if (valueAsString != text)
			{
				resultObj = null;
				err = KFFLocalization.Get("!!ERROR_PURCHASE_FAILED_RESULT_INVALID");
			}
			else
			{
				try
				{
					NotifyFulfillment((PurchaseReceipt)param);
				}
				catch (Exception ex2)
				{
					throw new Exception("Failed to convert param -> " + ex2);
				}
			}
		}
		if (verifyIAPReceiptCallback != null)
		{
			verifyIAPReceiptCallback(wwwinfo, resultObj, err, param, null);
			verifyIAPReceiptCallback = null;
		}
	}

	private void AmazonPurchaseResponseCallback(PurchaseResponse response)
	{
		if (response.Status == "SUCCESSFUL")
		{
			purchases.Add(response.PurchaseReceipt);
			if (productPurchaseCallback != null)
			{
				PurchaseManager.TransactionData transactionData = new PurchaseManager.TransactionData();
				transactionData.NativeTransaction = response;
				productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Success, transactionData, null);
			}
		}
		else if (productPurchaseCallback != null)
		{
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Failed, null, "failed");
		}
	}

	private void AmazonUserDataResponseCallback(GetUserDataResponse response)
	{
		if (response.Status == "SUCCESSFUL")
		{
			amazonUserID = response.AmazonUserData.UserId;
		}
	}

	private void AmazonProductDataRequestCallback(GetProductDataResponse response)
	{
		if (!awaitingProductData)
		{
			return;
		}
		awaitingProductData = false;
		if (response.Status == "SUCCESSFUL")
		{
			Dictionary<string, ProductData> productDataMap = response.ProductDataMap;
			List<string> unavailableSkus = response.UnavailableSkus;
			Singleton<PurchaseManager>.Instance.m_Products.Clear();
			foreach (KeyValuePair<string, ProductData> item in productDataMap)
			{
				PurchaseManager.ProductData productData = new PurchaseManager.ProductData();
				productData.ProductIdentifier = item.Value.Sku;
				productData.Title = item.Value.Title;
				productData.Price = item.Value.Price;
				productData.Description = item.Value.Description;
				productData.FormattedPrice = item.Value.Price;
				Singleton<PurchaseManager>.Instance.m_Products.Add(productData);
			}
			if (receivedProductDataCallback != null)
			{
				receivedProductDataCallback(true, Singleton<PurchaseManager>.Instance.m_Products, null);
			}
		}
		else if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(false, null, response.Status);
		}
	}

	private void AmazonPurchaseUpdatesResponseCallback(GetPurchaseUpdatesResponse response)
	{
		if (processPurchaseUpdates <= 0)
		{
			return;
		}
		processPurchaseUpdates--;
		if (!(response.Status == "SUCCESSFUL"))
		{
			return;
		}
		List<PurchaseReceipt> receipts = response.Receipts;
		foreach (PurchaseReceipt item in receipts)
		{
			long cancelDate = item.CancelDate;
			if (cancelDate == 0L)
			{
				bool success = true;
				NotifyFulfillment(item, success);
			}
		}
		if (response.HasMore)
		{
			AmazonIapV2Impl.Instance.GetPurchaseUpdates(new ResetInput
			{
				Reset = false
			});
		}
	}

	private void NotifyFulfillment(PurchaseReceipt receipt, bool success = true)
	{
		if (receipt != null)
		{
			AmazonIapV2Impl.Instance.NotifyFulfillment(new NotifyFulfillmentInput
			{
				ReceiptId = receipt.ReceiptId,
				FulfillmentResult = ((!success) ? "UNAVAILABLE" : "FULFILLED")
			});
		}
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
