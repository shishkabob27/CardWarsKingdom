using UnityEngine;

public class CreatureInfoPopup : Singleton<CreatureInfoPopup>
{
	public Color StatBuffColor;
	public Color StatDebuffColor;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowEffectTween;
	public UITweenController HideEffectTween;
	public UILabel Name;
	public UILabel Level;
	public UITexture Image;
	public UIGrid RarityStarsParent;
	public UILabel[] Stats;
	public Transform EffectDescPopup;
	public UILabel EffectName;
	public Transform EffectsParent;
	public GameObject EffectIconPrefab;
	public UIGrid EffectIconParentGrid;
	public GameObject StatusEffectDisplay;
	public UILabel Passive;
	public Transform CardsParent;
	public Transform ExCardsParent;
	public Collider ZoomCollider;
	public Transform ZoomPosition;
	public GameObject CycleArrowsGroup;
	public GameObject HelperLabel;
	public UILabel FactionName;
	public UISprite FactionIcon;
	public UILabel DragAttackCost;
	public int mShowingId;
}
