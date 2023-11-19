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
}
