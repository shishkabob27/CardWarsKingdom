using DarkTonic.MasterAudio;
using UnityEngine;

public class UISpawnFXOnClick : MonoBehaviour
{
	public GameObject fxPrefab;

	public Transform parentTr;

	public bool parentFlag;

	public bool audioFadeFlag;

	public string clipName;

	public bool ShouldMatchLayerToParent;

	public float DelayToSpawn;

	private GameObject mSpawnedObject;

	private void OnClick()
	{
		Trigger();
	}

	public void Trigger()
	{
		Invoke("SpawnEffect", DelayToSpawn);
	}

	private void SpawnEffect()
	{
		Vector3 position = Vector3.zero;
		if (parentTr != null)
		{
			position = parentTr.position;
		}
		if (parentFlag && parentTr != null)
		{
			mSpawnedObject = parentTr.InstantiateAsChild(fxPrefab);
			if (ShouldMatchLayerToParent)
			{
				mSpawnedObject.ChangeLayer(parentTr.gameObject.layer);
			}
		}
		else
		{
			mSpawnedObject = SLOTGame.InstantiateFX(fxPrefab, position, fxPrefab.transform.rotation) as GameObject;
		}
	}

	public void CleanUp()
	{
		if (mSpawnedObject != null)
		{
			if (audioFadeFlag && !string.IsNullOrEmpty(clipName))
			{
				MasterAudio.FadeOutAllOfSound(clipName, 0.5f);
			}
			NGUITools.Destroy(mSpawnedObject);
		}
		mSpawnedObject = null;
	}
}
