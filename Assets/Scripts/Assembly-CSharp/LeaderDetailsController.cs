using UnityEngine;

public class LeaderDetailsController : Singleton<LeaderDetailsController>
{
	public UITweenController ShowTween;
	public UITexture Portrait;
	public Transform CardsParent;
	public UILabel Name;
	public UILabel Age;
	public UILabel Height;
	public UILabel Weight;
	public UILabel Species;
	public UILabel Quote;
	public Transform ZoomPosition;
	public Collider ZoomCollider;
	public UILabel BuyCost;
	public GameObject BuyButton;
	public UIGrid ComboPipsParent;
	public GameObject LockedObject;
	public UILabel HardCurrencyLabel;
}
