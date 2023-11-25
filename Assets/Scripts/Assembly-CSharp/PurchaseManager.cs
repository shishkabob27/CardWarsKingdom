using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JsonFx.Json;
using Prime31;
using UnityEngine;

public class PurchaseManager : Singleton<PurchaseManager>
{
	public enum ProductDataRequestResult
	{
		Success,
		CannotMakePayment
	}

	public enum ProductPurchaseResult
	{
		Success,
		Failed,
		VerificationFailed,
		Cancelled
	}

	public class ProductData
	{
		public string ProductIdentifier;

		public string Title;

		public string Description;

		public string Price;

		public string CurrencySymbol;

		public string CurrencyCode;

		public string FormattedPrice;

		public string CountryCode;

		public override string ToString()
		{
			return string.Format("<ProductData>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nCurrency Symbol: {4}\nFormatted Price: {5}\nCurrency Code: {6}\nCountry Code: {7}", ProductIdentifier, Title, Description, Price, CurrencySymbol, FormattedPrice, CurrencyCode, CountryCode);
		}
	}

	public class TransactionData
	{
		public object NativeTransaction;

		public override string ToString()
		{
			GooglePurchase googlePurchase = (GooglePurchase)NativeTransaction;
			return string.Format("<GooglePurchase> ID: {0}, type: {1}, transactionIdentifier: {2}", googlePurchase.productId, googlePurchase.type, googlePurchase.orderId);
		}
	}

	public delegate void ReceivedProductDataCallback(bool success, List<ProductData> list, string err);

	public delegate void FinalProductPurchaseCallback(ProductPurchaseResult result);

	public delegate void ProductPurchaseCallback(ProductPurchaseResult result, TransactionData transaction, string err);

