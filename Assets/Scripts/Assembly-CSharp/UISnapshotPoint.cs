using UnityEngine;

[AddComponentMenu("NGUI/Internal/Snapshot Point")]
[ExecuteInEditMode]
public class UISnapshotPoint : MonoBehaviour
{
	public bool isOrthographic = true;

	public float nearClip = -100f;

	public float farClip = 100f;

	[Range(10f, 80f)]
	public int fieldOfView = 35;

	public float orthoSize = 30f;

	private void Start()
	{
		if (base.tag != "EditorOnly")
		{
			base.tag = "EditorOnly";
		}
	}
}
