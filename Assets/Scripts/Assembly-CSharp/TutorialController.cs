using UnityEngine;

public class TutorialController : Singleton<TutorialController>
{
	public int PopupTextPadding;
	public float PopupArrowSpacing;
	public float PopupSpacingCornerFactor;
	public float WidthRatio;
	public int MinimumWidth;
	public float GeneralDragDistance;
	public Vector2 CharacterTextureOffset;
	public float BoardYPosAdjust;
	public UITweenController ShowPopupTween;
	public UITweenController HidePopupTween;
	public UITweenController ShowArrowTween;
	public UITweenController HideArrowTween;
	public UITweenController ShowDragTween;
	public UITweenController ShowDragFadeTween;
	public UITweenController HideDragTween;
	public Transform PopupObject;
	public UILabel PopupText;
	public UILabel PopupTextTitle;
	public UILabel NameLabel;
	public UISprite NameBackground;
	public GameObject NextArrow;
	public UISprite PopupBackground;
	public Transform PointerObject;
	public GameObject ArrowPointer;
	public GameObject TapPointer;
	public GameObject FullScreenCollider;
	public Transform DragArrow;
	public UITexture CharacterTexture;
	public GameObject TapIndicator;
	public bool manulSetAutoFillMeterBar;
	[SerializeField]
	private UISprite _HexBgSprite;
	[SerializeField]
	private Color[] _BackgroundColors;
}
