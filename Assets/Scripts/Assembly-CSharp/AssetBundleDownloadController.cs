using System.Collections;
using UnityEngine;

public class AssetBundleDownloadController : MonoBehaviour
{
	public GameObject Background;

	public UITexture LoadingLogo;

	private void Start()
	{
		Background.SetActive(true);
		LoadingScreenController.LoadLoadingScreenLogo(LoadingLogo);
		StartCoroutine(LoadPrimaryBundles());
	}

	private IEnumerator LoadPrimaryBundles()
	{
		yield return null;
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadPrimaryBundlesCouroutine());
		Singleton<FontManager>.Instance.LoadLanguages();
		DetachedSingleton<SceneFlowManager>.Instance.LoadConnectSceneDirect();
	}

	private void OnDestroy()
	{
		LoadingLogo.UnloadTexture();
	}
}
