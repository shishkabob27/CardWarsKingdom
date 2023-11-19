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

	public void ShowPanel()
	{
		ShowPanelAtPosition(Vector3.zero);
	}

	public void ShowWeightPanel()
	{
		SwitchToWeight();
		ShowPanelAtPosition(Vector3.zero);
	}

	public void ShowPanelAtTransformPosition(Transform inTransform)
	{
		ShowPanelAtPosition(inTransform.position);
	}

	public void ShowWeightPanelAtTransformPosition(Transform inTransform)
	{
		SwitchToWeight();
		ShowPanelAtPosition(inTransform.position);
	}

	public void ShowPanelAtMousePosition()
	{
		Vector3 inPosition = Singleton<TownController>.Instance.GetUICam().ScreenToWorldPoint(Input.mousePosition);
		inPosition.z = 0f;
		ShowPanelAtPosition(inPosition);
	}

	private void ShowPanelAtPosition(Vector3 inPosition)
	{
		_Module.position = inPosition;
		_Panel.SetActive(true);
		_ShowTween.Play();
	}

	public void HidePanel()
	{
		_HideTween.PlayWithCallback(delegate
		{
			HideComplete();
		});
	}

	public void HideComplete()
	{
		SwitchToStats();
		_Panel.SetActive(false);
	}

	public void SwitchToWeight()
	{
		ContentLabel.text = KFFLocalization.Get("!!WEIGHT_DESCRIPTION");
	}

	public void SwitchToStats()
	{
		ContentLabel.text = KFFLocalization.Get("!!STATS_DESCRIPTION");
	}
}
