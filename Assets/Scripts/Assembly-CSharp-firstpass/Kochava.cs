using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using JsonFx.Json;
using UnityEngine;

[ExecuteInEditMode]
public class Kochava : MonoBehaviour
{
	public enum KochSessionTracking
	{
		full,
		basic,
		minimal,
		none
	}

	public enum KochLogLevel
	{
		error,
		warning,
		debug
	}

	public class LogEvent
	{
		public string text;

		public float time;

		public KochLogLevel level;

		public LogEvent(string text, KochLogLevel level)
		{
			this.text = text;
			time = Time.time;
			this.level = level;
		}
	}

	public class QueuedEvent
	{
		public float eventTime;

		public Dictionary<string, object> eventData;
	}

	public delegate void AttributionCallback(string callbackString);

	private const string CURRENCY_DEFAULT = "USD";

	public const string KOCHAVA_VERSION = "20150929";

	public const string KOCHAVA_PROTOCOL_VERSION = "4";

	private const int MAX_LOG_SIZE = 50;

	private const int MAX_QUEUE_SIZE = 75;

	private const int MAX_POST_TIME = 15;

	private const int POST_FAIL_RETRY_DELAY = 30;

	private const int QUEUE_KVINIT_WAIT_DELAY = 15;

	private const string API_URL = "https://control.kochava.com";

	private const string TRACKING_URL = "https://control.kochava.com/track/kvTracker?v4";

	private const string INIT_URL = "https://control.kochava.com/track/kvinit";

	private const string QUERY_URL = "https://control.kochava.com/track/kvquery";

	private const string AD_URL = "https://bidder.kochava.com/adserver/request/";

	private const string KOCHAVA_QUEUE_STORAGE_KEY = "kochava_queue_storage";

	private const int KOCHAVA_ATTRIBUTION_INITIAL_TIMER = 7;

	private const int KOCHAVA_ATTRIBUTION_DEFAULT_TIMER = 60;

	public string kochavaAppId = string.Empty;

	public string kochavaAppIdIOS = string.Empty;

	public string kochavaAppIdAndroid = string.Empty;

	public string kochavaAppIdKindle = string.Empty;

	public string kochavaAppIdBlackberry = string.Empty;

	public string kochavaAppIdWindowsPhone = string.Empty;

	public bool debugMode;

	public bool incognitoMode;

	public bool requestAttribution;

	private bool retrieveAttribution;

	private bool debugServer;

	[HideInInspector]
	public string appVersion = string.Empty;

	[HideInInspector]
	public string appIdentifier = string.Empty;

	[HideInInspector]
	public string partnerName = string.Empty;

	public bool appLimitAdTracking;

	[HideInInspector]
	public string userAgent = string.Empty;

	public bool adidSupressed;

	private static int device_id_delay = 60;

	private string whitelist;

	private static bool adidBlacklisted = false;

	private static AttributionCallback attributionCallback;

	private string appPlatform = "desktop";

	private string kochavaDeviceId = string.Empty;

	private string attributionDataStr = string.Empty;

	private List<string> devIdBlacklist = new List<string>();

	private List<string> eventNameBlacklist = new List<string>();

	public string appCurrency = "USD";

	public KochSessionTracking sessionTracking;

	private int KVTRACKER_WAIT = 60;

	private List<LogEvent> _EventLog = new List<LogEvent>();

	private Dictionary<string, object> hardwareIdentifierData = new Dictionary<string, object>();

	private Dictionary<string, object> hardwareIntegrationData = new Dictionary<string, object>();

	private Dictionary<string, object> appData;

	private Queue<QueuedEvent> eventQueue = new Queue<QueuedEvent>();

	private float processQueueKickstartTime;

	private bool queueIsProcessing;

	private float _eventPostingTime;

	private bool doReportLocation;

	private int locationAccuracy = 50;

	private int locationTimeout = 15;

	private int locationStaleness = 15;

	private int iAdAttributionAttempts = 3;

	private int iAdAttributionWait = 20;

	private int iAdRetryWait = 10;

	private bool send_id_updates;

	private static Kochava _S;

	private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private static float uptimeDelta;

	private static float uptimeDeltaUpdate;

	public static bool DebugMode
	{
		get
		{
			return _S.debugMode;
		}
		set
		{
			_S.debugMode = value;
		}
	}

	public static bool IncognitoMode
	{
		get
		{
			return _S.incognitoMode;
		}
		set
		{
			_S.incognitoMode = value;
		}
	}

	public static bool RequestAttribution
	{
		get
		{
			return _S.requestAttribution;
		}
		set
		{
			_S.requestAttribution = value;
		}
	}

	public static bool AppLimitAdTracking
	{
		get
		{
			return _S.appLimitAdTracking;
		}
		set
		{
			_S.appLimitAdTracking = value;
		}
	}

	public static bool AdidSupressed
	{
		get
		{
			return _S.adidSupressed;
		}
		set
		{
			_S.adidSupressed = value;
		}
	}

	public static string AttributionDataStr
	{
		get
		{
			return _S.attributionDataStr;
		}
		set
		{
			_S.attributionDataStr = value;
		}
	}

	public static List<string> DevIdBlacklist
	{
		get
		{
			return _S.devIdBlacklist;
		}
		set
		{
			_S.devIdBlacklist = value;
		}
	}

	public static List<string> EventNameBlacklist
	{
		get
		{
			return _S.eventNameBlacklist;
		}
		set
		{
			_S.eventNameBlacklist = value;
		}
	}

	public static KochSessionTracking SessionTracking
	{
		get
		{
			return _S.sessionTracking;
		}
		set
		{
			_S.sessionTracking = value;
		}
	}

	public static List<LogEvent> EventLog
	{
		get
		{
			return _S._EventLog;
		}
	}

	public static int eventQueueLength
	{
		get
		{
			return _S.eventQueue.Count;
		}
	}

	public static float eventPostingTime
	{
		get
		{
			return _S._eventPostingTime;
		}
	}

	public static void SetAttributionCallback(AttributionCallback callback)
	{
		attributionCallback = callback;
	}

