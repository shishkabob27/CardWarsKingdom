using System.Collections;
using UnityEngine;

public class SceneObjectsEnabler : MonoBehaviour
{
	private static SceneObjectsEnabler g_ObjectEnabler;

	public Transform[] AllParentTransforms;

	public MonoBehaviour[] AllMono;

	private void Awake()
	{
		g_ObjectEnabler = this;
	}

	public static SceneObjectsEnabler GetInstance()
	{
		return g_ObjectEnabler;
	}

	private void Start()
	{
		UICamera.LockInput();
		StartCoroutine(EnableAll());
	}

	public IEnumerator EnableAll()
	{
		LoadingScreenController loadingCtrlr = LoadingScreenController.GetInstance();
		int count = 0;
		Transform[] allParentTransforms = AllParentTransforms;
		foreach (Transform tr in allParentTransforms)
		{
			if (!(tr != null))
			{
				continue;
			}
			tr.gameObject.SetActive(true);
			yield return null;
			count++;
			float rate = (float)count / (float)AllParentTransforms.Length;
			if (loadingCtrlr != null)
			{
				if (loadingCtrlr.progressBar != null)
				{
					loadingCtrlr.progressBar.fillAmount = (rate * 80f + 20f) / 100f;
				}
				if (loadingCtrlr.progressPercent != null)
				{
					loadingCtrlr.progressPercent.text = (int)(rate * 80f) + 20 + "%";
				}
			}
		}
		yield return null;
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.FrontEnd && loadingCtrlr != null)
		{
			loadingCtrlr.HideLoading();
		}
		UICamera.UnlockInput();
	}

	private void Update()
	{
	}
}
