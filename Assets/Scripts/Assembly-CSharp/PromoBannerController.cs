using UnityEngine;

public class PromoBannerController : MonoBehaviour
{
	private enum TweenDirection
	{
		Left = 0,
		Right = 1,
		Down = 2,
		Up = 3,
	}

	[SerializeField]
	private TweenDirection _TweenDirection;
	[SerializeField]
	private float _SlideTweenSpeed;
	[SerializeField]
	private float _DelayBetweenSlides;
	[SerializeField]
	private float _VerticalSlideOffset;
	[SerializeField]
	private UIPanel _Panel;
	[SerializeField]
	private UIWidget[] _SlideWidgets;
	[SerializeField]
	private UITweenController[] _TweenInSlides;
	[SerializeField]
	private UITweenController[] _TweenOutSlides;
	[SerializeField]
	private PromoBannerItem[] _PromoBannerItemPrefab;
	[SerializeField]
	private bool _ShouldLocalizeText;
}
