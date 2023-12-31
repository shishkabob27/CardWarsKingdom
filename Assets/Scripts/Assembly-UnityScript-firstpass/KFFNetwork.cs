using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using CompilerGenerated;
using UnityEngine;

[Serializable]
public class KFFNetwork : MonoBehaviour
{
	[Serializable]
	public enum ErrorID
	{
		None,
		Generic,
		NotLoggedIn,
		InvalidClientVersion
	}

	[Serializable]
	[XmlRoot("RequestResult")]
	public class WWWRequestResult : KFFDictionary
	{
		public bool isValid()
		{
			return IsValid();
		}

		public bool IsValid()
		{
			int result;
			if (entries.Count < 2)
			{
				result = 0;
			}
			else
			{
				KFFDictionaryEntry kFFDictionaryEntry = entries[0];
				if (kFFDictionaryEntry.key != "ERROR_ID")
				{
					result = 0;
				}
				else if (int.Parse(kFFDictionaryEntry.value) != 0)
				{
					result = 0;
				}
				else
				{
					kFFDictionaryEntry = entries[1];
					result = ((!(kFFDictionaryEntry.key != "ERROR_MSG")) ? 1 : 0);
				}
			}
			return (byte)result != 0;
		}
	}

	[Serializable]
	public class WWWInfo
	{
		public WWW www;

		public __WWWInfo_callback_0024callable1_002464_32__ callback;

		public object callbackParam;

		public string url;

		public WWWForm form;

		public bool rawRequest;

		public byte[] postData;

		public bool queued;

		public bool active;

		public int version;

		public WWWInfo()
		{
			version = -1;
			form = null;
			rawRequest = false;
			postData = null;
		}
	}

	[NonSerialized]
	public static __KFFNetwork_deserializeJSONCallback_0024callable0_002419_53__ deserializeJSONCallback;

	private List<WWWInfo> wwwList;

	private int activeRequestCount;

	private int currSleepTimeout;

	[NonSerialized]
	private static KFFNetwork the_instance;

	[NonSerialized]
	public static bool manualSleepControl;

	[NonSerialized]
	public static int MAX_CONCURRENT_WWW_REQUEST_COUNT = 3;

	public KFFNetwork()
	{
		wwwList = new List<WWWInfo>();
	}

