using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HelpshiftConfig : ScriptableObject
{
	private const string helpshiftConfigAssetName = "HelpshiftConfig";

	private const string helpshiftConfigPath = "Helpshift/Resources";

	private static HelpshiftConfig instance;

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
	private string[] contactUsOptions = new string[3] { "always", "never", "after_viewing_faqs" };

	[SerializeField]
	private string unityGameObject;

	[SerializeField]
	private string notificationIcon;

	[SerializeField]
	private string notificationSound;

	public static HelpshiftConfig Instance
	{
		get
		{
			instance = Resources.Load("HelpshiftConfig") as HelpshiftConfig;
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<HelpshiftConfig>();
			}
			return instance;
		}
	}

	public bool GotoConversation
	{
		get
		{
			return gotoConversation;
		}
		set
		{
			if (gotoConversation != value)
			{
				gotoConversation = value;
				DirtyEditor();
			}
		}
	}

	public int ContactUs
	{
		get
		{
			return contactUsOption;
		}
		set
		{
			if (contactUsOption != value)
			{
				contactUsOption = value;
				DirtyEditor();
			}
		}
	}

	public bool PresentFullScreenOniPad
	{
		get
		{
			return presentFullScreen;
		}
		set
		{
			if (presentFullScreen != value)
			{
				presentFullScreen = value;
				DirtyEditor();
			}
		}
	}

	public bool EnableInAppNotification
	{
		get
		{
			return enableInApp;
		}
		set
		{
			if (enableInApp != value)
			{
				enableInApp = value;
				DirtyEditor();
			}
		}
	}

	public bool EnableDialogUIForTablets
	{
		get
		{
			return enableDialogUIForTablets;
		}
		set
		{
			if (enableDialogUIForTablets != value)
			{
				enableDialogUIForTablets = value;
				DirtyEditor();
			}
		}
	}

	public bool RequireEmail
	{
		get
		{
			return requireEmail;
		}
		set
		{
			if (requireEmail != value)
			{
				requireEmail = value;
				DirtyEditor();
			}
		}
	}

	public bool HideNameAndEmail
	{
		get
		{
			return hideNameAndEmail;
		}
		set
		{
			if (hideNameAndEmail != value)
			{
				hideNameAndEmail = value;
				DirtyEditor();
			}
		}
	}

	public bool EnablePrivacy
	{
		get
		{
			return enablePrivacy;
		}
		set
		{
			if (enablePrivacy != value)
			{
				enablePrivacy = value;
				DirtyEditor();
			}
		}
	}

	public bool ShowSearchOnNewConversation
	{
		get
		{
			return showSearchOnNewConversation;
		}
		set
		{
			if (showSearchOnNewConversation != value)
			{
				showSearchOnNewConversation = value;
				DirtyEditor();
			}
		}
	}

	public string ConversationPrefillText
	{
		get
		{
			return conversationPrefillText;
		}
		set
		{
			if (conversationPrefillText != value)
			{
				conversationPrefillText = value;
				DirtyEditor();
			}
		}
	}

	public string ApiKey
	{
		get
		{
			return apiKey;
		}
		set
		{
			if (apiKey != value)
			{
				apiKey = value;
				DirtyEditor();
			}
		}
	}

	public string DomainName
	{
		get
		{
			return domainName;
		}
		set
		{
			if (domainName != value)
			{
				domainName = value;
				DirtyEditor();
			}
		}
	}

	public string AndroidAppId
	{
		get
		{
			return androidAppId;
		}
		set
		{
			if (androidAppId != value)
			{
				androidAppId = value;
				DirtyEditor();
			}
		}
	}

	public string iOSAppId
	{
		get
		{
			return iosAppId;
		}
		set
		{
			if (iosAppId != value)
			{
				iosAppId = value;
				DirtyEditor();
			}
		}
	}

	public string UnityGameObject
	{
		get
		{
			return unityGameObject;
		}
		set
		{
			if (unityGameObject != value)
			{
				unityGameObject = value;
				DirtyEditor();
			}
		}
	}

	public string NotificationIcon
	{
		get
		{
			return notificationIcon;
		}
		set
		{
			if (notificationIcon != value)
			{
				notificationIcon = value;
				DirtyEditor();
			}
		}
	}

	public string NotificationSound
	{
		get
		{
			return notificationSound;
		}
		set
		{
			if (notificationSound != value)
			{
				notificationSound = value;
				DirtyEditor();
			}
		}
	}

	public Dictionary<string, string> InstallConfig
	{
		get
		{
			return instance.getInstallConfig();
		}
	}

	public Dictionary<string, object> ApiConfig
	{
		get
		{
			return instance.getApiConfig();
		}
	}

	private static void DirtyEditor()
	{
	}

	public void SaveConfig()
	{
	}

	public Dictionary<string, object> getApiConfig()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string value = instance.contactUsOptions[instance.contactUsOption];
		dictionary.Add("enableContactUs", value);
		dictionary.Add("gotoConversationAfterContactUs", (!instance.gotoConversation) ? "no" : "yes");
		dictionary.Add("presentFullScreenOniPad", (!instance.presentFullScreen) ? "no" : "yes");
		dictionary.Add("requireEmail", (!instance.requireEmail) ? "no" : "yes");
		dictionary.Add("hideNameAndEmail", (!instance.hideNameAndEmail) ? "no" : "yes");
		dictionary.Add("enableFullPrivacy", (!instance.enablePrivacy) ? "no" : "yes");
		dictionary.Add("showSearchOnNewConversation", (!instance.showSearchOnNewConversation) ? "no" : "yes");
		dictionary.Add("conversationPrefillText", instance.conversationPrefillText);
		return dictionary;
	}

	public Dictionary<string, string> getInstallConfig()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("unityGameObject", instance.unityGameObject);
		dictionary.Add("notificationIcon", instance.notificationIcon);
		dictionary.Add("notificationSound", instance.notificationSound);
		dictionary.Add("enableDialogUIForTablets", (!instance.enableDialogUIForTablets) ? "no" : "yes");
		dictionary.Add("enableInAppNotification", (!instance.enableInApp) ? "no" : "yes");
		dictionary.Add("__hs__apiKey", instance.ApiKey);
		dictionary.Add("__hs__domainName", instance.DomainName);
		dictionary.Add("__hs__appId", instance.AndroidAppId);
		return dictionary;
	}
}