	public void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if ((bool)_S)
		{
			Log("detected two concurrent integration objects - please place your integration object in a scene which will not be reloaded.");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (_S == null)
		{
			_S = this;
		}
		base.gameObject.name = "_Kochava Analytics";
		Log("Kochava SDK Initialized.\nVersion: 20150929\nProtocol Version: 4", KochLogLevel.debug);
		if (kochavaAppId.Length == 0 && kochavaAppIdIOS.Length == 0 && kochavaAppIdAndroid.Length == 0 && kochavaAppIdKindle.Length == 0 && kochavaAppIdBlackberry.Length == 0 && kochavaAppIdWindowsPhone.Length == 0 && partnerName.Length == 0)
		{
			Log("No Kochava App Id or Partner Name - SDK will terminate");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		loadQueue();
	}

	public void Start()
	{
		if (Application.isPlaying)
		{
			if (_S == null)
			{
				_S = this;
			}
			Init();
		}
	}

	public void OnEnable()
	{
		if (Application.isPlaying && _S == null)
		{
			_S = this;
		}
	}

	private void Init()
	{
		try
		{
			try
			{
				AndroidJNIHelper.debug = true;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
					string text = androidJavaClass2.CallStatic<string>("GetExternalKochavaDeviceIdentifiers_Android", new object[2] { androidJavaObject, AdidSupressed });
					Log("Hardware Integration Diagnostics: " + text);
					hardwareIdentifierData = JsonReader.Deserialize<Dictionary<string, object>>(text);
					Log("Received (" + hardwareIdentifierData.Count + ") parameters from Hardware Integration Library (identifiers): " + text);
				}
			}
			catch (Exception ex)
			{
				Log("Failed GetExternalKochavaDeviceIdentifiers_Android: " + ex, KochLogLevel.warning);
			}
			if (hardwareIdentifierData.ContainsKey("user_agent"))
			{
				userAgent = hardwareIdentifierData["user_agent"].ToString();
				Log("userAgent set to: " + userAgent, KochLogLevel.debug);
			}
			if (userAgent.Contains("kindle") || userAgent.Contains("silk"))
			{
				appPlatform = "kindle";
				if (kochavaAppIdKindle != string.Empty)
				{
					kochavaAppId = kochavaAppIdKindle;
				}
			}
			else
			{
				appPlatform = "android";
				if (kochavaAppIdAndroid != string.Empty)
				{
					kochavaAppId = kochavaAppIdAndroid;
				}
			}
			if (PlayerPrefs.HasKey("kochava_app_id"))
			{
				kochavaAppId = PlayerPrefs.GetString("kochava_app_id");
				Log("Loaded kochava_app_id from persistent storage: " + kochavaAppId, KochLogLevel.debug);
			}
			if (PlayerPrefs.HasKey("kochava_device_id"))
			{
				kochavaDeviceId = PlayerPrefs.GetString("kochava_device_id");
				Log("Loaded kochava_device_id from persistent storage: " + kochavaDeviceId, KochLogLevel.debug);
			}
			else if (incognitoMode)
			{
				kochavaDeviceId = "KA" + Guid.NewGuid().ToString().Replace("-", string.Empty);
				Log("Using autogenerated \"incognito\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
			}
			else
			{
				string text2 = string.Empty;
				if (PlayerPrefs.HasKey("data_orig_kochava_device_id"))
				{
					text2 = PlayerPrefs.GetString("data_orig_kochava_device_id");
				}
				if (text2 != string.Empty)
				{
					kochavaDeviceId = text2;
					Log("Using \"orig\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
				else if (hardwareIdentifierData.ContainsKey("kochava_device_id") && hardwareIdentifierData["kochava_device_id"].ToString().Length > 3)
				{
					kochavaDeviceId = hardwareIdentifierData["kochava_device_id"].ToString();
					Log("Using \"hardware integration\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
				else
				{
					kochavaDeviceId = "KU" + Guid.NewGuid().ToString().Replace("-", string.Empty);
					Log("Using autogenerated kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
			}
			if (!PlayerPrefs.HasKey("data_orig_kochava_app_id") && kochavaAppId != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_kochava_app_id", kochavaAppId);
			}
			if (!PlayerPrefs.HasKey("data_orig_kochava_device_id") && kochavaDeviceId != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_kochava_device_id", kochavaDeviceId);
			}
			if (!PlayerPrefs.HasKey("data_orig_session_tracking"))
			{
				PlayerPrefs.SetString("data_orig_session_tracking", sessionTracking.ToString());
			}
			if (!PlayerPrefs.HasKey("data_orig_currency") && appCurrency != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_currency", appCurrency);
			}
			if (PlayerPrefs.HasKey("currency"))
			{
				appCurrency = PlayerPrefs.GetString("currency");
				Log("Loaded currency from persistent storage: " + appCurrency, KochLogLevel.debug);
			}
			if (PlayerPrefs.HasKey("blacklist"))
			{
				try
				{
					string @string = PlayerPrefs.GetString("blacklist");
					devIdBlacklist = new List<string>();
					string[] array = JsonReader.Deserialize<string[]>(@string);
					for (int num = array.Length - 1; num >= 0; num--)
					{
						devIdBlacklist.Add(array[num]);
					}
					Log("Loaded device_id blacklist from persistent storage: " + @string, KochLogLevel.debug);
				}
				catch (Exception ex2)
				{
					Log("Failed loading device_id blacklist from persistent storage: " + ex2, KochLogLevel.warning);
				}
			}
			if (PlayerPrefs.HasKey("attribution"))
			{
				try
				{
					attributionDataStr = PlayerPrefs.GetString("attribution");
					Log("Loaded attribution data from persistent storage: " + attributionDataStr, KochLogLevel.debug);
				}
				catch (Exception ex3)
				{
					Log("Failed loading attribution data from persistent storage: " + ex3, KochLogLevel.warning);
				}
			}
			if (PlayerPrefs.HasKey("session_tracking"))
			{
				try
				{
					string string2 = PlayerPrefs.GetString("session_tracking");
					sessionTracking = (KochSessionTracking)(int)Enum.Parse(typeof(KochSessionTracking), string2, true);
					Log("Loaded session tracking mode from persistent storage: " + string2, KochLogLevel.debug);
				}
				catch (Exception ex4)
				{
					Log("Failed loading session tracking mode from persistent storage: " + ex4, KochLogLevel.warning);
				}
			}
			if (!PlayerPrefs.HasKey("kvinit_wait"))
			{
				PlayerPrefs.SetString("kvinit_wait", "60");
			}
			if (!PlayerPrefs.HasKey("kvinit_last_sent"))
			{
				PlayerPrefs.SetString("kvinit_last_sent", "0");
			}
			if (!PlayerPrefs.HasKey("kvtracker_wait"))
			{
				PlayerPrefs.SetString("kvtracker_wait", "60");
			}
			if (!PlayerPrefs.HasKey("last_location_time"))
			{
				PlayerPrefs.SetString("last_location_time", "0");
			}
			double num2 = double.Parse(PlayerPrefs.GetString("kvinit_last_sent"));
			double num3 = CurrentTime();
			double num4 = double.Parse(PlayerPrefs.GetString("kvinit_wait"));
			if (num3 - num2 > num4)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("partner_name", partnerName);
				dictionary.Add("package_name", appIdentifier);
				dictionary.Add("platform", appPlatform);
				dictionary.Add("session_tracking", sessionTracking.ToString());
				dictionary.Add("currency", (appCurrency != null && !(appCurrency == string.Empty)) ? appCurrency : "USD");
				dictionary.Add("os_version", SystemInfo.operatingSystem);
				Dictionary<string, object> dictionary2 = dictionary;
				if (requestAttribution && !PlayerPrefs.HasKey("attribution"))
				{
					retrieveAttribution = true;
				}
				Log("retrieve attrib: " + retrieveAttribution);
				if (hardwareIdentifierData.ContainsKey("IDFA"))
				{
					dictionary2.Add("idfa", hardwareIdentifierData["IDFA"]);
				}
				if (hardwareIdentifierData.ContainsKey("IDFV"))
				{
					dictionary2.Add("idfv", hardwareIdentifierData["IDFV"]);
				}
				dictionary = new Dictionary<string, object>();
				dictionary.Add("kochava_app_id", PlayerPrefs.GetString("data_orig_kochava_app_id"));
				dictionary.Add("kochava_device_id", PlayerPrefs.GetString("data_orig_kochava_device_id"));
				dictionary.Add("session_tracking", PlayerPrefs.GetString("data_orig_session_tracking"));
				dictionary.Add("currency", PlayerPrefs.GetString("data_orig_currency"));
				Dictionary<string, object> value = dictionary;
				dictionary = new Dictionary<string, object>();
				dictionary.Add("action", "init");
				dictionary.Add("data", dictionary2);
				dictionary.Add("data_orig", value);
				dictionary.Add("kochava_app_id", kochavaAppId);
				dictionary.Add("kochava_device_id", kochavaDeviceId);
				dictionary.Add("sdk_version", "Unity3D-20150929");
				dictionary.Add("sdk_protocol", "4");
				Dictionary<string, object> value2 = dictionary;
				StartCoroutine(Init_KV(JsonWriter.Serialize(value2)));
			}
			else
			{
				appData = new Dictionary<string, object>
				{
					{ "kochava_app_id", kochavaAppId },
					{ "kochava_device_id", kochavaDeviceId },
					{ "sdk_version", "Unity3D-20150929" },
					{ "sdk_protocol", "4" }
				};
				string[] array2 = JsonReader.Deserialize<string[]>(PlayerPrefs.GetString("eventname_blacklist"));
				List<string> list = new List<string>();
				for (int i = 0; i < array2.Length; i++)
				{
					list.Add(array2[i]);
				}
				eventNameBlacklist = list;
			}
		}
		catch (Exception ex5)
		{
			Log("Overall failure in init: " + ex5, KochLogLevel.warning);
		}
	}

	private IEnumerator Init_KV(string postData)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Log("internet not reachable", KochLogLevel.warning);
			yield return new WaitForSeconds(30f);
			StartCoroutine(Init_KV(postData));
			yield break;
		}
		Log("Initiating kvinit handshake...", KochLogLevel.debug);
		Dictionary<string, string> headers = new Dictionary<string, string>
		{
			{ "Content-Type", "application/xml" },
			{ "User-Agent", userAgent }
		};
		Log(postData, KochLogLevel.debug);
		float wwwLoadTime = Time.time;
		WWW www = new WWW("https://control.kochava.com/track/kvinit", Encoding.UTF8.GetBytes(postData), headers);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Log("Kvinit handshake failed: " + www.error + ", seconds: " + (Time.time - wwwLoadTime) + ")", KochLogLevel.warning);
			yield return new WaitForSeconds(30f);
			StartCoroutine(Init_KV(postData));
			yield break;
		}
		Dictionary<string, object> serverResponse = new Dictionary<string, object>();
		if (www.text != string.Empty)
		{
			try
			{
				serverResponse = JsonReader.Deserialize<Dictionary<string, object>>(www.text);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Log("Failed Deserialize JSON response to kvinit: " + e, KochLogLevel.warning);
			}
		}
		Log(www.text, KochLogLevel.debug);
		if (!serverResponse.ContainsKey("success"))
		{
			Log("Kvinit handshake parsing failed: " + www.text, KochLogLevel.warning);
			yield return new WaitForSeconds(30f);
			StartCoroutine(Init_KV(postData));
			yield break;
		}
		PlayerPrefs.SetString("kvinit_last_sent", CurrentTime().ToString());
		Log("...kvinit handshake complete, processing response flags...", KochLogLevel.debug);
		if (serverResponse.ContainsKey("flags"))
		{
			Dictionary<string, object> serverResponseFlags = (Dictionary<string, object>)serverResponse["flags"];
			if (serverResponseFlags.ContainsKey("kochava_app_id"))
			{
				kochavaAppId = serverResponseFlags["kochava_app_id"].ToString();
				PlayerPrefs.SetString("kochava_app_id", kochavaAppId);
				Log("Saved kochava_app_id to persistent storage: " + kochavaAppId, KochLogLevel.debug);
			}
			if (serverResponseFlags.ContainsKey("kochava_device_id"))
			{
				kochavaDeviceId = serverResponseFlags["kochava_device_id"].ToString();
			}
			if (serverResponseFlags.ContainsKey("resend_initial") && (bool)serverResponseFlags["resend_initial"])
			{
				PlayerPrefs.DeleteKey("watchlistProperties");
				Log("Refiring initial event, as requested by kvinit response flag", KochLogLevel.debug);
			}
			if (serverResponseFlags.ContainsKey("session_tracking"))
			{
				try
				{
					sessionTracking = (KochSessionTracking)(int)Enum.Parse(typeof(KochSessionTracking), serverResponseFlags["session_tracking"].ToString());
					PlayerPrefs.SetString("session_tracking", sessionTracking.ToString());
					Log("Saved session_tracking mode to persistent storage: " + sessionTracking, KochLogLevel.debug);
				}
				catch (Exception ex2)
				{
					Exception e2 = ex2;
					Log("Failed System.Enum.Parse of KochSessionTracking: " + e2, KochLogLevel.warning);
				}
			}
			if (serverResponseFlags.ContainsKey("currency"))
			{
				appCurrency = serverResponseFlags["currency"].ToString();
				if (appCurrency.Equals(string.Empty))
				{
					appCurrency = "USD";
				}
				PlayerPrefs.SetString("currency", appCurrency);
				Log("Saved currency to persistent storage: " + appCurrency, KochLogLevel.debug);
			}
			if (serverResponseFlags.ContainsKey("getattribution_wait"))
			{
				string attributionWaitString = serverResponseFlags["getattribution_wait"].ToString();
				int attributionWait = int.Parse(attributionWaitString);
				if (attributionWait < 1)
				{
					attributionWait = 1;
				}
				if (attributionWait > 30)
				{
					attributionWait = 30;
				}
				PlayerPrefs.SetString("getattribution_wait", attributionWait.ToString());
				Log("Saved getattribution_wait to persistent storage: " + attributionWait, KochLogLevel.debug);
			}
			if (serverResponseFlags.ContainsKey("delay_for_referrer_data"))
			{
				device_id_delay = (int)serverResponseFlags["delay_for_referrer_data"];
				Log("delay_for_referrer_data received: " + device_id_delay, KochLogLevel.debug);
				if (device_id_delay < 0)
				{
					Log("device_id_delay returned was less than 0 (" + device_id_delay + "), setting device_id_delay to 0.");
					device_id_delay = 0;
				}
				else if (device_id_delay > 120)
				{
					Log("device_id_delay returned was greater than 120 (" + device_id_delay + "), setting device_id_delay to 120.");
					device_id_delay = 120;
				}
				else
				{
					Log("setting device_id_delay to: " + device_id_delay);
				}
			}
			if (serverResponseFlags.ContainsKey("kvinit_wait"))
			{
				string kvinitWaitString = serverResponseFlags["kvinit_wait"].ToString();
				int kvinitWait = int.Parse(kvinitWaitString);
				if (kvinitWait < 60)
				{
					kvinitWait = 60;
				}
				if (kvinitWait > 604800)
				{
					kvinitWait = 604800;
				}
				PlayerPrefs.SetString("kvinit_wait", kvinitWait.ToString());
				Log("Saved kvinit_wait to persistent storage: " + kvinitWait, KochLogLevel.debug);
			}
			else
			{
				PlayerPrefs.SetString("kvinit_wait", "60");
				Log("Saved kvinit_wait to persistent storage: 60", KochLogLevel.debug);
			}
			if (serverResponseFlags.ContainsKey("kvtracker_wait"))
			{
				string kvtrackerWaitString = serverResponseFlags["kvtracker_wait"].ToString();
				int kvtrackerWait = int.Parse(kvtrackerWaitString);
				if (kvtrackerWait < 60)
				{
					kvtrackerWait = 60;
				}
				if (kvtrackerWait > 604800)
				{
					kvtrackerWait = 604800;
				}
				PlayerPrefs.SetString("kvtracker_wait", kvtrackerWait.ToString());
				Log("Saved kvtracker_wait to persistent storage: " + kvtrackerWait, KochLogLevel.debug);
				KVTRACKER_WAIT = kvtrackerWait;
			}
			else
			{
				PlayerPrefs.SetString("kvtracker_wait", "60");
				Log("Saved kvtracker_wait to persistent storage: 60", KochLogLevel.debug);
				KVTRACKER_WAIT = 60;
			}
			if (serverResponseFlags.ContainsKey("location_accuracy"))
			{
				string accuracyString = serverResponseFlags["location_accuracy"].ToString();
				int accuracy = int.Parse(accuracyString);
				if (accuracy < 10)
				{
					accuracy = 10;
				}
				if (accuracy > 5000)
				{
					accuracy = 5000;
				}
				locationAccuracy = accuracy;
			}
			if (serverResponseFlags.ContainsKey("location_timeout"))
			{
				string timeoutString = serverResponseFlags["location_timeout"].ToString();
				int timeout = int.Parse(timeoutString);
				if (timeout < 3)
				{
					timeout = 3;
				}
				if (timeout > 60)
				{
					timeout = 60;
				}
				locationTimeout = timeout;
			}
			if (serverResponseFlags.ContainsKey("location_staleness"))
			{
				string stalenessString = serverResponseFlags["location_staleness"].ToString();
				int staleness = int.Parse(stalenessString);
				if (staleness < 1)
				{
					staleness = 1;
				}
				if (staleness > 10080)
				{
					staleness = 10080;
				}
				locationStaleness = staleness;
			}
			if (serverResponseFlags.ContainsKey("iad_attribution_attempts"))
			{
				string attemptsString = serverResponseFlags["iad_attribution_attempts"].ToString();
				int attempts = int.Parse(attemptsString);
				if (attempts < 1)
				{
					attempts = 1;
				}
				if (attempts > 10)
				{
					attempts = 10;
				}
				iAdAttributionAttempts = attempts;
			}
			if (serverResponseFlags.ContainsKey("iad_attribution_wait"))
			{
				string waitString = serverResponseFlags["iad_attribution_wait"].ToString();
				int wait = int.Parse(waitString);
				if (wait < 1)
				{
					wait = 1;
				}
				if (wait > 120)
				{
					wait = 120;
				}
				iAdAttributionWait = wait;
			}
			if (serverResponseFlags.ContainsKey("iad_retry_wait"))
			{
				string retryWaitString = serverResponseFlags["iad_retry_wait"].ToString();
				int retryWait = int.Parse(retryWaitString);
				if (retryWait < 1)
				{
					retryWait = 1;
				}
				if (retryWait > 60)
				{
					retryWait = 60;
				}
				iAdRetryWait = retryWait;
			}
			if (serverResponseFlags.ContainsKey("send_id_updates") && (bool)serverResponseFlags["send_id_updates"])
			{
				send_id_updates = true;
			}
		}
		if (serverResponse.ContainsKey("blacklist"))
		{
			devIdBlacklist = new List<string>();
			if (serverResponse["blacklist"].GetType().GetElementType() == typeof(string))
			{
				try
				{
					string[] devIdBlacklistArr = (string[])serverResponse["blacklist"];
					for (int k = devIdBlacklistArr.Length - 1; k >= 0; k--)
					{
						devIdBlacklist.Add(devIdBlacklistArr[k]);
						if (devIdBlacklistArr[k].ToLower().Equals("adid"))
						{
							adidBlacklisted = true;
						}
					}
				}
				catch (Exception ex3)
				{
					Exception e6 = ex3;
					Log("Failed parsing device_identifier blacklist received from server: " + e6, KochLogLevel.warning);
				}
			}
			try
			{
				string devIdBlacklistStr = JsonWriter.Serialize(devIdBlacklist);
				PlayerPrefs.SetString("blacklist", devIdBlacklistStr);
				Log("Saved device_identifier blacklist (" + devIdBlacklist.Count + " elements) to persistent storage: " + devIdBlacklistStr, KochLogLevel.debug);
			}
			catch (Exception e5)
			{
				Log("Failed saving device_identifier blacklist to persistent storage: " + e5, KochLogLevel.warning);
			}
		}
		if (serverResponse.ContainsKey("whitelist") && serverResponse["whitelist"].GetType().GetElementType() == typeof(string))
		{
			string result2 = "{";
			try
			{
				string[] devIdWhitelistArr = (string[])serverResponse["whitelist"];
				for (int j = devIdWhitelistArr.Length - 1; j >= 0; j--)
				{
					if (devIdWhitelistArr[j] == "location")
					{
						doReportLocation = true;
					}
					result2 = ((j == 0) ? (result2 + devIdWhitelistArr[j]) : (result2 + devIdWhitelistArr[j] + ","));
				}
			}
			catch (Exception ex4)
			{
				Exception e4 = ex4;
				Log("Failed parsing device_identifier whitelist received from server: " + e4, KochLogLevel.warning);
			}
			result2 += "}";
			Log("whitelist string: " + result2);
			whitelist = result2;
		}
		if (serverResponse.ContainsKey("eventname_blacklist"))
		{
			if (serverResponse["eventname_blacklist"].GetType().GetElementType() == typeof(string))
			{
				try
				{
					string[] eventNameBlacklistArr = (string[])serverResponse["eventname_blacklist"];
					for (int i = eventNameBlacklistArr.Length - 1; i >= 0; i--)
					{
						eventNameBlacklist.Add(eventNameBlacklistArr[i]);
					}
				}
				catch (Exception ex5)
				{
					Exception e3 = ex5;
					Log("Failed parsing eventname_blacklist received from server: " + e3, KochLogLevel.warning);
				}
			}
			PlayerPrefs.SetString("eventname_blacklist", JsonWriter.Serialize(eventNameBlacklist));
		}
		appData = new Dictionary<string, object>
		{
			{ "kochava_app_id", kochavaAppId },
			{ "kochava_device_id", kochavaDeviceId },
			{ "sdk_version", "Unity3D-20150929" },
			{ "sdk_protocol", "4" }
		};
		PlayerPrefs.SetString("kochava_device_id", kochavaDeviceId);
		Log("Saved kochava_device_id to persistent storage: " + kochavaDeviceId, KochLogLevel.debug);
		if (sessionTracking == KochSessionTracking.full || sessionTracking == KochSessionTracking.basic)
		{
			_S._fireEvent("session", new Dictionary<string, object> { { "state", "launch" } });
		}
		AndroidJNIHelper.debug = true;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidActivity2 = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidContext2 = androidActivity2.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			AndroidJavaClass androidHelperClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
			androidHelperClass2.CallStatic<string>("GetExternalKochavaInfo_Android", new object[5]
			{
				androidContext2,
				whitelist,
				device_id_delay,
				PlayerPrefs.GetString("blacklist"),
				AdidSupressed
			});
		}
		if (!doReportLocation)
		{
			yield break;
		}
		double currtime = CurrentTime();
		double lastLocationTime = double.Parse(PlayerPrefs.GetString("last_location_time"));
		if (!(currtime - lastLocationTime > (double)(locationStaleness * 60)))
		{
			yield break;
		}
		using (AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidContext = androidActivity.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			AndroidJavaClass androidHelperClass = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
			androidHelperClass.CallStatic<string>("GetExternalLocationReport_Android", new object[3] { locationAccuracy, locationTimeout, locationStaleness });
			Log("Calling android location gather");
		}
	}

	private void DeviceInformationCallback(string deviceInfo)
	{
		try
		{
			hardwareIntegrationData = JsonReader.Deserialize<Dictionary<string, object>>(deviceInfo);
			Log("Received (" + hardwareIntegrationData.Count + ") parameters from Hardware Integration Library (device info): " + deviceInfo);
		}
		catch (Exception ex)
		{
			Log("Failed Deserialize hardwareIntegrationData: " + ex, KochLogLevel.warning);
		}
		if (!PlayerPrefs.HasKey("watchlistProperties"))
		{
			initInitial();
		}
		else
		{
			ScanWatchlistChanges();
		}
	}

	public static void InitInitial()
	{
		_S.initInitial();
	}

	private void initInitial()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			dictionary.Add("device", SystemInfo.deviceModel);
			if (hardwareIntegrationData.ContainsKey("package_name"))
			{
				dictionary.Add("package_name", hardwareIntegrationData["package_name"]);
			}
			else
			{
				dictionary.Add("package_name", appIdentifier);
			}
			if (hardwareIntegrationData.ContainsKey("app_version"))
			{
				dictionary.Add("app_version", hardwareIntegrationData["app_version"]);
			}
			else
			{
				dictionary.Add("app_version", appVersion);
			}
			if (hardwareIntegrationData.ContainsKey("app_short_string"))
			{
				dictionary.Add("app_short_string", hardwareIntegrationData["app_short_string"]);
			}
			else
			{
				dictionary.Add("app_short_string", appVersion);
			}
			dictionary.Add("currency", (!(appCurrency == string.Empty)) ? appCurrency : "USD");
			if (!devIdBlacklist.Contains("screen_size"))
			{
				dictionary.Add("disp_h", Screen.height);
				dictionary.Add("disp_w", Screen.width);
			}
			dictionary.Add("os_version", SystemInfo.operatingSystem);
			dictionary.Add("app_limit_tracking", appLimitAdTracking);
			if (!devIdBlacklist.Contains("hardware"))
			{
				dictionary.Add("device_processor", SystemInfo.processorType);
				dictionary.Add("device_cores", SystemInfo.processorCount);
				dictionary.Add("device_memory", SystemInfo.systemMemorySize);
				dictionary.Add("graphics_memory_size", SystemInfo.graphicsMemorySize);
				dictionary.Add("graphics_device_name", SystemInfo.graphicsDeviceName);
				dictionary.Add("graphics_device_vendor", SystemInfo.graphicsDeviceVendor);
				dictionary.Add("graphics_device_id", SystemInfo.graphicsDeviceID);
				dictionary.Add("graphics_device_vendor_id", SystemInfo.graphicsDeviceVendorID);
				dictionary.Add("graphics_device_version", SystemInfo.graphicsDeviceVersion);
				dictionary.Add("graphics_shader_level", SystemInfo.graphicsShaderLevel);
			}
			if (!devIdBlacklist.Contains("is_genuine") && Application.genuineCheckAvailable)
			{
				dictionary.Add("is_genuine", (!Application.genuine) ? "0" : "1");
			}
			if (!devIdBlacklist.Contains("idfa") && hardwareIntegrationData.ContainsKey("IDFA"))
			{
				dictionary.Add("idfa", hardwareIntegrationData["IDFA"]);
			}
			if (!devIdBlacklist.Contains("idfv") && hardwareIntegrationData.ContainsKey("IDFV"))
			{
				dictionary.Add("idfv", hardwareIntegrationData["IDFV"]);
			}
			if (!devIdBlacklist.Contains("udid") && hardwareIntegrationData.ContainsKey("UDID"))
			{
				dictionary.Add("udid", hardwareIntegrationData["UDID"]);
			}
			if (!devIdBlacklist.Contains("iad_attribution") && hardwareIntegrationData.ContainsKey("iad_attribution"))
			{
				dictionary.Add("iad_attribution", hardwareIntegrationData["iad_attribution"]);
			}
			if (!devIdBlacklist.Contains("app_purchase_date") && hardwareIntegrationData.ContainsKey("app_purchase_date"))
			{
				dictionary.Add("app_purchase_date", hardwareIntegrationData["app_purchase_date"]);
			}
			if (!devIdBlacklist.Contains("iad_impression_date") && hardwareIntegrationData.ContainsKey("iad_impression_date"))
			{
				dictionary.Add("iad_impression_date", hardwareIntegrationData["iad_impression_date"]);
			}
			if (!devIdBlacklist.Contains("iad_attribution_details") && hardwareIntegrationData.ContainsKey("iad_attribution_details"))
			{
				dictionary.Add("iad_attribution_details", hardwareIntegrationData["iad_attribution_details"]);
			}
			if (!devIdBlacklist.Contains("guid"))
			{
				string text = Guid.NewGuid().ToString();
				if (text != string.Empty)
				{
					dictionary.Add("guid", text);
				}
			}
			if (!devIdBlacklist.Contains("android_id") && hardwareIntegrationData.ContainsKey("android_id"))
			{
				dictionary.Add("android_id", hardwareIntegrationData["android_id"]);
			}
			if (!devIdBlacklist.Contains("adid") && hardwareIntegrationData.ContainsKey("adid"))
			{
				dictionary.Add("adid", hardwareIntegrationData["adid"]);
			}
			if (!devIdBlacklist.Contains("fb_attribution_id") && hardwareIntegrationData.ContainsKey("fb_attribution_id"))
			{
				dictionary.Add("fb_attribution_id", hardwareIntegrationData["fb_attribution_id"]);
			}
			if (hardwareIntegrationData.ContainsKey("device_limit_tracking"))
			{
				dictionary.Add("device_limit_tracking", hardwareIntegrationData["device_limit_tracking"]);
			}
			if (!devIdBlacklist.Contains("affinity_group") && hardwareIntegrationData.ContainsKey("affinity_group"))
			{
				dictionary.Add("affinity_group", hardwareIntegrationData["affinity_group"]);
			}
			if (!devIdBlacklist.Contains("bssid") && hardwareIntegrationData.ContainsKey("bssid"))
			{
				dictionary.Add("bssid", hardwareIntegrationData["bssid"]);
			}
			if (!devIdBlacklist.Contains("carrier_name") && hardwareIntegrationData.ContainsKey("carrier_name"))
			{
				dictionary.Add("carrier_name", hardwareIntegrationData["carrier_name"]);
			}
			if (!devIdBlacklist.Contains("volume") && hardwareIntegrationData.ContainsKey("volume"))
			{
				dictionary.Add("volume", hardwareIntegrationData["volume"]);
			}
			if (hardwareIntegrationData.ContainsKey("language"))
			{
				dictionary.Add("language", hardwareIntegrationData["language"]);
			}
			if (hardwareIntegrationData.ContainsKey("ids"))
			{
				dictionary.Add("ids", hardwareIntegrationData["ids"]);
			}
			if (hardwareIntegrationData.ContainsKey("conversion_type"))
			{
				dictionary.Add("conversion_type", hardwareIntegrationData["conversion_type"]);
			}
			if (hardwareIntegrationData.ContainsKey("conversion_data"))
			{
				dictionary.Add("conversion_data", hardwareIntegrationData["conversion_data"]);
			}
			dictionary.Add("usertime", (uint)CurrentTime());
			if ((uint)Time.time != 0)
			{
				dictionary.Add("uptime", (uint)Time.time);
			}
			float num = UptimeDelta();
			if (num >= 1f)
			{
				dictionary.Add("updelta", (uint)num);
			}
		}
		catch (Exception ex)
		{
			Log("Error preparing initial event: " + ex, KochLogLevel.error);
		}
		finally
		{
			_fireEvent("initial", dictionary);
			if (retrieveAttribution)
			{
				int num2 = 7;
				if (PlayerPrefs.HasKey("getattribution_wait"))
				{
					string @string = PlayerPrefs.GetString("getattribution_wait");
					num2 = int.Parse(@string);
				}
				Log("Will check for attribution in: " + num2);
				StartCoroutine("KochavaAttributionTimerFired", num2);
			}
		}
		try
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			if (hardwareIntegrationData.ContainsKey("device_limit_tracking"))
			{
				dictionary2.Add("device_limit_tracking", hardwareIntegrationData["device_limit_tracking"].ToString());
			}
			dictionary2.Add("os_version", SystemInfo.operatingSystem);
			dictionary2.Add("app_limit_tracking", appLimitAdTracking);
			if (hardwareIntegrationData.ContainsKey("language"))
			{
				dictionary2.Add("language", hardwareIntegrationData["language"].ToString());
			}
			if (hardwareIntegrationData.ContainsKey("app_version"))
			{
				dictionary2.Add("app_version", hardwareIntegrationData["app_version"].ToString());
			}
			else
			{
				dictionary2.Add("app_version", appVersion);
			}
			if (hardwareIntegrationData.ContainsKey("app_short_string"))
			{
				dictionary2.Add("app_short_string", hardwareIntegrationData["app_short_string"].ToString());
			}
			else
			{
				dictionary2.Add("app_short_string", appVersion);
			}
			if (!devIdBlacklist.Contains("idfa") && hardwareIntegrationData.ContainsKey("IDFA"))
			{
				dictionary2.Add("idfa", hardwareIntegrationData["IDFA"].ToString());
			}
			if (!devIdBlacklist.Contains("adid") && hardwareIntegrationData.ContainsKey("adid"))
			{
				dictionary2.Add("adid", hardwareIntegrationData["adid"]);
			}
			string text2 = JsonWriter.Serialize(dictionary2);
			PlayerPrefs.SetString("watchlistProperties", text2);
			Log("watchlistString: " + text2);
		}
		catch (Exception ex2)
		{
			Log("Error setting watchlist: " + ex2, KochLogLevel.error);
		}
	}

	public void ScanWatchlistChanges()
	{
		try
		{
			if (!PlayerPrefs.HasKey("watchlistProperties"))
			{
				return;
			}
			string @string = PlayerPrefs.GetString("watchlistProperties");
			Log("retrieve watchlist: " + @string);
			Dictionary<string, object> dictionary = null;
			dictionary = JsonReader.Deserialize<Dictionary<string, object>>(@string);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			if (dictionary.ContainsKey("app_version"))
			{
				if (hardwareIntegrationData.ContainsKey("app_version"))
				{
					if (dictionary["app_version"].ToString() != hardwareIntegrationData["app_version"].ToString())
					{
						dictionary2.Add("app_version", hardwareIntegrationData["app_version"].ToString());
						dictionary["app_version"] = hardwareIntegrationData["app_version"].ToString();
					}
				}
				else if (dictionary["app_version"].ToString() != appVersion)
				{
					dictionary2.Add("app_version", appVersion);
					dictionary["app_version"] = appVersion;
				}
			}
			if (dictionary.ContainsKey("app_short_string"))
			{
				if (hardwareIntegrationData.ContainsKey("app_short_string"))
				{
					if (dictionary["app_short_string"].ToString() != hardwareIntegrationData["app_short_string"].ToString())
					{
						dictionary2.Add("app_short_string", hardwareIntegrationData["app_short_string"].ToString());
						dictionary["app_short_string"] = hardwareIntegrationData["app_short_string"].ToString();
					}
				}
				else if (dictionary["app_short_string"].ToString() != appVersion)
				{
					dictionary2.Add("app_short_string", appVersion);
					dictionary["app_short_string"] = appVersion;
				}
			}
			if (dictionary.ContainsKey("os_version") && dictionary["os_version"].ToString() != SystemInfo.operatingSystem)
			{
				dictionary2.Add("os_version", SystemInfo.operatingSystem);
				dictionary["os_version"] = SystemInfo.operatingSystem;
			}
			if (dictionary.ContainsKey("language") && hardwareIntegrationData.ContainsKey("language") && dictionary["language"].ToString() != hardwareIntegrationData["language"].ToString())
			{
				dictionary2.Add("language", hardwareIntegrationData["language"].ToString());
				dictionary["language"] = hardwareIntegrationData["language"].ToString();
			}
			if (dictionary.ContainsKey("device_limit_tracking") && hardwareIntegrationData.ContainsKey("device_limit_tracking") && dictionary["device_limit_tracking"].ToString() != hardwareIntegrationData["device_limit_tracking"].ToString())
			{
				dictionary2.Add("device_limit_tracking", hardwareIntegrationData["device_limit_tracking"].ToString());
				dictionary["device_limit_tracking"] = hardwareIntegrationData["device_limit_tracking"].ToString();
			}
			if (dictionary.ContainsKey("app_limit_tracking") && bool.Parse(dictionary["app_limit_tracking"].ToString()) != appLimitAdTracking)
			{
				dictionary2.Add("app_limit_tracking", appLimitAdTracking);
				dictionary["app_limit_tracking"] = appLimitAdTracking;
			}
			if (send_id_updates)
			{
				if (!devIdBlacklist.Contains("idfa") && dictionary.ContainsKey("idfa") && hardwareIntegrationData.ContainsKey("IDFA") && dictionary["idfa"].ToString() != hardwareIntegrationData["IDFA"].ToString())
				{
					dictionary2.Add("idfa", hardwareIntegrationData["IDFA"].ToString());
					dictionary["idfa"] = hardwareIntegrationData["IDFA"].ToString();
				}
				if (!devIdBlacklist.Contains("adid") && dictionary.ContainsKey("adid") && hardwareIntegrationData.ContainsKey("adid") && dictionary["adid"].ToString() != hardwareIntegrationData["adid"].ToString())
				{
					dictionary2.Add("adid", hardwareIntegrationData["adid"].ToString());
					dictionary["adid"] = hardwareIntegrationData["adid"].ToString();
				}
			}
			if (dictionary2.Count > 0)
			{
				string text = JsonWriter.Serialize(dictionary);
				string text2 = JsonWriter.Serialize(dictionary2);
				Log("final watchlist: " + text);
				Log("changeData: " + text2);
				PlayerPrefs.SetString("watchlistProperties", text);
				_S._fireEvent("update", dictionary2);
			}
			else
			{
				Log("No watchdata changed");
			}
		}
		catch (Exception ex)
		{
			Log("Error scanning watchlist: " + ex, KochLogLevel.error);
		}
	}

	public void Update()
	{
		if (Application.isPlaying && processQueueKickstartTime != 0f && Time.time > processQueueKickstartTime)
		{
			processQueueKickstartTime = 0f;
			StartCoroutine("processQueue");
		}
	}

	public static string GetKochavaDeviceId()
	{
		if (PlayerPrefs.HasKey("kochava_device_id"))
		{
			return PlayerPrefs.GetString("kochava_device_id");
		}
		return string.Empty;
	}

	public static void SetLimitAdTracking(bool appLimitTracking)
	{
		AppLimitAdTracking = appLimitTracking;
		_S.ScanWatchlistChanges();
	}

	public static void FireEvent(Dictionary<string, object> properties)
	{
		_S._fireEvent("event", properties);
	}

	public static void FireEvent(Hashtable propHash)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (DictionaryEntry item in propHash)
		{
			dictionary.Add((string)item.Key, item.Value);
		}
		_S._fireEvent("event", dictionary);
	}

	public static void FireEvent(string eventName, string eventData)
	{
		if (!EventNameBlacklist.Contains(eventName))
		{
			_S._fireEvent("event", new Dictionary<string, object>
			{
				{ "event_name", eventName },
				{
					"event_data",
					(eventData != null) ? eventData : string.Empty
				}
			});
		}
	}

	public static void FireSpatialEvent(string eventName, float x, float y)
	{
		FireSpatialEvent(eventName, x, y, 0f, string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, string eventData)
	{
		FireSpatialEvent(eventName, x, y, 0f, (eventData != null) ? eventData : string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, float z)
	{
		FireSpatialEvent(eventName, x, y, z, string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, float z, string eventData)
	{
		_S._fireEvent("spatial", new Dictionary<string, object>
		{
			{ "event_name", eventName },
			{ "event_data", eventData },
			{ "x", x },
			{ "y", y },
			{ "z", z }
		});
	}

	public static void IdentityLink(string key, string val)
	{
		_S._fireEvent("identityLink", new Dictionary<string, object> { { key, val } });
	}

	public static void IdentityLink(Dictionary<string, object> identities)
	{
		_S._fireEvent("identityLink", identities);
	}

	public static void DeeplinkEvent(string uri, string sourceApp)
	{
		_S._fireEvent("deeplink", new Dictionary<string, object>
		{
			{ "uri", uri },
			{ "source_app", sourceApp }
		});
	}

	private void _fireEvent(string eventAction, Dictionary<string, object> eventData)
	{
		if (eventData.ContainsKey("event_name") && (eventData["event_name"] == null || eventData["event_name"].Equals(string.Empty)))
		{
			Log("Cannot create event with null/empty event name.");
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!eventData.ContainsKey("usertime"))
		{
			eventData.Add("usertime", (uint)CurrentTime());
		}
		if (!eventData.ContainsKey("uptime") && (uint)Time.time != 0)
		{
			eventData.Add("uptime", (uint)Time.time);
		}
		float num = UptimeDelta();
		if (!eventData.ContainsKey("updelta") && num >= 1f)
		{
			eventData.Add("updelta", (uint)num);
		}
		dictionary.Add("action", eventAction);
		dictionary.Add("data", eventData);
		if (eventPostingTime != 0f)
		{
			dictionary.Add("last_post_time", eventPostingTime);
		}
		if (debugMode)
		{
			dictionary.Add("debug", true);
		}
		if (debugServer)
		{
			dictionary.Add("debugServer", true);
		}
		bool isInitial = false;
		if (eventAction == "initial")
		{
			isInitial = true;
		}
		postEvent(dictionary, isInitial);
	}

	private void postEvent(Dictionary<string, object> data, bool isInitial)
	{
		QueuedEvent queuedEvent = new QueuedEvent();
		queuedEvent.eventTime = Time.time;
		queuedEvent.eventData = data;
		eventQueue.Enqueue(queuedEvent);
		if (isInitial)
		{
			StartCoroutine("processQueue");
		}
		else if (eventQueue.Count >= 75)
		{
			StartCoroutine("processQueue");
		}
		else
		{
			processQueueKickstartTime = Time.time + (float)KVTRACKER_WAIT;
		}
	}

	private void LocationReportCallback(string locationInfo)
	{
		Log("location info: " + locationInfo);
		PlayerPrefs.SetString("last_location_time", CurrentTime().ToString());
		try
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary = JsonReader.Deserialize<Dictionary<string, object>>(locationInfo);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("location", dictionary);
			_S._fireEvent("update", dictionary2);
		}
		catch (Exception ex)
		{
			Log("Failed Deserialize hardwareIntegrationData: " + ex, KochLogLevel.warning);
		}
	}

	private IEnumerator processQueue()
	{
		if (queueIsProcessing)
		{
			yield break;
		}
		queueIsProcessing = true;
		while (appData == null)
		{
			yield return new WaitForSeconds(15f);
			if (appData == null)
			{
				Log("Event posting delayed (AppData null, kvinit handshake incomplete or Unity reloaded assemblies)", KochLogLevel.debug);
			}
		}
		List<object> requestArray = new List<object>();
		List<object> saveArray = new List<object>();
		float postTime = Time.time;
		while (eventQueue.Count > 0)
		{
			QueuedEvent queuedEvent = eventQueue.Peek();
			saveArray.Add(queuedEvent);
			try
			{
				Dictionary<string, object> eventData = queuedEvent.eventData;
				foreach (KeyValuePair<string, object> row in appData)
				{
					if (!eventData.ContainsKey(row.Key))
					{
						eventData.Add(row.Key, row.Value);
					}
				}
				requestArray.Add(eventData);
				eventQueue.Dequeue();
			}
			catch (Exception e)
			{
				Log("Event posting failure: " + e, KochLogLevel.error);
			}
		}
		if (requestArray.Count > 0)
		{
			string postData = JsonWriter.Serialize(requestArray);
			Log("Posting event: " + postData.Replace("{", "{\n").Replace(",", ",\n"), KochLogLevel.debug);
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				{ "Content-Type", "application/json" },
				{ "User-Agent", userAgent }
			};
			Log(postData, KochLogLevel.debug);
			WWW www = new WWW("https://control.kochava.com/track/kvTracker?v4", Encoding.UTF8.GetBytes(postData), headers);
			yield return www;
			try
			{
				Dictionary<string, object> serverResponse = new Dictionary<string, object>();
				if (www.error == null && www.text != string.Empty)
				{
					Log("Server Response Received: " + WWW.UnEscapeURL(www.text), KochLogLevel.debug);
					serverResponse = JsonReader.Deserialize<Dictionary<string, object>>(www.text);
				}
				bool retry = true;
				bool success = serverResponse.ContainsKey("success");
				if (!string.IsNullOrEmpty(www.error) || !success)
				{
					_eventPostingTime = -1f;
					if (!string.IsNullOrEmpty(www.error))
					{
						Log("Event Posting Failed: " + www.error, KochLogLevel.error);
					}
					else
					{
						Log("Event Posting Did Not Succeed: " + ((!(www.text == string.Empty)) ? www.text : "(Blank response from server)"), KochLogLevel.error);
						if (serverResponse.ContainsKey("error") || www.text == string.Empty)
						{
							retry = false;
						}
					}
					RequeuePostEvents(saveArray);
					if (retry)
					{
						processQueueKickstartTime = Time.time + 30f;
						queueIsProcessing = false;
						yield break;
					}
					Log("Event posting failure, event dequeued: " + serverResponse["error"], KochLogLevel.warning);
				}
				else
				{
					_eventPostingTime = Time.time - postTime;
					Log("Event Posted (" + _eventPostingTime + " seconds to upload)");
					if (serverResponse.ContainsKey("cta") && serverResponse["CTA"].ToString() == "1")
					{
						Application.OpenURL(serverResponse["URL"].ToString());
					}
				}
			}
			catch (Exception e2)
			{
				Log("Event posting response processing failure: " + e2, KochLogLevel.error);
			}
		}
		queueIsProcessing = false;
	}

	public void RequeuePostEvents(List<object> saveArray)
	{
		for (int i = 0; i < saveArray.Count; i++)
		{
			QueuedEvent item = (QueuedEvent)saveArray[i];
			eventQueue.Enqueue(item);
		}
	}

	public void OnApplicationPause(bool didPause)
	{
		if (sessionTracking == KochSessionTracking.full && appData != null)
		{
			_S._fireEvent("session", new Dictionary<string, object> { 
			{
				"state",
				(!didPause) ? "resume" : "pause"
			} });
		}
		if (didPause)
		{
			saveQueue();
			return;
		}
		Log("received - app resume");
		AndroidJNIHelper.debug = true;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
			androidJavaClass2.CallStatic<string>("GetExternalKochavaInfo_Android", new object[5]
			{
				androidJavaObject,
				whitelist,
				device_id_delay,
				PlayerPrefs.GetString("blacklist"),
				AdidSupressed
			});
		}
		if (doReportLocation)
		{
			double num = CurrentTime();
			double num2 = double.Parse(PlayerPrefs.GetString("last_location_time"));
			if (!(num - num2 > (double)(locationStaleness * 60)))
			{
			}
		}
	}

	public void OnApplicationQuit()
	{
		if (sessionTracking == KochSessionTracking.full || sessionTracking == KochSessionTracking.basic || sessionTracking == KochSessionTracking.minimal)
		{
			_S._fireEvent("session", new Dictionary<string, object> { { "state", "quit" } });
		}
		saveQueue();
	}

	private void saveQueue()
	{
		if (eventQueue.Count > 0)
		{
			try
			{
				string text = JsonWriter.Serialize(eventQueue);
				PlayerPrefs.SetString("kochava_queue_storage", text);
				Log("Event Queue saved: " + text, KochLogLevel.debug);
			}
			catch (Exception ex)
			{
				Log("Failure saving event queue: " + ex, KochLogLevel.error);
			}
		}
	}

	private void loadQueue()
	{
		try
		{
			if (!PlayerPrefs.HasKey("kochava_queue_storage"))
			{
				return;
			}
			string @string = PlayerPrefs.GetString("kochava_queue_storage");
			int num = 0;
			QueuedEvent[] array = JsonReader.Deserialize<QueuedEvent[]>(@string);
			QueuedEvent[] array2 = array;
			foreach (QueuedEvent item in array2)
			{
				if (!eventQueue.Contains(item))
				{
					eventQueue.Enqueue(item);
					num++;
				}
			}
			Log("Loaded (" + num + ") events from persistent storage", KochLogLevel.debug);
			PlayerPrefs.DeleteKey("kochava_queue_storage");
			StartCoroutine("processQueue");
		}
		catch (Exception ex)
		{
			Log("Failure loading event queue: " + ex, KochLogLevel.debug);
		}
	}

	public static void ClearQueue()
	{
		_S.StartCoroutine("clearQueue");
	}

	private IEnumerator clearQueue()
	{
		try
		{
			Log("Clearing (" + eventQueueLength + ") events from upload queue...");
			_S.StopCoroutine("processQueue");
		}
		catch (Exception ex)
		{
			Exception e2 = ex;
			Log("Failure clearing event queue: " + e2, KochLogLevel.error);
		}
		yield return null;
		try
		{
			_S.queueIsProcessing = false;
			_S.eventQueue = new Queue<QueuedEvent>();
		}
		catch (Exception e)
		{
			Log("Failure clearing event queue: " + e, KochLogLevel.error);
		}
	}

	public void GetAd(int webView, int height, int width)
	{
		Log("Adserver Implementation Pending");
	}

	private static string[] Chop(string value, int length)
	{
		int num = value.Length;
		int num2 = (num + length - 1) / length;
		string[] array = new string[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = value.Substring(i * length, Mathf.Min(length, num));
			num -= length;
		}
		return array;
	}

	private void Log(string msg)
	{
		Log(msg, KochLogLevel.warning);
	}

	private void Log(string msg, KochLogLevel level)
	{
		if (msg.Length > 1000)
		{
			string[] array = Chop(msg, 1000);
			if (level == KochLogLevel.error || debugMode)
			{
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
			}
		}
		else if (level != 0 && !debugMode)
		{
		}
		if (debugMode || level == KochLogLevel.error || level == KochLogLevel.warning)
		{
			_EventLog.Add(new LogEvent(msg, level));
		}
		if (_EventLog.Count > 50)
		{
			_EventLog.RemoveAt(0);
		}
	}

	public static void ClearLog()
	{
		_S._EventLog.Clear();
	}

	protected internal static double CurrentTime()
	{
		return (DateTime.UtcNow - Jan1st1970).TotalSeconds;
	}

	protected internal static float UptimeDelta()
	{
		uptimeDelta = Time.time - uptimeDeltaUpdate;
		uptimeDeltaUpdate = Time.time;
		return uptimeDelta;
	}

	private string CalculateMD5Hash(string input)
	{
		//Discarded unreachable code: IL_005e, IL_0084
		try
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Log("Failure calculating MD5 hash: " + ex, KochLogLevel.error);
			return string.Empty;
		}
	}

	private string CalculateSHA1Hash(string input)
	{
		//Discarded unreachable code: IL_0056, IL_007c
		try
		{
			byte[] array = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(input));
			string text = string.Empty;
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				text += b.ToString("x2");
			}
			return text;
		}
		catch (Exception ex)
		{
			Log("Failure calculating SHA1 hash: " + ex, KochLogLevel.error);
			return string.Empty;
		}
	}

	public IEnumerator KochavaAttributionTimerFired(int delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		Log("attribution timer wait completed");
		Dictionary<string, object> queryData = new Dictionary<string, object>
		{
			{ "action", "get_attribution" },
			{ "kochava_app_id", kochavaAppId },
			{ "kochava_device_id", kochavaDeviceId },
			{ "sdk_version", "Unity3D-20150929" },
			{ "sdk_protocol", "4" }
		};
		string queryString = JsonWriter.Serialize(queryData);
		Dictionary<string, string> headers = new Dictionary<string, string>
		{
			{ "Content-Type", "application/xml" },
			{ "User-Agent", userAgent }
		};
		Log(queryString, KochLogLevel.debug);
		float wwwLoadTime = Time.time;
		WWW www = new WWW("https://control.kochava.com/track/kvquery", Encoding.UTF8.GetBytes(queryString), headers);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Log("kvquery (attribution) handshake failed: " + www.error + ", seconds: " + (Time.time - wwwLoadTime) + ")", KochLogLevel.warning);
			StartCoroutine("KochavaAttributionTimerFired", 60);
			yield break;
		}
		Dictionary<string, object> serverResponse = new Dictionary<string, object>();
		Log("server response: " + www.text);
		if (www.text != string.Empty)
		{
			try
			{
				serverResponse = JsonReader.Deserialize<Dictionary<string, object>>(www.text);
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Log("Failed Deserialize JSON response to kvquery (attribution): " + e2, KochLogLevel.warning);
			}
		}
		Log(www.text, KochLogLevel.debug);
		if (!serverResponse.ContainsKey("success"))
		{
			Log("kvquery (attribution) handshake parsing failed: " + www.text, KochLogLevel.warning);
			StartCoroutine("KochavaAttributionTimerFired", 60);
		}
		else if (int.Parse(serverResponse["success"].ToString()) == 0)
		{
			Log("kvquery (attribution) did not return success = true " + www.text, KochLogLevel.warning);
			StartCoroutine("KochavaAttributionTimerFired", 60);
		}
		else
		{
			if (!serverResponse.ContainsKey("data"))
			{
				yield break;
			}
			Dictionary<string, object> attributionDataChunk = new Dictionary<string, object>();
			try
			{
				attributionDataChunk = (Dictionary<string, object>)serverResponse["data"];
			}
			catch (Exception ex2)
			{
				Exception e = ex2;
				Log("Failed Deserialize JSON attribution data chunk: " + e, KochLogLevel.warning);
			}
			int retry = 0;
			if (attributionDataChunk.ContainsKey("retry"))
			{
				retry = int.Parse(attributionDataChunk["retry"].ToString());
				Log("attribution retry: " + retry, KochLogLevel.warning);
			}
			if (retry == -1 && attributionDataChunk.ContainsKey("attribution"))
			{
				string attributionString = JsonWriter.Serialize(attributionDataChunk["attribution"]);
				PlayerPrefs.SetString("attribution", attributionString);
				attributionDataStr = attributionString;
				Log("Saved attribution chunk to persistent storage: " + attributionString, KochLogLevel.warning);
				if (attributionCallback != null)
				{
					attributionCallback(attributionString);
				}
			}
			if (retry == 0)
			{
				StartCoroutine("KochavaAttributionTimerFired", 60);
			}
			if (retry > 0)
			{
				StartCoroutine("KochavaAttributionTimerFired", retry);
			}
		}
	}

	public static string GetAttributionData()
	{
		return AttributionDataStr;
	}
}
