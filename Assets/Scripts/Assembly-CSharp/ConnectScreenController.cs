using System;
using UnityEngine;

public class ConnectScreenController : MonoBehaviour
{
	public UITweenController ShowBusy;

	public UITweenController HideBusy;

	public TermsOfServicePanel TOSPanel;

	public UITexture ChinaScreenTexture;

	public UITexture LoadingLogo;

	private bool IsComplete;

	private bool mPlayerInfoLoading;

	private bool mFirstStepGuestLoginStarted;

	private bool mRealLoginStarted;

	private bool mLoginConfirmed;

	private bool mReadyCheck;

	private bool mUseFacebook;

	private LoginController mloginController;

	public bool IsLogoTweenCompleted;

	private void OnLogingError()
	{
		if (HideBusy != null)
		{
			HideBusy.Play();
		}
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), RetryLogin, KFFLocalization.Get("!!RETRY"));
	}

	private void RetryLogin()
	{
		Singleton<PlayerInfoScript>.Instance.Login();
	}

	private void OnEnable()
	{
		Singleton<PlayerInfoScript>.Instance.OnLoginFail += OnLogingError;
	}

	private void OnDisable()
	{
		if (Singleton<PlayerInfoScript>.Instance != null)
		{
			Singleton<PlayerInfoScript>.Instance.OnLoginFail -= OnLogingError;
			if (mloginController != null)
			{
				mloginController.OnGuestLogin -= GuestLoginSelected;
			}
		}
	}

	private void OnDestroy()
	{
		try
		{
			LoadingLogo.UnloadTexture();
		}
		catch (Exception)
		{
		}
	}

	public void PreGuestLogin()
	{
		if (HideBusy != null)
		{
			HideBusy.End();
		}
		if (Singleton<PlayerInfoScript>.Instance.HasAuthedBefore)
		{
			mRealLoginStarted = true;
			mLoginConfirmed = true;
		}
		if (ShowBusy != null)
		{
			ShowBusy.Play();
		}
		mFirstStepGuestLoginStarted = true;
	}

	public void LoginPrompt()
	{
		if (HideBusy != null)
		{
			HideBusy.Play();
		}
		LoginController loginController = UnityEngine.Object.FindObjectOfType<LoginController>();
		if (loginController != null)
		{
			loginController.OnGuestLogin += GuestLoginSelected;
			mLoginConfirmed = false;
			loginController.ActivateLoginDialog();
		}
		else
		{
			if (ShowBusy != null)
			{
				ShowBusy.Play();
			}
			Singleton<PlayerInfoScript>.Instance.Login();
			mLoginConfirmed = true;
		}
		mRealLoginStarted = true;
	}

	public void GuestLoginSelected()
	{
		mLoginConfirmed = true;
		mUseFacebook = false;
		if (ShowBusy != null)
		{
			ShowBusy.Play();
		}
	}

	private void Start()
	{
		ChinaScreenTexture.gameObject.SetActive(true);
		LoadingScreenController.LoadLoadingScreenLogo(LoadingLogo);
	}

	private void Update()
	{
		if (IsComplete)
		{
			return;
		}
		if (!mPlayerInfoLoading)
		{
			Singleton<PlayerInfoScript>.Instance.Login();
			mPlayerInfoLoading = true;
		}
		if (!Singleton<PlayerInfoScript>.Instance.IsReady())
		{
			return;
		}
		if (!mFirstStepGuestLoginStarted)
		{
			PreGuestLogin();
		}
		if (!mRealLoginStarted)
		{
			GuestLoginSelected();
		}
		if (!mLoginConfirmed)
		{
			return;
		}
		mReadyCheck = true;
		if (!mReadyCheck)
		{
			return;
		}
		IsComplete = true;
		if (HideBusy != null)
		{
			HideBusy.Play();
		}
		if (Singleton<PlayerInfoScript>.Instance.SaveData.ConfirmedTOSVersion == 0)
		{
			TOSPanel.Show(delegate
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.ConfirmedTOSVersion = 1;
				Proceed();
			});
		}
		else
		{
			Proceed();
		}
	}

	private void Proceed()
	{
		if (!Singleton<TutorialController>.Instance.IsBlockComplete("IntroBattle"))
		{
			Singleton<KFFSocialManager>.Instance.ReportAchievement(KFFSocialManager.AchievementIDs.DW_NEWGAME);
			Singleton<TutorialController>.Instance.LoadInitialTutorialQuest();
			Singleton<PlayerInfoScript>.Instance.SaveData.SetInstalledDate(SessionManager.Instance.theSession.ThePlayer.InstalledDate);
		}
		else
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadFrontEndScene();
		}
	}
}
