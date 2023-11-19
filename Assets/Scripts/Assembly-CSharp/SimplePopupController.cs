using System;
using System.Collections.Generic;
using UnityEngine;

public class SimplePopupController : Singleton<SimplePopupController>
{
	public enum PopupPriority
	{
		None,
		Normal,
		QuitButton,
		PvpError,
		ServerError
	}

	public delegate void PopupButtonCallback();

	public UITweenController ShowTween;

	public UITweenController ShowNameEntryTween;

	public UITweenController ShowDateEntryTween;

	public UITweenController HideTween;

	public GameObject MainPanel;

	public UILabel Title;

	public UILabel Body;

	public UILabel InputLabel;

	public UILabel YesButtonLabel;

	public UILabel NoButtonLabel;

	public UILabel OkButtonLabel;

	public Transform YesButton;

	public Transform NoButton;

	public Transform OkButton;

	public Transform Input;

	public UIInput InputObject;

	public GameObject FacebookLoginButtons;

	public GameObject FacebookLoginIcon;

	public DateEntryItem MonthEntry;

	public DateEntryItem DayEntry;

	public DateEntryItem YearEntry;

	public Transform InventoryGroup;

	public UIGrid InventoryGrid;

	public UILabel InventoryTitle;

	public UILabel InventoryBody;

	private PopupButtonCallback mNoCallback;

	private PopupButtonCallback mYesCallback;

	private PopupButtonCallback mCloseCallback;

	private bool mCanCloseFromBackButton;

	private PopupPriority mCurrentPriority;

