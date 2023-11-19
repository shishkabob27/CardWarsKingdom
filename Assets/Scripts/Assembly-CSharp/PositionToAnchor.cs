using UnityEngine;

public class PositionToAnchor : MonoBehaviour
{
	[SerializeField]
	private bool _ShouldAffectX;

	[SerializeField]
	private bool _ShouldAffectY;

	[SerializeField]
	private bool _ShouldAffectZ;

	[SerializeField]
	private float _Ratio = 1f;

	[SerializeField]
	private Vector3 _Offset = Vector3.zero;

	[SerializeField]
	private Transform _AnchoredObject;

	private void Update()
	{
		if (_AnchoredObject != null)
		{
			float x = ((!_ShouldAffectX) ? base.transform.position.x : (_AnchoredObject.position.x * _Ratio + _Offset.x));
			float y = ((!_ShouldAffectY) ? base.transform.position.y : (_AnchoredObject.position.y * _Ratio + _Offset.y));
			float z = ((!_ShouldAffectZ) ? base.transform.position.z : (_AnchoredObject.position.z * _Ratio + _Offset.z));
			base.transform.position = new Vector3(x, y, z);
		}
	}
}
