using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : DetachedSingleton<SceneFlowManager>
{
	public enum Scene
	{
		Loading,
		BootVideoScene,
		VersionCheck,
		AssetBundleDownload,
		Connect,
		FrontEnd,
		Battle,
		SceneCount,
		None
	}

	public enum ReturnLocation
	{
		NotSet,
		Town,
		Map,
		SpecialQuests,
		Pvp
	}

	private string[] mSceneMap = new string[7];

	private string[] mLowResSceneMap = new string[7];

	private Scene mPreviousScene = Scene.None;

	private Scene mCurrentScene = Scene.None;

	private ReturnLocation mReturnLocation;

	public SceneFlowManager()
	{
		mSceneMap[0] = "LoadingScreen";
		mSceneMap[1] = "BootVideoScene";
		mSceneMap[2] = "VersionCheckScene";
		mSceneMap[3] = "AssetBundleDownloadScene";
		mSceneMap[4] = "ConnectScene";
		mSceneMap[5] = "FrontEndScene";
		mSceneMap[6] = "BattleScene";
		mLowResSceneMap[0] = "low_LoadingScreen";
		mLowResSceneMap[1] = string.Empty;
		mLowResSceneMap[2] = "low_VersionCheckScene";
		mLowResSceneMap[3] = "low_AssetBundleDownloadScene";
		mLowResSceneMap[4] = "low_ConnectScene";
		mLowResSceneMap[5] = "low_FrontEndScene";
		mLowResSceneMap[6] = "low_BattleScene";
		mCurrentScene = GetSceneFromName(SceneManager.GetActiveScene().name);
	}

	public ReturnLocation GetReturnLocation()
	{
		return mReturnLocation;
	}

	public void ClearReturnLocation()
	{
		mReturnLocation = ReturnLocation.NotSet;
	}

	public void LoadFrontEndScene(ReturnLocation locationToReturnTo = ReturnLocation.Town)
	{
		mReturnLocation = locationToReturnTo;
		LoadScene(Scene.FrontEnd);
	}

	public void LoadBattleScene()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			string upsightEvent = "Multiplayer.MatchSynchronization.Success";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("duration", Singleton<PVPPrepScreenController>.Instance.syncTime);
			Singleton<PVPPrepScreenController>.Instance.syncTime = 0f;
			Singleton<PVPPrepScreenController>.Instance.matchFound = false;
		}
		LoadScene(Scene.Battle);
	}

	public void LoadVersionCheckScene()
	{
		LoadScene(Scene.VersionCheck);
	}

	public void LoadBootVideoSceneDirect()
	{
		LoadScene(Scene.BootVideoScene, true);
	}

	public void LoadVersionCheckSceneDirect()
	{
		LoadScene(Scene.VersionCheck, true);
	}

	public void LoadAssetBundleSceneDirect()
	{
		LoadScene(Scene.AssetBundleDownload, true);
	}

	public void LoadConnectSceneDirect()
	{
		LoadScene(Scene.Connect, true);
	}

	private void LoadScene(Scene scene, bool bypass = false)
	{
		mPreviousScene = mCurrentScene;
		mCurrentScene = scene;
		MenuStackManager.ClearStack();
		UICamera.ForceUnlockInput();
		DoManualAnimationCleanup();
		if (!bypass)
		{
			SceneManager.LoadScene(GetNameFromScene(Scene.Loading));
		}
		else
		{
			SceneManager.LoadScene(GetNameFromScene(mCurrentScene));
		}
	}

	public void OnLoadingScreenLoaded()
	{
	}

	private Scene GetSceneFromName(string name)
	{
		for (int i = 0; i < mSceneMap.Length; i++)
		{
			if (mSceneMap[i] == name)
			{
				return (Scene)i;
			}
		}
		return Scene.None;
	}

	public string GetNameFromScene(Scene scene)
	{
		if (scene == Scene.SceneCount || scene == Scene.None)
		{
			return null;
		}
		string result = mSceneMap[(int)scene];
		if (KFFLODManager.IsLowEndDevice() && !string.IsNullOrEmpty(mLowResSceneMap[(int)scene]))
		{
			result = mLowResSceneMap[(int)scene];
		}
		return result;
	}

	public bool InBattleScene()
	{
		return mCurrentScene == Scene.Battle;
	}

	public bool InFrontEnd()
	{
		return mCurrentScene == Scene.FrontEnd;
	}

	public Scene GetCurrentScene()
	{
		return mCurrentScene;
	}

	public static void DoManualAnimationCleanup()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(Animation));
		for (int i = 0; i < array.Length; i++)
		{
			Animation animation = (Animation)array[i];
			animation.Stop();
			List<string> list = new List<string>();
			foreach (AnimationState item in animation)
			{
				list.Add(item.clip.name);
			}
			foreach (string item2 in list)
			{
				try
				{
					if (animation.GetClip(item2) != null)
					{
						animation.RemoveClip(item2);
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
