using UnityEngine;
using System.Collections.Generic;

public class UIFont : MonoBehaviour
{
	[SerializeField]
	private Material mMat;
	[SerializeField]
	private Rect mUVRect;
	[SerializeField]
	private BMFont mFont;
	[SerializeField]
	private UIAtlas mAtlas;
	[SerializeField]
	private UIFont mReplacement;
	[SerializeField]
	private List<BMSymbol> mSymbols;
	[SerializeField]
	private Font mDynamicFont;
	[SerializeField]
	private int mDynamicFontSize;
	[SerializeField]
	private FontStyle mDynamicFontStyle;
}
