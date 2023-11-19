using UnityEngine;

public class QuickMessageController : Singleton<QuickMessageController>
{
	public float Cooldown;

	public GameObject ChatListPrefab;

	public UITweenController ShowButtonTween;

	public UITweenController HideButtonTween;

	public UITweenController ShowPanelTween;

	public UITweenController HidePanelTween;

	public UIStreamingGrid ChatGrid;

	private UIStreamingGridDataSource<QuickChatData> mChatGridDatasource = new UIStreamingGridDataSource<QuickChatData>();

	private bool mShowing;

	private float mCooldown = -1f;

	private QuickChatData mMessageToShow;

	private void Update()
	{
		if (mCooldown > 0f)
		{
			mCooldown -= Time.deltaTime;
			if (mCooldown <= 0f)
			{
				mCooldown = -1f;
			}
		}
		else if (mMessageToShow != null)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendQuickChat(mMessageToShow);
			Singleton<BattleHudController>.Instance.ShowChatMessage(true, mMessageToShow);
			mMessageToShow = null;
			mCooldown = Cooldown;
		}
	}

	public void ShowButton()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			ShowButtonTween.Play();
			UICamera.AlwaysAllowedColliderRoots.Add(base.transform);
		}
	}

	public void HideButton()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			HideButtonTween.Play();
			UICamera.AlwaysAllowedColliderRoots.Remove(base.transform);
		}
	}

	public void HideIfShowing()
	{
		if (mShowing)
		{
			mShowing = false;
			HidePanelTween.Play();
		}
	}

	public void OnClickChatButton()
	{
		if (!mShowing)
		{
			mShowing = true;
			ShowPanelTween.Play();
			mChatGridDatasource.Init(ChatGrid, ChatListPrefab, QuickChatDataManager.Instance.GetDatabase());
		}
	}

	public void OnClickClose()
	{
		if (mShowing)
		{
			mShowing = false;
			HidePanelTween.Play();
		}
	}

	public void OnClickChatEntry(QuickChatData data)
	{
		if (mShowing)
		{
			mMessageToShow = data;
			mShowing = false;
			HidePanelTween.Play();
		}
	}

	public void ShowOpponentChatMessage(QuickChatData data)
	{
		Singleton<BattleHudController>.Instance.ShowChatMessage(false, data);
	}

	public void Unload()
	{
		mShowing = false;
		mChatGridDatasource.Clear();
	}
}
