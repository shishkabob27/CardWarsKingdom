public class MissionCompletePopupController : Singleton<MissionCompletePopupController>
{
	public UITweenController ShowTween;

	public UILabel MissionCompleteLabel;

	private int mCountToShow = -1;

	private bool mShowing;

	public void Show(int missionCount)
	{
		mCountToShow = missionCount;
		Update();
	}

	public bool Showing()
	{
		return mShowing;
	}

	private void Update()
	{
		if (mCountToShow == -1 || !Singleton<TownController>.Instance.IsIntroDone() || LoadingScreenController.ShowingLoadingScreen())
		{
			return;
		}
		if (Singleton<TutorialController>.Instance.IsFTUETutorialActive() || Singleton<TutorialController>.Instance.IsGachaTutorialActive())
		{
			mCountToShow = -1;
			return;
		}
		mShowing = true;
		ShowTween.Play();
		if (mCountToShow == 1)
		{
			MissionCompleteLabel.text = KFFLocalization.Get("!!MISSION_COMPLETE_POPUP");
		}
		else
		{
			MissionCompleteLabel.text = KFFLocalization.Get("!!MISSIONS_COMPLETE_POPUP").Replace("<val1>", mCountToShow.ToString());
		}
		mCountToShow = -1;
	}

	public void OnFinished()
	{
		mShowing = false;
	}
}
