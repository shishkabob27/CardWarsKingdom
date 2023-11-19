using UnityEngine;

public class UIKeyBinding : MonoBehaviour
{
	public enum Modifier
	{
		None = 0,
		Shift = 1,
		Control = 2,
		Alt = 3,
	}

	public enum Action
	{
		PressAndClick = 0,
		Select = 1,
	}

	public KeyCode keyCode;
	public Modifier modifier;
	public Action action;
}
