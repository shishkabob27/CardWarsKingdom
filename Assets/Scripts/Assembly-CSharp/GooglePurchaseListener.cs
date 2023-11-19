using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Prime31;
using UnityEngine;

public class GooglePurchaseListener : IPurchaseListener
{
	private static string IAP_RECEIPT_VERIFICATION_URL = "verify_purchase_android.php";

	private bool BillingSupported = true;

	private int busyCounter;

	private static int uniqhandle;

	private PurchaseManager.ReceivedProductDataCallback receivedProductDataCallback;

	private PurchaseManager.ProductPurchaseCallback productPurchaseCallback;

	private PurchaseManager.VerifyIAPReceiptCallback verifyIAPReceiptCallback;

	private PurchaseManager.VerifyGMReceiptCallback m_receiptCallback;

	private List<GooglePurchase> oldPurchases = new List<GooglePurchase>();

	public void OnEnable()
	{
		GoogleIABManager.billingSupportedEvent += AndBillingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += AndBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += AndQueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += AndQueryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += AndPurchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += AndPurchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += AndPurchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += AndConsumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += AndConsumePurchaseFailedEvent;
		string empty = string.Empty;
		empty = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCJbbeSj9zObpxV4zXRwKk0fU6bCsMbL0O3kCwLH/MVPdQz0RkNHeNUuqP9ITZx3zt1tXobPJ440rC/t2lWZ7SDNzxJ9MXRjctvUBqwoerb6EpTHYC/4bPcmICkOKNPPzx1R7wRU8K5LIspqD+yPY0Dh28570yY5ZKa7s5CTH9rOWGslC3/lPPFoErxp+XY9cG9TH7xk8zwtHytD7kLhmHBZuCoCcUVeduZFIRIaNdbTJcpvgY4s6CTgmdKdipGBY3f2BtI0dXjW4bHWMBvlCy4+GCABc27y0Q86FGQ8R8wp4k+TYDX4s0wGFETvOBwC/8PXeNb6Gtqol5OOAflLLQIDAQAB";
		GoogleIAB.init(empty);
	}

	public void OnDisable()
	{
		GoogleIABManager.billingSupportedEvent -= AndBillingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= AndBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= AndQueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= AndQueryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= AndPurchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= AndPurchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= AndPurchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= AndConsumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= AndConsumePurchaseFailedEvent;
	}

