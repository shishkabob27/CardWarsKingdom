using UnityEngine;

public class PauseController : Singleton<PauseController>
{
	public UITweenController ShowPauseTween;
	public UITweenController HidePauseTween;
	public UITweenController ShowPauseButtonTween;
	public UITweenController HidePauseButtonTween;
	public GameObject PauseButton;
	public GameObject ResumeButton;
	public GameObject QuitButton;
	public GameObject PauseBackCollider;
	public GameObject PauseButtonParent;
	public GameObject InputBlockCollider;
	public GameObject MusicButtonOff;
	public GameObject MusicButtonOn;
	public GameObject SoundButtonOff;
	public GameObject SoundButtonOn;
}
