using System.Collections;
using UnityEngine;

public class DestroyAndReload : MonoBehaviour
{
	public float reloadDelaySecs;

	private void Start()
	{
		StartCoroutine(ReloadCo());
	}

	private IEnumerator ReloadCo()
	{
		while (Singleton<SLOTResourceManager>.Instance.ResourceLoadInProgress)
		{
			yield return null;
		}
		if (Singleton<LoadingManager>.Instance != null)
		{
			Singleton<LoadingManager>.Instance.UnloadAll();
		}
		SessionManager sessionMan = SessionManager.Instance;
		if (sessionMan != null)
		{
			sessionMan.Destroy();
		}
		SQUtils.FlushCache();
		object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		object[] array = allObjects;
		foreach (object obj in array)
		{
			if (base.gameObject != obj)
			{
				GameObject gameObject = obj as GameObject;
				if (!(gameObject == null) && !(gameObject.name == "UnityFacebookSDKPlugin") && !(gameObject.name == "EveryplayController") && !(gameObject.name == "Chartboost") && !(gameObject.name == "UpsightManager") && !(gameObject.name == "prime[31]") && (!(gameObject.transform.parent != null) || !(gameObject.transform.parent.name == "prime[31]")) && gameObject.activeInHierarchy)
				{
					Object.Destroy(gameObject);
				}
			}
		}
		yield return null;
		Application.LoadLevel(0);
	}
}
