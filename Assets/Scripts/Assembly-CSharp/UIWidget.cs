using UnityEngine;

public class UIWidget : UIRect
{
	public enum Pivot
	{
		TopLeft = 0,
		Top = 1,
		TopRight = 2,
		Left = 3,
		Center = 4,
		Right = 5,
		BottomLeft = 6,
		Bottom = 7,
		BottomRight = 8,
	}

	public enum AspectRatioSource
	{
		Free = 0,
		BasedOnWidth = 1,
		BasedOnHeight = 2,
	}

	[SerializeField]
	protected Color mColor;
	[SerializeField]
	protected Pivot mPivot;
	[SerializeField]
	protected int mWidth;
	[SerializeField]
	protected int mHeight;
	[SerializeField]
	protected int mDepth;
	public bool autoResizeBoxCollider;
	public bool hideIfOffScreen;
	public AspectRatioSource keepAspectRatio;
	public float aspectRatio;
}
