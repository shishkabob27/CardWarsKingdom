using UnityEngine;

public class ScreenFadeController : Singleton<ScreenFadeController>
{
	public UITweenController ShowLoadScreenTween;

	public UITweenController HideLoadScreenTween;

	public GameObject Panel;

	public UITexture LoadingLogo;

	private void Start()
	{
		HideLoadScreen();
	}

	public void HideLoadScreen()
	{
		HideLoadScreenTween.Play();
	}

	public void ShowLoadScreen()
	{
		ShowLoadScreenTween.Play();
		LoadingScreenController.LoadLoadingScreenLogo(LoadingLogo);
	}

	public void ShowLoadScreen(EventDelegate.Callback onFinished = null)
	{
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		if (onFinished != null)
		{
			ShowLoadScreenTween.PlayWithCallback(onFinished);
			LoadingScreenController.LoadLoadingScreenLogo(LoadingLogo);
		}
	}

	private void OnDestroy()
	{
		LoadingLogo.UnloadTexture();
	}
}
