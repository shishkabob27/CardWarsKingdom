using UnityEngine;

public class UIKeyNavigation : MonoBehaviour
{
	public enum Constraint
	{
		None = 0,
		Vertical = 1,
		Horizontal = 2,
		Explicit = 3,
	}

	public Constraint constraint;
	public GameObject onUp;
	public GameObject onDown;
	public GameObject onLeft;
	public GameObject onRight;
	public GameObject onClick;
	public bool startsSelected;
}
