using UnityEngine;

public class ConnectRetryController : MonoBehaviour
{
	public ConnectScreenController controller;

	public void OnClickRetry()
	{
		controller.PreGuestLogin();
	}
}
