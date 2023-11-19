using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	public class FacebookSettings : ScriptableObject
	{
		[Serializable]
		public class UrlSchemes
		{
			[SerializeField]
			private List<string> list;

			public List<string> Schemes
			{
				get
				{
					return list;
				}
				set
				{
					list = value;
				}
			}

			public UrlSchemes(List<string> schemes = null)
			{
				list = ((schemes != null) ? schemes : new List<string>());
			}
		}

		private const string FacebookSettingsAssetName = "FacebookSettings";

		private const string FacebookSettingsPath = "FacebookSDK/SDK/Resources";

		private const string FacebookSettingsAssetExtension = ".asset";

		private static FacebookSettings instance;

		[SerializeField]
		private int selectedAppIndex;

		[SerializeField]
		private List<string> appIds = new List<string> { "0" };

		[SerializeField]
		private List<string> appLabels = new List<string> { "App Name" };

		[SerializeField]
		private bool cookie = true;

		[SerializeField]
		private bool logging = true;

		[SerializeField]
		private bool status = true;

		[SerializeField]
		private bool xfbml;

		[SerializeField]
		private bool frictionlessRequests = true;

		[SerializeField]
		private string iosURLSuffix = string.Empty;

		[SerializeField]
		private List<UrlSchemes> appLinkSchemes = new List<UrlSchemes>
		{
			new UrlSchemes()
		};

		public static int SelectedAppIndex
		{
			get
			{
				return Instance.selectedAppIndex;
			}
			set
			{
				if (Instance.selectedAppIndex != value)
				{
					Instance.selectedAppIndex = value;
					DirtyEditor();
				}
			}
		}

		public static List<string> AppIds
		{
			get
			{
				return Instance.appIds;
			}
			set
			{
				if (Instance.appIds != value)
				{
					Instance.appIds = value;
					DirtyEditor();
				}
			}
		}

		public static List<string> AppLabels
		{
			get
			{
				return Instance.appLabels;
			}
			set
			{
				if (Instance.appLabels != value)
				{
					Instance.appLabels = value;
					DirtyEditor();
				}
			}
		}

		public static string AppId
		{
			get
			{
				return AppIds[SelectedAppIndex];
			}
		}

		public static bool IsValidAppId
		{
			get
			{
				return AppId != null && AppId.Length > 0 && !AppId.Equals("0");
			}
		}

		public static bool Cookie
		{
			get
			{
				return Instance.cookie;
			}
			set
			{
				if (Instance.cookie != value)
				{
					Instance.cookie = value;
					DirtyEditor();
				}
			}
		}

		public static bool Logging
		{
			get
			{
				return Instance.logging;
			}
			set
			{
				if (Instance.logging != value)
				{
					Instance.logging = value;
					DirtyEditor();
				}
			}
		}

		public static bool Status
		{
			get
			{
				return Instance.status;
			}
			set
			{
				if (Instance.status != value)
				{
					Instance.status = value;
					DirtyEditor();
				}
			}
		}

		public static bool Xfbml
		{
			get
			{
				return Instance.xfbml;
			}
			set
			{
				if (Instance.xfbml != value)
				{
					Instance.xfbml = value;
					DirtyEditor();
				}
			}
		}

		public static string IosURLSuffix
		{
			get
			{
				return Instance.iosURLSuffix;
			}
			set
			{
				if (Instance.iosURLSuffix != value)
				{
					Instance.iosURLSuffix = value;
					DirtyEditor();
				}
			}
		}

		public static string ChannelUrl
		{
			get
			{
				return "/channel.html";
			}
		}

		public static bool FrictionlessRequests
		{
			get
			{
				return Instance.frictionlessRequests;
			}
			set
			{
				if (Instance.frictionlessRequests != value)
				{
					Instance.frictionlessRequests = value;
					DirtyEditor();
				}
			}
		}

		public static List<UrlSchemes> AppLinkSchemes
		{
			get
			{
				return Instance.appLinkSchemes;
			}
			set
			{
				if (Instance.appLinkSchemes != value)
				{
					Instance.appLinkSchemes = value;
					DirtyEditor();
				}
			}
		}

		private static FacebookSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("FacebookSettings") as FacebookSettings;
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<FacebookSettings>();
					}
				}
				return instance;
			}
		}

		public static void SettingsChanged()
		{
			DirtyEditor();
		}

		private static void DirtyEditor()
		{
		}
	}
}
