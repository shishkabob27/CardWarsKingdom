using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	private Transform mCamera;

	private void Awake()
	{
		mCamera = GameObject.Find("3DFrontEnd").transform;
		TextMesh component = GetComponent<TextMesh>();
		if (component != null)
		{
			component.text = KFFLocalization.Get(component.text);
		}
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(mCamera.transform.forward);
	}
}
