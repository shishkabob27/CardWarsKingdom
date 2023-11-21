using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InventoryBarController : MonoBehaviour
{
	public enum FilterType
	{
		CREATURE,
		CAKE,
		CARD,
		INGREDIENT,
		SHARD
	}

	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public delegate void DoFilter();

	public UILabel InventoryCounter;

	public GameObject ScreenScriptGameObject;

	public UIButton inventoryAddBtn;

	public UIScrollView ScrollView;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public Dictionary<int, UIToggle> filterDict;

	public UIToggle[] InventoryToggleButtons;

	private bool doNotFireEvent;

	public static event DoFilter onDoFilter;

	private void Start()
	{
		filterDict = new Dictionary<int, UIToggle>();
		filterDict.Add(0, InventoryToggleButtons[0]);
		filterDict.Add(1, InventoryToggleButtons[1]);
		filterDict.Add(2, InventoryToggleButtons[2]);
		filterDict.Add(3, InventoryToggleButtons[3]);
		filterDict.Add(4, InventoryToggleButtons[4]);
	}

	public void SetFilters(bool creatureFilterActive, bool cakeFilterActive, bool cardFilterActive, bool ingredientFilterActive, bool shardFilterActive)
	{
		doNotFireEvent = true;
		filterDict[0].value = creatureFilterActive;
		filterDict[1].value = cakeFilterActive;
		filterDict[2].value = cardFilterActive;
		filterDict[3].value = ingredientFilterActive;
		filterDict[4].value = shardFilterActive;
		HandleSortCreatureTogglePress(InventoryToggleButtons[0]);
		HandleSortCakeTogglePress(InventoryToggleButtons[1]);
		HandleSortCardTogglePress(InventoryToggleButtons[2]);
		HandleSortIngredientTogglePress(InventoryToggleButtons[3]);
		HandleSortShardTogglePress(InventoryToggleButtons[4]);
		doNotFireEvent = false;
		UpdateFilters();
	}

	public bool GetFilterState(FilterType filterType)
	{
		if (filterDict.ContainsKey((int)filterType))
		{
			return filterDict[(int)filterType].value;
		}
		return false;
	}

	public bool GetFilterState(InventorySlotItem item)
	{
		switch (item.SlotType)
		{
		case InventorySlotType.Creature:
			return GetFilterState(FilterType.CREATURE);
		case InventorySlotType.XPMaterial:
			return GetFilterState(FilterType.CAKE);
		case InventorySlotType.Card:
			return GetFilterState(FilterType.CARD);
		case InventorySlotType.EvoMaterial:
			if (item.EvoMaterial.AwakenMat)
			{
				return GetFilterState(FilterType.SHARD);
			}
			return GetFilterState(FilterType.INGREDIENT);
		default:
			return false;
		}
	}

	public ReadOnlyCollection<InventorySlotItem> GetFilteredInventory()
	{
		ReadOnlyCollection<InventorySlotItem> inventorySlots = Singleton<PlayerInfoScript>.Instance.SaveData.InventorySlots;
		List<InventorySlotItem> list = new List<InventorySlotItem>();
		foreach (InventorySlotItem item in inventorySlots)
		{
			if (!GetFilterState(item))
			{
				list.Add(item);
			}
		}
		return list.AsReadOnly();
	}

	public static void onDoFilterNull()
	{
		InventoryBarController.onDoFilter = null;
	}

	private void Update()
	{
		if (!mWaitForUserAction)
		{
			return;
		}
		if (mUserActionProceed == NextAction.PROCEED)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			if (mNextFunction != null)
			{
				mNextFunction();
			}
			mWaitForUserAction = false;
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

	public void OnClickExpandInventory()
	{
		if (Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency < MiscParams.InventorySpacePurchaseCost)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), KFFLocalization.Get("!!INVENTORY_SLOTS_NOBUY"), MiscParams.InventorySpacePurchaseCost, OnClickConfirmPurchaseSlots);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), string.Empty, MiscParams.InventorySpacePurchaseCost, ConfirmExpandInventory);
		}
	}

	private void OnClickConfirmPurchaseSlots()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string value = MiscParams.InventorySpacePurchaseCost.ToString();
		string upsightEvent = "Economy.GemExit.IncreaseInventory";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("cost", value);
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
	}

	private void ConfirmExpandInventory()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string empty = string.Empty;
		string empty2 = string.Empty;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		empty = MiscParams.InventorySpacePurchaseCost.ToString();
		empty2 = "Economy.GemExit.IncreaseInventory";
		dictionary.Add("cost", empty);
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
	}

	private void InventorySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyInventorySlots(MiscParams.InventorySpacePerPurchase);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<BuyInventoryPopupController>.Instance.Show();
		UpdateInventoryCounter();
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

	private void OnCloseServerAccessErrorPopup()
	{
	}

	public void UpdateInventoryCounter()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty = saveData.InventorySpace.ToString();
		empty2 = saveData.FilledInventoryCount.ToString();
		if (InventoryCounter != null)
		{
			InventoryCounter.text = empty2 + " / " + empty;
		}
		if (ScreenScriptGameObject == null)
		{
			ScreenScriptGameObject = base.gameObject;
		}
		if (ScreenScriptGameObject.GetComponent<EditDeckController>() != null)
		{
			ScreenScriptGameObject.GetComponent<EditDeckController>().PopulateCreatureList();
		}
		else if (ScreenScriptGameObject.GetComponent<EvoScreenController>() != null)
		{
			ScreenScriptGameObject.GetComponent<EvoScreenController>().PopulateCreatureList();
		}
		else if (ScreenScriptGameObject.GetComponent<XpFusionController>() != null)
		{
			ScreenScriptGameObject.GetComponent<XpFusionController>().PopulateCreatureList();
		}
		else if (ScreenScriptGameObject.GetComponent<EnhanceCreatureScreenController>() != null)
		{
			ScreenScriptGameObject.GetComponent<EnhanceCreatureScreenController>().PopulateCreatureList();
		}
		else if (ScreenScriptGameObject.GetComponent<SetMyHelperController>() != null)
		{
			ScreenScriptGameObject.GetComponent<SetMyHelperController>().PopulateCreatureList();
		}
		else if (ScreenScriptGameObject.GetComponent<SellScreenController>() != null)
		{
			ScreenScriptGameObject.GetComponent<SellScreenController>().PopulateItemList();
		}
		else if (ScreenScriptGameObject.GetComponent<ExpeditionStartController>() != null)
		{
			ScreenScriptGameObject.GetComponent<ExpeditionStartController>().PopulateItemList();
		}
	}

	public void ResetScrollView()
	{
	}

	private void ResetScrollViewDelayed()
	{
		if (ScrollView != null)
		{
			ScrollView.ResetPosition();
		}
	}

	public void onDeckEditMainShown()
	{
		if (inventoryAddBtn != null)
		{
			inventoryAddBtn.isEnabled = Singleton<TutorialController>.Instance.IsBlockComplete("UseGacha");
		}
	}

	public void HandleSortCreatureTogglePress(UIToggle inToggle)
	{
		UISprite component = inToggle.GetComponent<UISprite>();
		component.spriteName = ((!inToggle.value) ? "Button_Sort_Creature" : "Button_Sort_Creature_Pressed");
		UpdateFilters();
	}

	public void HandleSortCakeTogglePress(UIToggle inToggle)
	{
		UISprite component = inToggle.GetComponent<UISprite>();
		component.spriteName = ((!inToggle.value) ? "Button_Sort_Cake" : "Button_Sort_Cake_Pressed");
		UpdateFilters();
	}

	public void HandleSortCardTogglePress(UIToggle inToggle)
	{
		UISprite component = inToggle.GetComponent<UISprite>();
		component.spriteName = ((!inToggle.value) ? "Button_Sort_Card" : "Button_Sort_Card_Pressed");
		UpdateFilters();
	}

	public void HandleSortIngredientTogglePress(UIToggle inToggle)
	{
		UISprite component = inToggle.GetComponent<UISprite>();
		component.spriteName = ((!inToggle.value) ? "Button_Sort_Ingredient" : "Button_Sort_Ingredient_Pressed");
		UpdateFilters();
	}

	public void HandleSortShardTogglePress(UIToggle inToggle)
	{
		UISprite component = inToggle.GetComponent<UISprite>();
		component.spriteName = ((!inToggle.value) ? "Button_Sort_Shard" : "Button_Sort_Shard_Pressed");
		UpdateFilters();
	}

	private void UpdateFilters()
	{
		if (!doNotFireEvent && InventoryBarController.onDoFilter != null)
		{
			InventoryBarController.onDoFilter();
		}
	}
}