	public static KFFNetwork GetInstance()
	{
		if (!the_instance)
		{
			the_instance = (KFFNetwork)UnityEngine.Object.FindObjectOfType(typeof(KFFNetwork));
		}
		if (Application.isPlaying && !the_instance)
		{
			GameObject gameObject = new GameObject();
			if ((bool)gameObject)
			{
				the_instance = (KFFNetwork)gameObject.AddComponent(typeof(KFFNetwork));
			}
			if ((bool)gameObject)
			{
				gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
				gameObject.name = "AutomaticallyCreatedNetwork";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		return the_instance;
	}

	public WWWInfo SendWWWRequest(string url, __WWWInfo_callback_0024callable1_002464_32__ WWWRequestCallback, object callbackParam)
	{
		return SendWWWRequestWithForm(null, url, WWWRequestCallback, callbackParam, rawrequest: false);
	}

	public WWWInfo SendWWWRequestWithForm(WWWForm form, string url, __WWWInfo_callback_0024callable1_002464_32__ WWWRequestCallback, object callbackParam)
	{
		return SendWWWRequestWithForm(form, url, WWWRequestCallback, callbackParam, rawrequest: false);
	}

	public WWWInfo SendWWWRequestWithForm(WWWForm form, string url, __WWWInfo_callback_0024callable1_002464_32__ WWWRequestCallback, object callbackParam, bool rawrequest)
	{
		return SendWWWRequestWithForm(form, url, WWWRequestCallback, callbackParam, rawrequest, null);
	}

	public WWWInfo SendWWWRequestWithForm(WWWForm form, string url, __WWWInfo_callback_0024callable1_002464_32__ WWWRequestCallback, object callbackParam, bool rawrequest, byte[] postData)
	{
		WWWInfo wWWInfo = null;
		if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
		{
			WWW wWW = null;
			wWW = (form == null ? new WWW(url) : new WWW(url, form));
			if (wWW != null)
			{
				wWWInfo = new WWWInfo();
				wWWInfo.www = wWW;
				wWWInfo.queued = false;
				wWWInfo.active = true;
				activeRequestCount++;
			}
		}
		else
		{
			wWWInfo = new WWWInfo();
			wWWInfo.www = null;
			wWWInfo.queued = true;
			wWWInfo.active = false;
		}
		if (wWWInfo != null)
		{
			wWWInfo.callback = WWWRequestCallback;
			wWWInfo.callbackParam = callbackParam;
			wWWInfo.url = url;
			wWWInfo.form = form;
			wWWInfo.rawRequest = rawrequest;
			wWWInfo.postData = postData;
			wwwList.Add(wWWInfo);
		}
		else if (WWWRequestCallback != null)
		{
			WWWRequestCallback(null, null, null, callbackParam);
		}
		return wWWInfo;
	}

	public WWWInfo LoadFromCacheOrDownload(string url, int version, __WWWInfo_callback_0024callable1_002464_32__ WWWRequestCallback, object callbackParam)
	{
		WWWInfo wWWInfo = null;
		if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
		{
			WWW wWW = WWW.LoadFromCacheOrDownload(url, version);
			if (wWW != null)
			{
				wWWInfo = new WWWInfo();
				wWWInfo.www = wWW;
				wWWInfo.queued = false;
				wWWInfo.active = true;
				wWWInfo.version = version;
				activeRequestCount++;
			}
		}
		else
		{
			wWWInfo = new WWWInfo();
			wWWInfo.www = null;
			wWWInfo.queued = true;
			wWWInfo.active = false;
			wWWInfo.version = version;
		}
		if (wWWInfo != null)
		{
			wWWInfo.callback = WWWRequestCallback;
			wWWInfo.callbackParam = callbackParam;
			wWWInfo.url = url;
			wWWInfo.form = null;
			wWWInfo.rawRequest = true;
			wwwList.Add(wWWInfo);
		}
		else if (WWWRequestCallback != null)
		{
			WWWRequestCallback(null, null, null, callbackParam);
		}
		return wWWInfo;
	}
	public void CancelWWWRequest(WWWInfo info)
	{
		if (info != null)
		{
			if ((info.active || info.queued) && info.callback != null)
			{
				info.callback(info, null, null, info.callbackParam);
			}
			if (info.active && activeRequestCount > 0)
			{
				activeRequestCount--;
			}
			wwwList.Remove(info);
		}
	}

	public static void ToggleManualSleepControl(bool enable)
	{
		manualSleepControl = enable;
	}

	public void Update()
	{
		int num = ((wwwList.Count <= 0) ? (-2) : (-1));
		if (!manualSleepControl && currSleepTimeout != num)
		{
			currSleepTimeout = num;
			Screen.sleepTimeout = num;
		}
		for (int num2 = wwwList.Count - 1; num2 >= 0; num2--)
		{
			WWWInfo wWWInfo = wwwList[num2];
			if (wWWInfo.queued)
			{
				if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
				{
					string url = wWWInfo.url;
					WWW wWW = null;
					wWW = ((wWWInfo.version >= 0) ? WWW.LoadFromCacheOrDownload(url, wWWInfo.version) : ((wWWInfo.form == null) ? new WWW(url) : new WWW(url, wWWInfo.form)));
					if (wWW != null)
					{
						wWWInfo.www = wWW;
						wWWInfo.queued = false;
						wWWInfo.active = true;
						activeRequestCount++;
					}
					else
					{
						if (wWWInfo.callback != null && wWWInfo.callback != null)
						{
							wWWInfo.callback(wWWInfo, null, null, wWWInfo.callbackParam);
						}
						wwwList.RemoveAt(num2);
					}
				}
			}
			else if ((wWWInfo.www == null) || wWWInfo.www.isDone || !string.IsNullOrEmpty(wWWInfo.www.error))
			{
				object arg = null;
				string text = null;
				bool flag = true;
				if ((wWWInfo.www != null) && wWWInfo.www.error == null)
				{
					if (wWWInfo.rawRequest)
					{
						arg = wWWInfo.www;
						text = ((wWWInfo.www == null) ? null : wWWInfo.www.error);
					}
					else
					{
                        try
                        {
                            object obj = null;
                            if (deserializeJSONCallback != null)
                            {
                                obj = deserializeJSONCallback(wWWInfo.www.text);
                            }

                            Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                            WWWRequestResult wWWRequestResult = null;

                            if (dictionary != null)
                            {
                                wWWRequestResult = new WWWRequestResult();

                                foreach (string key in dictionary.Keys)
                                {
                                    object valueObj = dictionary[key];

                                    if (valueObj != null)
                                    {
                                        Type valueType = valueObj.GetType();

                                        if (valueType == typeof(float))
                                        {
                                            float floatValue = (float)valueObj;
                                            wWWRequestResult.SetValue(key, floatValue);
                                        }
                                        else if (valueType == typeof(long))
                                        {
                                            int intValue = (int)(long)valueObj;
                                            wWWRequestResult.SetValue(key, intValue);
                                        }
                                        else if (valueType == typeof(string))
                                        {
                                            wWWRequestResult.SetValue(key, valueObj as string);
                                        }
                                    }
                                    else
                                    {
                                        wWWRequestResult.SetValue(key, null);
                                    }
                                }
                            }

                            string errorText = (wWWInfo.www == null) ? null : wWWInfo.www.error;

                            if (wWWRequestResult != null)
                            {
                                arg = wWWRequestResult;
                                wWWRequestResult.CreateDictionary();
                            }
                        }
                        catch (Exception rhs)
                        {
                            Debug.Log("error: " + rhs + " www.text: " + wWWInfo.www.text);
                            text = "Error parsing JSON: " + wWWInfo.url + "\n\nerror: " + rhs + "\n\nwww.text:\n" + wWWInfo.www.text;
                        }
                    }
				}
				else
				{
					text = ((wWWInfo.www == null) ? null : wWWInfo.www.error);
				}
				if (wWWInfo.callback != null && flag)
				{
					wWWInfo.callback(wWWInfo, arg, text, wWWInfo.callbackParam);
				}
				wwwList.RemoveAt(num2);
				activeRequestCount--;
			}
		}
	}

	public static void SetMaxConcurrentWWWRequestCount(int count)
	{
		MAX_CONCURRENT_WWW_REQUEST_COUNT = count;
	}

	public static int GetMaxConcurrentWWWRequestCount()
	{
		return MAX_CONCURRENT_WWW_REQUEST_COUNT;
	}

	public void Main()
	{
	}
}
