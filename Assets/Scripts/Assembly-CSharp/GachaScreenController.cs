using UnityEngine;

public class GachaScreenController : Singleton<GachaScreenController>
{
	public int TestRolls;
	public float CreatureWaitTime;
	public float RotatingCardScale;
	public float RotatingCardOffset;
	public float InitialKeyMoveSpeed;
	public float InitialKeyRotateSpeed;
	public float KeyReturnSpeed;
	public float KeyLockDragDistance;
	public UITweenController EventActiveTween;
	public UITweenController[] ShowGachaMainTweens;
	public UITweenController HideGachaMainTween;
	public UITweenController RotateCreaturesTween;
	public UITweenController FadeCreatureStatsTween;
	public UITweenController ShowFeaturedCreatureTween;
	public UITweenController HideFeaturedCreatureTween;
	public UITweenController PulseStatsAndTitleTween;
	public UITweenController ShowDragKeyTween;
	public UITweenController ArrowLoopTween;
	public UITweenController HideDragKeyTween;
	public UITweenController DisableDragKeyTween;
	public UILabel GoldAvailable;
	public UILabel SoftCurrencyAvailable;
	public UILabel HardCurrencyAvailable;
	public UILabel KeysAvailable;
	public UITexture KeysAvailableTexture;
	public GameObject KeysAvailableParent;
	public UILabel CostLabel;
	public UILabel OpenLabel;
	public UISprite CostIcon;
	public UITexture CostTexture;
	public GameObject CostParent;
	public UILabel ChestTimer;
	public Transform ChestNode;
	public GameObject MultiPullParent;
	public UILabel MultiPullCostLabel;
	public UILabel MultiPullOpenLabel;
	public UISprite MultiPullCostIcon;
	public UITexture MultiPullCostTexture;
	public Transform MultiPullChestNode;
	public GameObject MultiPullBonusParent;
	public UILabel MultiPullBonusCostLabel;
	public UILabel MultiPullBonusOpenLabel;
	public UISprite MultiPullBonusCostIcon;
	public UITexture MultiPullBonusCostTexture;
	public Transform MultiPullBonusChestNode;
	public GameObject EventParent;
	public UILabel EventName;
	public UILabel EventDesc;
	public UILabel EventTimer;
	public UILabel GachaTypeLabel;
	public UILabel TypeDescription;
	public UILabel EggCountStackLabel;
	public UILabel BonusEggCountStackLabel;
	public UIGrid ButtonGrid;
	public UILabel NextSlotName;
	public UILabel PrevSlotName;
	public UISprite NextSlotChestIcon;
	public UISprite PrevSlotChestIcon;
	public Transform[] RotatingCreatureNodes;
	public Camera CreatureCamera;
	public GameObject RotatingStatsContentsParent;
	public CreatureStatsPanel[] RotatingStatsTypes;
	public Transform FeaturedCreatureNode;
	public CreatureStatsPanel FeaturedCreatureStats;
	public GachaDragKey KeyDragObject;
	public Transform KeyParentObject;
	public Transform KeyEndPosition;
	public Transform Panel3D;
	public GameObject rareFXParent;
	public GameObject algebraicFXParent;
	public string DailyChestID;
}
