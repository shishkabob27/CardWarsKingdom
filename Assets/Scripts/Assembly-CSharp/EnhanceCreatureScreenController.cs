using UnityEngine;

public class EnhanceCreatureScreenController : Singleton<EnhanceCreatureScreenController>
{
	public GameObject EnhanceFX;
	public GameObject StarUpFX;
	public UITweenController ShowTween;
	public UITweenController HideMainTween;
	public UITweenController ShowSlotsTween;
	public UITweenController HideSlotsTween;
	public UITweenController ShowAfterEnhanceTween;
	public UITweenController HideForEnhanceTween;
	public UITweenController CantAffordTween;
	public UITweenController ShowPowerUpGroup;
	public UITweenController HidePowerUpGroup;
	public UITweenController ShowGuages;
	public UITweenController HideGuages;
	public UITweenController MovePlatformToCenterTween;
	public UITweenController DropPlatformTween;
	public UITweenController PopCreatureTween;
	public UITweenController MovePlatformBackTween;
	public UITweenController ShakeStartLoopTween;
	public UITweenController ShakeStopTween;
	public UIStreamingGrid CreatureGrid;
	public BoxCollider PowerUpSlot;
	public GameObject PowerUpSlotSprite;
	public GameObject CreatureInfoGroup;
	public CreatureStatsPanel StatsPanel;
	public UILabel SoftCurrencyLabel;
	public UILabel HardCurrencyLabel;
	public UILabel CostLabel;
	public GameObject PowerUpGroup;
	public GameObject GuagesGroup;
	public Transform SortButton;
	public Transform[] RecipeNodes;
	public GameObject[] SlotHighlights;
	public GameObject[] PlatformLights;
	public Material FlashMaterial;
	public GameObject upgradeGlow;
	public LaboratorySequence RevealSequenceScript;
	public Camera CreatureCamera_Masked;
	public Camera CreatureCamera_Full;
	public InventoryBarController inventoryBar;
	public Transform CreatureSpawnPoint;
}