	public void Fetch()
	{
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		if (BillingSupported)
		{
			receivedProductDataCallback = a_Callback;
			busyCounter++;
			GoogleIAB.queryInventory(a_ProductIDs);
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		if (a_Callback != null)
		{
			a_Callback(false, null, "ProductDataRequestResult.CannotMakePayment");
		}
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void GettingProductDataTimedOut()
	{
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
		productPurchaseCallback = a_Callback;
		GoogleIAB.purchaseProduct(a_ProductID);
	}

	public void ConsumeProduct(string a_productID)
	{
		GoogleIAB.consumeProduct(a_productID);
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback restorePurchaseCB = null)
	{
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		verifyIAPReceiptCallback = callback;
		return _VerifyIAPReceipt(((GooglePurchase)transaction.NativeTransaction).originalJson, ((GooglePurchase)transaction.NativeTransaction).signature, ((GooglePurchase)transaction.NativeTransaction).productId);
	}

	private KFFNetwork.WWWInfo _VerifyIAPReceipt(string a_OriginalJson, string a_Signature, string a_ProductID)
	{
		string text = PurchaseManager.IAP_VERIFICATION_SERVER_URL + "/" + IAP_RECEIPT_VERIFICATION_URL + "?";
		if (PurchaseManager.IAP_SANDBOX)
		{
			text += "sandbox=1&";
		}
		text = text + "language=" + KFFCSUtils.GetLanguageCode();
		text += "&subdirectory=data_1.01";
		text += "&json=1";
		string playerCode = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
		text = text + "&userid=" + playerCode;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("signedData", a_OriginalJson);
		wWWForm.AddField("signature", a_Signature);
		return KFFNetwork.GetInstance().SendWWWRequestWithForm(wWWForm, text, WWWVerifyReceiptCallback, a_ProductID);
	}

	public void ProcessOldPurchases()
	{
		if (oldPurchases.Count > 0 && PurchaseManager.ValidateRedeemOldPurchaseEvent())
		{
			if (oldPurchases[0].purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				VerifyOldReceiptGameServer(oldPurchases[0].originalJson, oldPurchases[0].signature, oldPurchases[0].productId);
			}
			else
			{
				GoogleIAB.consumeProduct(oldPurchases[0].productId);
			}
			oldPurchases.RemoveAt(0);
		}
	}

	public void VerifyOldReceiptGameServer(string originalJson, string signature, string productId)
	{
		Session theSession = SessionManager.Instance.theSession;
		string partial = Singleton<PurchaseManager>.Instance.m_partial;
		VerifyReceiptGameServer(theSession, null, originalJson, productId, signature, partial, null);
	}

	public int VerifyReceiptGameServer(Session session, PurchaseManager.TransactionData transaction, string receipt, string product_id, string transaction_id, string partial, PurchaseManager.VerifyGMReceiptCallback callback)
	{
		string playerCode = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
		string ret_product_id = string.Empty;
		string ret_transaction_id = string.Empty;
		string ret_key1 = string.Empty;
		string ret_key2 = string.Empty;
		int ret_handle = -1;
		m_receiptCallback = callback;
		uniqhandle++;
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
			{
				string text = "ff689bd#41e5_44fabae87theb7ea";
				if (data.ContainsKey("data"))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					if (dictionary.Count > 0 && dictionary.ContainsKey("purchase"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["purchase"];
						if (dictionary2.Count > 0 && dictionary2.ContainsKey("product_id"))
						{
							ret_product_id = dictionary2["product_id"].ToString();
						}
					}
					if (dictionary.Count > 0 && dictionary.ContainsKey("transaction_key1"))
					{
						ret_key1 = dictionary["transaction_key1"].ToString();
					}
					if (dictionary.Count > 0 && dictionary.ContainsKey("transaction_key2"))
					{
						ret_key2 = dictionary["transaction_key2"].ToString();
					}
					if (dictionary.Count > 0 && dictionary.ContainsKey("transaction_id"))
					{
						ret_transaction_id = dictionary["transaction_id"].ToString();
					}
					if (dictionary.Count > 0 && dictionary.ContainsKey("handle"))
					{
						ret_handle = Convert.ToInt32(dictionary["handle"]);
					}
				}
				bool flag = true;
				using (HMACSHA256 hMACSHA = new HMACSHA256())
				{
					hMACSHA.Key = Encoding.UTF8.GetBytes(text + partial);
					byte[] bytes = Encoding.UTF8.GetBytes(ret_transaction_id);
					byte[] array = hMACSHA.ComputeHash(bytes);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.AppendFormat("{0:X2}", array[i]);
					}
					int startIndex = 19;
					int length = 16;
					if (string.Compare(ret_key1, stringBuilder.ToString().Substring(startIndex, length), true) != 0)
					{
						flag = false;
					}
					startIndex = 1;
					if (string.Compare(ret_key2, stringBuilder.ToString().Substring(startIndex, length), true) != 0)
					{
						flag = false;
					}
					if (product_id != ret_product_id)
					{
						flag = false;
					}
				}
				if (flag)
				{
					if (m_receiptCallback != null)
					{
						m_receiptCallback(transaction, ret_handle, true);
					}
					else
					{
						PurchaseManager.StartRedeemOldPurchase(product_id, ret_handle);
					}
				}
				else if (m_receiptCallback != null)
				{
					m_receiptCallback(transaction, ret_handle, false);
				}
				else
				{
					PurchaseManager.StartConsumeOldPurchase(product_id);
				}
			}
			else if (status == HttpStatusCode.NotFound)
			{
				if (m_receiptCallback != null)
				{
					m_receiptCallback(transaction, ret_handle, false);
				}
				else
				{
					PurchaseManager.StartConsumeOldPurchase(product_id);
				}
			}
			else if (m_receiptCallback != null)
			{
				m_receiptCallback(transaction, ret_handle, false);
			}
			else
			{
				PurchaseManager.StartConsumeOldPurchase(product_id);
			}
			m_receiptCallback = null;
		};
		string sandbox = ((!PurchaseManager.IAP_SANDBOX) ? "0" : "1");
		session.Server.SavePurchase(transaction_id, "GooglePlay", sandbox, partial, SQSettings.BundleIdentifier, product_id, playerCode, receipt, callback2);
		return ret_handle;
	}

