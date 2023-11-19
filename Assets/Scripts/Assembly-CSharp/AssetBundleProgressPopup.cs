using UnityEngine;

public class AssetBundleProgressPopup : Singleton<AssetBundleProgressPopup>
{
	public float DelayBeforeSpinnerAppearing;

	public float DelayBeforeBarAppearing;

	public float SingleBarLabelOffset;

	public GameObject Panel;

	public GameObject ProgressBarParent;

	public UISprite ProgressBar;

	public UISprite FileProgressBar;

	public UILabel ProgressLabel;

	public Transform LoadingLabel;

	private float mTimeLoading = -1f;

	private Vector3 mBaseLabelPos;

	private bool mShowFileProgress;

	private void Awake()
	{
		mBaseLabelPos = LoadingLabel.localPosition;
	}

	private void Update()
	{
		float totalProgress;
		float fileProgress;
		Singleton<SLOTResourceManager>.Instance.GetResourceLoadProgress(out totalProgress, out fileProgress);
		fileProgress = -1f;
		bool flag = LoadingScreenController.ShowingLoadingScreenNotFadingOut() || DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.AssetBundleDownload;
		if (totalProgress != -1f)
		{
			float num = mTimeLoading;
			if (mTimeLoading == -1f)
			{
				if (flag)
				{
					mTimeLoading = DelayBeforeBarAppearing;
				}
				else
				{
					mTimeLoading = 0f;
				}
				UICamera.LockInput();
			}
			mTimeLoading += Time.deltaTime;
			if (num < DelayBeforeSpinnerAppearing && mTimeLoading >= DelayBeforeSpinnerAppearing)
			{
				Panel.SetActive(true);
				ProgressBarParent.SetActive(false);
			}
			if (num < DelayBeforeBarAppearing && mTimeLoading >= DelayBeforeBarAppearing)
			{
				ProgressBarParent.SetActive(true);
				Singleton<BusyIconPanelController>.Instance.Show();
			}
			if (!ProgressBarParent.activeSelf)
			{
				return;
			}
			ProgressBar.fillAmount = totalProgress;
			ProgressLabel.text = (int)(totalProgress * 100f) + "%";
			if (fileProgress == -1f)
			{
				if (mShowFileProgress)
				{
					FileProgressBar.fillAmount = 1f;
					return;
				}
				FileProgressBar.transform.parent.gameObject.SetActive(false);
				Vector3 localPosition = mBaseLabelPos;
				localPosition.y += SingleBarLabelOffset;
				LoadingLabel.localPosition = localPosition;
			}
			else if (fileProgress != 1f || mShowFileProgress)
			{
				FileProgressBar.transform.parent.gameObject.SetActive(true);
				LoadingLabel.localPosition = mBaseLabelPos;
				FileProgressBar.fillAmount = fileProgress;
				mShowFileProgress = true;
			}
		}
		else
		{
			if (mTimeLoading != -1f)
			{
				Panel.SetActive(false);
				Singleton<BusyIconPanelController>.Instance.Hide();
				UICamera.UnlockInput();
			}
			mTimeLoading = -1f;
			mShowFileProgress = false;
		}
	}
}
