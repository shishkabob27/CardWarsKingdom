using UnityEngine;

public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2,
		PressAndHold = 3,
	}

	public Restriction restriction;
	public bool cloneOnDrag;
	public float pressAndHoldDelay;
}
