using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScreenController : Singleton<StoreScreenController>
{
	public enum StoreKPITracking
	{
		BuyHeroes,
		ReplenishStamina,
		IncreaseInventory,
		ExpandAllyList,
		BuyProduct
	}

	public class FoundProductData
	{
		public PurchaseManager.ProductData ProductData;

		public CurrencyPackageData PackageData;
	}

	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	private const string IntroPackageName = "DW_Special_Sale";

	public int BlankLeaderSlots;

	public float HardCurrencyTickTime;

	public GameObject MainPanel;

	public GameObject StoreEntryPrefab;

	public GameObject HeroPrefab;

	public GameObject[] StoreEntryVFXPrefabs = new GameObject[6];

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController HardCurrencyGainTween;

	public UILabel TitleLabel;

	public UILabel StaminaRefillCost;

	public UILabel InventoryIncreaseCost;

	public UILabel InventoryIncreaseAmount;

	public UILabel AllyIncreaseCost;

	public UILabel AllyIncreaseAmount;

	public UILabel HardCurrencyAvailable;

	public UILabel CustomizationCurrencyAvailable;

	public UIStreamingGrid HeroGrid;

	private UIStreamingGridDataSource<LeaderData> mHeroGridDataSource = new UIStreamingGridDataSource<LeaderData>();

	public UILabel HeroName;

	public UILabel HeroAge;

	public UILabel HeroHeight;

	public UILabel HeroWeight;

	public UILabel HeroSpecies;

	public UITexture HeroImage;

	public GameObject HeroCardNodeParent;

	private List<GameObject> HeroCardNodes = new List<GameObject>();

	public UILabel HeroCost;

	public GameObject HeroPurchaseButton;

	public UIStreamingGrid StoreGrid;

	private UIStreamingGridDataSource<FoundProductData> mStoreGridDataSource = new UIStreamingGridDataSource<FoundProductData>();

	public GameObject HeroButtonLock;

	public Collider HeroButtonCollider;

	public GameObject CustomizationButtonLock;

	public Collider CustomizationButtonCollider;

	public UIStreamingGrid SkinGrid;

	private UIStreamingGridDataSource<LeaderData> mSkinGridDataSource = new UIStreamingGridDataSource<LeaderData>();

	public UIGrid CustomizationPurchaseGrid;

	public UILabel SkinName;

	public UITexture SkinImage;

	public UILabel SkinCost;

	public UILabel SkinButtonLabel;

	public GameObject BuySkinButton;

	public GameObject SkinEquippedParent;

	public UIGrid TabsGrid;

	public UIToggle HeroTabButton;

	public UIToggle HardCurrencyTabButton;

	public UIToggle CustomizationTabButton;

	public GameObject SaleButton;

	public UILabel HeroAvailableLabel;

	[Header("Loc Strings for UIToggle Titles")]
	public string SaleTitleText = "!!SALE";

	public string GemsTitleText = "!!CURRENCY_HARD";

	public string ExtrasTitleText = "!!EXTRAS";

	public string HeroesTitleText = "!!HEROES";

	public string CustomizeTitleText = "!!CUSTOMIZE";

	private List<FoundProductData> mFetchedList;

	private List<FoundProductData> mHiddenFetchedList;

	private static PurchaseManager.ProductData mProductIdBeingPurchased;

	private static bool mSubscribedToEvent;

	private LeaderData mHighlightedLeader;

	private List<CardPrefabScript> mSpawnedCards = new List<CardPrefabScript>();

	private LeaderData mHighlightedSkin;

	private float mDisplayedHardCurrency;

	private float mHardCurrencyTickRate;

	private List<LeaderData> mHeroList;

	private List<LeaderData> mSkinList;

	private bool mWaitForUpsight;

	private float mUpsightWaitTimer = -1f;

	private static bool mWaitForGrantProduct;

	private static string mGrantProductId = string.Empty;

	private static bool mSkipHardCurrencyTick;

	private static bool mWaitForGrantSaleProduct;

	private static SpecialSaleData mSaleData;

	private static PurchaseManager.ProductData mProductData;

	private static bool mShowResultsNow;

	private static bool mWaitForCancelPurchase;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public bool OpenedFromTown { get; set; }

	public bool OpenToHardCurrency { get; set; }

	public bool OpenToCustomization { get; set; }

	public bool OpenToHeroes { get; set; }

	public bool isProcessingPurchase { get; set; }

	public void Awake()
	{
		isProcessingPurchase = false;
		if (!mSubscribedToEvent)
		{
			PurchaseManager.RedeemOldProductEvent += GrantOldProduct;
			mSubscribedToEvent = true;
		}
		for (int i = 0; i < 5; i++)
		{
			HeroCardNodes.Add(HeroCardNodeParent.FindInChildren("ActionCardSpawn_0" + (i + 1)));
		}
		Singleton<PurchaseManager>.Instance.ProcessOldPurchases();
	}

	public void Populate()
	{
		OpenedFromTown = true;
		OpenToHardCurrency = true;
		mDisplayedHardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		mHardCurrencyTickRate = 9999f;
		StaminaRefillCost.text = MiscParams.StaminaRefillCost.ToString();
		InventoryIncreaseCost.text = MiscParams.InventorySpacePurchaseCost.ToString();
		InventoryIncreaseAmount.text = "+" + MiscParams.InventorySpacePerPurchase;
		AllyIncreaseCost.text = MiscParams.AllyBoxPurchaseCost.ToString();
		AllyIncreaseAmount.text = "+" + MiscParams.AllyBoxPerPurchase;
		PopulateHeroList();
		PopulateSkinList();
		bool flag = Singleton<TutorialController>.Instance.IsBlockActive("BuyHero");
		LeaderData leaderData = null;
		foreach (LeaderData mHero in mHeroList)
		{
			if (flag)
			{
				if (mHero == MiscParams.TutorialHero)
				{
					leaderData = mHero;
					break;
				}
				continue;
			}
			if (leaderData == null)
			{
				leaderData = mHero;
			}
			if (Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(mHero))
			{
				continue;
			}
			leaderData = mHero;
			break;
		}
		if (leaderData == null)
		{
			leaderData = mHeroList[0];
		}
		PopulateLeader(leaderData);
		bool flag2 = Singleton<PlayerInfoScript>.Instance.CanBuyLeaders();
		HeroButtonLock.SetActive(!flag2);
		HeroButtonCollider.enabled = flag2;
		bool flag3 = flag2 && mSkinList.Count > 0;
		CustomizationButtonLock.SetActive(!flag3);
		CustomizationButtonCollider.enabled = flag3;
		if (flag3)
		{
			if (mHighlightedSkin != null)
			{
				PopulateSkin(mHighlightedSkin);
			}
			else
			{
				PopulateSkin(SkinGrid.transform.GetChild(0).GetComponent<StoreHeroPrefab>().Leader);
			}
		}
		if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
		{
			Singleton<VersionChecker>.Instance.CheckVersion(OnVersionCheckComplete);
		}
		else
		{
			OnVersionCheckComplete();
		}
	}

	private void OnVersionCheckComplete()
	{
		Singleton<BusyIconPanelController>.Instance.Show();
		//Singleton<PurchaseManager>.Instance.GetProductData(ProductDataCallback);
		PopulateDummyData();
	}

	public void SetTitleLabel(string inLabelText)
	{
		if (TitleLabel != null)
		{
			TitleLabel.text = KFFLocalization.Get("!!BUILDING_STORE") + ": " + KFFLocalization.Get(inLabelText);
		}
	}

	private void PopulateHeroList()
	{
		mHeroList = new List<LeaderData>(LeaderDataManager.Instance.GetDatabase().FindAll((LeaderData m) => m.ShowInSaleList()));
		mHeroGridDataSource.Init(HeroGrid, HeroPrefab, mHeroList);
	}

	private void PopulateSkinList()
	{
		mSkinList = new List<LeaderData>();
		foreach (LeaderData mHero in mHeroList)
		{
			if (mHero == null)
			{
				continue;
			}
			LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(mHero.ID);
			foreach (LeaderData alternateSkin in mHero.AlternateSkins)
			{
				if (alternateSkin.BuyableSkin || (leaderItem != null && leaderItem.OwnedSkins.Contains(alternateSkin)))
				{
					mSkinList.Add(alternateSkin);
				}
			}
		}
		mSkinGridDataSource.Init(SkinGrid, HeroPrefab, mSkinList);
	}

	private StoreHeroPrefab FindHeroTile(LeaderData leader)
	{
		for (int i = 0; i < HeroGrid.transform.childCount; i++)
		{
			StoreHeroPrefab component = HeroGrid.transform.GetChild(i).GetComponent<StoreHeroPrefab>();
			if (component.Leader == leader)
			{
				return component;
			}
		}
		return null;
	}

	private void Update()
	{
		RefreshCurrency();
		if (mWaitForUpsight)
		{
			bool flag = true;
			mUpsightWaitTimer -= Time.deltaTime;
			if (mUpsightWaitTimer <= 0f)
			{
				mUpsightWaitTimer = -1f;
				flag = true;
			}
			if (flag)
			{
				mWaitForUpsight = false;
				Singleton<BusyIconPanelController>.Instance.Hide();
				if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd() && Singleton<PlayerInfoScript>.Instance.GetSaleToDisplay(true) != null)
				{
					SaleButton.SetActive(true);
					TabsGrid.Reposition();
					SpecialSaleData saleToDisplay = Singleton<PlayerInfoScript>.Instance.GetSaleToDisplay();
					//if (saleToDisplay != null)
					//{
					//	Singleton<SalePopupController>.Instance.Show(saleToDisplay, SalePopupController.ShowLocation.StoreScreen);
					//}
					//else
					//{
						AdvanceTutorialPastSale();
					//}
				}
				else
				{
					SaleButton.SetActive(false);
					TabsGrid.Reposition();
					AdvanceTutorialPastSale();
				}
			}
		}
		if (mWaitForUserAction)
		{
			if (mUserActionProceed == NextAction.PROCEED)
			{
				Singleton<BusyIconPanelController>.Instance.Hide();
				mWaitForUserAction = false;
				mUserActionProceed = NextAction.NONE;
				if (mNextFunction != null)
				{
					mNextFunction();
				}
				mUserActionProceed = NextAction.NONE;
			}
			if (mUserActionProceed == NextAction.ERROR)
			{
				mWaitForUserAction = false;
				mUserActionProceed = NextAction.NONE;
				Singleton<BusyIconPanelController>.Instance.Hide();
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), OnCloseServerAccessErrorPopup);
			}
		}
		if (mWaitForGrantProduct)
		{
			mWaitForGrantProduct = false;
			ExecGrantProduct(mGrantProductId, mSkipHardCurrencyTick);
		}
		if (mWaitForGrantSaleProduct)
		{
			mWaitForGrantSaleProduct = false;
			ExecGrantSaleProduct(mSaleData, mProductData, mShowResultsNow);
		}
		if (mWaitForCancelPurchase)
		{
			mWaitForCancelPurchase = false;
			Singleton<BusyIconPanelController>.Instance.Hide();
		}
	}

	private void OnCloseServerAccessErrorPopup()
	{
	}

	private void AdvanceTutorialPastSale()
	{
		Singleton<TutorialController>.Instance.AdvanceIfOnState("BA_ExplainBank");
		Singleton<TutorialController>.Instance.AdvanceIfOnState("BA_InSale");
	}

	private IEnumerator JumpToTab()
	{
		yield return new WaitForEndOfFrame();
		if (OpenToHeroes)
		{
			OpenToHeroes = false;
			HeroTabButton.gameObject.SendMessage("OnClick");
		}
		else if (OpenToCustomization)
		{
			OpenToCustomization = false;
			CustomizationTabButton.gameObject.SendMessage("OnClick");
		}
		else if (OpenToHardCurrency)
		{
			OpenToCustomization = false;
			HardCurrencyTabButton.gameObject.SendMessage("OnClick");
		}
	}

	private int GetCurrency(string googleCurrecny)
	{
		char[] anyOf = new char[23]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'.', ',', '０', '１', '２', '３', '４', '５', '６', '７',
			'８', '９', '，'
		};
		int num = googleCurrecny.IndexOfAny(anyOf);
		int num2 = googleCurrecny.LastIndexOfAny(anyOf);
		int num3 = num2 - num + 1;
		if (num < 0 || num2 < 0 || num3 < 0)
		{
			return 0;
		}
		string s = googleCurrecny.Substring(num, num3);
		int result = 0;
		try
		{
			float num4 = float.Parse(s) * 100f;
			result = (int)num4;
			return result;
		}
		catch (Exception)
		{
			return result;
		}
	}

	private void ProductDataCallback(bool a_Success, List<PurchaseManager.ProductData> a_ProductList, string error)
	{
		Debug.Log(string.Format("StoreScreenController::ProductDataCallback, success={0} error={1}", a_Success, error));
		/*
		if (a_Success && a_ProductList != null && a_ProductList.Count > 0)
		{
			Debug.Log(string.Format("StoreScreenController::ProductDataCallback, num products={0}", a_ProductList.Count));
			mFetchedList = new List<FoundProductData>();
			mHiddenFetchedList = new List<FoundProductData>();
			a_ProductList.Sort((PurchaseManager.ProductData a, PurchaseManager.ProductData b) => GetCurrency(a.Price) - GetCurrency(b.Price));
			foreach (PurchaseManager.ProductData a_Product in a_ProductList)
			{
				FoundProductData foundProductData = new FoundProductData();
				foundProductData.ProductData = a_Product;
				foundProductData.PackageData = CurrencyPackageDataManager.Instance.GetData(a_Product.ProductIdentifier);
				if (foundProductData.PackageData != null)
				{
					if (foundProductData.PackageData.ShowInStore)
					{
						mFetchedList.Add(foundProductData);
					}
					else
					{
						mHiddenFetchedList.Add(foundProductData);
					}
				}
			}
			ShowTween.Play();
			StartCoroutine(JumpToTab());
			mStoreGridDataSource.Init(StoreGrid, StoreEntryPrefab, mFetchedList);
			CustomizationPurchaseGrid.transform.DestroyAllChildren();
			for (int i = 1; i <= 2; i++)
			{
				string id = "dw.customization.package.0" + i;
				FoundProductData foundProductData2 = mHiddenFetchedList.Find((FoundProductData m) => m.PackageData.ID == id);
				if (foundProductData2 != null)
				{
					StoreEntryScript component = CustomizationPurchaseGrid.transform.InstantiateAsChild(StoreEntryPrefab).GetComponent<StoreEntryScript>();
					component.Populate(foundProductData2);
				}
			}
			CustomizationPurchaseGrid.Reposition();
			Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Store");
			RefreshCurrency();
			Singleton<BusyIconPanelController>.Instance.Hide();
			if (!Singleton<TutorialController>.Instance.IsBlockActive("Bank") && !Singleton<TutorialController>.Instance.IsBlockActive("BuyHero"))
			{
				ShowIntroSalePurchasePrompt();
			}
		}
		else
		{*/
			Singleton<BusyIconPanelController>.Instance.Hide();
			if (Singleton<TutorialController>.Instance.IsBlockActive("Bank"))
			{
				StartCoroutine(SkipStoreTutorial());
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), RetryConnection, CancelConnection, KFFLocalization.Get("!!RETRY"), KFFLocalization.Get("!!CANCEL"));
			}
		//}
	}

	private void RetryConnection()
	{
		Singleton<BusyIconPanelController>.Instance.Show();
		PopulateDummyData();
	}

	private void CancelConnection()
	{
		if (OpenedFromTown)
		{
			Singleton<TownHudController>.Instance.ReturnToTownView();
		}
		if (Singleton<TutorialController>.Instance.IsStateActive("BA_TapBank"))
		{
			for (int i = 0; i < 3; i++)
			{
				Singleton<TutorialController>.Instance.AdvanceTutorialState();
			}
		}
		else if (Singleton<TutorialController>.Instance.IsStateActive("HR_TapStore"))
		{
			if (!Singleton<PlayerInfoScript>.Instance.SaveData.Leaders.Contains((LeaderItem m) => m.Form == MiscParams.TutorialHero))
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.Leaders.Add(new LeaderItem(MiscParams.TutorialHero));
			}
			for (int j = 0; j < 6; j++)
			{
				Singleton<TutorialController>.Instance.AdvanceTutorialState();
			}
		}
	}

	public void ShowIntroSalePurchasePrompt()
	{
		mUpsightWaitTimer = 5f;
		mWaitForUpsight = true;
		Singleton<BusyIconPanelController>.Instance.Show();
		UpsightRequester.RequestContent("store_menu");
	}

	public void OnEntryClicked(FoundProductData productData)
	{
		if (isProcessingPurchase)
		{
			return;
		}
		mWaitForUpsight = false;
		isProcessingPurchase = true;
		Singleton<PlayerInfoScript>.Instance.Save(delegate(bool success)
		{
			if (!success)
			{
				HideTween.Play();
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!RELOAD_DATA_PROMPT"), ReloadFromServer, SimplePopupController.PopupPriority.ServerError);
			}
			else
			{
				mProductIdBeingPurchased = productData.ProductData;
				Singleton<BusyIconPanelController>.Instance.Show();
				Singleton<PurchaseManager>.Instance.PurchaseProduct(mProductIdBeingPurchased.ProductIdentifier, PurchaseCallback);
			}
			isProcessingPurchase = false;
		});
	}

	private void ReloadFromServer()
	{
		SessionManager.Instance.theSession.WebFileServer.DeleteETagFile();
		SessionManager.Instance.theSession.ReloadGame();
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result)
	{
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			GrantProduct(mProductIdBeingPurchased.ProductIdentifier);
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
			CancelPurchese();
			break;
		default:
			CancelPurchese();
			break;
		}
	}

	public void RefreshCurrency()
	{
		if (HardCurrencyAvailable.gameObject.activeInHierarchy)
		{
			int hardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
			if ((float)hardCurrency > mDisplayedHardCurrency)
			{
				mDisplayedHardCurrency = mDisplayedHardCurrency.TickTowards(hardCurrency, mHardCurrencyTickRate);
			}
			else
			{
				mDisplayedHardCurrency = hardCurrency;
			}
			HardCurrencyAvailable.text = ((int)(mDisplayedHardCurrency + 0.001f)).ToString();
			CustomizationCurrencyAvailable.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
		}
	}

	public void TriggerHardCurrencyGainTick()
	{
		Singleton<StoreScreenController>.Instance.StartHardCurrencyTick();
	}

	private void StartHardCurrencyTick()
	{
		float num = (float)Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency - mDisplayedHardCurrency;
		if (num > 0f)
		{
			HardCurrencyGainTween.Play();
			if (num > 2f)
			{
				mHardCurrencyTickRate = num / HardCurrencyTickTime;
			}
			else
			{
				mDisplayedHardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
			}
		}
		else
		{
			mDisplayedHardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		}
	}

	public void Unload()
	{
		mHeroList.Clear();
		mSkinList.Clear();
		mStoreGridDataSource.Clear();
		mHeroGridDataSource.Clear();
		mSkinGridDataSource.Clear();
		HeroImage.UnloadTexture();
		SkinImage.UnloadTexture();
		CustomizationPurchaseGrid.transform.DestroyAllChildren();
		UnloadCards();
		if (OpenedFromTown)
		{
			Singleton<TownHudController>.Instance.ReturnToTownView();
		}
	}

	private void UnloadCards()
	{
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			NGUITools.Destroy(mSpawnedCard.gameObject);
		}
		mSpawnedCards.Clear();
	}

	public static void GrantOldProduct(string productId)
	{
		SpecialSaleData specialSaleData = SpecialSaleDataManager.Instance.GetDatabase().Find((SpecialSaleData m) => m.ProductID == productId);
		if (specialSaleData != null)
		{
			GrantSaleProduct(specialSaleData, null);
		}
		else
		{
			GrantProduct(productId);
		}
	}

	public static void GrantSaleProduct(SpecialSaleData sale, PurchaseManager.ProductData productData, bool showResultsNow = false)
	{
		mSaleData = sale;
		mProductData = productData;
		mShowResultsNow = showResultsNow;
		mWaitForGrantSaleProduct = true;
	}

	public static void CancelPurchese()
	{
		mWaitForCancelPurchase = true;
	}

	public static void GrantProduct(string productId, bool skipHardCurrencyTick = false)
	{
		mGrantProductId = productId;
		mSkipHardCurrencyTick = skipHardCurrencyTick;
		mWaitForGrantProduct = true;
	}

	private static void ExecGrantProduct(string productId, bool skipHardCurrencyTick)
	{
		CurrencyPackageData data = CurrencyPackageDataManager.Instance.GetData(productId);
		if (data != null)
		{
			PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
			if (mProductIdBeingPurchased != null)
			{
				Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(data.PaidHardCurrency, data.FreeHardCurrency, productId, Singleton<PurchaseManager>.Instance.getLastHandle, mProductIdBeingPurchased.Price);
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(data.PaidHardCurrency, data.FreeHardCurrency, productId, Singleton<PurchaseManager>.Instance.getLastHandle, string.Empty);
			}
			saveData.PvPCurrency += data.SocialCurrency;
			saveData.SoftCurrency += data.SoftCurrency;
			mProductIdBeingPurchased = null;
			Singleton<BusyIconPanelController>.Instance.Hide();
		}
	}

	public static void ExecGrantSaleProduct(SpecialSaleData sale, PurchaseManager.ProductData productData, bool showResultsNow = false)
	{
		foreach (GeneralReward item in sale.Items)
		{
			if (productData != null)
			{
				item.Grant(sale.ID, productData.CountryCode, productData.Price);
			}
			else
			{
				item.Grant(sale.ID);
			}
		}
		Singleton<PlayerInfoScript>.Instance.Save();
		if (Singleton<StoreScreenController>.Instance != null)
		{
			Singleton<StoreScreenController>.Instance.SaleButton.SetActive(false);
			Singleton<StoreScreenController>.Instance.TabsGrid.Reposition();
		}
		Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Evo);
		SalePopupController.SetGrantedSale(sale, showResultsNow);
		Singleton<BusyIconPanelController>.Instance.Hide();
	}

	private void PopulateDummyData()
	{
		List<PurchaseManager.ProductData> list = new List<PurchaseManager.ProductData>();
		foreach (CurrencyPackageData item in CurrencyPackageDataManager.Instance.GetDatabase())
		{
			PurchaseManager.ProductData productData = new PurchaseManager.ProductData();
			productData.ProductIdentifier = item.ID;
			productData.FormattedPrice = "$0.00";
			list.Add(productData);
		}
		ProductDataCallback(true, list, string.Empty);
	}

	public void OnClickRefillStamina()
	{
		if (!isProcessingPurchase)
		{
			isProcessingPurchase = true;
			if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests) >= Singleton<PlayerInfoScript>.Instance.RankData.Stamina && DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Pvp) >= MiscParams.MaxPvpStamina)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!STAMINA_ALREADY_FULL"));
				isProcessingPurchase = false;
			}
			else if (Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency < MiscParams.StaminaRefillCost)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!STAMINA_NEED_STONES"));
				isProcessingPurchase = false;
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!STAMINA_BUY_PROMPT").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), string.Empty, MiscParams.InventorySpacePurchaseCost, ConfirmRefillStamina, cancelPopup);
			}
		}
	}

	private void ConfirmRefillStamina()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.StaminaRefillCost, "stamina refill", UserActionCallback);
		mNextFunction = StaminaRefillExecute;
		isProcessingPurchase = false;
	}

	public void UserActionCallback(PlayerSaveData.ActionResult result)
	{
		if (result.success)
		{
			mUserActionProceed = NextAction.PROCEED;
		}
		else
		{
			mUserActionProceed = NextAction.ERROR;
		}
	}

	private void StaminaRefillExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<BuyStaminaPopupController>.Instance.Show();
	}

	public void OnClickExpandInventory()
	{
		if (!isProcessingPurchase)
		{
			isProcessingPurchase = true;
			if (Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency < MiscParams.InventorySpacePurchaseCost)
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), KFFLocalization.Get("!!INVENTORY_SLOTS_NOBUY"), MiscParams.InventorySpacePurchaseCost, OnClickConfirmPurchaseSlots);
				isProcessingPurchase = false;
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), string.Empty, MiscParams.InventorySpacePurchaseCost, ConfirmExpandInventory, cancelPopup);
			}
		}
	}

	private void OnClickConfirmPurchaseSlots()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
	}

	private void cancelPopup()
	{
		isProcessingPurchase = false;
	}

	private void ConfirmExpandInventory()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
		isProcessingPurchase = false;
	}

	private void InventorySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyInventorySlots(MiscParams.InventorySpacePerPurchase);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<BuyInventoryPopupController>.Instance.Show();
	}

	public void OnClickExpandAllyList()
	{
		if (!isProcessingPurchase)
		{
			isProcessingPurchase = true;
			if (Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency < MiscParams.AllyBoxPurchaseCost)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ALLY_LIST_NEED_STONES"));
				isProcessingPurchase = false;
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), string.Empty, MiscParams.AllyBoxPurchaseCost, ConfirmExpandAllyList, cancelPopup);
			}
		}
	}

	private void ConfirmExpandAllyList()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.AllyBoxPurchaseCost, "ally space", UserActionCallback);
		mNextFunction = AllySpaceExecute;
		isProcessingPurchase = false;
	}

	private void AllySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyAllySlots(MiscParams.AllyBoxPerPurchase);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ALLY_SLOTS_BOUGHT").Replace("<val1>", saveData.AllyBoxSpace.ToString()));
	}

	public void OnClickBuySoftCurrency()
	{
		if (!isProcessingPurchase)
		{
			isProcessingPurchase = true;
			if (Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency < MiscParams.BuySoftCurrencyCost)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!BUY_GOLD_NEED_STONES"));
				isProcessingPurchase = false;
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!BUY_GOLD_CONFIRM").Replace("<val2>", MiscParams.BuySoftCurrencyAmount.ToString()), string.Empty, MiscParams.BuySoftCurrencyCost, ConfirmBuySoftCurrency, cancelPopup);
			}
		}
	}

	private void ConfirmBuySoftCurrency()
	{
		Singleton<PlayerInfoScript>.Instance.BuyGold(1);
	}

	public void OnLeaderClicked(StoreHeroPrefab heroTile)
	{
		PopulateLeader(heroTile.Leader);
	}

	private void PopulateLeader(LeaderData leader)
	{
		mHighlightedLeader = leader;
		HeroName.text = leader.Name;
		HeroAge.text = leader.FlvAge;
		HeroHeight.text = leader.FlvHeight;
		HeroWeight.text = leader.FlvWeight;
		HeroSpecies.text = leader.FlvSpecies;
		HeroCost.text = leader.BuyCost.ToString();
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(leader.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", HeroImage);
		UnloadCards();
		for (int i = 0; i < 5; i++)
		{
			CardPrefabScript component = HeroCardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
			component.gameObject.ChangeLayer(base.gameObject.layer);
			component.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
			component.Populate(leader.ActionCards[i]);
			component.AdjustDepth(1);
			mSpawnedCards.Add(component);
		}
		bool flag = Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(leader);
		HeroPurchaseButton.SetActive(!flag);
		mHeroGridDataSource.RepopulateObjects();
		if (!leader.AlwaysAvailable && !flag)
		{
			StartEndDate activeAvailablePeriod = leader.GetActiveAvailablePeriod();
			if (activeAvailablePeriod != null)
			{
				TimeSpan timeSpan = activeAvailablePeriod.EndDate - TFUtils.ServerTime;
				HeroAvailableLabel.text = KFFLocalization.Get("!!AVAILABLE_FOR_X").Replace("<val1>", PlayerInfoScript.FormatTimeString((int)timeSpan.TotalSeconds));
			}
			else
			{
				HeroAvailableLabel.text = string.Empty;
			}
		}
		else
		{
			HeroAvailableLabel.text = string.Empty;
		}
	}

	public void OnClickPurchaseHero()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("BuyHero"))
		{
			LeaderPurchaseExecute();
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!HIRE_HERO_CONFIRM").Replace("<val2>", mHighlightedLeader.Name), KFFLocalization.Get("!!HIRE_HERO_NOBUY").Replace("<val2>", mHighlightedLeader.Name), mHighlightedLeader.BuyCost, ConfirmHeroPurchase, null, false);
		}
	}

	private void ConfirmHeroPurchase()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(mHighlightedLeader.BuyCost, "leader purchase", UserActionCallback);
		mNextFunction = LeaderPurchaseExecute;
	}

	private void LeaderPurchaseExecute()
	{
		Singleton<PlayerInfoScript>.Instance.SaveData.Leaders.Add(new LeaderItem(mHighlightedLeader.ID));
		Singleton<PlayerInfoScript>.Instance.Save();
		PopulateHeroList();
		PopulateLeader(mHighlightedLeader);
		mSkinGridDataSource.RepopulateObjects();
		PopulateSkin(mHighlightedLeader);
		Singleton<BuyHeroPopupController>.Instance.Show(mHighlightedLeader);
	}

	public void OnSkinClicked(StoreHeroPrefab skinTile)
	{
		PopulateSkin(skinTile.Leader);
	}

	private void PopulateSkin(LeaderData skin)
	{
		mHighlightedSkin = skin;
		SkinName.text = skin.Name;
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(skin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", SkinImage);
		SkinCost.transform.SetParentActive(false);
		SkinButtonLabel.transform.SetParentActive(false);
		BuySkinButton.SetActive(false);
		SkinEquippedParent.SetActive(false);
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(skin.SkinParentLeader);
		if (leaderItem != null)
		{
			if (leaderItem.OwnedSkins.Contains(skin))
			{
				if (leaderItem.SelectedSkin == skin)
				{
					SkinEquippedParent.SetActive(true);
				}
				else
				{
					BuySkinButton.SetActive(true);
					SkinButtonLabel.transform.SetParentActive(true);
					SkinButtonLabel.text = KFFLocalization.Get("!!EQUIP");
				}
			}
			else
			{
				BuySkinButton.SetActive(true);
				SkinCost.transform.SetParentActive(true);
				SkinCost.text = skin.SkinBuyCost.ToString();
			}
		}
		else
		{
			BuySkinButton.SetActive(true);
			SkinButtonLabel.transform.SetParentActive(true);
			SkinButtonLabel.text = KFFLocalization.Get("!!BUY_HERO");
		}
		mSkinGridDataSource.RepopulateObjects();
	}

	public void OnClickSkinButton()
	{
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(mHighlightedSkin.SkinParentLeader);
		if (leaderItem != null)
		{
			if (leaderItem.OwnedSkins.Contains(mHighlightedSkin))
			{
				leaderItem.SelectedSkin = mHighlightedSkin;
				PopulateSkin(mHighlightedSkin);
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowCustomizationPurchasePrompt(KFFLocalization.Get("!!BUY_SKIN_PROMPT"), KFFLocalization.Get("!!BUY_SKIN_PROMPT_CANT_AFFORD"), mHighlightedSkin.SkinBuyCost, ConfirmSkinPurchase, false);
			}
		}
		else
		{
			HeroTabButton.gameObject.SendMessage("OnClick");
			StoreHeroPrefab heroTile = FindHeroTile(mHighlightedSkin.SkinParentLeader);
			OnLeaderClicked(heroTile);
		}
	}

	private void ConfirmSkinPurchase()
	{
		Singleton<PlayerInfoScript>.Instance.Save(delegate(bool success)
		{
			if (!success)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!RELOAD_DATA_PROMPT"), ReloadFromServer, SimplePopupController.PopupPriority.ServerError);
			}
			else
			{
				mWaitForUserAction = true;
				mUserActionProceed = NextAction.WAITING;
				Singleton<BusyIconPanelController>.Instance.Show();
				Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.StaminaRefillCost, "skin purchase", UserActionCallback);
				mNextFunction = SkinPurchaseExecute;
			}
		});
	}

	private void SkinPurchaseExecute()
	{
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(mHighlightedSkin.SkinParentLeader);
		leaderItem.OwnedSkins.Add(mHighlightedSkin);
		leaderItem.SelectedSkin = mHighlightedSkin;
		Singleton<PlayerInfoScript>.Instance.Save();
		PopulateSkinList();
		PopulateSkin(mHighlightedSkin);
	}

	public bool IsHeroHighlighted(LeaderData leader)
	{
		return mHighlightedLeader == leader;
	}

	public bool IsSkinHighlighted(LeaderData skin)
	{
		return mHighlightedSkin == skin;
	}

	private IEnumerator SkipStoreTutorial()
	{
		Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Store");
		while (true)
		{
			Singleton<TutorialController>.Instance.AdvanceIfOnState("BA_InSale");
			if (Singleton<TutorialController>.Instance.IsStateActive("BA_CloseBank"))
			{
				break;
			}
			yield return null;
		}
		Singleton<TutorialController>.Instance.AdvanceTutorialState();
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}

	public void OnClickSaleButton()
	{
		SpecialSaleData saleToDisplay = Singleton<PlayerInfoScript>.Instance.GetSaleToDisplay(true);
		if (saleToDisplay == null)
		{
			SaleButton.SetActive(false);
			TabsGrid.Reposition();
		}
		else
		{
			Singleton<SalePopupController>.Instance.Show(saleToDisplay, SalePopupController.ShowLocation.StoreScreen);
		}
	}

	public void OnClickClose()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
		{
			Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		}
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Singleton<PauseController>.Instance.HideIfShowing();
		}
		HideTween.Play();
	}
}
