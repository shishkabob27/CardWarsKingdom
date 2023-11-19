public class PvPMatchRequestPopupController : Singleton<PvPMatchRequestPopupController>
{
	public UITweenController ShowTween;

	private bool mShouldShow;

	public void Show()
	{
		mShouldShow = true;
		Update();
	}

	private void Update()
	{
		if (mShouldShow && Singleton<TownController>.Instance.IsIntroDone() && !LoadingScreenController.ShowingLoadingScreen() && !Singleton<MissionCompletePopupController>.Instance.Showing())
		{
			ShowTween.Play();
			mShouldShow = false;
		}
	}
}
