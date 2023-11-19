using UnityEngine;

public class PvPMainScreenController : MonoBehaviour
{
	public delegate void Callback();

	public bool Visible;

	public UITweenController ShowPVPMain;

	private Callback mFinishedCallback;

	private static PvPMainScreenController m_instance;

	private void Awake()
	{
		m_instance = this;
	}

	public static PvPMainScreenController GetActive()
	{
		return m_instance;
	}

	public void Populate()
	{
		Update();
		Visible = true;
	}

	private void Update()
	{
	}

	public void Show(Callback finishedCallback = null)
	{
		ShowPVPMain.Play();
		mFinishedCallback = finishedCallback;
	}

	public void PrepRandomMatch()
	{
	}

	public void OnClickClose()
	{
		if (mFinishedCallback != null)
		{
			mFinishedCallback();
			mFinishedCallback = null;
		}
	}

	public void OnCloseTweenFinished()
	{
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}
}
