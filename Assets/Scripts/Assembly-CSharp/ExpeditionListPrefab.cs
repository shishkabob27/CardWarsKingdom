using UnityEngine;

public class ExpeditionListPrefab : UIStreamingGridListItem
{
	public UIProgressBar ProgressBar;

	public GameObject ContentsParent;

	public GameObject NotStartedParent;

	public GameObject InProgressParent;

	public GameObject BuySlotParent;

	public GameObject CompleteParent;

	public UILabel Name;

	public UILabel InProgress;

	public UILabel Time;

	public UILabel BuyCost;

	public UILabel CreatureCount;

	public UILabel FavoredClass;

	public UILabel Duration;

	public UILabel DifficultyLabel;

	public UISprite TopBarOutline;

	public UISprite PreferredClassBgRect;

	public UISprite Background;

	public UISprite FactionIcon;

	public UISprite FactionIconOutline;

	public UISprite TopBarFill;

	public UISprite MeterArrow;

	public UISprite[] MeterSegments = new UISprite[6];

	public int NormalWidth;

	public int SelectedWidth;

	[Header("Tweens")]
	public UITweenController SelectedTween;

	public UITweenController SelectedBgTween;

	public UITweenController ResetColorsTween;

	public UITweenController ResetBgColorsTween;

	public UITweenController UnlockTween;

	public UITweenController CompleteTween;

	public UITweener TimerTickTween;

	public TweenColor SelectedTweenColor;

	public TweenColor SelectedBgTweenColor;

	public TweenColor ResetBgTweenColor;

	private ExpeditionItem mExpedition;

	public override void Populate(object dataObj)
	{
		if (dataObj != null)
		{
			BuySlotParent.SetActive(false);
			ContentsParent.SetActive(true);
			ExpeditionItem expeditionItem = dataObj as ExpeditionItem;
			mExpedition = dataObj as ExpeditionItem;
			string inDifficultyName = KFFLocalization.Get(mExpedition.Difficulty.Name);
			SetDifficulty(mExpedition.Difficulty.Difficulty, inDifficultyName);
			if (CreatureCount != null)
			{
				if (mExpedition.CreatureCount == 1)
				{
					CreatureCount.text = KFFLocalization.Get("!!1_CREATURE");
				}
				else
				{
					CreatureCount.text = KFFLocalization.Get("!!X_CREATURES").Replace("<val1>", mExpedition.CreatureCount.ToString());
				}
			}
			if (FavoredClass != null)
			{
				if (mExpedition.FavoredClass == CreatureFaction.Count)
				{
					FavoredClass.text = string.Empty;
				}
				else
				{
					FavoredClass.text = KFFLocalization.Get("!!X_CLASS_PREFERRED").Replace("<val1>", mExpedition.FavoredClass.ClassDisplayName());
				}
			}
			ColorizeMenuByFaction(mExpedition.FavoredClass);
			if (mExpedition.IsComplete)
			{
				NotStartedParent.SetActive(false);
				InProgressParent.SetActive(false);
				CompleteParent.SetActive(true);
			}
			else if (mExpedition.InProgress)
			{
				NotStartedParent.SetActive(false);
				InProgressParent.SetActive(true);
				CompleteParent.SetActive(false);
				Duration.text = string.Empty;
				UpdateRewardMeter();
			}
			else
			{
				NotStartedParent.SetActive(true);
				InProgressParent.SetActive(false);
				CompleteParent.SetActive(false);
				Time.text = PlayerInfoScript.FormatTimeString(mExpedition.Duration, true, true);
				InProgress.text = string.Empty;
				string newValue = PlayerInfoScript.FormatTimeString(mExpedition.Duration, true, true);
				Duration.text = KFFLocalization.Get("!!EXPEDITION_DURATION").Replace("<val1>", newValue);
			}
			Name.text = mExpedition.NameData.Name;
			if (mExpedition == Singleton<ExpeditionStartController>.Instance.ShowingExpedition())
			{
				if (!Singleton<ExpeditionStartController>.Instance.AnimatingExtraSlotBuy)
				{
					Select();
				}
			}
			else
			{
				Deselect();
			}
			Update();
		}
		else
		{
			mExpedition = null;
			DifficultyLabel.gameObject.SetActive(false);
			ContentsParent.SetActive(false);
			BuySlotParent.SetActive(true);
			int num = Mathf.Max(0, ExpeditionSlotCostDataManager.Instance.GetNextSlotPurchaseCost());
			BuyCost.text = num.ToString();
			Singleton<ExpeditionStartController>.Instance.BuySlotItem = this;
		}
	}

	private void Select()
	{
		if (GetComponent<UIWidget>().width != SelectedWidth)
		{
			SelectedTween.Play();
			SelectedBgTween.Play();
		}
		Singleton<ExpeditionStartController>.Instance.SelectedExpeditionPrefab = this;
	}

	public void Deselect()
	{
		if (GetComponent<UIWidget>().width != NormalWidth)
		{
			SelectedTween.PlayReverse();
			SelectedBgTween.PlayReverse();
		}
		else
		{
			ResetColorsTween.Play();
			ResetBgColorsTween.Play();
		}
	}

	private void SetDifficulty(int inDifficulty, string inDifficultyName)
	{
		DifficultyLabel.gameObject.SetActive(true);
		ExpeditionsColorPalette expeditionsColorPalette = Singleton<ExpeditionStartController>.Instance.DifficultyPalette[inDifficulty - 1];
		DifficultyLabel.text = inDifficultyName;
		DifficultyLabel.color = expeditionsColorPalette.Colors[0];
		DifficultyLabel.effectColor = expeditionsColorPalette.Colors[1];
		DifficultyLabel.GetComponent<TweenScale>().duration = 0.6f - (float)inDifficulty * 0.065f;
	}

