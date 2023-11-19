using UnityEngine;

public class DebugMenuController : Singleton<DebugMenuController>
{
	private void Awake()
	{
		Object.Destroy(GameObject.Find("DebugMenuButton"));
	}

	public void OnDebugButtonClicked()
	{
	}

	public void OnDebugMenuEntryClicked()
	{
	}
}
