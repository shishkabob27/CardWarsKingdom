using System.Collections;
using System.Collections.Generic;
using Allies;
using Messages;
using MiniJSON;
using UnityEngine;

public class MailController : Singleton<MailController>
{
	public delegate void MailButtonCallback();

	public float CurrencyOffset;

	public GameObject MailPrefab;

	public UITweenController GainCurrencyTween;

	public UITweenController GainCurrencyFinishTween;

	public UITweenController ItemGainedTween;

	public UIStreamingGrid MailGrid;

	private UIStreamingGridDataSource<MailItem> mRewardsGridDataSource = new UIStreamingGridDataSource<MailItem>();

	private List<MailItem> mMailList = new List<MailItem>();

	private bool mWeveGotMail;

	private bool mFetchingMail;

	private bool mShowingInbox;

	private List<Dictionary<string, object>> mAdminMessageDict = new List<Dictionary<string, object>>();

	public GameObject Information;

	public GameObject Prompt;

	public UILabel TitleLabel;

	public UILabel BodyLabel;

	public Transform BodyTextParent;

	public GameObject BodyCurrencyParent;

	public UISprite GainedCurrencySprite;

	public UILabel GainedCurrencyLabel;

	public UIPanel BodyPanel;

	public GameObject RewardedItemNode;

	public UILabel RewardedItemAmount;

	private MailButtonCallback mDeleteCallback;

	private Coroutine mGainedCurrencyCoroutine;

	public bool IsMessageRetrieveDone()
	{
		return !mFetchingMail;
	}

	public void RetrieveMailsAndAllyInvites()
	{
		mWeveGotMail = false;
		if (!mFetchingMail)
		{
			StartCoroutine(RetrieveMailsAndAllyInvitesCo());
		}
	}

	private IEnumerator RetrieveMailsAndAllyInvitesCo()
	{
		mFetchingMail = true;
		List<MailItem> retrievingMailList = new List<MailItem>();
		Singleton<PlayerInfoScript>.Instance.SaveData.RemoveMails((MailItem m) => !m.IsMailApplicable());
		retrievingMailList.AddRange(Singleton<PlayerInfoScript>.Instance.SaveData.Mails);
		foreach (MailData data in MailDataManager.Instance.GetDatabase())
		{
			if (Singleton<PlayerInfoScript>.Instance.SaveData.FindMail((MailItem m) => m.ID == data.ID) == null && MailDataManager.Instance.IsMailApplicable(data))
			{
				MailItem item = new MailItem(data);
				item.MailType = MailType.Scheduled;
				item.SetTime();
				retrievingMailList.Add(item);
				Singleton<PlayerInfoScript>.Instance.SaveData.AddMail(item);
				mWeveGotMail = true;
			}
		}
		yield return StartCoroutine(RetrieveAdminMail(retrievingMailList));
		yield return StartCoroutine(RetrieveAllyInvites(retrievingMailList));
		if (mWeveGotMail)
		{
			Singleton<PlayerInfoScript>.Instance.Save();
			mWeveGotMail = false;
		}
		retrievingMailList.Reverse();
		mMailList = retrievingMailList;
		mFetchingMail = false;
		if (mShowingInbox)
		{
			PopulateGrid();
		}
	}

	private IEnumerator RetrieveAdminMail(List<MailItem> workingList)
	{
		List<string> adminMailList = null;
		bool waiting = true;
		Message.GetMessages(SessionManager.Instance.theSession, delegate(List<string> messages, Messages.ResponseFlag flag)
		{
			if (flag == Messages.ResponseFlag.Success)
			{
				adminMailList = messages;
			}
			waiting = false;
		});
		while (waiting)
		{
			yield return null;
		}
		if (adminMailList == null || adminMailList.Count == 0)
		{
			yield break;
		}
		foreach (string adminMessage in adminMailList)
		{
			Dictionary<string, object> dict = Json.Deserialize(adminMessage) as Dictionary<string, object>;
			MailData data = new MailData();
			MailItem item = new MailItem(MailType.AdminMessage);
			item.HardQuantity = int.Parse(dict["hard_currency"].ToString());
			item.SoftQuantity = int.Parse(dict["soft_currency"].ToString());
			item.MailTitle = dict["subject"].ToString();
			item.MailBody = dict["message"].ToString();
			item.SetTime();
			workingList.Add(item);
			Singleton<PlayerInfoScript>.Instance.SaveData.AddMail(item);
		}
		mWeveGotMail = true;
	}

	private IEnumerator RetrieveAllyInvites(List<MailItem> workingList)
	{
		List<AllyData> invitesList = null;
		bool waiting = true;
		Ally.GetAllyRequestList(SessionManager.Instance.theSession, delegate(List<AllyData> allies, Allies.ResponseFlag flag)
		{
			if (flag == Allies.ResponseFlag.Success)
			{
				invitesList = allies;
			}
			waiting = false;
		});
		while (waiting)
		{
			yield return null;
		}
		if (invitesList == null || invitesList.Count == 0)
		{
			yield break;
		}
		foreach (AllyData ally in invitesList)
		{
			MailItem mail = new MailItem(MailType.AllyInvite);
			HelperItem inviter = (mail.Inviter = new HelperItem(ally));
			mail.MailTitle = KFFLocalization.Get("!!ALLY_INVITE").Replace("<val1>", inviter.HelperName);
			mail.SetTime();
			workingList.Add(mail);
		}
		mWeveGotMail = true;
	}

