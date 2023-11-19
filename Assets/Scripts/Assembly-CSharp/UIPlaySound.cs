using UnityEngine;

public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick = 0,
		OnMouseOver = 1,
		OnMouseOut = 2,
		OnPress = 3,
		OnRelease = 4,
		Custom = 5,
	}

	public AudioClip audioClip;
	public Trigger trigger;
	public float delay;
	public float volume;
	public float pitch;
}