	private void ColorizeMenuByFaction(CreatureFaction inFavoredClass)
	{
		ExpeditionsColorPalette expeditionsColorPalette = Singleton<ExpeditionStartController>.Instance.ColorPalette[(int)inFavoredClass];
		FavoredClass.color = expeditionsColorPalette.Colors[4];
		PreferredClassBgRect.color = expeditionsColorPalette.Colors[3];
		FactionIconOutline.color = expeditionsColorPalette.Colors[4];
		FactionIcon.spriteName = inFavoredClass.IconTexture();
		FactionIcon.gameObject.SetActive(!string.IsNullOrEmpty(FactionIcon.spriteName));
		Color effectColor = expeditionsColorPalette.Colors[1];
		effectColor.a = 0.1f;
		Name.effectColor = effectColor;
		Duration.color = expeditionsColorPalette.Colors[2];
		TopBarOutline.color = expeditionsColorPalette.Colors[1];
		TopBarFill.color = expeditionsColorPalette.Colors[0];
		SelectedTweenColor.from = expeditionsColorPalette.SelectorColors[0];
		SelectedTweenColor.to = expeditionsColorPalette.SelectorColors[1];
		Background.color = expeditionsColorPalette.BackgroundColors[1];
		SelectedBgTweenColor.from = expeditionsColorPalette.BackgroundColors[0];
		SelectedBgTweenColor.to = expeditionsColorPalette.BackgroundColors[1];
		ResetBgTweenColor.from = expeditionsColorPalette.BackgroundColors[1];
		ResetBgTweenColor.to = expeditionsColorPalette.BackgroundColors[1];
	}

	private void UpdateRewardMeter()
	{
		float num = 0f;
		for (int i = 0; i < mExpedition.UsedCreatureUIDs.Count; i++)
		{
			int uniqueId = mExpedition.UsedCreatureUIDs[i];
			InventorySlotItem creatureItem = Singleton<PlayerInfoScript>.Instance.SaveData.GetCreatureItem(uniqueId);
			if (creatureItem != null)
			{
				float num2 = creatureItem.Creature.StarRating;
				if (creatureItem.Creature.Form.Faction == mExpedition.FavoredClass)
				{
					num2 *= ExpeditionParams.FavoredFactionValue;
				}
				num += num2;
			}
		}
		int tierFromStars = ExpeditionDifficultyDataManager.Instance.GetTierFromStars((int)num);
		tierFromStars = Mathf.Min(tierFromStars, mExpedition.Difficulty.Difficulty);
		if (tierFromStars > 0)
		{
			Vector3 position = MeterArrow.transform.position;
			position.x = MeterSegments[tierFromStars - 1].transform.position.x;
			MeterArrow.transform.position = position;
		}
		for (int j = 0; j < MeterSegments.Length; j++)
		{
			MeterSegments[j].gameObject.SetActive(j < mExpedition.Difficulty.Difficulty);
		}
	}

	private void Update()
	{
		if (mExpedition != null && !mExpedition.IsComplete && mExpedition.InProgress)
		{
			uint num = TFUtils.ServerTime.UnixTimestamp();
			if (num < mExpedition.EndTime)
			{
				int num2 = (int)(mExpedition.EndTime - num);
				Time.text = KFFLocalization.Get("!!IN_PROGRESS");
				InProgress.text = PlayerInfoScript.FormatTimeString(num2, true);
				ProgressBar.value = 1f - (float)num2 / (float)mExpedition.Duration;
				TimerTickTween.enabled = true;
				float tweenFactor = (float)TFUtils.ServerTime.Millisecond / 1000f;
				TimerTickTween.tweenFactor = tweenFactor;
			}
			else
			{
				mExpedition.IsComplete = true;
				Time.text = KFFLocalization.Get("!!COMPLETE");
				InProgress.text = KFFLocalization.Get("!!CLAIM");
				ProgressBar.value = 1f;
				TimerTickTween.enabled = false;
				InProgress.transform.localScale = Vector3.one;
				SetCompletedState();
			}
		}
	}

	private void SetCompletedState()
	{
		CompleteParent.SetActive(true);
		ContentsParent.SetActive(false);
		Name.leftAnchor.absolute = 95;
		CompleteTween.Play();
	}

	public void OnClick()
	{
		if (mExpedition == null)
		{
			if (!Singleton<ExpeditionStartController>.Instance.AnimatingExtraSlotBuy)
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!BUY_EXPEDITION_SLOT"), KFFLocalization.Get("!!CANT_AFFORD_EXPEDITION_SLOT"), ExpeditionSlotCostDataManager.Instance.GetNextSlotPurchaseCost(), ConfirmBuySlot);
			}
		}
		else if (mExpedition.InProgress && TFUtils.ServerTime.UnixTimestamp() >= mExpedition.EndTime)
		{
			ClaimExpeditionRewards();
		}
		else
		{
			Singleton<ExpeditionStartController>.Instance.ShowExpedition(mExpedition, false);
		}
	}

	private void ConfirmBuySlot()
	{
		Singleton<ExpeditionStartController>.Instance.BuyExtraSlot(this);
	}

	private void ClaimExpeditionRewards()
	{
		Singleton<ExpeditionRewardsController>.Instance.ShowRewards(mExpedition);
	}

	public void OnClickSpeedUp()
	{
		Singleton<ExpeditionStartController>.Instance.ShowSpeedUpPopup(mExpedition);
	}
}
