using UnityEngine;

public class ToggleDimmer : MonoBehaviour
{
	[SerializeField]
	private UIWidget[] _Widgets = new UIWidget[0];

	[SerializeField]
	private Color _ActiveColor = Color.white;

	[SerializeField]
	private Color _InactiveColor = Color.grey;

	private void OnEnable()
	{
		UIWidget[] widgets = _Widgets;
		foreach (UIWidget uIWidget in widgets)
		{
			if (uIWidget != null)
			{
				uIWidget.color = _ActiveColor;
			}
		}
	}

	private void OnDisable()
	{
		UIWidget[] widgets = _Widgets;
		foreach (UIWidget uIWidget in widgets)
		{
			if (uIWidget != null)
			{
				uIWidget.color = _InactiveColor;
			}
		}
	}
}
