using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KFFAssetBundleManager : Singleton<KFFAssetBundleManager>
{
	private class AssetInfo
	{
		public string extension;

		public AssetBundle assetBundle;
	}

	private static AssetBundleManifest mainManifest = null;

	private bool mainManifestLoading;

	private static Dictionary<string, AssetInfo> assetInfoDict = new Dictionary<string, AssetInfo>();

	private static List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

	private static Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();

	private WWW activeWWW;

	private int getExtensionsCount;

	private int getExtensionsProgress;

	private float loadProgress;

	public bool pauseTest;

	public bool IsBusy { get; private set; }

	private IEnumerator GetExtensionsFromAssetBundle(AssetBundle assetBundle, string assetBundleName)
	{
		if (loadedAssetBundles.Contains(assetBundle))
		{
			yield break;
		}
		loadedAssetBundles.Add(assetBundle);
		assetBundleDict[assetBundleName] = assetBundle;
		string[] assetNames = assetBundle.GetAllAssetNames();
		getExtensionsCount = assetNames.Length;
		getExtensionsProgress = 0;
		float timestamp = Time.realtimeSinceStartup;
		float interval = 1f / 60f;
		string[] array = assetNames;
		foreach (string assetName in array)
		{
			string extension = Path.GetExtension(assetName);
			string pathWithoutExtension = assetName.Substring(0, assetName.Length - extension.Length);
			AssetInfo info = new AssetInfo
			{
				extension = extension,
				assetBundle = assetBundle
			};
			assetInfoDict[pathWithoutExtension] = info;
			if (Time.realtimeSinceStartup - timestamp >= interval)
			{
				yield return null;
				timestamp = Time.realtimeSinceStartup;
			}
			getExtensionsProgress++;
		}
		getExtensionsCount = 0;
	}

	public IEnumerator LoadAssetBundleCoroutine(string assetBundleBaseURL, string assetBundleName, Action<bool, string, AssetBundle> doneCallback)
	{
		IsBusy = true;
		loadProgress = 0f;
		bool success = false;
		string errMsg = null;
		AssetBundle retBundle = null;
		if (!string.IsNullOrEmpty(assetBundleBaseURL))
		{
			string basePlatformURL2 = assetBundleBaseURL;
			basePlatformURL2 += "Android/";
			yield return StartCoroutine(LoadMainManifest(basePlatformURL2));
			if (mainManifest != null)
			{
				while (!Caching.ready)
				{
					yield return null;
				}
				Hash128 hash = mainManifest.GetAssetBundleHash(assetBundleName);
				string url = basePlatformURL2 + assetBundleName.ToLower();
				WWW www = (activeWWW = WWW.LoadFromCacheOrDownload(url, hash, 0u));
				yield return www;
				activeWWW = null;
				loadProgress = 1f;
				if (www.error != null || www.assetBundle == null)
				{
					errMsg = www.error;
				}
				else
				{
					success = true;
					retBundle = www.assetBundle;
					yield return StartCoroutine(GetExtensionsFromAssetBundle(www.assetBundle, assetBundleName));
				}
			}
		}
		if (doneCallback != null)
		{
			doneCallback(success, errMsg, retBundle);
		}
		while (pauseTest)
		{
			yield return null;
		}
		IsBusy = false;
	}

	private IEnumerator LoadMainManifest(string basePlatformURL)
	{
		while (mainManifestLoading)
		{
			yield return null;
		}
		if (mainManifest == null)
		{
			mainManifestLoading = true;
			string manifestURL2 = basePlatformURL;
			manifestURL2 += "Android";
			WWW www = new WWW(manifestURL2);
			yield return www;
			if (www.error == null && www.assetBundle != null)
			{
				AssetBundle bundle = www.assetBundle;
				mainManifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
			}
			mainManifestLoading = false;
		}
	}

	public string GetResourceExtension(string resourcePath)
	{
		if (assetInfoDict != null && !string.IsNullOrEmpty(resourcePath))
		{
			resourcePath = resourcePath.ToLower();
			if (assetInfoDict.ContainsKey(resourcePath))
			{
				return assetInfoDict[resourcePath].extension;
			}
		}
		return null;
	}

	public AssetBundle GetAssetBundleForResource(string resourcePath)
	{
		if (assetInfoDict != null && !string.IsNullOrEmpty(resourcePath))
		{
			resourcePath = resourcePath.ToLower();
			if (assetInfoDict.ContainsKey(resourcePath))
			{
				return assetInfoDict[resourcePath].assetBundle;
			}
		}
		return null;
	}

	public AssetBundle GetAssetBundleByName(string assetBundleName)
	{
		assetBundleName = assetBundleName.ToLower();
		if (assetBundleDict != null && !string.IsNullOrEmpty(assetBundleName) && assetBundleDict.ContainsKey(assetBundleName))
		{
			return assetBundleDict[assetBundleName];
		}
		return null;
	}

	public WWW GetActiveWWW()
	{
		return activeWWW;
	}

	public float GetLoadProgress()
	{
		if (activeWWW != null)
		{
			return activeWWW.progress;
		}
		return loadProgress;
	}

	public float GetAssetBundleInitProgress()
	{
		if (getExtensionsCount <= 0)
		{
			return -1f;
		}
		return (float)getExtensionsProgress / (float)getExtensionsCount;
	}

	public void UnloadAllAssetBundles()
	{
		foreach (AssetBundle loadedAssetBundle in loadedAssetBundles)
		{
			if (loadedAssetBundle != null)
			{
				loadedAssetBundle.Unload(true);
			}
		}
		loadedAssetBundles.Clear();
		assetBundleDict.Clear();
		mainManifest = null;
	}
}