	private void WWWVerifyReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
		bool flag = false;
		string text = (string)param;
		if (wWWRequestResult == null)
		{
		}
		if (wWWRequestResult != null)
		{
			if (wWWRequestResult.IsValid())
			{
				string key = "RZrb({~IXRD{Oen[UXZA(a5my-5bZe17JeJMyR-DcuOI2";
				string text2 = "szTn-kDeI=niczdj8BIw{oNG=6|]{{EY%[Ym";
				string valueAsString = wWWRequestResult.GetValueAsString(key);
				if (valueAsString != text2)
				{
					resultObj = null;
					err = KFFLocalization.Get("!!ERROR_PURCHASE_FAILED_RESULT_INVALID");
				}
				else
				{
					flag = true;
				}
			}
			GoogleIAB.consumeProduct(text);
		}
		if (verifyIAPReceiptCallback != null)
		{
			verifyIAPReceiptCallback(wwwinfo, resultObj, err, param, null);
			verifyIAPReceiptCallback = null;
		}
		else if (flag)
		{
			PurchaseManager.RedeemOldPurchase(text);
		}
	}

	private void AndBillingSupportedEvent()
	{
	}

	private void AndBillingNotSupportedEvent(string error)
	{
		BillingSupported = false;
	}

	private void AndQueryInventorySucceededEvent(List<GooglePurchase> a_Purchases, List<GoogleSkuInfo> a_ProductList)
	{
		oldPurchases.Clear();
		Singleton<PurchaseManager>.Instance.m_Products.Clear();
		foreach (GooglePurchase a_Purchase in a_Purchases)
		{
			oldPurchases.Add(a_Purchase);
		}
		ProcessOldPurchases();
		busyCounter--;
		foreach (GoogleSkuInfo a_Product in a_ProductList)
		{
			PurchaseManager.ProductData productData = new PurchaseManager.ProductData();
			productData.ProductIdentifier = a_Product.productId;
			productData.Title = a_Product.title;
			productData.Price = a_Product.price;
			productData.Description = a_Product.description;
			productData.FormattedPrice = a_Product.price;
			Singleton<PurchaseManager>.Instance.m_Products.Add(productData);
		}
		if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(true, Singleton<PurchaseManager>.Instance.m_Products, null);
		}
	}

	private void AndQueryInventoryFailedEvent(string error)
	{
		if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(false, null, null);
		}
	}

	private void AndPurchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
	}

	private void AndPurchaseSucceededEvent(GooglePurchase a_Transaction)
	{
		busyCounter--;
		if (productPurchaseCallback != null)
		{
			PurchaseManager.TransactionData transactionData = new PurchaseManager.TransactionData();
			transactionData.NativeTransaction = a_Transaction;
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Success, transactionData, null);
		}
	}

	private void AndPurchaseFailedEvent(string error, int arg)
	{
		busyCounter--;
		ProcessOldPurchases();
		if (productPurchaseCallback != null)
		{
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Failed, null, error);
		}
	}

	private void AndConsumePurchaseSucceededEvent(GooglePurchase purchase)
	{
	}

	private void AndConsumePurchaseFailedEvent(string error)
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
