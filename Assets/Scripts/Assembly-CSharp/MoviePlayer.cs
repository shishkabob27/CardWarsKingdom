using System.Collections;
using UnityEngine;

public class MoviePlayer : MonoBehaviour
{
	public string[] Videos;

	public UITexture Background;

	public UITexture LoadingLogo;

	public string HiResBackground;

	public string LowResBackground;

	public static bool NeedToPlayMovies = true;

	private void Start()
	{
		if (KFFLODManager.IsLowEndDevice())
		{
			Background.mainTexture = Resources.Load<Texture>(LowResBackground);
		}
		else
		{
			Background.mainTexture = Resources.Load<Texture>(HiResBackground);
		}
		string text = KFFLocalization.Get("!!LOADING_LOGO");
		if (KFFLODManager.IsLowEndDevice())
		{
			int num = text.LastIndexOf("/");
			if (num != -1)
			{
				text = text.Insert(num, "low_");
			}
		}
		LoadingLogo.mainTexture = Resources.Load<Texture>(text);
		StartCoroutine(VideoCo());
	}

	private IEnumerator VideoCo()
	{
		Background.color = Color.black;
		LoadingLogo.color = Color.black;
		yield return null;
		for (int i = 0; i < Videos.Length; i++)
		{
#if UNITY_ANDROID || UNITY_IOS
			if (!Handheld.PlayFullScreenMovie(Videos[i], Color.black, FullScreenMovieControlMode.CancelOnInput))
			{
			}
#endif
			Background.color = Color.black;
			LoadingLogo.color = Color.black;
			yield return null;
			yield return null;
			yield return null;
		}
		Background.color = Color.white;
		LoadingLogo.color = Color.white;
		yield return null;
		yield return null;
		yield return null;
		NeedToPlayMovies = false;
		DetachedSingleton<SceneFlowManager>.Instance.LoadVersionCheckSceneDirect();
	}
}
