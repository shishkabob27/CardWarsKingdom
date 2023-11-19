public class GachaMultiResultController : Singleton<GachaMultiResultController>
{
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowBlackFadeTween;
	public UITweenController SummaryFrameShowTween;
	public UITweenController SummaryFrameHideTween;
	public UIGrid DropsGrid;
	public UIWidget DropGridSizeKey;
	public UILabel SummaryLabel;
	public bool FastForward;
	public bool canClose;
	public UIButton CollectAllButton;
	public UILabel CollectAllLabel;
	public float LootRevealInterval;
}
