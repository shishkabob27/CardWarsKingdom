using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public class SLOTResourceManager : Singleton<SLOTResourceManager>
{
	private class LoadAssetBundleQueueInfo
	{
		public string path;

		public Type t;

		public string assetBundleName;

		public Action<object> loadAssetBundleCallback;

		public bool async;
	}

	private class QueuedResourceLoad
	{
		public string AssetBundle;

		public string AssetPath;

		public Action<UnityEngine.Object> Callback;

		public bool PreloadOnly;

		public bool BackgroundLoad;

		public bool SilentFail;

		public bool UseHighResLowRes;
	}

	private class QueuedTextureLoad
	{
		public string AssetBundle;

		public string AssetPath;

		public UITexture TextureObject;
	}

	public enum PreloadBundlesPoint
	{
		IntroBattle,
		Q1,
		Q2
	}

	public const string MainResourcesBundle = "MainResourcesBundle";

	public const string MainScenesBundle = "MainScenesBundle";

	public const string GeneralUIBundle = "GeneralBundle";

	public const string FtueUIBundle = "FTUEBundle";

	public const string FtueAudioBundle = "FTUEAudioBundle";

	public const string MainAudioBundle = "MainAudioBundle";

	private const string LOCAL_TEXTURE_CACHE_DIRECTORY = "TextureCache";

	private const string ETAG_EXTENSION = ".DragonWarsEtag";

	private const int MAX_RETRY_COUNT = 3;

	public const string CreaturePortraitPlaceholder = "UI/UI/LoadingPlaceholder";

	public const string CardArtPlaceholder = "UI/UI/LoadingPlaceholder";

	public const string LeagueBannerPlaceholder = "UI/UI/LoadingPlaceholder";

	public const string HeroPortraitPlaceholder = "UI/UI/LoadingPlaceholder";

	public List<string> BundlesToLoadUpFront = new List<string>();

	public string resourcePathPrefix_hires;

	public string resourcePathPrefix_lores;

	public List<UIAtlas> UiAtlases;

	public UIAtlas lowResAtlas;

	private List<AssetBundle> assetBundles = new List<AssetBundle>();

	private static AssetBundle mResourcesBundle;

	private static AssetBundle mScenesBundle;

	private List<LoadAssetBundleQueueInfo> loadAssetBundleQueue = new List<LoadAssetBundleQueueInfo>();

	private List<QueuedResourceLoad> mQueuedResourceLoads = new List<QueuedResourceLoad>();

	private Coroutine mQueuedResourceLoadCoroutine;

	private int mBundleIndex;

	private int mTotalBundles = -1;

	private WWW mInProgressWWW;

	private bool mCurrentlyBackgroundLoading;

	private List<QueuedTextureLoad> mQueuedTextureLoads = new List<QueuedTextureLoad>();

	private Coroutine mQueuedTextureLoadCoroutine;

	public string assetBundleBaseURL { get; set; }

	public string resourcePathPrefix { get; set; }

	public bool ResourceLoadInProgress
	{
		get
		{
			return mQueuedResourceLoadCoroutine != null;
		}
	}

	private void Start()
	{
		GetBaseURLs();
		resourcePathPrefix = ((!KFFLODManager.IsLowEndDevice()) ? resourcePathPrefix_hires : resourcePathPrefix_lores);
	}

	private void GetBaseURLs()
	{
		string empty = string.Empty;
		string streamingAssetsFile = TFUtils.GetStreamingAssetsFile("asset_bundle_settings.json");
		empty = ((!streamingAssetsFile.Contains("://")) ? File.ReadAllText(streamingAssetsFile) : SQSettings.getJsonPath(streamingAssetsFile));
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(empty);
		assetBundleBaseURL = (string)dictionary["asset_bundle_url"];
	}

	public static string GetResourceName(string name)
	{
		return GetResourceName(name, KFFLODManager.IsLowEndDevice());
	}

	public static string GetResourceName(string name, bool lowRes)
	{
		if (lowRes)
		{
			if (name.Contains("low_"))
			{
				return name;
			}
			int num = name.LastIndexOf("/");
			if (num >= 0)
			{
				string text = name.Substring(0, num);
				string text2 = name.Substring(num + 1);
				return text + "/low_" + text2;
			}
			return "low_" + name;
		}
		return name;
	}

	public bool IsHiLoRezResource(string path)
	{
		if (path.Contains("Flags/"))
		{
			return false;
		}
		return true;
	}

	public UnityEngine.Object LoadResource(string path, Type t = null)
	{
		path = path.Replace("low_", string.Empty);
		if (IsUsingAssetBundles() && mResourcesBundle != null)
		{
			string text = Path.GetFileNameWithoutExtension(path);
			if (text == string.Empty)
			{
				return null;
			}
			if (IsHiLoRezResource(path))
			{
				text = GetResourceName(text);
			}
			UnityEngine.Object @object = ((t == null) ? mResourcesBundle.LoadAsset(text) : mResourcesBundle.LoadAsset(text, t));
			if (@object != null)
			{
				return @object;
			}
		}
		if (KFFLODManager.IsLowEndDevice() && IsHiLoRezResource(path))
		{
			string resourceName = GetResourceName(path);
			UnityEngine.Object @object = ((t == null) ? Resources.Load(resourceName) : Resources.Load(resourceName, t));
			if (@object != null)
			{
				return @object;
			}
		}
		if (t != null)
		{
			return Resources.Load(path, t);
		}
		return Resources.Load(path);
	}

	public void QueueResourceLoad(string path, string assetBundleName, Action<UnityEngine.Object> loadResourceCallback, bool isPreload = false, bool silentFail = false, bool useHighRes = true, bool backgroundLoad = false)
	{
		QueuedResourceLoad queuedResourceLoad = new QueuedResourceLoad();
		queuedResourceLoad.AssetPath = path;
		queuedResourceLoad.AssetBundle = assetBundleName;
		queuedResourceLoad.Callback = loadResourceCallback;
		queuedResourceLoad.PreloadOnly = isPreload;
		if (isPreload)
		{
			queuedResourceLoad.BackgroundLoad = true;
		}
		else
		{
			queuedResourceLoad.BackgroundLoad = backgroundLoad;
		}
		queuedResourceLoad.SilentFail = silentFail;
		queuedResourceLoad.UseHighResLowRes = useHighRes;
		mQueuedResourceLoads.Add(queuedResourceLoad);
		if (mQueuedResourceLoadCoroutine == null)
		{
			mQueuedResourceLoadCoroutine = StartCoroutine(LoadResourceCoroutine());
		}
	}

	private IEnumerator LoadResourceCoroutine()
	{
		while (mQueuedResourceLoads.Count > 0)
		{
			QueuedResourceLoad queuedLoad = mQueuedResourceLoads[0];
			mQueuedResourceLoads.RemoveAt(0);
			mCurrentlyBackgroundLoading = queuedLoad.BackgroundLoad;
			if (IsUsingAssetBundles())
			{
				queuedLoad.AssetBundle = queuedLoad.AssetBundle.ToLower();
				if (KFFLODManager.IsLowEndDevice() && IsHiRezLowRezBundle(queuedLoad.AssetBundle))
				{
					queuedLoad.AssetBundle = "low_" + queuedLoad.AssetBundle;
				}
				AssetBundle assetBundle;
				if (queuedLoad.PreloadOnly)
				{
					assetBundle = Singleton<KFFAssetBundleManager>.Instance.GetAssetBundleByName(queuedLoad.AssetBundle);
				}
				else
				{
					if (queuedLoad.UseHighResLowRes)
					{
						queuedLoad.AssetPath = FixPath(GetResourceName(queuedLoad.AssetPath));
					}
					else
					{
						queuedLoad.AssetPath = FixPath(queuedLoad.AssetPath, false);
					}
					assetBundle = Singleton<KFFAssetBundleManager>.Instance.GetAssetBundleForResource(queuedLoad.AssetPath);
				}
				if (assetBundle == null)
				{
					while (true)
					{
						bool wasSuccess = false;
						yield return StartCoroutine(Singleton<KFFAssetBundleManager>.Instance.LoadAssetBundleCoroutine(assetBundleBaseURL, queuedLoad.AssetBundle, delegate(bool success, string errMsg, AssetBundle loadedBundle)
						{
							wasSuccess = success;
							if (success)
							{
								assetBundle = loadedBundle;
							}
							else
							{
								assetBundle = null;
							}
						}));
						if (wasSuccess)
						{
							break;
						}
						if (ProgressHiddenDuringPreload())
						{
							continue;
						}
						if (queuedLoad.SilentFail)
						{
							break;
						}
						int selection = -1;
						Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ERROR_LOADING_ASSET_BODY"), delegate
						{
							selection = 1;
						}, KFFLocalization.Get("!!RETRY"));
						while (true)
						{
							switch (selection)
							{
							case -1:
								yield return null;
								continue;
							case 0:
								goto end_IL_019a;
							}
							break;
						}
						continue;
						end_IL_019a:
						break;
					}
				}
				if (assetBundle == null)
				{
					queuedLoad.Callback(null);
					continue;
				}
				if (queuedLoad.PreloadOnly)
				{
					queuedLoad.Callback(null);
					continue;
				}
				queuedLoad.AssetPath += Singleton<KFFAssetBundleManager>.Instance.GetResourceExtension(queuedLoad.AssetPath);
				AssetBundleRequest bundleRequest = assetBundle.LoadAssetAsync(queuedLoad.AssetPath);
				UnityEngine.Object result2 = null;
				if (bundleRequest != null)
				{
					yield return bundleRequest;
					result2 = bundleRequest.asset;
				}
				queuedLoad.Callback(result2);
			}
			else if (queuedLoad.PreloadOnly)
			{
				queuedLoad.Callback(null);
			}
			else
			{
				string aPath = queuedLoad.AssetPath;
				if (queuedLoad.UseHighResLowRes)
				{
					aPath = GetResourceName(queuedLoad.AssetPath);
				}
				ResourceRequest resourceRequest = Resources.LoadAsync(aPath);
				if (resourceRequest == null && KFFLODManager.IsLowEndDevice())
				{
					resourceRequest = Resources.LoadAsync(queuedLoad.AssetPath);
				}
				UnityEngine.Object result = null;
				if (resourceRequest != null)
				{
					yield return resourceRequest;
					result = resourceRequest.asset;
				}
				queuedLoad.Callback(result);
			}
		}
		mQueuedResourceLoadCoroutine = null;
	}

	public bool IsUsingAssetBundles()
	{
		return false;
	}

	private string FixPath(string path, bool useHighResLowRes = true)
	{
		if (path.StartsWith("Assets/Resources/"))
		{
			if (useHighResLowRes)
			{
				path = path.Replace("Assets/Resources/", resourcePathPrefix);
			}
		}
		else if (!path.StartsWith("Assets/"))
		{
			path = ((!useHighResLowRes) ? ("Assets/Resources/" + path) : (resourcePathPrefix + path));
		}
		return path;
	}

	public static UnityEngine.Object GetAsset(AsyncOperation a)
	{
		ResourceRequest resourceRequest = a as ResourceRequest;
		if (resourceRequest != null)
		{
			return resourceRequest.asset;
		}
		AssetBundleRequest assetBundleRequest = a as AssetBundleRequest;
		if (assetBundleRequest != null)
		{
			return assetBundleRequest.asset;
		}
		return null;
	}

	public void StartResourceLoadProgress(int totalBundles)
	{
		mBundleIndex = 0;
		mTotalBundles = totalBundles;
	}

	private bool ProgressHiddenDuringPreload()
	{
		return mCurrentlyBackgroundLoading && !LoadingScreenController.ShowingLoadingScreenNotFadingOut();
	}

	public void GetResourceLoadProgress(out float totalProgress, out float fileProgress)
	{
		totalProgress = -1f;
		fileProgress = -1f;
		if (mTotalBundles <= 0 || ProgressHiddenDuringPreload())
		{
			return;
		}
		totalProgress = (float)mBundleIndex / (float)mTotalBundles;
		WWW activeWWW = Singleton<KFFAssetBundleManager>.Instance.GetActiveWWW();
		if (activeWWW != null)
		{
			mInProgressWWW = activeWWW;
		}
		if (mInProgressWWW != null)
		{
			totalProgress += mInProgressWWW.progress / (float)mTotalBundles;
			if ((LoadingScreenController.ShowingLoadingScreenNotFadingOut() || DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.AssetBundleDownload) && mTotalBundles > 1)
			{
				fileProgress = mInProgressWWW.progress;
			}
		}
	}

	private void CheckResourceLoadStart(int defaultAmount = 1)
	{
		if (mTotalBundles <= 0)
		{
			mBundleIndex = 0;
			mTotalBundles = defaultAmount;
		}
	}

	public void OnResourceLoadDone()
	{
		mInProgressWWW = null;
		mBundleIndex++;
		if (mBundleIndex >= mTotalBundles)
		{
			mTotalBundles = -1;
		}
	}

	public IEnumerator LoadCreatureResources(CreatureData creature, Action<UnityEngine.GameObject, Texture2D> callback, bool lockInput = true)
	{
		CheckResourceLoadStart(2);
		GameObject objData = null;
		Texture2D texture = null;
		string rootFolder = "Creatures/" + creature.Prefab + "/";
		if (lockInput)
		{
			UICamera.LockInput();
		}
		bool done2 = false;
		//QueueResourceLoad(rootFolder + creature.Prefab, creature.Prefab, delegate(UnityEngine.Object loadedResouce)
		//{
            objData = Resources.Load(rootFolder + creature.Prefab, typeof(GameObject)) as GameObject;
            done2 = true;
		//}, false, false, true, !lockInput);
		while (!done2)
		{
			yield return null;
		}
		OnResourceLoadDone();
		done2 = false;
		//QueueResourceLoad(string.Concat(rootFolder, "Textures/", creature.Faction, "/", creature.PrefabTexture), creature.Prefab, delegate(UnityEngine.Object loadedResouce)
		//{
			texture = Resources.Load(rootFolder + "Textures/" + creature.Faction + "/" + creature.PrefabTexture, typeof(Texture2D)) as Texture2D;
			done2 = true;
		//}, false, false, false, !lockInput);
		while (!done2)
		{
			yield return null;
		}
		OnResourceLoadDone();
		if (lockInput)
		{
			UICamera.UnlockInput();
		}
		callback(objData, texture);
	}

	public IEnumerator LoadLeaderResources(LeaderData leader, Action<UnityEngine.GameObject> callback)
	{
		CheckResourceLoadStart();
		GameObject objData = null;
		UICamera.LockInput();
		bool done = false;
		//QueueResourceLoad("Characters/" + leader.Prefab + "/" + leader.Prefab, leader.Prefab, delegate(UnityEngine.Object loadedResouce)
		//{
			var resource = Resources.LoadAsync("Characters/" + leader.Prefab + "/" + leader.Prefab, typeof(GameObject));
		objData = resource.asset as GameObject;
			done = true;
		//});
		while (!done)
		{
			yield return null;
		}
		OnResourceLoadDone();
		UICamera.UnlockInput();
		callback(objData);
	}

	public IEnumerator LoadEnvironmentResources(QuestData quest, Action<UnityEngine.Object> callback)
	{
		CheckResourceLoadStart();
		UnityEngine.Object objData = null;
		UICamera.LockInput();
		bool done = false;
		QueueResourceLoad("Environment/" + quest.LevelPrefab + "/" + quest.LevelPrefab, quest.LevelPrefab, delegate(UnityEngine.Object loadedResouce)
		{
			objData = loadedResouce;
			done = true;
		});
		while (!done)
		{
			yield return null;
		}
		OnResourceLoadDone();
		UICamera.UnlockInput();
		callback(objData);
	}

	public IEnumerator LoadGameBoardResources(QuestData quest, Action<UnityEngine.Object> callback)
	{
		CheckResourceLoadStart();
		UnityEngine.Object objData = null;
		UICamera.LockInput();
		bool done = false;
		QueueResourceLoad("GameBoard/" + quest.BoardPrefab + "/" + quest.BoardPrefab, quest.BoardPrefab, delegate(UnityEngine.Object loadedResouce)
		{
			objData = loadedResouce;
			done = true;
		});
		while (!done)
		{
			yield return null;
		}
		OnResourceLoadDone();
		UICamera.UnlockInput();
		callback(objData);
	}

	public void LoadAudioResource(string clipName, string bundleName, Action<AudioClip> callback)
	{
		CheckResourceLoadStart();
		bool lockInput = LoadingScreenController.ShowingLoadingScreenNotFadingOut();
		if (lockInput)
		{
			UICamera.LockInput();
		}
		QueueResourceLoad(clipName, bundleName, delegate(UnityEngine.Object loadedResouce)
		{
			OnResourceLoadDone();
			if (lockInput)
			{
				UICamera.UnlockInput();
			}
			callback(loadedResouce as AudioClip);
		}, false, false, false);
	}

	public void QueueUITextureLoad(string texture, string assetBundle, string placeholderTexture, UITexture textureObject)
	{
		Texture textureFromCache = UITexture.GetTextureFromCache(Path.GetFileName(texture));
		if (textureFromCache != null)
		{
			textureObject.UnloadTexture();
			textureObject.mainTexture = textureFromCache;
			return;
		}
		QueuedTextureLoad queuedTextureLoad = new QueuedTextureLoad();
		queuedTextureLoad.AssetBundle = assetBundle;
		queuedTextureLoad.AssetPath = texture;
		queuedTextureLoad.TextureObject = textureObject;
		mQueuedTextureLoads.Add(queuedTextureLoad);
		textureObject.ReplaceTexture(placeholderTexture);
		textureObject.StreamingInTexture = queuedTextureLoad.AssetPath;
		if (mQueuedTextureLoadCoroutine == null)
		{
			mQueuedTextureLoadCoroutine = StartCoroutine(QueuedTextureLoadCoroutine());
		}
	}

	private IEnumerator QueuedTextureLoadCoroutine()
	{
		while (mQueuedTextureLoads.Count > 0)
		{
			QueuedTextureLoad nextLoad = mQueuedTextureLoads[0];
			mQueuedTextureLoads.RemoveAt(0);
			if (nextLoad.TextureObject == null || nextLoad.TextureObject.StreamingInTexture != nextLoad.AssetPath)
			{
				continue;
			}
			Texture alreadyLoaded = UITexture.GetTextureFromCache(Path.GetFileName(nextLoad.AssetPath));
			if (alreadyLoaded != null)
			{
				nextLoad.TextureObject.UnloadTexture();
				nextLoad.TextureObject.mainTexture = alreadyLoaded;
				continue;
			}
			Texture texture = null;
			bool done = false;
			QueueResourceLoad(nextLoad.AssetPath, nextLoad.AssetBundle, delegate(UnityEngine.Object loadedResouce)
			{
				texture = loadedResouce as Texture;
				done = true;
			}, false, true);
			while (!done)
			{
				yield return null;
			}
			if (!(nextLoad.TextureObject == null) && !(nextLoad.TextureObject.StreamingInTexture != nextLoad.AssetPath) && texture != null)
			{
				nextLoad.TextureObject.UnloadTexture();
				nextLoad.TextureObject.mainTexture = texture;
				UITexture.AddLoadedTextureToCache(texture);
			}
		}
		mQueuedTextureLoadCoroutine = null;
	}

	public void StartAssetBundlePreload(PreloadBundlesPoint preloadPoint)
	{
		switch (preloadPoint)
		{
		case PreloadBundlesPoint.IntroBattle:
		{
			QuestLoadoutData data2 = QuestLoadoutDataManager.Instance.GetData("StartDeck");
			QuestLoadoutData data3 = QuestLoadoutDataManager.Instance.GetData("MAIN_L1_Q1");
			int totalBundles3 = 3 + 2 * (data2.Entries.Count + data3.Entries.Count);
			StartResourceLoadProgress(totalBundles3);
			QueueResourceLoad(null, "MainAudioBundle", OnAssetPreloadDone, true);
			QueueResourceLoad(null, "FTUE", OnAssetPreloadDone, true);
			QueueResourceLoad(null, "FTUEBundle", OnAssetPreloadDone, true);
			foreach (QuestLoadoutEntry entry in data2.Entries)
			{
				QueueCreatureResourceLoad(entry.Creature);
			}
			{
				foreach (QuestLoadoutEntry entry2 in data3.Entries)
				{
					QueueCreatureResourceLoad(entry2.Creature);
				}
				break;
			}
		}
		case PreloadBundlesPoint.Q1:
		{
			QuestLoadoutData data = QuestLoadoutDataManager.Instance.GetData("MAIN_L1_Q2");
			int totalBundles2 = 2 * data.Entries.Count;
			StartResourceLoadProgress(totalBundles2);
			{
				foreach (QuestLoadoutEntry entry3 in data.Entries)
				{
					QueueCreatureResourceLoad(entry3.Creature);
				}
				break;
			}
		}
		case PreloadBundlesPoint.Q2:
		{
			int totalBundles = 2;
			StartResourceLoadProgress(totalBundles);
			QueueResourceLoad(null, "GeneralBundle", OnAssetPreloadDone, true);
			QueueResourceLoad(null, "BaseGame", OnAssetPreloadDone, true);
			break;
		}
		}
	}

	private void QueueCreatureResourceLoad(CreatureData creature)
	{
		QueueResourceLoad("Creatures/" + creature.Prefab + "/" + creature.Prefab, creature.Prefab, OnAssetPreloadDone, true);
		QueueResourceLoad(string.Concat("Creatures/", creature.Prefab, "/Textures/", creature.Faction, "/", creature.PrefabTexture), creature.Prefab, OnAssetPreloadDone, true);
	}

	private void OnAssetPreloadDone(UnityEngine.Object loadedAsset)
	{
		OnResourceLoadDone();
	}

	private bool IsHiRezLowRezBundle(string bundleName)
	{
		bundleName = bundleName.ToLower();
		if (bundleName == "MainResourcesBundle".ToLower() || bundleName == "MainScenesBundle".ToLower() || bundleName == "FTUEAudioBundle".ToLower() || bundleName == "MainAudioBundle".ToLower())
		{
			return false;
		}
		return true;
	}

	public IEnumerator LoadPrimaryBundlesCouroutine()
	{
		if (!IsUsingAssetBundles())
		{
			yield break;
		}
		List<string> bundlesToLoad = new List<string>(BundlesToLoadUpFront);
		if (mResourcesBundle == null)
		{
			bundlesToLoad.Add("MainResourcesBundle");
		}
		if (mScenesBundle == null)
		{
			bundlesToLoad.Add("MainScenesBundle");
		}
		if (bundlesToLoad.Count == 0)
		{
			yield break;
		}
		StartResourceLoadProgress(bundlesToLoad.Count);
		mCurrentlyBackgroundLoading = false;
		for (int i = 0; i < bundlesToLoad.Count; i++)
		{
			string bundleName = bundlesToLoad[i];
			if (KFFLODManager.IsLowEndDevice() && IsHiRezLowRezBundle(bundlesToLoad[i]))
			{
				bundleName = "low_" + bundleName;
			}
			if (Singleton<KFFAssetBundleManager>.Instance.GetAssetBundleByName(bundleName) != null)
			{
				OnResourceLoadDone();
				continue;
			}
			while (true)
			{
				bool wasSuccess = false;
				yield return StartCoroutine(Singleton<KFFAssetBundleManager>.Instance.LoadAssetBundleCoroutine(assetBundleBaseURL, bundleName, delegate(bool success, string errMsg, AssetBundle loadedBundle)
				{
					wasSuccess = success;
					if (success)
					{
						if (bundleName == "MainResourcesBundle")
						{
							mResourcesBundle = loadedBundle;
						}
						else if (bundleName == "MainScenesBundle")
						{
							mScenesBundle = loadedBundle;
						}
						OnResourceLoadDone();
					}
				}));
				if (wasSuccess)
				{
					break;
				}
				int selection = -1;
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ERROR_LOADING_ASSET_BODY"), delegate
				{
					selection = 1;
				}, KFFLocalization.Get("!!RETRY"));
				while (selection == -1)
				{
					yield return null;
				}
			}
		}
	}
}
