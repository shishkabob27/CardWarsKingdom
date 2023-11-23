using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
	private static LoadingScreenController g_loadingScreenController;

	public UISprite progressBar;

	public UILabel progressPercent;

	public UITexture LoadingScreenTexture;

	public UITexture LoadingLogo;

	public UITweenController HideLoadingScreenTween;

	private void Awake()
	{
		g_loadingScreenController = this;
	}

	public static LoadingScreenController GetInstance()
	{
		return g_loadingScreenController;
	}

	private void Start()
	{
		StartCoroutine(DisplayLoadingScreen());
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_Badlands");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_IceKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_CandyKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_FireKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_LumpySpace");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_MarcelinesCave");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_OilRig");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_TreeFort");
	}

	private IEnumerator DisplayLoadingScreen()
	{
		LoadingScreenTexture.gameObject.SetActive(true);
		if (LoadingScreenTexture.mainTexture == null)
		{
			LoadingScreenTexture.ReplaceTexture("LoadingScreen");
		}
		LoadLoadingScreenLogo(LoadingLogo);
		SceneFlowManager.Scene mCurrentScene = DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene();
		AsyncOperation async = SceneManager.LoadSceneAsync(DetachedSingleton<SceneFlowManager>.Instance.GetNameFromScene(mCurrentScene));
		float progress2 = 0f;
		while (!async.isDone)
		{
			progress2 = async.progress;
			switch (mCurrentScene)
			{
			}
			progressBar.fillAmount = progress2;
			progressPercent.text = (int)(progress2 * 100f) + " %";
			if (async.progress >= 0.9f)
			{
				break;
			}
			yield return null;
		}
		while (Singleton<SLOTResourceManager>.Instance.ResourceLoadInProgress)
		{
			yield return null;
		}
		if (mCurrentScene == SceneFlowManager.Scene.FrontEnd)
		{
			yield return StartCoroutine(Singleton<SLOTMusic>.Instance.PlayFrontEndMusic());
			HideLoading();
		}
	}

	public void HideLoading(EventDelegate.Callback doneHidingCallback = null)
	{
		if (doneHidingCallback != null)
		{
			HideLoadingScreenTween.PlayWithCallback(doneHidingCallback);
		}
		else
		{
			HideLoadingScreenTween.Play();
		}
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		LoadingScreenTexture.UnloadTexture();
		LoadingLogo.UnloadTexture();
	}

	public static bool ShowingLoadingScreen()
	{
		return g_loadingScreenController != null;
	}

	public static bool ShowingLoadingScreenNotFadingOut()
	{
		return g_loadingScreenController != null && !g_loadingScreenController.HideLoadingScreenTween.AnyTweenPlaying();
	}

	public static void LoadLoadingScreenLogo(UITexture texture)
	{
		string text = KFFLocalization.Get("!!LOADING_LOGO");
		if (string.IsNullOrEmpty(text))
		{
			texture.UnloadTexture();
		}
		else
		{
			texture.ReplaceTexture(text);
		}
	}
}
