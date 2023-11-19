using UnityEngine;

public class UISlider : UIProgressBar
{
	private enum Direction
	{
		Horizontal = 0,
		Vertical = 1,
		Upgraded = 2,
	}

	[SerializeField]
	private Transform foreground;
	[SerializeField]
	private float rawValue;
	[SerializeField]
	private Direction direction;
	[SerializeField]
	protected bool mInverted;
}
