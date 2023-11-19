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
	private float _Ratio;
	[SerializeField]
	private Vector3 _Offset;
	[SerializeField]
	private Transform _AnchoredObject;
}
