using UnityEngine;

public class AppSignCheckTest : MonoBehaviour
{
	private void Start()
	{
		if (AppSignCheck.GetResult() == 0)
		{
		}
	}

	private void OnCloseAppSignErrorPopup()
	{
	}
}
