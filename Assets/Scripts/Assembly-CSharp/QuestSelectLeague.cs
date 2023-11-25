using UnityEngine;

public class QuestSelectLeague : UIStreamingGridListItem
{
	public UITweenController MoveTween;

	public UITweenController MoveAlphaTween;

	public UITweenController CompleteTween;

	public UITweenController UnlockTween;

	public UITweenController ResetTween;

	public UITweenController FlashBonus;

	public UILabel Name;

	public UILabel Battles;

	public UITexture Background;

	public UITexture BossPortrait;

	public GameObject Lock;

	public GameObject HideWhenLocked;

	public UILabel TimeLabel;

	public GameObject UnavailableOverlay;

	public UIButton Button;

	public UILabel BonusText;

	public GameObject LeagueInfoParent;

	public Vector3 StoredPosition;

	public string EnterText;

	public string ExitText;

	public UILabel EnterExitLabel;

	public float DelayToShowUnlockedElements = 2f;

	[SerializeField]
	private Color[] _TitleOutlineColors = new Color[0];

	[SerializeField]
	private Color[] _BonusOutlineColors = new Color[0];

	private bool mClickable;

	private QuestSelectController.SpecialLeagueEntry mSpecialLeagueEntry;

	public LeagueData League { get; set; }

	public override void Populate(object dataObj)
	{
		if (dataObj is LeagueData)
		{
			ResetTween.Play();
			League = dataObj as LeagueData;
			PopulateCommonData();
			mClickable = Singleton<PlayerInfoScript>.Instance.IsLeagueUnlocked(League);
			if (Lock != null)
			{
				Lock.SetActive(!mClickable);
			}
			if (HideWhenLocked != null)
			{
				HideWhenLocked.SetActive(mClickable);
			}
		}
		else if (dataObj is QuestSelectController.SpecialLeagueEntry)
		{
			mSpecialLeagueEntry = dataObj as QuestSelectController.SpecialLeagueEntry;
			League = mSpecialLeagueEntry.League;
			LeagueInfoParent.SetActive(League.ID != "DungeonMaps");
			PopulateCommonData();
			PopulateSpecialLeague();
		}
	}

	public override void Unload()
	{
		BossPortrait.UnloadTexture();
		if (Background != null)
		{
			Background.UnloadTexture();
		}
	}

	private void PopulateCommonData()
	{
		Name.text = League.Name;
		SetLabelEffectColors(League.ID);
		if (League.ID == "DailyRandom")
		{
			Battles.text = KFFLocalization.Get("!!FLOOR_X").Replace("<val1>", Singleton<PlayerInfoScript>.Instance.SaveData.RandomDungeonLevel.ToString());
		}
		else if (League.Quests.Count == 1)
		{
			Battles.text = KFFLocalization.Get("!!1_BATTLE");
		}
		else
		{
			Battles.text = KFFLocalization.Get("!!X_BATTLES").Replace("<val1>", League.Quests.Count.ToString());
		}
		if (Background != null)
		{
			Background.ReplaceTexture(League.FrameTexture);
		}
		if (League.ID != "DailyRandom" && League.Quests.Count > 0)
		{
			QuestData questData = League.Quests[League.Quests.Count - 1];
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(questData.Opponent.QuestPortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", BossPortrait);
		}
		else
		{
			BossPortrait.ReplaceTexture(string.Empty);
		}
	}

	public void PopulateSpecialLeague()
	{
		mClickable = mSpecialLeagueEntry.Clickable;
		TimeLabel.text = mSpecialLeagueEntry.FullTextString;
		UnavailableOverlay.SetActive(!mSpecialLeagueEntry.Clickable);
		if (Button != null)
		{
			Button.pressedSprite = ((!mSpecialLeagueEntry.Clickable) ? "UI_Action_Button_Green" : "UI_Action_Button_Green_Pressed");
			Button.GetComponent<ButtonDepressEffect>().enabled = mSpecialLeagueEntry.Clickable;
		}
	}

	public void OnLeagueClicked()
	{
		if (mClickable)
		{
			Singleton<QuestSelectController>.Instance.OnLeagueClicked(this);
		}
	}

	public void DelayedShowUnlockedElements()
	{
		UnlockTween.Play();
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_LockShake");
		Invoke("ShowHideWhenLockedElements", DelayToShowUnlockedElements);
	}

	private void ShowHideWhenLockedElements()
	{
		if (HideWhenLocked != null)
		{
			HideWhenLocked.SetActive(true);
		}
	}

	private void OnDrag()
	{
		Singleton<QuestSelectController>.Instance.OnLeagueDrag();
	}

	private void SetLabelEffectColors(string inLeagueID)
	{
		int num = 0;
		switch (inLeagueID)
		{
		case "League1":
		case "League8":
			num = 1;
			break;
		case "League2":
		case "League9":
			num = 2;
			break;
		case "League3":
		case "League10":
			num = 3;
			break;
		case "League4":
		case "League11":
			num = 4;
			break;
		case "League5":
		case "League12":
			num = 5;
			break;
		case "League6":
		case "League13":
			num = 6;
			break;
		case "League7":
		case "League14":
			num = 7;
			break;
		case "League15":
			num = 8;
			break;
		default:
			num = 0;
			break;
		}
		if (num < _TitleOutlineColors.Length)
		{
			Name.effectColor = _TitleOutlineColors[num];
		}
		if (num < _BonusOutlineColors.Length)
		{
			BonusText.effectColor = _BonusOutlineColors[num];
		}
	}
}