	public delegate void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param, string hash);

	public delegate void RestorePurchasesCallback(bool success);

	public delegate void VerifyGMReceiptCallback(TransactionData store, int handle, bool success);

	private const float productDataTimeoutLimit = 5f;

	private bool m_Purchasing;

	public List<ProductData> m_Products = new List<ProductData>();

	private bool AmazonDevice;

	private IPurchaseListener m_Listener;

	private FinalProductPurchaseCallback m_PurchaseCallback;

	private KFFNetwork.WWWInfo m_VerifyPurchase;

	private static int m_VerifyPurchaseHandle = -1;

	private ProductData m_RequestedProduct;

	private bool m_ReceiptVerificationEnd;

	private TransactionData m_storeKit;

	private bool m_success;

	public string m_partial = "4294246h";

	private string m_oldReceipt;

	private static string m_OldPurchaseProduct = string.Empty;

	private static bool m_WaitForOldPurchase;

	private static bool m_WaitForOldConsume;

	public static string CHECK_CLIENT_VERSION_URL = "https://iap-loadbalancer.kffgames.com/DragonWars/IAPReceiptVerificationServer/check_client_version.php";

	public static string CHECK_ASSET_DOWNLOADS_URL = "https://iap-loadbalancer.kffgames.com/DragonWars/IAPReceiptVerificationServer/check_asset_downloads.php";

	public bool InPurchaseProcess
	{
		get
		{
			return m_Purchasing;
		}
	}

	public bool IsAmazon
	{
		get
		{
			return AmazonDevice;
		}
		private set
		{
		}
	}

	public int getLastHandle
	{
		get
		{
			int verifyPurchaseHandle = m_VerifyPurchaseHandle;
			m_VerifyPurchaseHandle = -1;
			return verifyPurchaseHandle;
		}
	}

	public static string IAP_VERIFICATION_SERVER_URL
	{
		get
		{
			return "https://iap-loadbalancer.kffgames.com/DragonWars/IAPReceiptVerificationServer";
		}
	}

	public static bool IAP_SANDBOX
	{
		get
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return false;
			case RuntimePlatform.Android:
				return MiscParams.UseGooglePlayKFFKey;
			default:
				return false;
			}
		}
	}

	public static event Action<string> RedeemOldProductEvent;

	public void GetPriceInfo(string ProductID, out float Price, out string CurrencyType)
	{
		foreach (ProductData product in m_Products)
		{
			if (!(product.ProductIdentifier == ProductID))
			{
				continue;
			}
			string text = string.Empty;
			bool flag = false;
			for (int i = 0; i < product.Price.Length; i++)
			{
				if (char.IsDigit(product.Price[i]))
				{
					text += product.Price[i];
				}
				if (char.IsPunctuation(product.Price[i]))
				{
					flag = true;
				}
			}
			Price = float.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
			if (flag)
			{
				Price /= 100f;
			}
			CurrencyType = product.CurrencyCode;
			if (CurrencyType == null)
			{
				CurrencyType = "USD";
			}
			return;
		}
		Price = 0f;
		CurrencyType = "USD";
	}

	public void GetformattedPrice(string ProductID, out string Price)
	{
		foreach (ProductData product in m_Products)
		{
			if (product.ProductIdentifier == ProductID)
			{
				Price = product.FormattedPrice;
				return;
			}
		}
		Price = "0";
	}

	public void GetProductItem(string ProductID, out ProductData product)
	{
		foreach (ProductData product2 in m_Products)
		{
			if (product2.ProductIdentifier == ProductID)
			{
				product = product2;
				return;
			}
		}
		product = null;
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		DetectAmazonDevice();
		m_Listener = new GooglePurchaseListener();
	}

	private void OnEnable()
	{
		if (m_Listener != null)
		{
			m_Listener.OnEnable();
		}
	}

	private void OnDisable()
	{
		if (m_Listener != null)
		{
			m_Listener.OnDisable();
		}
	}

	private void OnDestroy()
	{
		if (m_Listener != null)
		{
			m_Listener.OnDisable();
		}
	}

	private void Update()
	{
		if (m_Listener != null)
		{
			m_Listener.Fetch();
		}
		if (m_ReceiptVerificationEnd)
		{
			m_ReceiptVerificationEnd = false;
			VerifyCallbackGM();
		}
		if (m_WaitForOldPurchase)
		{
			m_WaitForOldPurchase = false;
			RedeemOldPurchase(m_OldPurchaseProduct);
			Singleton<PurchaseManager>.Instance.ConsumeProduct(m_OldPurchaseProduct);
		}
		if (m_WaitForOldConsume)
		{
			m_WaitForOldConsume = false;
			GoogleIAB.consumeProduct(m_OldPurchaseProduct);
		}
	}

	private void DetectAmazonDevice()
	{
		AmazonDevice = false;
	}

	public ProductDataRequestResult GetProductData(string[] a_StringIDs, ReceivedProductDataCallback a_Callback)
	{
		if (a_StringIDs.Length > 0)
		{
		}
		return m_Listener.GetProductData(a_StringIDs, a_Callback);
	}

	private IEnumerator StartGetProductDataTimer()
	{
		yield return new WaitForSeconds(5f);
		m_Listener.GettingProductDataTimedOut();
	}

	public void PurchaseProduct(string a_productID, FinalProductPurchaseCallback a_Callback)
	{
		m_Purchasing = true;
		GetProductItem(a_productID, out m_RequestedProduct);
		if (m_RequestedProduct != null)
		{
		}
		if (m_Listener != null)
		{
			m_PurchaseCallback = a_Callback;
			m_Listener.PurchaseProduct(a_productID, InternalPurchaseCallback);
		}
	}

	private void InternalPurchaseCallback(ProductPurchaseResult result, TransactionData transaction, string err)
	{
		if (!m_Purchasing)
		{
			return;
		}
		m_Purchasing = false;
		switch (result)
		{
		case ProductPurchaseResult.Success:
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			GooglePurchase googlePurchase = (GooglePurchase)transaction.NativeTransaction;
			empty = googlePurchase.originalJson;
			empty2 = googlePurchase.productId;
			empty3 = googlePurchase.signature;
			if (empty == m_oldReceipt)
			{
				m_PurchaseCallback(ProductPurchaseResult.Cancelled);
				break;
			}
			m_oldReceipt = googlePurchase.originalJson;
			Session theSession = SessionManager.Instance.theSession;
			m_ReceiptVerificationEnd = false;
			m_VerifyPurchaseHandle = Singleton<PurchaseManager>.Instance.VerifyReceiptGameServer(theSession, transaction, empty, empty2, empty3, m_partial, VerifyCallbackSet);
			break;
		}
		case ProductPurchaseResult.Failed:
			m_PurchaseCallback(result);
			break;
		case ProductPurchaseResult.Cancelled:
			m_PurchaseCallback(result);
			break;
		case ProductPurchaseResult.VerificationFailed:
			break;
		}
	}

	private void VerifyCallbackSet(TransactionData storekit, int handle, bool success)
	{
		m_storeKit = storekit;
		m_success = success;
		m_ReceiptVerificationEnd = true;
		m_VerifyPurchaseHandle = handle;
	}

	private void VerifyCallbackGM()
	{
		if (!m_success)
		{
			m_PurchaseCallback(ProductPurchaseResult.VerificationFailed);
			return;
		}
		TransactionData storeKit = m_storeKit;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string text;
		GooglePurchase googlePurchase = (GooglePurchase)m_storeKit.NativeTransaction;
		text = googlePurchase.productId;
		empty = googlePurchase.originalJson;
		empty2 = googlePurchase.productId;
		empty3 = googlePurchase.signature;
		float Price;
		string CurrencyType;
		GetPriceInfo(text, out Price, out CurrencyType);
		Singleton<PurchaseManager>.Instance.ConsumeProduct(text);
		m_PurchaseCallback(ProductPurchaseResult.Success);
		m_storeKit = null;
		m_success = false;
	}

	private void VerifyCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object para, string hash)
	{
	}

	public void ProcessOldPurchases()
	{
		List<CurrencyPackageData> database = CurrencyPackageDataManager.Instance.GetDatabase();
		List<string> list = new List<string>();
		foreach (CurrencyPackageData item in database)
		{
			list.Add(item.ID);
		}
		string[] a_StringIDs = list.ToArray();
		GetProductData(a_StringIDs, null);
	}

	public void ConsumeProduct(string a_productID)
	{
		if (m_Listener != null)
		{
			m_Listener.ConsumeProduct(a_productID);
		}
	}

	public void RestorePurchases(RestorePurchasesCallback callback = null)
	{
		Debug.Log("calling KFF.RestorePurchases");
		StartCoroutine(CoroutineRestorePurchase(callback));
	}

	private IEnumerator CoroutineRestorePurchase(RestorePurchasesCallback callback)
	{
		yield return null;
		if (m_Listener != null)
		{
			m_Listener.RestorePurchases(callback);
		}
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(TransactionData transaction, VerifyIAPReceiptCallback callback)
	{
		if (m_Listener != null)
		{
			return m_Listener.VerifyIAPReceipt(transaction, callback);
		}
		return null;
	}

	public int VerifyReceiptGameServer(Session session, TransactionData transaction, string receipt, string productid, string transactionid, string partial, VerifyGMReceiptCallback callback)
	{
		if (m_Listener != null)
		{
			return m_Listener.VerifyReceiptGameServer(session, transaction, receipt, productid, transactionid, partial, callback);
		}
		return -1;
	}

	public void ExecuteSaveCurrentOldPurchases()
	{
	}

	public static bool ValidateRedeemOldPurchaseEvent()
	{
		return PurchaseManager.RedeemOldProductEvent != null;
	}

	public static void RedeemOldPurchase(string a_ProductID)
	{
		if (ValidateRedeemOldPurchaseEvent())
		{
			PurchaseManager.RedeemOldProductEvent(a_ProductID);
		}
	}

	public static void StartRedeemOldPurchase(string a_ProductID, int handle)
	{
		m_WaitForOldPurchase = true;
		m_OldPurchaseProduct = a_ProductID;
		m_VerifyPurchaseHandle = handle;
	}

	public static void StartConsumeOldPurchase(string a_ProductID)
	{
		m_WaitForOldConsume = true;
		m_OldPurchaseProduct = a_ProductID;
	}

	public ProductDataRequestResult GetProductData(ReceivedProductDataCallback a_Callback)
	{
		List<CurrencyPackageData> database = CurrencyPackageDataManager.Instance.GetDatabase();
		List<string> list = new List<string>();
		foreach (CurrencyPackageData item in database)
		{
			list.Add(item.ID);
		}
		string[] a_StringIDs = list.ToArray();
		return GetProductData(a_StringIDs, a_Callback);
	}
}
