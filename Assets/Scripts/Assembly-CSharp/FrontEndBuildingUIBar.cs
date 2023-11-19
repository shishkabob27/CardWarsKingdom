using System.Collections;
using UnityEngine;

public class FrontEndBuildingUIBar : MonoBehaviour
{
	public UILabel NameLabel;

	public GameObject StackParent;

	public UILabel StackLabel;

	public UISprite IconSprite;

	public UISprite BadgeSprite;

	public UISprite Background;

	public SpriteAnimator GlintAnim;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController AttractTween;

	private Camera mTownCam;

	private Camera mUICam;

	private Vector3 mBarAttachPoint;

	private TownBuildingData mBuildingData;

	private TownScheduleData mTownScheduleData;

	private bool hasAttractBeenShownThisBoot;

	public TownBuildingScript mBuilding { get; set; }

	public bool Visible { get; set; }

	public void Init()
	{
		mBuildingData = TownBuildingDataManager.Instance.GetData(mBuilding.BuildingId);
		mTownScheduleData = TownScheduleDataManager.Instance.GetCurrentScheduledTownData();
		NameLabel.text = mBuildingData.Name;
		NameLabel.color = mTownScheduleData.BannerTextColor;
		NameLabel.effectColor = mTownScheduleData.BannerTextOutlineColor;
		NameLabel.GetComponent<LabelShadow>().ShadowTextColor = mTownScheduleData.BannerTextShadowColor;
		NameLabel.GetComponent<LabelShadow>().ShadowEffectColor = mTownScheduleData.BannerTextShadowOutlineColor;
		if (NameLabel.LabelShadow != null)
		{
			NameLabel.LabelShadow.RefreshShadowLabel();
		}
		BadgeSprite.color = mTownScheduleData.BadgeColor;
		Background.spriteName = mTownScheduleData.BannerSprite;
		IconSprite.spriteName = GetSpriteName(mBuildingData.ID);
		mTownCam = Singleton<TownController>.Instance.GetTownCam();
		mUICam = Singleton<TownController>.Instance.GetUICam();
	}

	public string GetSpriteName(string inBuildingId)
	{
		switch (inBuildingId)
		{
		case "TBuilding_Collection":
			return "BuildingIcon_Museum";
		case "TBuilding_Dungeon":
			return "BuildingIcon_Dungeons";
		case "TBuilding_EditDeck":
			return "BuildingIcon_MyTeam";
		case "TBuilding_Gacha":
			return "BuildingIcon_TreasureCave";
		case "TBuilding_Lab":
			return "BuildingIcon_Laboratory";
		case "TBuilding_PVP":
			return "BuildingIcon_Multiplayer";
		case "TBuilding_Quests":
			return "BuildingIcon_Battles";
		case "TBuilding_Store":
			return "BuildingIcon_Store";
		case "TBuilding_Social":
			return "BuildingIcon_PostOffice";
		case "TBuilding_Sell":
			return "BuildingIcon_ChooseGoose";
		default:
			return string.Empty;
		}
	}

	private void Update()
	{
		mBarAttachPoint = mBuilding.NameBarAttachPoint.transform.position;
		Vector2 vector = mTownCam.WorldToScreenPoint(mBarAttachPoint);
		Vector3 position = mUICam.ScreenToWorldPoint(vector);
		position.z = 0f;
		base.transform.position = position;
		int num = 0;
		if (mBuildingData.Badge != BadgeEnum.None)
		{
			num = Singleton<PlayerInfoScript>.Instance.StateData.BadgeCounts[(int)mBuildingData.Badge];
		}
		if (num > 0)
		{
			StackParent.SetActive(true);
			if (num > 999)
			{
				StackLabel.fontSize = 22;
				StackLabel.text = "...";
				return;
			}
			if (num > 99)
			{
				StackLabel.fontSize = 22;
			}
			else if (num > 9)
			{
				StackLabel.fontSize = 24;
			}
			else
			{
				StackLabel.fontSize = 26;
			}
			StackLabel.text = num.ToString();
		}
		else
		{
			StackParent.SetActive(false);
		}
	}

	public void ShowAttract()
	{
		if (!Singleton<TutorialController>.Instance.IsAnyTutorialActive() && !(mBuildingData.ID != "TBuilding_Gacha") && !hasAttractBeenShownThisBoot)
		{
			hasAttractBeenShownThisBoot = true;
			if (Singleton<CalendarGiftController>.Instance.MainPanel.activeInHierarchy)
			{
				TryToShowAttract();
			}
			else if (Singleton<SalePopupController>.Instance.MainPanel1.activeInHierarchy)
			{
				TryToShowAttract();
			}
			else if (Singleton<SalePopupController>.Instance.MainPanel2.activeInHierarchy)
			{
				TryToShowAttract();
			}
			else if (Singleton<BattleResultsLevelUpController>.Instance.MainPanel.activeInHierarchy)
			{
				TryToShowAttract();
			}
			else if (Singleton<TownHudController>.Instance.BuildingUnlockPanel.activeInHierarchy)
			{
				TryToShowAttract();
			}
			else if (!Singleton<TutorialController>.Instance.PopupObject.gameObject.activeInHierarchy && !Singleton<TutorialController>.Instance.PopupObject.gameObject.activeInHierarchy && !Singleton<TutorialController>.Instance.PointerObject.gameObject.activeInHierarchy)
			{
				AttractTween.Play();
			}
		}
	}

	public IEnumerator ShowAttractAfterDelay()
	{
		yield return new WaitForSeconds(1f);
		ShowAttract();
	}

	public void TryToShowAttract()
	{
	}
}
