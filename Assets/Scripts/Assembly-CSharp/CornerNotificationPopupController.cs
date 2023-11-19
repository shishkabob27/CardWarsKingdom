using System.Collections.Generic;

public class CornerNotificationPopupController : Singleton<CornerNotificationPopupController>
{
	public enum PopupTypeEnum
	{
		MissionsComplete,
		PvpRequest,
		ExpeditionsComplete
	}

	private class QueuedPopup
	{
		public PopupTypeEnum PopupType;

		public int Val;
	}

	public UITweenController ShowTween;

	public UILabel TopLine;

	public UILabel BottomLine;

	private List<QueuedPopup> mQueuedPopups = new List<QueuedPopup>();

	private bool mShowing;

	public void Show(PopupTypeEnum type, int val = -1)
	{
		QueuedPopup queuedPopup = new QueuedPopup();
		queuedPopup.PopupType = type;
		queuedPopup.Val = val;
		mQueuedPopups.Add(queuedPopup);
		Update();
	}

	private void Update()
	{
		if (mShowing || mQueuedPopups.Count <= 0 || !Singleton<TownController>.Instance.IsIntroDone() || LoadingScreenController.ShowingLoadingScreen() || Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			return;
		}
		ShowTween.Play();
		mShowing = true;
		QueuedPopup queuedPopup = mQueuedPopups[0];
		mQueuedPopups.RemoveAt(0);
		if (queuedPopup.PopupType == PopupTypeEnum.MissionsComplete)
		{
			if (queuedPopup.Val == 1)
			{
				TopLine.text = KFFLocalization.Get("!!MISSION_COMPLETE_POPUP");
			}
			else
			{
				TopLine.text = KFFLocalization.Get("!!MISSIONS_COMPLETE_POPUP").Replace("<val1>", queuedPopup.Val.ToString());
			}
			BottomLine.text = KFFLocalization.Get("!!COLLECT_MISSION_REWARDS");
		}
		else if (queuedPopup.PopupType == PopupTypeEnum.PvpRequest)
		{
			TopLine.text = KFFLocalization.Get("!!MATCH_REQUEST_RECEIVED");
			BottomLine.text = KFFLocalization.Get("!!MATCH_REQUEST_ACCEPT_DECLINE");
		}
		else if (queuedPopup.PopupType == PopupTypeEnum.ExpeditionsComplete)
		{
			if (queuedPopup.Val == 1)
			{
				TopLine.text = KFFLocalization.Get("!!EXPEDITION_COMPLETE_POPUP");
			}
			else
			{
				TopLine.text = KFFLocalization.Get("!!EXPEDITIONS_COMPLETE_POPUP").Replace("<val1>", queuedPopup.Val.ToString());
			}
			BottomLine.text = KFFLocalization.Get("!!COLLECT_MISSION_REWARDS");
		}
	}

	public void OnTweenFinished()
	{
		mShowing = false;
	}
}
