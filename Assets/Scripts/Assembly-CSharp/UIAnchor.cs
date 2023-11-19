using UnityEngine;

public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft = 0,
		Left = 1,
		TopLeft = 2,
		Top = 3,
		TopRight = 4,
		Right = 5,
		BottomRight = 6,
		Bottom = 7,
		Center = 8,
	}

	public Camera uiCamera;
	public GameObject container;
	public Side side;
	public bool runOnlyOnce;
	public Vector2 relativeOffset;
	public Vector2 pixelOffset;
	[SerializeField]
	private UIWidget widgetContainer;
}
