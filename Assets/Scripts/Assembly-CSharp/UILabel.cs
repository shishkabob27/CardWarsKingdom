using UnityEngine;

public class UILabel : UIWidget
{
	public enum Crispness
	{
		Never = 0,
		OnDesktop = 1,
		Always = 2,
	}

	public enum Effect
	{
		None = 0,
		Shadow = 1,
		Outline = 2,
	}

	public enum Overflow
	{
		ShrinkContent = 0,
		ClampContent = 1,
		ResizeFreely = 2,
		ResizeHeight = 3,
	}

	public Crispness keepCrispWhenShrunk;
	[SerializeField]
	private Font mTrueTypeFont;
	[SerializeField]
	private UIFont mFont;
	[SerializeField]
	[MultilineAttribute]
	private string mText;
	[SerializeField]
	private int mFontSize;
	[SerializeField]
	private FontStyle mFontStyle;
	[SerializeField]
	private NGUIText.Alignment mAlignment;
	[SerializeField]
	private float mCurveRadius;
	[SerializeField]
	private bool mEncoding;
	[SerializeField]
	private int mMaxLineCount;
	[SerializeField]
	private Effect mEffectStyle;
	[SerializeField]
	private Color mEffectColor;
	[SerializeField]
	private NGUIText.SymbolStyle mSymbols;
	[SerializeField]
	private Vector2 mEffectDistance;
	[SerializeField]
	private Overflow mOverflow;
	[SerializeField]
	private Material mMaterial;
	[SerializeField]
	private bool mApplyGradient;
	[SerializeField]
	private Color mGradientTop;
	[SerializeField]
	private Color mGradientBottom;
	[SerializeField]
	private int mSpacingX;
	[SerializeField]
	private int mSpacingY;
	public LabelShadow LabelShadow;
	[SerializeField]
	private bool mShrinkToFit;
	[SerializeField]
	private int mMaxLineWidth;
	[SerializeField]
	private int mMaxLineHeight;
	[SerializeField]
	private float mLineWidth;
	[SerializeField]
	private bool mMultiline;
}
