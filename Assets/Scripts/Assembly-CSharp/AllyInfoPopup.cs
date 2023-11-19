public class AllyInfoPopup : Singleton<AllyInfoPopup>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UILabel AllyName;

	private HelperItem mAlly;

	public void Show(HelperItem ally)
	{
		mAlly = ally;
		ShowTween.Play();
		AllyName.text = ally.HelperName;
	}

	public void OnClickRemove()
	{
		string title = KFFLocalization.Get("!!REMOVE_ALLY");
		string body = KFFLocalization.Get("!!REMOVE_ALLY_CONFIRM");
		Singleton<HelperRequestController>.Instance.CurrentHelper = mAlly;
		Singleton<HelperRequestController>.Instance.ShowRemoveAllyConfirm(title, body, ConfirmRemove);
	}

	private void ConfirmRemove()
	{
		Singleton<HelperRequestController>.Instance.SendRemoveAlly();
		HideTween.Play();
	}

	public void OnClickChallenge()
	{
		if (!mAlly.OnlineStatus)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_OFFLINE"));
		}
		else
		{
			Singleton<PVPSendMatchRequestController>.Instance.SendMatchRequest(mAlly);
		}
	}
}
