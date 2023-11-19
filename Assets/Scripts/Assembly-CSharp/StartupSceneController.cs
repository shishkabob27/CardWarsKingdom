using System.Collections;
using UnityEngine;

public class StartupSceneController : MonoBehaviour
{
	private const float forceLoadLimit = 3f;

	public AndroidPermissions androidPermissionScreen;

	private float timer;

	private bool attemptedToLoadScene;

	private bool isVideoSceneLoaded;

	private void Start()
	{
		QualitySettings.antiAliasing = 0;
		StartCoroutine(DelayedAndroidPermissionRequest());
	}

	private IEnumerator DelayedAndroidPermissionRequest()
	{
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(androidPermissionScreen.StartAndroidPermissionsRoutine());
	}
}
