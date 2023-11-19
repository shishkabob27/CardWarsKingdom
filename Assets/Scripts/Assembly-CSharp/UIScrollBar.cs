using UnityEngine;

public class UIScrollBar : UISlider
{
	private enum Direction
	{
		Horizontal = 0,
		Vertical = 1,
		Upgraded = 2,
	}

	[SerializeField]
	protected float mSize;
	[SerializeField]
	private float mScroll;
	[SerializeField]
	private Direction mDir;
}
