using UnityEngine;

public class ExpeditionStartController : Singleton<ExpeditionStartController>
{
	public string[] LootMeterTextures;
	public float NeedleLerpSpeed;
	public float NeedleRandomBounceSpeed;
	public float NeedleRandomBounceAmount;
	public float NeedleRestingAngle;
	public GameObject ExpeditionPrefab;
	public UILabel Name;
	public UILabel Duration;
	public UILabel FavoredClass;
	public UILabel DragCreaturesPromptLabel;
	public UILabel TimeUntilRefreshLabel;
	public UIGrid SlotGrid;
	public UITexture ArtTexture;
	public UIStreamingGrid ItemGrid;
	public Transform SortButton;
	public GameObject StartButton;
	public GameObject SpeedUpButton;
	public GameObject CancelButton;
	public GameObject DragCreaturesPrompt;
	public UILabel ClaimCompleteLabel;
	public GameObject BigCheckmarkIcon;
	public UIStreamingGrid AvailableGrid;
	public Transform LootNeedle;
	public UITexture LootMeterGraphic;
	public GameObject[] LootMeterSegments;
	public GameObject ExpeditionsAvailableParent;
	public GameObject NoExpeditionsAvailableParent;
	public UILabel RepopulateCostLabel;
	public UITexture CreaturesFrame;
	public ParticleSystem LootMeterGraphicChangeVFX;
	public GameObject ItemTileDropVFX;
	public InventoryBarController InventoryBar;
	public UITexture frame;
	[SerializeField]
	private UIGrid _ExpeditionRewardsPopUpGrid;
	[SerializeField]
	private UIScrollView _ExpeditionRewardsPopUpScrollView;
	[SerializeField]
	private ExpeditionRewardsMenuItem _ExpeditionRewardsMenuItemPrefab;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowRewardsPopupTween;
	public UITweenController HideRewardsPopupTween;
	public UITweenController ExpeditionCompleteTween;
	[SerializeField]
	private UISprite _TitleBarOutline;
	[SerializeField]
	private UISprite _TitleBarFill;
	[SerializeField]
	private UISprite _BgRect;
	[SerializeField]
	private UISprite _MiddleBarOutline;
	[SerializeField]
	private UISprite _MiddleBarFill;
	[SerializeField]
	private UITexture _BottomChiseledField;
	[SerializeField]
	private UISprite _LeftArrow;
	[SerializeField]
	private UISprite _RightArrow;
	[SerializeField]
	private UISprite _FactionIcon;
	[SerializeField]
	private TweenColor _DragPromptTweenColor;
	public ExpeditionsColorPalette[] ColorPalette;
	public ExpeditionsColorPalette[] DifficultyPalette;
}
