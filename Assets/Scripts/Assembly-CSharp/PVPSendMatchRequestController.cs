using UnityEngine;

public class PVPSendMatchRequestController : Singleton<PVPSendMatchRequestController>
{
	public UITweenController ShowWaitingForResponseTween;

	public UITweenController HideWaitingForResponseTween;

	public GameObject RequestCancelButton;

	private bool mAllyRequestInProgress;

	private HelperItem mSendingRequestToAlly;

	private ChatMetaData mSendingRequestToChatPlayer;

	private void Update()
	{
		if (mAllyRequestInProgress && !mSendingRequestToAlly.OnlineStatus)
		{
			Reset();
			HideWaitingForResponseTween.Play();
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_OFFLINE"));
		}
	}

	private void Reset()
	{
		mAllyRequestInProgress = false;
		mSendingRequestToAlly = null;
		mSendingRequestToChatPlayer = null;
	}

	private string RequestPlayerName()
	{
		if (mSendingRequestToAlly != null)
		{
			return mSendingRequestToAlly.HelperName;
		}
		return mSendingRequestToChatPlayer.Name;
	}

	private string RequestPlayerId()
	{
		if (mSendingRequestToAlly != null)
		{
			return mSendingRequestToAlly.HelperID;
		}
		return mSendingRequestToChatPlayer.UserId;
	}

	public void SendMatchRequest(HelperItem ally)
	{
		Reset();
		mSendingRequestToAlly = ally;
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!SEND_MATCH_INVITE_PROMPT").Replace("<val1>", RequestPlayerName()), ConfirmSendRequest, null);
	}

	public void SendMatchRequest(ChatMetaData chatPlayer)
	{
		Reset();
		mSendingRequestToChatPlayer = chatPlayer;
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!SEND_MATCH_INVITE_PROMPT").Replace("<val1>", RequestPlayerName()), ConfirmSendRequest, null);
	}

	private void ConfirmSendRequest()
	{
		ShowWaitingForResponseTween.Play();
		RequestCancelButton.SetActive(true);
		if (mSendingRequestToAlly != null)
		{
			mAllyRequestInProgress = true;
		}
		Singleton<MultiplayerMessageHandler>.Instance.SendMatchRequestToPlayer(RequestPlayerName(), RequestPlayerId());
	}

	public void OnJoinProcessStarted()
	{
		MenuStackManager.RemoveTopItemFromStack(true);
		RequestCancelButton.SetActive(false);
	}

	public void OnClickCancelMatchRequest()
	{
		Reset();
		HideWaitingForResponseTween.Play();
		Singleton<MultiplayerMessageHandler>.Instance.CancelMatchRequest();
	}

	public void OnMatchRequestAccepted()
	{
		HideWaitingForResponseTween.Play();
		Singleton<PlayerInfoScript>.Instance.PvPData.AmIPrimary = true;
		Singleton<PVPPrepScreenController>.Instance.Show(PvpMode.Friend, RequestPlayerName());
		Reset();
	}

	public void OnMatchRequestDeclined(MultiplayerMessageHandler.MatchRequestRejectReason reason)
	{
		Reset();
		HideWaitingForResponseTween.Play();
		switch (reason)
		{
		case MultiplayerMessageHandler.MatchRequestRejectReason.NotAvailable:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_UNAVAILABLE"));
			break;
		case MultiplayerMessageHandler.MatchRequestRejectReason.NotUnlocked:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_HASNT_UNLOCKED_PVP"));
			break;
		case MultiplayerMessageHandler.MatchRequestRejectReason.Blocked:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_BLOCKED_YOU"));
			break;
		case MultiplayerMessageHandler.MatchRequestRejectReason.Incompatible:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_INCOMPATIBLE"));
			break;
		default:
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!MATCH_INVITE_DECLINED"));
			break;
		}
	}
}
