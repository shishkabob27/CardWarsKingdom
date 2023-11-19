using System;
using UnityEngine;

[Serializable]
public class HelpshiftConfig : ScriptableObject
{
	[SerializeField]
	private string apiKey;
	[SerializeField]
	private string domainName;
	[SerializeField]
	private string iosAppId;
	[SerializeField]
	private string androidAppId;
	[SerializeField]
	private int contactUsOption;
	[SerializeField]
	private bool gotoConversation;
	[SerializeField]
	private bool presentFullScreen;
	[SerializeField]
	private bool enableDialogUIForTablets;
	[SerializeField]
	private bool enableInApp;
	[SerializeField]
	private bool requireEmail;
	[SerializeField]
	private bool hideNameAndEmail;
	[SerializeField]
	private bool enablePrivacy;
	[SerializeField]
	private bool showSearchOnNewConversation;
	[SerializeField]
	private string conversationPrefillText;
	[SerializeField]
	private string[] contactUsOptions;
	[SerializeField]
	private string unityGameObject;
	[SerializeField]
	private string notificationIcon;
	[SerializeField]
	private string notificationSound;
}
