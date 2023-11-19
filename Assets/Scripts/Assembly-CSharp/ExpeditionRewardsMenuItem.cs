using UnityEngine;

public class ExpeditionRewardsMenuItem : MonoBehaviour
{
	[SerializeField]
	private UILabel _TierLabel;
	[SerializeField]
	private UILabel _SoftCurrencyLabel;
	[SerializeField]
	private UILabel _HardCurrencyLabel;
	[SerializeField]
	private UILabel _RankXPLabel;
	[SerializeField]
	private UISprite _BackgroundSprite;
	[SerializeField]
	private UITexture[] _ShardsTextures;
	[SerializeField]
	private UISprite[] _ShardFrames;
	[SerializeField]
	private UITexture[] _CakeTextures;
	[SerializeField]
	private UISprite[] _CakeFrames;
	[SerializeField]
	private UITexture[] _SpeedUpsTextures;
	[SerializeField]
	private GameObject _SpeedUpsParent;
	[SerializeField]
	private UIGrid _OuterGrid;
	[SerializeField]
	private UIGrid _InnerGrid;
	[SerializeField]
	private AnimationCurve _EaseInOutCurve;
}
