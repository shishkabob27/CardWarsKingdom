using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarGiftController : Singleton<CalendarGiftController>
{
	public GameObject MainPanel;

	public GameObject DayTemplatePrefab;

	public UITweenController RewardFXShow;

	public UITweenController RewardFXHide;

	public UITweenController CalendarShow;

	public UITweenController CalendarHide;

	public UITweenController MonthRewardClaim;

	public UISprite MonthRewardIcon;

	public UITexture MonthRewardTexture;

	public UILabel MonthRewardAmount;

	public UILabel FinaleLabel;

	public UILabel TitleLabel;

	public GameObject CalendarButtonClose;

	public GameObject EffectsObject;

	public Transform DayNodesParent;

	public UITexture Background;

	private bool mShowing;

	public void PopupGift()
	{
		StartCoroutine(GiftAndShowReward());
	}

	private IEnumerator GiftAndShowReward()
	{
		mShowing = true;
		bool bgLoaded = false;
		Singleton<SLOTResourceManager>.Instance.QueueResourceLoad("UI/GeneralBundle/Calendar_Frame", "GeneralBundle", delegate(Object loadedResouce)
		{
			Background.UnloadTexture();
			Background.mainTexture = loadedResouce as Texture;
			bgLoaded = true;
		});
		while (!bgLoaded)
		{
			yield return null;
		}
		if (Singleton<PlayerInfoScript>.Instance.SaveData.ActiveCalendar.Entries[0].Repeating)
		{
			TitleLabel.text = KFFLocalization.Get("!!REPEATING_CALENDAR");
		}
		else
		{
			TitleLabel.text = KFFLocalization.Get("!!DAILY_CALENDAR");
		}
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		List<CalendarGift> gifts = Singleton<PlayerInfoScript>.Instance.GetCalendarGifts();
		List<DailyCalendarGiftItem> dayPrefabs = new List<DailyCalendarGiftItem>();
		for (int i = 0; i < gifts.Count - 1; i++)
		{
			string nodeName = "Node_Day_" + (i + 1).ToString("D2");
			Transform dayNode = DayNodesParent.Find(nodeName);
			if (!(dayNode == null))
			{
				DailyCalendarGiftItem dayPrefab = dayNode.InstantiateAsChild(DayTemplatePrefab).GetComponent<DailyCalendarGiftItem>();
				dayPrefabs.Add(dayPrefab);
				dayPrefab.DayLabel.text = KFFLocalization.Get("!!DAY_X").Replace("<val1>", (i + 1).ToString());
				if (gifts[i].Texture != null)
				{
					dayPrefab.RewardIcon.gameObject.SetActive(false);
					dayPrefab.RewardTexture.gameObject.SetActive(true);
					dayPrefab.RewardTexture.ReplaceTexture(gifts[i].Texture);
				}
				else
				{
					dayPrefab.RewardTexture.gameObject.SetActive(false);
					dayPrefab.RewardIcon.gameObject.SetActive(true);
					dayPrefab.RewardIcon.spriteName = gifts[i].Icon;
				}
				dayPrefab.SetClaimed(i < (int)saveData.OneTimeCalendarDaysClaimed);
			}
		}
		CalendarGift finalGift = gifts[gifts.Count - 1];
		if (finalGift.Texture != null)
		{
			MonthRewardIcon.gameObject.SetActive(false);
			MonthRewardTexture.gameObject.SetActive(true);
			MonthRewardTexture.ReplaceTexture(finalGift.Texture);
		}
		else
		{
			MonthRewardTexture.gameObject.SetActive(false);
			MonthRewardIcon.gameObject.SetActive(true);
			MonthRewardIcon.spriteName = finalGift.Icon;
		}
		MonthRewardAmount.text = gifts[gifts.Count - 1].Quantity.ToString();
		FinaleLabel.text = KFFLocalization.Get("!!DAY_X").Replace("<val1>", gifts.Count.ToString());
		int grantingGiftIndex = saveData.OneTimeCalendarDaysClaimed;
		CalendarGift grantingGift = gifts[grantingGiftIndex];
		++saveData.OneTimeCalendarDaysClaimed;
		saveData.LastOneTimeCalendarDateClaimed = TFUtils.ServerTime;
		InventorySlotItem slotItem;
		CardBackData cardBack;
		string displayName = grantingGift.Grant(out slotItem, out cardBack);
		Singleton<PlayerInfoScript>.Instance.Save();
		RewardFXShow.Play();
		CalendarShow.Play();
		CalendarButtonClose.SetActive(false);
		while (LoadingScreenController.ShowingLoadingScreen())
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_Calendar_Show");
		if (grantingGiftIndex == gifts.Count - 1)
		{
			MonthRewardClaim.Play();
		}
		else
		{
			dayPrefabs[grantingGiftIndex].Claim();
		}
		yield return new WaitForSeconds(1.5f);
		RewardFXHide.Play();
		EffectsObject.SetActive(false);
		if (cardBack != null)
		{
			Singleton<DailyGiftBannerController>.Instance.Init(displayName, cardBack);
		}
		else if (slotItem != null)
		{
			Singleton<DailyGiftBannerController>.Instance.Init(displayName, slotItem);
		}
		else
		{
			Singleton<DailyGiftBannerController>.Instance.Init(displayName, grantingGift.Icon);
		}
		yield return StartCoroutine(Singleton<DailyGiftBannerController>.Instance.ShowBannerNowCo());
		EffectsObject.SetActive(true);
		RewardFXShow.Play();
		CalendarButtonClose.SetActive(true);
		while (mShowing)
		{
			yield return null;
		}
		foreach (DailyCalendarGiftItem prefab in dayPrefabs)
		{
			prefab.RewardTexture.UnloadTexture();
			NGUITools.Destroy(prefab.gameObject);
		}
		MonthRewardTexture.UnloadTexture();
	}

	public void HideCalendar()
	{
		Singleton<TownController>.Instance.ToggleTownColliders(true);
		Singleton<TownController>.Instance.AdvanceIntroState();
	}

	public void Unload()
	{
		Background.UnloadTexture();
		mShowing = false;
	}
}
