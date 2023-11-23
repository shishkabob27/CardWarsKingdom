using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class TownSettingsController : Singleton<TownSettingsController>
{
	public enum OptionTypeKPI
	{
		Music,
		SFX,
		CameraTilt,
		Language
	}

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowMenuTween;

	public UITweenController HideMenuTween;

	public UILabel PlayerName;

	public UILabel PlayerID;

	public UILabel ButtonConnectLabel;

	public GameObject PrivacyPolicyButton;

	public UIGrid ButtonsParent;

	public GameObject FaqButton;

	public GameObject DragonStonesButton;

	public GameObject SctlButton;

	public GameObject SettlementsButton;

	public GameObject FacebookButton;

	public UITexture LeaderTex;

	public UILabel versionLbl;

	public UIPopupList langList;

	public PopupListLocalizationKeys langListPopupKeys;

	public UILabel langLbl;

	private string mLeaderName;

	private bool mAudioInitialized;

	private float mMusicVol;

	private float mSoundVol;

	private bool mTiltCam;

	public static List<string> langeSettingList = new List<string> { "ENGLISH", "SPANISH", "FRENCH", "ITALIAN", "JAPANESE", "KOREAN", "PORTUGUESE", "RUSSIAN", "TURKISH" };

	public UIToggle AccountViewToggle;

	public UIToggle OptionsToggle;

	public UIToggle HelpToggle;

	public GameObject[] TogglePanels;

	private UIToggle mCurrentToggle;

	public GameObject MusicButtonOn;

	public GameObject MusicButtonOff;

	public GameObject SoundButtonOn;

	public GameObject SoundButtonOff;

	public GameObject TiltCamButtonOn;

	public GameObject TiltCamButtonOff;

	private bool isTabInitialized;

	public bool Privacy = true;

	public bool Custom;

	private string LinkPrivacy = "http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a";

	private string LinkTerms = "http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a";

	public string LinkCustom;

	public static ResponseFlag CodeRedeemFlag { get; set; }

	private void Start()
	{
		showVersionNumber();
		langListPopupKeys.LocalizedKeys = new List<string>();
		for (int i = 0; i < langeSettingList.Count; i++)
		{
			langListPopupKeys.LocalizedKeys.Add("!!" + langeSettingList[i]);
		}
		UpdateLangListPosition();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
	}

	public void Show()
	{
		ShowTween.Play();
		if (FaqButton != null)
		{
			FaqButton.SetActive(true);
		}
		if (DragonStonesButton != null)
		{
			DragonStonesButton.SetActive(false);
		}
		if (SctlButton != null)
		{
			SctlButton.SetActive(false);
		}
		if (SettlementsButton != null)
		{
			SettlementsButton.SetActive(false);
		}
		FacebookButton.SetActive(Singleton<PlayerInfoScript>.Instance.IsPastAgeGate(MiscParams.fbAgeGate));
		if (ButtonsParent != null)
		{
			ButtonsParent.Reposition();
		}
		Singleton<HelpScreenController>.Instance.Populate();
		mTiltCam = Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam;
		mMusicVol = Singleton<SLOTAudioManager>.Instance.musicVolume;
		mSoundVol = Singleton<SLOTAudioManager>.Instance.soundVolume;
		if (mTiltCam)
		{
			ToggleTiltCamOn();
		}
		else
		{
			ToggleTiltCamOff();
		}
		if (mMusicVol > 0f)
		{
			ToggleMusicOn();
		}
		else
		{
			ToggleMusicOff();
		}
		if (mSoundVol > 0f)
		{
			ToggleSoundOn();
		}
		else
		{
			ToggleSoundOff();
		}
		UpdatePanelState();
	}

	public void OnClickClose()
	{
		if (mTiltCam != Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam || mMusicVol != Singleton<SLOTAudioManager>.Instance.musicVolume || mSoundVol != Singleton<SLOTAudioManager>.Instance.soundVolume)
		{
			Singleton<PlayerInfoScript>.Instance.Save();
		}
		HideTween.Play();
	}

	public void OnClickMenuClose()
	{
		HideMenuTween.Play();
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
		if (Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
			currentLoadout.Leader.Form.ParseKeywords();
			if (string.IsNullOrEmpty(mLeaderName) || mLeaderName != currentLoadout.Leader.SelectedSkin.Name)
			{
				Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(currentLoadout.Leader.SelectedSkin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", LeaderTex);
				mLeaderName = currentLoadout.Leader.Form.Name;
			}
		}
	}

	private void UpdatePanelState()
	{
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		PlayerID.text = instance.GetFormattedPlayerCode();
		PlayerName.text = instance.GetPlayerName();
		ButtonConnectLabel.text = KFFLocalization.Get((!instance.IsFacebookLogin()) ? "!!FB_CONNECT_BUTTON" : "!!FB_LOGOUT");
	}

	public void OnClickCustomize()
	{
		Singleton<PlayerCustomizationController>.Instance.ShowDefault();
	}

	public void OnClickFacebook()
	{
		PlayerInfoScript pInfo = Singleton<PlayerInfoScript>.Instance;
		pInfo.Save();
		if (!pInfo.IsFacebookLogin())
		{
			Singleton<SimplePopupController>.Instance.ShowPrompt(KFFLocalization.Get("!!FB_CONNECT_TITLE"), KFFLocalization.Get("!!FB_CONNECT_PROMPT"), delegate
			{
				pInfo.FB_Connect();
				Singleton<ChatManager>.Instance.EndChat();
			}, delegate
			{
			});
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowPrompt(KFFLocalization.Get("!!FB_LOGOUT_TITLE"), KFFLocalization.Get("!!FB_LOGOUT_PROMPT"), delegate
			{
				pInfo.FB_Logout();
				Singleton<ChatManager>.Instance.EndChat();
			}, delegate
			{
			});
		}
	}

	public void OnClickUnblockPlayers()
	{
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!CLEAR_IGNORE_LIST_PROMPT"), ConfirmUnblock, null);
	}

	private void ConfirmUnblock()
	{
		Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.Clear();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public void SwitchTab()
	{
		if (!isTabInitialized)
		{
			isTabInitialized = true;
			mCurrentToggle = UIToggle.GetActiveToggle(AccountViewToggle.group);
			for (int i = 0; i < TogglePanels.Length; i++)
			{
				TogglePanels[i].SetActive(i == mCurrentToggle.toggleId);
			}
		}
		ShowMenuTween.Play();
	}

	public void OnSoundVolumeChange()
	{
		Singleton<SLOTAudioManager>.Instance.SetSoundVolumeMasterAudio();
	}

	public void OnMusicVolumeChange()
	{
		Singleton<SLOTAudioManager>.Instance.SetMusicVolumeMasterAudio();
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

	public void ToggleTiltCamOn()
	{
		Singleton<MouseOrbitCamera>.Instance.EnableTiltCam(true);
		TiltCamButtonOn.SetActive(false);
		TiltCamButtonOff.SetActive(true);
		PlayerPrefs.SetInt("EnableTiltCam", 1);
	}

	public void ToggleTiltCamOff()
	{
		Singleton<MouseOrbitCamera>.Instance.EnableTiltCam(false);
		TiltCamButtonOff.SetActive(false);
		TiltCamButtonOn.SetActive(true);
		PlayerPrefs.SetInt("EnableTiltCam", 0);
	}

	private string GetLinkPrivacy()
	{
		return LinkPrivacy;
	}

	private string GetLinkTerms()
	{
		return LinkTerms;
	}

	public void OpenUrl()
	{
		if (testInternetConnection())
		{
			if (Privacy)
			{
				string text = KFFLocalization.Get("!!PRIVACY_LINK");
				if (!string.IsNullOrEmpty(text))
				{
					text = LinkPrivacy;
				}
				Application.OpenURL(text);
			}
			else if (Custom)
			{
				Application.OpenURL(LinkCustom);
			}
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public void OpenPrivacyPolicy()
	{
		if (testInternetConnection())
		{
			string text = KFFLocalization.Get("!!PRIVACY_LINK");
			if (string.IsNullOrEmpty(text))
			{
				text = GetLinkPrivacy();
			}
			Application.OpenURL(text);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public void OpenTermsOfUse()
	{
		if (testInternetConnection())
		{
			string text = KFFLocalization.Get("!!TERMSOFUSE_LINK");
			if (string.IsNullOrEmpty(text))
			{
				text = GetLinkTerms();
			}
			Application.OpenURL(text);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public static bool testInternetConnection()
	{
		bool flag = false;
		if (Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return false;
		}
		return true;
	}

	private void showInternetAlert()
	{
	}

	public void OnClickDragonStonesButton()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string body = string.Format(KFFLocalization.Get("!!DRAGON_STONES_INFO_POPUP"), saveData.HardCurrency.ToString(), saveData.PaidHardCurrency.ToString(), saveData.FreeHardCurrency.ToString());
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body);
	}

	public void OnClickSCTLButton()
	{
		if (testInternetConnection())
		{
			Application.OpenURL("http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a");
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public void OnClickSettlementsButton()
	{
		if (testInternetConnection())
		{
			Application.OpenURL("http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a");
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public void showVersionNumber()
	{
		if (versionLbl != null)
		{
			versionLbl.text = SQSettings.SERVER_PREFIX + " v." + Application.version;
		}
	}

	public void changeLangSetting()
	{
		try
		{
			string value = langeSettingList[getLangSettingListIndex()];
			SystemLanguage systemLanguage = (SystemLanguage)(int)Enum.Parse(typeof(SystemLanguage), value, true);
			LanguageCode languageCode = Language.LanguageNameToCode(systemLanguage);
			if (languageCode != Language.CurrentLanguage())
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.selectedLang = languageCode;
				PlayerPrefs.SetString("M2H_lastLanguage", languageCode.ToString());
				Language.SwitchLanguage(languageCode);
				langList.Close();
				SessionManager.Instance.theSession.ReloadGame();
			}
		}
		catch
		{
		}
	}

	private int getLangSettingListIndex()
	{
		for (int i = 0; i < langList.items.Count; i++)
		{
			if (langList.items[i].Equals(langList.value))
			{
				return i;
			}
		}
		return 0;
	}

	public void UpdateLangListPosition()
	{
		string text = Language.LanguageCodeToName(Language.CurrentLanguage()).ToString();
		for (int i = 0; i < langeSettingList.Count; i++)
		{
			if (text.ToUpper().Equals(langeSettingList[i]))
			{
				langList.value = langList.items[i];
				langLbl.text = KFFLocalization.Get("!!" + langeSettingList[i]);
				langList.UpdatetextLabelToSelection(langLbl);
			}
		}
	}
}
