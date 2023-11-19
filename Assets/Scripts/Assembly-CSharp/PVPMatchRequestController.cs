public class PVPMatchRequestController : Singleton<PVPMatchRequestController>
{
	public UITweenController ShowRequestTween;

	public UITweenController HideRequestTween;

	public UILabel PlayerNameLabel;

	private static MultiplayerMessageHandler.ReceivedMatchRequest mShowingRequest;

	private void Update()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.FrontEnd)
		{
			if (LoadingScreenController.ShowingLoadingScreen() || !Singleton<TownEnvironmentHolder>.Instance.Loaded || !Singleton<TownController>.Instance.IsIntroDone() || !Singleton<MultiplayerMessageHandler>.Instance.AnyMatchRequestsPending())
			{
				return;
			}
			string text = Singleton<MultiplayerMessageHandler>.Instance.EnteringFriendsGame();
			if (!Singleton<PlayerInfoScript>.Instance.CanPvp())
			{
				DeclineNextRequest(MultiplayerMessageHandler.MatchRequestRejectReason.NotUnlocked);
			}
			else if (Singleton<PVPPrepScreenController>.Instance.InFriendMatchLobby() || Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				DeclineNextRequest(MultiplayerMessageHandler.MatchRequestRejectReason.NotAvailable);
			}
			else
			{
				if (mShowingRequest != null)
				{
					return;
				}
				MultiplayerMessageHandler.ReceivedMatchRequest nextMatchRequest = Singleton<MultiplayerMessageHandler>.Instance.GetNextMatchRequest();
				if (text != null)
				{
					if (text != nextMatchRequest.PlayerID)
					{
						Singleton<MultiplayerMessageHandler>.Instance.DeclineMatchRequest(nextMatchRequest, MultiplayerMessageHandler.MatchRequestRejectReason.NotAvailable);
					}
					return;
				}
				mShowingRequest = nextMatchRequest;
				ShowRequestTween.Play();
				PlayerNameLabel.text = mShowingRequest.PlayerName;
				if (Singleton<MouseOrbitCamera>.Instance.IsZoomedInToBuilding())
				{
					Singleton<CornerNotificationPopupController>.Instance.Show(CornerNotificationPopupController.PopupTypeEnum.PvpRequest);
				}
			}
		}
		else
		{
			DeclineNextRequest(MultiplayerMessageHandler.MatchRequestRejectReason.NotAvailable);
		}
	}

	private void DeclineNextRequest(MultiplayerMessageHandler.MatchRequestRejectReason reason)
	{
		MultiplayerMessageHandler.ReceivedMatchRequest nextMatchRequest = Singleton<MultiplayerMessageHandler>.Instance.GetNextMatchRequest();
		if (nextMatchRequest != null)
		{
			Singleton<MultiplayerMessageHandler>.Instance.DeclineMatchRequest(nextMatchRequest, reason);
		}
	}

	public void OnClickAccept()
	{
		MultiplayerMessageHandler.ReceivedMatchRequest request = mShowingRequest;
		mShowingRequest = null;
		Singleton<MultiplayerMessageHandler>.Instance.AcceptMatchRequest(request);
		MenuStackManager.RemoveTopItemFromStack(true);
	}

	public void OnClickDecline()
	{
		MultiplayerMessageHandler.ReceivedMatchRequest request = mShowingRequest;
		mShowingRequest = null;
		Singleton<MultiplayerMessageHandler>.Instance.DeclineMatchRequest(request, MultiplayerMessageHandler.MatchRequestRejectReason.Declined);
	}

	public void OnClickBlock()
	{
		string body = KFFLocalization.Get("!!IGNORE_PLAYER_PROMPT").Replace("<val1>", mShowingRequest.PlayerName);
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body, ConfirmBlock, null);
	}

	private void ConfirmBlock()
	{
		if (!Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.ContainsKey(mShowingRequest.PlayerID))
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.Add(mShowingRequest.PlayerID, null);
			Singleton<PlayerInfoScript>.Instance.Save();
		}
		ChatWindowController.Instance.PurgeBlockedUser(mShowingRequest.PlayerID);
		MultiplayerMessageHandler.ReceivedMatchRequest request = mShowingRequest;
		mShowingRequest = null;
		Singleton<MultiplayerMessageHandler>.Instance.DeclineMatchRequest(request, MultiplayerMessageHandler.MatchRequestRejectReason.Blocked);
		HideRequestTween.Play();
		MenuStackManager.RemoveTopItemFromStack(true);
	}

	public static bool ShowingRequestFromPlayer(string playerID)
	{
		return mShowingRequest != null && mShowingRequest.PlayerID == playerID;
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (mShowingRequest != null)
			{
				HideRequestTween.Play();
				mShowingRequest = null;
			}
			Singleton<MultiplayerMessageHandler>.Instance.ClearAllMatchRequests();
		}
	}
}
