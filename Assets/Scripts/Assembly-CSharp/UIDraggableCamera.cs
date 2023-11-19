using UnityEngine;

public class UIDraggableCamera : MonoBehaviour
{
	public Transform rootForBounds;
	public Vector2 scale;
	public float scrollWheelFactor;
	public UIDragObject.DragEffect dragEffect;
	public bool smoothDragStart;
	public float momentumAmount;
}
