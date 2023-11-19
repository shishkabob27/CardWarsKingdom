using UnityEngine;

public class CWHaloFaceCam : MonoBehaviour
{
	private Camera mCamera;

	private void Start()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			mCamera = Singleton<DWGameCamera>.Instance.MainCam;
		}
	}

	private void Update()
	{
		if (mCamera != null)
		{
			base.transform.rotation = Quaternion.LookRotation(mCamera.transform.forward);
		}
		else
		{
			base.transform.rotation = Quaternion.LookRotation(Vector3.forward);
		}
	}
}
