using UnityEngine;

public class UIBasicSprite : UIWidget
{
	public enum Type
	{
		Simple = 0,
		Sliced = 1,
		Tiled = 2,
		Filled = 3,
		Advanced = 4,
	}

	public enum FillDirection
	{
		Horizontal = 0,
		Vertical = 1,
		Radial90 = 2,
		Radial180 = 3,
		Radial360 = 4,
	}

	public enum Flip
	{
		Nothing = 0,
		Horizontally = 1,
		Vertically = 2,
		Both = 3,
	}

	public enum AdvancedType
	{
		Invisible = 0,
		Sliced = 1,
		Tiled = 2,
	}

	[SerializeField]
	protected Type mType;
	[SerializeField]
	protected FillDirection mFillDirection;
	[SerializeField]
	protected float mFillAmount;
	[SerializeField]
	protected bool mInvert;
	[SerializeField]
	protected Flip mFlip;
	public AdvancedType centerType;
	public AdvancedType leftType;
	public AdvancedType rightType;
	public AdvancedType bottomType;
	public AdvancedType topType;
}
