using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizationController : Singleton<PlayerCustomizationController>
{
	public enum Page
	{
		CardBacks,
		Portraits,
		HeroSkins,
		Count,
		None
	}

	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public delegate void CloseCallback();

	public GameObject PortraitListPrefab;

	public GameObject CardBackListPrefab;

	public GameObject HeroSkinListPrefab;

	public UITweenController ShowTween;

	private bool isAnimating;

	public GameObject[] PageParents = new GameObject[3];

	public GameObject[] PageButtons = new GameObject[3];

	public UIGrid ButtonsGrid;

	public UIScrollView PortraitScrollView;

	public UIStreamingGrid PortraitGrid;

	private UIStreamingGridDataSource<PlayerPortraitData> mPortraitGridDataSource = new UIStreamingGridDataSource<PlayerPortraitData>();

	public PortraitListEntry SelectedPortraitPrefab;

	public UILabel SelectedPortraitName;

	private List<PlayerPortraitData> mAvailablePortraits = new List<PlayerPortraitData>();

	public UIScrollView CardBackScrollView;

	public UIStreamingGrid CardBackGrid;

	private UIStreamingGridDataSource<CardBackData> mCardBackGridDataSource = new UIStreamingGridDataSource<CardBackData>();

	public CardBackListEntry SelectedCardBackPrefab;

	public UILabel SelectedCardBackName;

	public UIScrollView HeroSkinScrollView;

	public UIStreamingGrid HeroSkinGrid;

	private UIStreamingGridDataSource<LeaderData> mHeroSkinDataSource = new UIStreamingGridDataSource<LeaderData>();

	public StoreHeroPrefab SelectedHeroSkinPrefab;

	public UILabel SelectedHeroSkinName;

	public UILabel SkinCustCurrency;

	private LeaderData mLastShownLeader;

	private Page mShowingPage = Page.None;

	private List<Page> mAllowedPages;

	private LeaderData mBuyingSkin;

	private CloseCallback mCloseCallback;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public void ShowFromEditTeam(CloseCallback closeCallback)
	{
		mCloseCallback = closeCallback;
		mAllowedPages = new List<Page>
		{
			Page.CardBacks,
			Page.HeroSkins
		};
		Show();
	}

	public void ShowDefault()
	{
		mAllowedPages = new List<Page> { Page.CardBacks };
		Show();
	}

	private void Show()
	{
		ShowTween.Play();
		for (int i = 0; i < 3; i++)
		{
			PageButtons[i].SetActive(mAllowedPages.Contains((Page)i));
		}
		ButtonsGrid.Reposition();
		int inButtonIndex = (int)(mAllowedPages.Contains(mShowingPage) ? mShowingPage : Page.CardBacks);
		StartCoroutine(DelayedPressButtonCo(inButtonIndex));
	}

	private IEnumerator DelayedPressButtonCo(int inButtonIndex)
	{
		yield return new WaitForEndOfFrame();
		PageButtons[inButtonIndex].SendMessage("OnClick");
	}

	public void CardBackButtonPress()
	{
		if (!isAnimating)
		{
			ShowPage(Page.CardBacks);
		}
	}

	public void PortraitButtonPress()
	{
		if (!isAnimating)
		{
			ShowPage(Page.Portraits);
		}
	}

	public void HeroSkinButtonPress()
	{
		if (!isAnimating)
		{
			ShowPage(Page.HeroSkins);
		}
	}

	private void ShowPage(Page page)
	{
		mShowingPage = page;
		for (int i = 0; i < 3; i++)
		{
			PageParents[i].SetActive(i == (int)mShowingPage);
		}
		switch (mShowingPage)
		{
		case Page.CardBacks:
			RefreshCardBacks(true);
			break;
		case Page.Portraits:
			RefreshPortraits();
			break;
		case Page.HeroSkins:
			RefreshHeroSkins();
			break;
		}
	}

	private void PopulateAvailablePortraits()
	{
		mAvailablePortraits.Clear();
		mAvailablePortraits.Add(PlayerPortraitDataManager.Instance.GetData("Default"));
		if (Singleton<PlayerInfoScript>.Instance.IsFacebookLogin())
		{
			mAvailablePortraits.Add(PlayerPortraitDataManager.Instance.GetData("Facebook"));
		}
		mAvailablePortraits.AddRange(Singleton<PlayerInfoScript>.Instance.SaveData.UnlockedPortraits);
	}

	private void RefreshPortraits()
	{
		mPortraitGridDataSource.Init(PortraitGrid, PortraitListPrefab, mAvailablePortraits);
		SelectedPortraitPrefab.Populate(Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait);
		SelectedPortraitName.text = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait.Name;
		PortraitScrollView.ResetPosition();
	}

	public void OnClickPortraitEntry(PortraitListEntry entry)
	{
		if (entry.Data != Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait = entry.Data;
			Singleton<TownHudController>.Instance.UpdateLeaderIcon();
			RefreshPortraits();
		}
	}

	private void RefreshCardBacks(bool inShouldResetScrollView)
	{
		List<CardBackData> list = Singleton<PlayerInfoScript>.Instance.SaveData.UnlockedCardBacks.Copy();
		while (list.Count < 5)
		{
			list.Add(null);
		}
		mCardBackGridDataSource.Init(CardBackGrid, CardBackListPrefab, list);
		SelectedCardBackPrefab.Populate(Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack);
		SelectedCardBackName.text = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack.Name;
		if (inShouldResetScrollView)
		{
			CardBackScrollView.ResetPosition();
		}
	}

	public void OnClickCardBackEntry(CardBackListEntry entry)
	{
		if (entry.Data != Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack = entry.Data;
			RefreshCardBacks(false);
		}
	}

	private void RefreshHeroSkins()
	{
		LeaderItem leader = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader;
		SelectedHeroSkinPrefab.Populate(leader.SelectedSkin);
		SelectedHeroSkinName.text = leader.SelectedSkin.Name;
		SkinCustCurrency.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
		List<LeaderData> list = new List<LeaderData>();
		list.Add(leader.Form);
		foreach (LeaderData alternateSkin in leader.Form.AlternateSkins)
		{
			if (alternateSkin.BuyableSkin || leader.OwnedSkins.Contains(alternateSkin))
			{
				list.Add(alternateSkin);
			}
		}
		mHeroSkinDataSource.Init(HeroSkinGrid, HeroSkinListPrefab, list, leader.Form != mLastShownLeader);
		mLastShownLeader = leader.Form;
		HeroSkinScrollView.ResetPosition();
	}

	public void OnClickHeroSkinEntry(LeaderData data)
	{
		LeaderItem leader = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader;
		if (leader.Form == data || leader.OwnedSkins.Contains(data))
		{
			if (leader.SelectedSkin != data)
			{
				leader.SelectedSkin = data;
				RefreshHeroSkins();
			}
		}
		else
		{
			mBuyingSkin = data;
			Singleton<SimplePopupController>.Instance.ShowCustomizationPurchasePrompt(KFFLocalization.Get("!!BUY_SKIN_PROMPT"), KFFLocalization.Get("!!BUY_SKIN_PROMPT_CANT_AFFORD"), data.SkinBuyCost, ConfirmSkinPurchase);
		}
	}

	private void ConfirmSkinPurchase()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(mBuyingSkin.SkinBuyCost, "skin purchase", UserActionCallback);
		mNextFunction = SkinPurchaseExecute;
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

	private void SkinPurchaseExecute()
	{
		LeaderItem leader = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader;
		leader.OwnedSkins.Add(mBuyingSkin);
		leader.SelectedSkin = mBuyingSkin;
		Singleton<PlayerInfoScript>.Instance.Save();
		RefreshHeroSkins();
	}

	public void OnClickClose()
	{
		if (mCloseCallback != null)
		{
			mCloseCallback();
		}
		mCloseCallback = null;
	}

	public void Unload()
	{
		Singleton<PlayerInfoScript>.Instance.Save();
		mPortraitGridDataSource.Clear();
		SelectedPortraitPrefab.Unload();
		mAvailablePortraits.Clear();
		mCardBackGridDataSource.Clear();
		SelectedCardBackPrefab.Unload();
		mHeroSkinDataSource.Clear();
		SelectedHeroSkinPrefab.Unload();
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

	private void OnCloseServerAccessErrorPopup()
	{
	}
}
