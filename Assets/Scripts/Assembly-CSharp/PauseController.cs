using System.Collections;
using UnityEngine;

public class PauseController : Singleton<PauseController>
{
	public UITweenController ShowPauseTween;

	public UITweenController HidePauseTween;

	public UITweenController ShowPauseButtonTween;

	public UITweenController HidePauseButtonTween;

	public GameObject PauseButton;

	public GameObject ResumeButton;

	public GameObject QuitButton;

	public GameObject PauseBackCollider;

	public GameObject PauseButtonParent;

	public GameObject InputBlockCollider;

	public GameObject MusicButtonOff;

	public GameObject MusicButtonOn;

	public GameObject SoundButtonOff;

	public GameObject SoundButtonOn;

	private bool mPaused;

	private bool mPauseShowing;

	private bool mAudioInitialized;

	public bool Paused
	{
		get
		{
			return mPaused;
		}
	}

	public bool ButtonShowing { get; private set; }

	private void Start()
	{
		if (!Singleton<TutorialController>.Instance.IsBlockComplete("IntroBattle") || Singleton<TutorialController>.Instance.IsFTUETutorialActive())
		{
			PauseButtonParent.SetActive(false);
			ButtonShowing = false;
		}
		else
		{
			PauseButtonParent.SetActive(true);
			ButtonShowing = true;
		}
	}

	private void Update()
	{
		if (!mAudioInitialized && Singleton<SLOTAudioManager>.Instance.IsInitialized())
		{
			float musicVolume = Singleton<SLOTAudioManager>.Instance.musicVolume;
			float soundVolume = Singleton<SLOTAudioManager>.Instance.soundVolume;
			if (musicVolume > 0f)
			{
				ToggleMusicOn();
			}
			else
			{
				ToggleMusicOff();
			}
			if (soundVolume > 0f)
			{
				ToggleSoundOn();
			}
			else
			{
				ToggleSoundOff();
			}
			mAudioInitialized = true;
		}
	}

	public void ShowButton()
	{
		if (Singleton<TutorialController>.Instance.IsBlockComplete("IntroBattle") && !Singleton<TutorialController>.Instance.IsFTUETutorialActive())
		{
			ButtonShowing = true;
			ShowPauseButtonTween.Play();
		}
	}

	public void HideButton()
	{
		ButtonShowing = false;
		UICamera.AlwaysAllowedColliders.RemoveAll((GameObject m) => m == PauseButton);
		HidePauseButtonTween.Play();
	}

	public void OnClickPause()
	{
		if (!mPaused && !mPauseShowing)
		{
			mPauseShowing = true;
			Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_PauseShow");
			ShowPauseTween.Play();
		}
	}

	public void HideIfShowing()
	{
		if (mPaused)
		{
			OnClickUnpause();
		}
	}

	public void PauseFromAppMinimize()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode || !ButtonShowing)
		{
			return;
		}
		foreach (DWBattleLaneObject item in Singleton<DWBattleLane>.Instance.BattleLaneObjects[(int)PlayerType.User])
		{
			if (item != null)
			{
				item.CancelPress();
			}
		}
		OnClickPause();
	}

	public void OnMenuShown()
	{
		UICamera.AlwaysAllowedColliders.Add(ResumeButton);
		UICamera.AlwaysAllowedColliders.Add(QuitButton);
		UICamera.AlwaysAllowedColliders.Add(PauseBackCollider);
		mPaused = true;
	}

	public void OnClickUnpause()
	{
		if (mPaused)
		{
			UICamera.AlwaysAllowedColliders.RemoveAll((GameObject m) => m == ResumeButton || m == QuitButton || m == PauseBackCollider);
			Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_PauseHide");
			HidePauseTween.Play();
			mPaused = false;
			mPauseShowing = false;
			MenuStackManager.RemoveAnyFromStack(ShowPauseTween);
		}
	}

	public void OnClickQuit()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			if (Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
			{
				Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!QUIT_RANKED_CONFIRM"), ConfirmQuit, null);
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!QUIT_UNRANKED_CONFIRM"), ConfirmQuit, null);
			}
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!QUIT_CONFIRM"), ConfirmQuit, null);
		}
	}

	private void ConfirmQuit()
	{
		UICamera.AlwaysAllowedColliders.RemoveAll((GameObject m) => m == ResumeButton || m == QuitButton || m == PauseBackCollider);
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendLeaveGame("leave");
			PvpMatchResultDetails resultDetails = Singleton<PlayerInfoScript>.Instance.RegisterPvpMatchResult(false);
			if (Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
			{
				Singleton<PvpBattleResultsController>.Instance.RegisterResults(resultDetails);
				Singleton<PlayerInfoScript>.Instance.Save();
				Singleton<BattleHudController>.Instance.CloseLeaderPopupIfShowing();
				Singleton<HandCardController>.Instance.UnzoomCard();
				OnClickUnpause();
				Singleton<PauseController>.Instance.HideButton();
				Singleton<QuickMessageController>.Instance.HideButton();
				Singleton<SLOTMusic>.Instance.PlayLoserMusic();
				Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
				UICamera.UnlockInput();
                if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
                {
                    Singleton<DWGame>.Instance.turnNumber = 0;
                    Singleton<DWGame>.Instance.battleDuration = 0f;
                }
                Singleton<DWGame>.Instance.SetGameState(GameState.P1Defeated);
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.Save();
				Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
				Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
				{
					StartCoroutine(ChangeScene());
				});
			}
		}
		else
		{
			Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
			Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
			{
				StartCoroutine(ChangeScene());
			});
		}
	}

	private IEnumerator ChangeScene()
	{
		yield return new WaitForSeconds(0.6f);
		Singleton<DWBattleLane>.Instance.ClearPooledData();
		DetachedSingleton<SceneFlowManager>.Instance.LoadFrontEndScene();
		yield return null;
	}

	public void BlockInput(bool blocked)
	{
		InputBlockCollider.SetActive(blocked);
	}

	public bool IsInputBlocked()
	{
		return InputBlockCollider.activeInHierarchy;
	}

	public void ToggleMusicOn()
	{
		Singleton<SLOTAudioManager>.Instance.SetMusicVolumeMasterAudio(1f);
		MusicButtonOff.SetActive(true);
		MusicButtonOn.SetActive(false);
	}

	public void ToggleMusicOff()
	{
		Singleton<SLOTAudioManager>.Instance.SetMusicVolumeMasterAudio(0f);
		MusicButtonOff.SetActive(false);
		MusicButtonOn.SetActive(true);
	}

	public void ToggleSoundOn()
	{
		Singleton<SLOTAudioManager>.Instance.SetSoundVolumeMasterAudio(1f);
		SoundButtonOff.SetActive(true);
		SoundButtonOn.SetActive(false);
	}

	public void ToggleSoundOff()
	{
		Singleton<SLOTAudioManager>.Instance.SetSoundVolumeMasterAudio(0f);
		SoundButtonOff.SetActive(false);
		SoundButtonOn.SetActive(true);
	}
}