	private void Awake()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			base.gameObject.ChangeLayer(LayerMask.NameToLayer("TopGUI"));
		}
	}

	private void OnEnable()
	{
		UICamera.AlwaysAllowedColliders.Add(YesButton.gameObject);
		UICamera.AlwaysAllowedColliders.Add(NoButton.gameObject);
		UICamera.AlwaysAllowedColliders.Add(OkButton.gameObject);
	}

	private void OnDisable()
	{
		if (YesButton != null)
		{
			UICamera.AlwaysAllowedColliders.Remove(YesButton.gameObject);
		}
		if (NoButton != null)
		{
			UICamera.AlwaysAllowedColliders.Remove(NoButton.gameObject);
		}
		if (OkButton != null)
		{
			UICamera.AlwaysAllowedColliders.Remove(OkButton.gameObject);
		}
		UICamera.AlwaysAllowedColliders.RemoveAll((GameObject m) => m == null);
	}

	public void ShowMessage(string title, string body)
	{
		ShowMessage(title, body, null);
	}

	public void ShowMessage(string title, string body, bool playErrorSound)
	{
		if (playErrorSound)
		{
			PlayErrorSound();
		}
		ShowMessage(title, body, null);
	}

	private void PlayErrorSound()
	{
		Singleton<SLOTAudioManager>.Instance.PlaySound("UI_ErrorSound");
	}

	public void ShowMessage(string title, string body, PopupButtonCallback callback, PopupPriority priority = PopupPriority.Normal)
	{
		ShowMessage(title, body, callback, KFFLocalization.Get("!!OK"), priority);
	}

	public void ShowMessage(string title, string body, PopupButtonCallback callback, string buttonText, PopupPriority priority = PopupPriority.Normal)
	{
		if (priority > mCurrentPriority)
		{
			mCurrentPriority = priority;
			mYesCallback = callback;
			mCloseCallback = callback;
			mCanCloseFromBackButton = true;
			Title.text = title;
			Body.text = body;
			OkButtonLabel.text = buttonText;
			YesButton.gameObject.SetActive(false);
			NoButton.gameObject.SetActive(false);
			OkButton.gameObject.SetActive(true);
			Input.gameObject.SetActive(false);
			InventoryGroup.gameObject.SetActive(false);
			FacebookLoginButtons.SetActive(false);
			MenuStackManager.RegisterPopup();
			ShowTween.Play();
		}
	}

	public void ShowInput(string title, PopupButtonCallback callback = null, PopupButtonCallback cancelCallback = null, PopupPriority priority = PopupPriority.Normal)
	{
		if (priority > mCurrentPriority)
		{
			mCurrentPriority = priority;
			mYesCallback = callback;
			mNoCallback = cancelCallback;
			mCloseCallback = cancelCallback;
			mCanCloseFromBackButton = true;
			Title.text = title;
			Body.text = string.Empty;
			YesButtonLabel.text = "OK";
			NoButtonLabel.text = "Cancel";
			YesButton.gameObject.SetActive(true);
			NoButton.gameObject.SetActive(true);
			OkButton.gameObject.SetActive(false);
			Input.gameObject.SetActive(true);
			InventoryGroup.gameObject.SetActive(false);
			FacebookLoginButtons.SetActive(false);
			MenuStackManager.RegisterPopup();
			ShowNameEntryTween.Play();
			InputObject.ManualSelect();
		}
	}

	public void ShowInputNoCancel(string title, PopupButtonCallback callback = null, PopupPriority priority = PopupPriority.Normal)
	{
		if (priority > mCurrentPriority)
		{
			mCurrentPriority = priority;
			mYesCallback = callback;
			mCloseCallback = null;
			mCanCloseFromBackButton = false;
			Title.text = title;
			Body.text = string.Empty;
			YesButton.gameObject.SetActive(false);
			NoButton.gameObject.SetActive(false);
			OkButton.gameObject.SetActive(true);
			Input.gameObject.SetActive(true);
			InventoryGroup.gameObject.SetActive(false);
			FacebookLoginButtons.SetActive(false);
			ShowNameEntryTween.Play();
			InputObject.ManualSelect();
		}
	}

	public string GetInputValue()
	{
		return InputLabel.text;
	}

	public void ShowPromptWithInventoryItems(string title, string body, PopupButtonCallback yesCallback, PopupButtonCallback noCallback, List<InventorySlotItem> items, PopupPriority priority = PopupPriority.Normal)
	{
		ShowTween.Play();
		FacebookLoginButtons.SetActive(false);
		InventoryGroup.gameObject.SetActive(true);
		InventoryTile[] componentsInChildren = InventoryGrid.GetComponentsInChildren<InventoryTile>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			InventoryTile inventoryTile = componentsInChildren[i];
			componentsInChildren[i] = null;
			UnityEngine.Object.Destroy(inventoryTile.gameObject);
		}
		InventoryTile.ClearDelegates(false);
		InventoryGrid.transform.DestroyAllChildren();
		foreach (InventorySlotItem item in items)
		{
			if (item != null)
			{
				GameObject gameObject = InventoryGrid.transform.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.transform.localScale = Vector3.one * 0.7f;
				gameObject.ChangeLayer(base.gameObject.layer);
				InventoryTile component = gameObject.GetComponent<InventoryTile>();
				component.Populate(item);
				gameObject.GetComponent<Collider>().enabled = false;
			}
		}
		InventoryGrid.Reposition();
		mCurrentPriority = priority;
		mYesCallback = yesCallback;
		mNoCallback = noCallback;
		mCloseCallback = noCallback;
		mCanCloseFromBackButton = true;
		Title.text = string.Empty;
		Body.text = string.Empty;
		YesButtonLabel.text = KFFLocalization.Get("!!YES");
		NoButtonLabel.text = KFFLocalization.Get("!!NO");
		YesButton.gameObject.SetActive(true);
		NoButton.gameObject.SetActive(true);
		OkButton.gameObject.SetActive(false);
		Input.gameObject.SetActive(false);
		InventoryBody.text = body;
		InventoryTitle.text = title;
		MenuStackManager.RegisterPopup();
	}

	public void ShowPrompt(string title, string body, PopupButtonCallback yesCallback, PopupButtonCallback noCallback, PopupPriority priority = PopupPriority.Normal)
	{
		ShowPrompt(title, body, yesCallback, noCallback, KFFLocalization.Get("!!YES"), KFFLocalization.Get("!!NO"), priority);
	}

	public void ShowPrompt(string title, string body, PopupButtonCallback yesCallback, PopupButtonCallback noCallback, string yesButtonText, string noButtonText, PopupPriority priority = PopupPriority.Normal)
	{
		if (priority > mCurrentPriority)
		{
			mCurrentPriority = priority;
			mYesCallback = yesCallback;
			mNoCallback = noCallback;
			mCloseCallback = mNoCallback;
			mCanCloseFromBackButton = true;
			Title.text = title;
			Body.text = body;
			YesButtonLabel.text = yesButtonText;
			NoButtonLabel.text = noButtonText;
			YesButton.gameObject.SetActive(true);
			NoButton.gameObject.SetActive(true);
			NoButton.gameObject.GetComponent<BoxCollider>().enabled = !Singleton<TutorialController>.Instance.IsBlockActive("UseGacha");
			NoButton.gameObject.GetComponent<UIButton>().enabled = !Singleton<TutorialController>.Instance.IsBlockActive("UseGacha");
			OkButton.gameObject.SetActive(false);
			Input.gameObject.SetActive(false);
			InventoryGroup.gameObject.SetActive(false);
			FacebookLoginButtons.SetActive(false);
			MenuStackManager.RegisterPopup();
			ShowTween.Play();
		}
	}

	public void ShowFacebookLoginPrompt(PopupButtonCallback yesCallback, PopupButtonCallback noCallback)
	{
		ShowPrompt(string.Empty, KFFLocalization.Get("!!FB_CONNECT_PROMPT"), yesCallback, noCallback, string.Empty, string.Empty);
		YesButton.gameObject.SetActive(false);
		NoButton.gameObject.SetActive(false);
		OkButton.gameObject.SetActive(false);
		Input.gameObject.SetActive(false);
		FacebookLoginButtons.SetActive(true);
	}

	private string HardCurrencyString(int amount)
	{
		return (amount != 1) ? KFFLocalization.Get("!!CURRENCY_HARD") : KFFLocalization.Get("!!CURRENCY_HARD_1");
	}

	private string CustomizationCurrencyString(int amount)
	{
		return (amount != 1) ? KFFLocalization.Get("!!CURRENCY_CUSTOMIZATION_TOKENS") : KFFLocalization.Get("!!CURRENCY_CUSTOMIZATION_TOKENS_1");
	}

	public void ShowPurchasePrompt(string confirmString, string insufficientString, int coinAmount, PopupButtonCallback confirmCallback, PopupButtonCallback declineCallback = null, bool takeToBank = true)
	{
		int hardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		if (coinAmount > hardCurrency)
		{
			ShowCantAffordPurchasePrompt(insufficientString, coinAmount, declineCallback, takeToBank);
			return;
		}
		string text = KFFLocalization.Get("!!YOUR_BALANCE");
		string body = confirmString.Replace("<val1>", coinAmount + " " + HardCurrencyString(coinAmount)) + "\n\n" + text + ": " + hardCurrency + " " + HardCurrencyString(hardCurrency);
		ShowPrompt(string.Empty, body, confirmCallback, declineCallback);
	}

	public void ShowCantAffordPurchasePrompt(string message, int coinAmount, PopupButtonCallback declineCallback = null, bool takeToBank = true)
	{
		string text = KFFLocalization.Get("!!YOUR_BALANCE");
		int hardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		string body = message.Replace("<val1>", coinAmount + " " + HardCurrencyString(coinAmount)) + "\n\n" + text + ": " + hardCurrency + " " + HardCurrencyString(hardCurrency);
		if (!DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd() && !DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			takeToBank = false;
		}
		if (takeToBank)
		{
			ShowPrompt(string.Empty, body, ShowBank, declineCallback, KFFLocalization.Get("!!BUY_HARD_CURRENCY"), KFFLocalization.Get("!!CANCEL"));
		}
		else
		{
			ShowMessage(string.Empty, body, declineCallback);
		}
	}

	public void ShowCustomizationPurchasePrompt(string confirmString, string insufficientString, int coinAmount, PopupButtonCallback confirmCallback, bool takeToBank = true)
	{
		int hardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		if (coinAmount > hardCurrency)
		{
			ShowCantAffordCustomizationPurchasePrompt(insufficientString, coinAmount, takeToBank);
			return;
		}
		string text = KFFLocalization.Get("!!YOUR_BALANCE");
		string body = confirmString.Replace("<val1>", coinAmount + " " + HardCurrencyString(coinAmount)) + "\n\n" + text + ": " + hardCurrency + " " + HardCurrencyString(hardCurrency);
		ShowPrompt(string.Empty, body, confirmCallback, null);
	}

	public void ShowCantAffordCustomizationPurchasePrompt(string message, int coinAmount, bool takeToBank = true)
	{
		string text = KFFLocalization.Get("!!YOUR_BALANCE");
		int hardCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency;
		string body = message.Replace("<val1>", coinAmount + " " + HardCurrencyString(coinAmount)) + "\n\n" + text + ": " + hardCurrency + " " + HardCurrencyString(hardCurrency);
		if (!DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd() && !DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			takeToBank = false;
		}
		if (takeToBank)
		{
			ShowPrompt(string.Empty, body, ShowBank, null, KFFLocalization.Get("!!BUY_HARD_CURRENCY"), KFFLocalization.Get("!!CANCEL"));
		}
		else
		{
			ShowMessage(string.Empty, body);
		}
	}

	private void ShowBank()
	{
		Singleton<StoreScreenController>.Instance.OpenToCustomization = false;
		Singleton<StoreScreenController>.Instance.OpenToHardCurrency = true;
		Singleton<StoreScreenController>.Instance.Populate();
		Singleton<StoreScreenController>.Instance.OpenedFromTown = false;
	}

	private void ShowBankCustomization()
	{
		Singleton<StoreScreenController>.Instance.OpenToCustomization = true;
		Singleton<StoreScreenController>.Instance.OpenToHardCurrency = false;
		Singleton<StoreScreenController>.Instance.Populate();
		Singleton<StoreScreenController>.Instance.OpenedFromTown = false;
	}

	public void OnInputSubmit()
	{
		OnClickYes();
	}

	public void OnClickYes()
	{
		HideTween.Play();
		mCurrentPriority = PopupPriority.None;
		if (mCanCloseFromBackButton)
		{
			MenuStackManager.RemoveTopItemFromStack();
		}
		PopupButtonCallback popupButtonCallback = mYesCallback;
		ClearCallbacks();
		if (popupButtonCallback != null)
		{
			popupButtonCallback();
		}
	}

	public void OnClickFacebookYes()
	{
		OnClickYes();
	}

	public void OnClickNo()
	{
		HideTween.Play();
		mCurrentPriority = PopupPriority.None;
		if (mCanCloseFromBackButton)
		{
			MenuStackManager.RemoveTopItemFromStack();
		}
		PopupButtonCallback popupButtonCallback = mNoCallback;
		ClearCallbacks();
		if (popupButtonCallback != null)
		{
			popupButtonCallback();
		}
	}

	private void ClearCallbacks()
	{
		mYesCallback = null;
		mNoCallback = null;
		mCloseCallback = null;
		mCanCloseFromBackButton = false;
	}

	public bool CanCloseFromBackButton()
	{
		return mCanCloseFromBackButton;
	}

	public void CloseFromBackButton()
	{
		if (mCanCloseFromBackButton)
		{
			HideTween.Play();
			mCurrentPriority = PopupPriority.None;
			PopupButtonCallback popupButtonCallback = mCloseCallback;
			ClearCallbacks();
			if (popupButtonCallback != null)
			{
				popupButtonCallback();
			}
		}
	}

	public void ShowDateEntryPrompt(string text, PopupButtonCallback callback, PopupPriority priority = PopupPriority.Normal)
	{
		mCurrentPriority = priority;
		mYesCallback = callback;
		mCloseCallback = callback;
		mCanCloseFromBackButton = false;
		Body.text = text;
		Title.text = string.Empty;
		OkButtonLabel.text = KFFLocalization.Get("!!OK");
		YesButton.gameObject.SetActive(false);
		NoButton.gameObject.SetActive(false);
		OkButton.gameObject.SetActive(true);
		Input.gameObject.SetActive(false);
		InventoryGroup.gameObject.SetActive(false);
		FacebookLoginButtons.SetActive(false);
		MenuStackManager.RegisterPopup();
		ShowDateEntryTween.Play();
		int year = DateTime.Now.Year;
		int day = 1;
		int month = 1;
		MonthEntry.Populate(DateEntryItem.Slot.Month, year, day, month);
		DayEntry.Populate(DateEntryItem.Slot.Day, year, day, month);
		YearEntry.Populate(DateEntryItem.Slot.Year, year, day, month);
	}

	public DateTime GetEnteredDate()
	{
		return new DateTime(YearEntry.GetSelectedValue(), MonthEntry.GetSelectedValue(), DayEntry.GetSelectedValue());
	}

	public void OnDateUpdated()
	{
		int selectedValue = YearEntry.GetSelectedValue();
		int selectedValue2 = MonthEntry.GetSelectedValue();
		int selectedValue3 = DayEntry.GetSelectedValue();
		MonthEntry.Populate(DateEntryItem.Slot.Month, selectedValue, selectedValue3, selectedValue2);
		YearEntry.Populate(DateEntryItem.Slot.Year, selectedValue, selectedValue3, selectedValue2);
		DayEntry.Populate(DateEntryItem.Slot.Day, selectedValue, selectedValue3, selectedValue2);
	}

	public void Unload()
	{
		MonthEntry.Clear();
		DayEntry.Clear();
		YearEntry.Clear();
	}
}
