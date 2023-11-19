using UnityEngine;

public class UIScrollView : MonoBehaviour
{
	public enum Movement
	{
		Horizontal = 0,
		Vertical = 1,
		Unrestricted = 2,
		Custom = 3,
	}

	public enum DragEffect
	{
		None = 0,
		Momentum = 1,
		MomentumAndSpring = 2,
	}

	public enum ShowCondition
	{
		Always = 0,
		OnlyIfNeeded = 1,
		WhenDragging = 2,
	}

	public Movement movement;
	public DragEffect dragEffect;
	public bool restrictWithinPanel;
	public bool disableDragIfFits;
	public bool smoothDragStart;
	public bool iOSDragEmulation;
	public float scrollWheelFactor;
	public float momentumAmount;
	public UIProgressBar horizontalScrollBar;
	public UIProgressBar verticalScrollBar;
	public ShowCondition showScrollBars;
	public Vector2 customMovement;
	public UIWidget.Pivot contentPivot;
	[SerializeField]
	private Vector3 scale;
	[SerializeField]
	private Vector2 relativePositionOnReset;
}
