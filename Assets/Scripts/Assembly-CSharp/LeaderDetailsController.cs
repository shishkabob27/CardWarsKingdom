using System.Collections.Generic;
using UnityEngine;

public class LeaderDetailsController : Singleton<LeaderDetailsController>
{
	public delegate void OnClosedCallback();

	public UITweenController ShowTween;

	public UITexture Portrait;

	public Transform CardsParent;

	private Transform[] CardNodes = new Transform[5];

	public UILabel Name;

	public UILabel Age;

	public UILabel Height;

	public UILabel Weight;

	public UILabel Species;

	public UILabel Quote;

	public Transform ZoomPosition;

	public Collider ZoomCollider;

	public UILabel BuyCost;

	public GameObject BuyButton;

	public UIGrid ComboPipsParent;

	private UISprite[] ComboPips = new UISprite[7];

	public GameObject LockedObject;

	public UILabel HardCurrencyLabel;

	private LeaderData mLeaderData;

	private LeaderItem mLeaderItem;

	private List<CardPrefabScript> mSpawnedCards = new List<CardPrefabScript>();

	private LeaderData mDisplayedSkin;

	private OnClosedCallback mOnClosedCallback;

	public void Awake()
	{
		for (int i = 0; i < 5; i++)
		{
			CardNodes[i] = CardsParent.FindChild("ActionCardSpawn_" + (i + 1).ToString("D2"));
		}
		for (int j = 0; j < 7; j++)
		{
			ComboPips[j] = ComboPipsParent.transform.FindChild("ComboPip_" + (j + 1).ToString("D2")).GetComponent<UISprite>();
		}
	}

	public void Show(LeaderData leader)
	{
		ShowTween.Play();
		mLeaderData = leader;
		mLeaderItem = null;
		mDisplayedSkin = leader;
		PopulateCurrentLeader();
		RefreshCurrency();
	}

	public void Show(LeaderItem leader, OnClosedCallback closeCallback)
	{
		ShowTween.Play();
		mOnClosedCallback = closeCallback;
		mLeaderData = null;
		mLeaderItem = leader;
		mDisplayedSkin = leader.SelectedSkin;
		PopulateCurrentLeader();
		RefreshCurrency();
	}

	private void RefreshCurrency()
	{
		HardCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
	}

	private void PopulateCurrentLeader()
	{
		LeaderData leaderData = ((mLeaderItem == null) ? mLeaderData : mLeaderItem.Form);
		leaderData.ParseKeywords();
		int layer = base.gameObject.layer;
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = CardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
			gameObject.ChangeLayer(layer);
			CardPrefabScript component = gameObject.GetComponent<CardPrefabScript>();
			component.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
			component.Populate(leaderData.ActionCards[i]);
			component.AdjustDepth(i + 1);
			component.SetCardState(CardPrefabScript.HandCardState.InHand);
			mSpawnedCards.Add(component);
		}
		Name.text = leaderData.Name;
		Age.text = leaderData.FlvAge;
		Height.text = leaderData.FlvHeight;
		Weight.text = leaderData.FlvWeight;
		Species.text = leaderData.FlvSpecies;
		Quote.text = leaderData.FlvQuote;
		for (int j = 0; j < 7; j++)
		{
			if (j < leaderData.AttackCombo.Count && leaderData.AttackCombo[j] != 0)
			{
				ComboPips[j].gameObject.SetActive(true);
				ComboPips[j].spriteName = leaderData.AttackCombo[j].ComboPipTexture();
			}
			else
			{
				ComboPips[j].gameObject.SetActive(false);
			}
		}
		ComboPipsParent.Reposition();
		LockedObject.SetActive(false);
		BuyButton.SetActive(false);
		HardCurrencyLabel.transform.SetParentActive(false);
		if (mLeaderItem != null && !Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(mLeaderItem.Form))
		{
			BuyCost.text = leaderData.BuyCost.ToString();
			BuyButton.SetActive(true);
			LockedObject.SetActive(true);
			HardCurrencyLabel.transform.SetParentActive(true);
		}
		RefreshLeaderTexture();
	}

	private void RefreshLeaderTexture()
	{
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(mDisplayedSkin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", Portrait);
	}

	public void OnClickClose()
	{
		OnClosedCallback onClosedCallback = mOnClosedCallback;
		mOnClosedCallback = null;
		if (onClosedCallback != null)
		{
			onClosedCallback();
		}
	}

	public void Unload()
	{
		Portrait.UnloadTexture();
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			NGUITools.Destroy(mSpawnedCard.gameObject);
		}
		mSpawnedCards.Clear();
		mLeaderData = null;
		mLeaderItem = null;
	}

	public void UnzoomCard()
	{
		foreach (CardPrefabScript mSpawnedCard in mSpawnedCards)
		{
			mSpawnedCard.Unzoom();
		}
	}

	public void OnClickPurchase()
	{
		Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!HIRE_HERO_CONFIRM").Replace("<val2>", mLeaderItem.Form.Name), KFFLocalization.Get("!!HIRE_HERO_NOBUY").Replace("<val2>", mLeaderItem.Form.Name), mLeaderItem.Form.BuyCost, ConfirmPurchase);
	}

	private void ConfirmPurchase()
	{
		Singleton<EditDeckController>.Instance.OnPurchaseLeader();
	}

	public void OnPurchaseComplete()
	{
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(mLeaderItem.Form);
		SendKPITrack(leaderItem);
		Unload();
		mLeaderItem = leaderItem;
		PopulateCurrentLeader();
		RefreshCurrency();
	}

	private void SendKPITrack(LeaderItem newlyCreatedLeader)
	{
		string value = newlyCreatedLeader.SelectedSkin.BuyCost.ToString();
		string upsightEvent = "Economy.GemExit.Heroes";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("heroesID", newlyCreatedLeader.SelectedSkin.ID);
		dictionary.Add("cost", value);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
	}

	public void PrevSkin()
	{
	}

	public void NextSkin()
	{
	}

	private void SetSkin(LeaderData skin)
	{
	}

	public void OnClickBuySkin()
	{
	}

	private void ConfirmBuySkin()
	{
	}
}
