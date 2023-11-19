using UnityEngine;

public class StartUpLogoTweenController : Singleton<StartUpLogoTweenController>
{
	public UITweenController ShowKFFTween;

	public UITweenController SkipAll;

	private void Start()
	{
	}

	public void OnTweenComplete()
	{
		ConnectScreenController connectScreenController = Object.FindObjectOfType<ConnectScreenController>();
		connectScreenController.IsLogoTweenCompleted = true;
	}

	private void Update()
	{
	}
}
