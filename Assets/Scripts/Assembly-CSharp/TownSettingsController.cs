using UnityEngine;

public class TownSettingsController : Singleton<TownSettingsController>
{
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
	public UIToggle AccountViewToggle;
	public UIToggle OptionsToggle;
	public UIToggle HelpToggle;
	public GameObject[] TogglePanels;
	public GameObject MusicButtonOn;
	public GameObject MusicButtonOff;
	public GameObject SoundButtonOn;
	public GameObject SoundButtonOff;
	public GameObject TiltCamButtonOn;
	public GameObject TiltCamButtonOff;
	public bool Privacy;
	public bool Custom;
	public string LinkCustom;
}