	public void ShowInbox()
	{
		mShowingInbox = true;
		Information.SetActive(false);
		Prompt.SetActive(true);
		PopulateGrid();
	}

	private void PopulateGrid()
	{
		mRewardsGridDataSource.Init(MailGrid, MailPrefab, mMailList);
	}

	public int GetUnreadMailCount()
	{
		int num = 0;
		foreach (MailItem mMail in mMailList)
		{
			if (mMail.MailType == MailType.AllyInvite || !mMail.Opened)
			{
				num++;
			}
		}
		return num;
	}

	public int GetReadMailCount()
	{
		int num = 0;
		foreach (MailItem mMail in mMailList)
		{
			if (mMail.MailType == MailType.AllyInvite || mMail.Opened)
			{
				num++;
			}
		}
		return num;
	}

	public List<MailData> GetUnreadPopupMail()
	{
		List<MailData> list = new List<MailData>();
		foreach (MailItem mMail in mMailList)
		{
			if (!mMail.Opened && mMail.ID != null)
			{
				MailData data = MailDataManager.Instance.GetData(mMail.ID);
				if (data != null && data.ShowInPopop)
				{
					mMail.Opened = true;
					list.Add(data);
				}
			}
		}
		return list;
	}

	public void Clear()
	{
		mShowingInbox = false;
		mRewardsGridDataSource.Clear();
		RewardedItemNode.transform.DestroyAllChildren();
	}

	public void DeleteAllyInvite(HelperItem helper)
	{
		MailItem mailItem = mMailList.Find((MailItem m) => m.Inviter == helper);
		if (mailItem != null)
		{
			mMailList.Remove(mailItem);
		}
	}

	public void ShowMailPreview(string title, string body, int rewardedCurrencyAmount, int rewardedCurrencyTotal, string rewardedCurrencySprite, InventorySlotItem rewardedItem, int rewardedItemAmount, MailButtonCallback deleteCallback)
	{
		RewardedItemNode.transform.DestroyAllChildren();
		TitleLabel.text = title;
		BodyLabel.text = body;
		mDeleteCallback = deleteCallback;
		Information.SetActive(true);
		Prompt.SetActive(false);
		if (mGainedCurrencyCoroutine != null)
		{
			StopCoroutine(mGainedCurrencyCoroutine);
			mGainedCurrencyCoroutine = null;
		}
		RewardedItemAmount.text = string.Empty;
		if (rewardedCurrencyAmount > 0 && rewardedCurrencySprite != null)
		{
			mGainedCurrencyCoroutine = StartCoroutine(GainCurrencyAnimation(rewardedCurrencyAmount, rewardedCurrencyTotal, rewardedCurrencySprite));
		}
		else if (rewardedItem != null)
		{
			BodyCurrencyParent.SetActive(false);
			InventoryTile.ClearDelegates(true);
			InventoryTile component = RewardedItemNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component.Populate(rewardedItem);
			if (rewardedItemAmount > 0)
			{
				RewardedItemAmount.text = "x" + rewardedItemAmount;
			}
			ItemGainedTween.Play();
		}
		else
		{
			BodyCurrencyParent.SetActive(false);
		}
		StartCoroutine(ResetMailPosCo());
	}

	private IEnumerator GainCurrencyAnimation(int rewardedCurrencyAmount, int rewardedCurrencyTotal, string rewardedCurrencySprite)
	{
		BodyCurrencyParent.SetActive(true);
		GainedCurrencySprite.spriteName = rewardedCurrencySprite;
		GainedCurrencyLabel.text = "+" + rewardedCurrencyAmount;
		yield return StartCoroutine(GainCurrencyTween.PlayAsCoroutine());
		GainedCurrencyLabel.text = " " + rewardedCurrencyTotal;
		GainCurrencyFinishTween.Play();
	}

	private IEnumerator ResetMailPosCo()
	{
		ResetMailPos();
		yield return null;
		ResetMailPos();
	}

	private void ResetMailPos()
	{
		BodyPanel.clipOffset = Vector2.zero;
		BodyPanel.GetComponent<UIScrollView>().UpdateScrollbars();
		SpringPanel component = BodyPanel.GetComponent<SpringPanel>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	public void ClearMailPreview()
	{
		Information.SetActive(false);
		Prompt.SetActive(true);
	}

	public void OnClickDelete()
	{
		MailButtonCallback mailButtonCallback = mDeleteCallback;
		mDeleteCallback = null;
		if (mailButtonCallback != null)
		{
			mailButtonCallback();
		}
		Information.SetActive(false);
		Prompt.SetActive(true);
	}

	public void DeleteMail(MailItem item)
	{
		mMailList.Remove(item);
	}

	public void DeleteAllRewardedMails()
	{
		for (int num = Singleton<PlayerInfoScript>.Instance.SaveData.Mails.Count - 1; num >= 0; num--)
		{
			MailItem mailItem = Singleton<PlayerInfoScript>.Instance.SaveData.Mails[num];
			if (mailItem.Rewarded)
			{
				if (mailItem.ID != string.Empty)
				{
					Singleton<PlayerInfoScript>.Instance.SaveData.DeleteMail(mailItem.ID);
				}
				Singleton<PlayerInfoScript>.Instance.SaveData.RemoveMail(num);
				mMailList.Remove(mailItem);
			}
		}
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<SocialController>.Instance.RefreshCurrentTab();
	}
}
