using UnityEngine;

public class GachaDragKey : MonoBehaviour
{
	public bool Pressed { get; private set; }

	private void OnPress(bool pressed)
	{
		Pressed = pressed;
	}
}
