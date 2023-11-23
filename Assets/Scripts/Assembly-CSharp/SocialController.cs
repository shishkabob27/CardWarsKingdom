using System;
using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class SocialController : Singleton<SocialController>
{
	public delegate void UnloadDoneCallBack();

	public UILabel TitleLabel;

	public UIToggle InviteFriendsToggle;

	public UIToggle MailToggle;

	public UIToggle AllyBoxToggle;

	public UIToggle SetMyHelperToggle;

	public GameObject MailStackObject;

	public UILabel MailStackLabel;

	public GameObject FBInviteRewardVFX;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public FriendInviteRewardsController InviteRewards;

	public FriendList Friends;

	public GameObject FacebookNotConnectGroup;

	public GameObject FacebookConnectGroup;

	public UIGrid ButtonsGrid;

	public GameObject InviteRewardsButton;

	public Transform SendParent;

	public GameObject RedeemParent;

	public Transform RedeemCreatureTileNode;

	public UILabel InviteCodeLabel;

	private UIToggle mCurrentToggle;

	private bool mPrevToggleWasHelper;

	private AllyListController mAllyListController;

	[Header("Loc Strings for UIToggle Titles")]
	public string HelperTitleText = "!!HELPER";

	public string AlliesTitleText = "!!ALLIES";

	public string MailTitleText = "!!MAIL_TITLE";

	public string GiftCodesTitleText = "!!GIFT_CODES";

	private Vector3 mBaseInviteParentPos;

	private UnloadDoneCallBack mUnloadDoneCallback;

	private bool mFinishBannerNow;

	private float mBannerTimer;

	public AllyListController GetAllyListController()
	{
		return mAllyListController;
	}

	private void Awake()
	{
		mAllyListController = GetComponent<AllyListController>();
		mBaseInviteParentPos = SendParent.localPosition;
	}

	public void Populate()
	{
		InviteRewardsButton.SetActive(MiscParams.ShowInviteButtons);
		ButtonsGrid.Reposition();
		Singleton<MailController>.Instance.RetrieveMailsAndAllyInvites();
		AllyBoxToggle.gameObject.SetActive(Singleton<PlayerInfoScript>.Instance.CanUseHelper());
		Invoke("RefreshCurrentTab", 0.1f);
		Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Social");
		UpdateBadgeCount();
		InviteCodeLabel.text = global::Multiplayer.Multiplayer.GenerateRedeemCode(Singleton<PlayerInfoScript>.Instance.GetPlayerCode());
	}

	public void SetTitleLabel(string inLabelText)
	{
		if (TitleLabel != null)
		{
			TitleLabel.text = KFFLocalization.Get(inLabelText);
		}
	}

	private void Update()
	{
		if (TitleLabel.gameObject.activeInHierarchy)
		{
			UpdateBadgeCount();
		}
	}

	private void UpdateBadgeCount()
	{
		int num = Singleton<PlayerInfoScript>.Instance.StateData.BadgeCounts[0];
		if (num > 0)
		{
			MailStackObject.SetActive(true);
			MailStackLabel.text = num.ToString();
		}
		else
		{
			MailStackObject.SetActive(false);
		}
	}

	public void OnCloseClicked()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		Singleton<SetMyHelperController>.Instance.HideCreatureList();
		if (Singleton<SetMyHelperController>.Instance.SetHelper)
		{
			Singleton<SetMyHelperController>.Instance.SetHelper = false;
			Singleton<PlayerInfoScript>.Instance.Save();
			RewardManager.UpdatePvPInfo();
		}
		Unload(OnUnloadDone);
	}

	private void OnUnloadDone()
	{
		HideTween.PlayWithCallback(OnPanelCloseDone);
	}

	public void OnPanelCloseDone()
	{
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}

	public void Unload(UnloadDoneCallBack callBack = null)
	{
		FacebookNotConnectGroup.SetActive(false);
		InviteRewards.Clear();
		Singleton<MailController>.Instance.Clear();
		mAllyListController.Unload();
		Friends.Clear();
		Singleton<SetMyHelperController>.Instance.Unload();
		if (callBack != null)
		{
			callBack();
		}
	}

	public void RefreshCurrentTab()
	{
		Unload();
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		if (mCurrentToggle == null)
		{
			mCurrentToggle = MailToggle;
			MailToggle.gameObject.SendMessage("OnClick");
		}
		else
		{
			mCurrentToggle = UIToggle.GetActiveToggle(MailToggle.group);
		}
		if (mCurrentToggle == MailToggle)
		{
			Singleton<MailController>.Instance.ShowInbox();
			Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Mail);
		}
		else if (mCurrentToggle == AllyBoxToggle)
		{
			mAllyListController.ShowInbox(true);
		}
		else if (mCurrentToggle == SetMyHelperToggle)
		{
			Singleton<SetMyHelperController>.Instance.ShowHelperPanel();
		}
		else if (mCurrentToggle == InviteFriendsToggle)
		{
			if (Singleton<PlayerInfoScript>.Instance.SaveData.InviteCodeRedeemed)
			{
				RedeemParent.SetActive(false);
				Vector3 localPosition = mBaseInviteParentPos;
				localPosition.x = 0f;
				SendParent.localPosition = localPosition;
			}
			else
			{
				RedeemParent.SetActive(true);
				SendParent.localPosition = mBaseInviteParentPos;
			}
		}
		Invoke("RefreshHelperBackground", 0.1f);
	}

	public void RefreshHelperBackground()
	{
		if (mCurrentToggle != SetMyHelperToggle && mPrevToggleWasHelper)
		{
			mPrevToggleWasHelper = false;
			base.gameObject.GetComponent<SetMyHelperController>().HidebackgroundTween.Play();
		}
		if (mCurrentToggle == SetMyHelperToggle)
		{
			mPrevToggleWasHelper = true;
		}
	}

	public void IncrementFriendRequestsSent(int count)
	{
	}

	private void FinishBannerNow()
	{
		mFinishBannerNow = true;
	}

	public IEnumerator ShowFBRewardBanner()
	{
		Singleton<DailyGiftBannerController>.Instance.ShowBannerNow();
		for (mBannerTimer = 0f; mBannerTimer < 3f; mBannerTimer += Time.deltaTime)
		{
			yield return null;
			if (mFinishBannerNow)
			{
				mBannerTimer = 3f;
			}
		}
		if (!mFinishBannerNow)
		{
			Singleton<DailyGiftBannerController>.Instance.HideBannerNow();
		}
		yield return new WaitForSeconds(0.5f);
		mFinishBannerNow = false;
	}

	public IEnumerator ShowBanner()
	{
		float delay = Singleton<DailyGiftBannerController>.Instance.GetDelay();
		Singleton<DailyGiftBannerController>.Instance.ShowBannerNow();
		yield return new WaitForSeconds(delay);
		Singleton<DailyGiftBannerController>.Instance.HideBannerNow();
		yield return null;
	}

	public static void GiveInviteReward(InviteReward reward)
	{
		switch (reward.RewardType)
		{
		case "StaminaPvE":
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Quests, reward.RewardQuantity);
			break;
		case "StaminaPvP":
			DetachedSingleton<StaminaManager>.Instance.AddExtraStamina(StaminaType.Pvp, reward.RewardQuantity);
			break;
		case "HardCurrency":
			Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(0, reward.RewardQuantity, "invite reward", -1, string.Empty);
			break;
		case "SoftCurrency":
			Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency += reward.RewardQuantity;
			break;
		case "PvPCurrency":
			Singleton<PlayerInfoScript>.Instance.SaveData.PvPCurrency += reward.RewardQuantity;
			break;
		}
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public void OnClickRedeemCode()
	{
		if (TownSettingsController.testInternetConnection())
		{
			StartCoroutine(RedeemCodeCoroutine());
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	private IEnumerator RedeemCodeCoroutine()
	{
		bool waiting = true;
		bool canceled = false;
		Singleton<SimplePopupController>.Instance.ShowInput(Language.Get("!!ENTER_REDEMPTION_CODE"), delegate
		{
			waiting = false;
		}, delegate
		{
			waiting = false;
			canceled = true;
		});
		while (waiting)
		{
			yield return null;
		}
		if (canceled)
		{
			yield break;
		}
		string code = Singleton<SimplePopupController>.Instance.GetInputValue();
		if (code.Length == 0)
		{
			yield break;
		}
		string id = global::Multiplayer.Multiplayer.DecodeRedeemCode(code);
		if (id == null || id.Length == 0)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, Language.Get("!!REDEMPTION_CODE_ERROR"), true);
			yield break;
		}
		ResponseFlag resultFlag = ResponseFlag.None;
		Singleton<BusyIconPanelController>.Instance.Show();
		RewardManager.RedeemCodeReward(id, delegate(ResponseFlag flag)
		{
			resultFlag = flag;
		});
		while (resultFlag == ResponseFlag.None)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (resultFlag != 0)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, Language.Get("!!REDEMPTION_CODE_ERROR"), true);
			yield break;
		}
		Singleton<PlayerInfoScript>.Instance.SaveData.InviteCodeRedeemed = true;
		Singleton<PlayerInfoScript>.Instance.Save();
		RefreshCurrentTab();
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CODE_REDEEMED"));
	}

	public void RedeemPopupClosed()
	{
		RedeemCreatureTileNode.DestroyAllChildren();
	}

	public void OnClickSendEmail()
	{
		EMailer.PrefillAndShow(null, RedemptionCodeMessager.GenerateMessageSubject(), RedemptionCodeMessager.GenerateLongMessageBody());
	}

	public void OnClickSendFacebook()
	{
		FacebookPoster.PrefillAndShow(RedemptionCodeMessager.GenerateMessageSubject(), RedemptionCodeMessager.GenerateLongMessageSubject(), RedemptionCodeMessager.GenerateTweetMessage());
	}

	public void OnClickSendTweet()
	{
		Twitterer.Tweet(RedemptionCodeMessager.GenerateTweetMessage());
	}
}
