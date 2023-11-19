using UnityEngine;

public class StatsDescriptionPopup : Singleton<StatsDescriptionPopup>
{
	[SerializeField]
	private Transform _Module;
	[SerializeField]
	private GameObject _Panel;
	[SerializeField]
	private UITweenController _ShowTween;
	[SerializeField]
	private UITweenController _HideTween;
	public UILabel ContentLabel;
}
