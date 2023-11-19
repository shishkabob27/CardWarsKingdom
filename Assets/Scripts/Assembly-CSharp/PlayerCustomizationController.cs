using UnityEngine;

public class PlayerCustomizationController : Singleton<PlayerCustomizationController>
{
	public GameObject PortraitListPrefab;
	public GameObject CardBackListPrefab;
	public GameObject HeroSkinListPrefab;
	public UITweenController ShowTween;
	public GameObject[] PageParents;
	public GameObject[] PageButtons;
	public UIGrid ButtonsGrid;
	public UIScrollView PortraitScrollView;
	public UIStreamingGrid PortraitGrid;
	public PortraitListEntry SelectedPortraitPrefab;
	public UILabel SelectedPortraitName;
	public UIScrollView CardBackScrollView;
	public UIStreamingGrid CardBackGrid;
	public CardBackListEntry SelectedCardBackPrefab;
	public UILabel SelectedCardBackName;
	public UIScrollView HeroSkinScrollView;
	public UIStreamingGrid HeroSkinGrid;
	public StoreHeroPrefab SelectedHeroSkinPrefab;
	public UILabel SelectedHeroSkinName;
	public UILabel SkinCustCurrency;
}
