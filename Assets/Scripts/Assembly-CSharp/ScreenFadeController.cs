using UnityEngine;

public class ScreenFadeController : Singleton<ScreenFadeController>
{
	public UITweenController ShowLoadScreenTween;
	public UITweenController HideLoadScreenTween;
	public GameObject Panel;
	public UITexture LoadingLogo;
}
