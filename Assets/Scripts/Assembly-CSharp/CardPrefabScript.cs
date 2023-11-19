using UnityEngine;

public class CardPrefabScript : UIStreamingGridListItem
{
	public enum CardMode
	{
		Hand = 0,
		GeneralFrontEnd = 1,
		BattlePopup = 2,
		Opponent = 3,
		Loot = 4,
	}

	public enum HandCardState
	{
		FastDrawing = 0,
		FailedDrawing = 1,
		InHand = 2,
		ZoomingOutToGrid = 3,
		ZoomedIn = 4,
		DraggingInHand = 5,
		DraggingOnBoard = 6,
		Discarding = 7,
		ManualDiscarding = 8,
		Playing = 9,
		OpponentPlaying = 10,
		CreatureDeployAnim = 11,
		LogZoom = 12,
		ClosingLogZoom = 13,
	}

	public CardMode PrintMode;
	public HandCardState PrintState;
	public float KeywordZoomOffset;
	public float DrawSpinRate;
	public float UnplayableOffset;
	public float DragOffset;
	public UITexture Art;
	public UITexture Frame;
	public UITexture CakeFrame;
	public UISprite CardBackSprite;
	public UILabel Name;
	public UILabel Text;
	public UILabel FlavorText;
	public UILabel Cost;
	public UILabel ActionType;
	public Collider CardCollider;
	public UILabel KeywordsDescription;
	public Collider CardTextCollider;
	public GameObject CanAffordIndicator;
	public Transform PulseTweener;
	public Collider ZoomCollider;
	public UIWidget ZoomColliderWidget;
	public UILabel GroupNameLabel;
	public GameObject HelperLabel;
	public GameObject Blackout;
	public GameObject SelectionVFX;
	public GameObject CostGlowVFX;
	public Vector3 Velocity;
	public int DepthAdjust;
	public GameObject InfoParentObject;
	public UITweenController ShowKeywordsTween;
	public UITweenController HideKeywordsTween;
	public UITweenController ShowBlackoutTween;
	public UITweenController HideBlackoutTween;
	public UITweenController ShowDiscardTween;
	public UITweenController HandFullTween;
	public float TargetSelectionPosOffset;
}
