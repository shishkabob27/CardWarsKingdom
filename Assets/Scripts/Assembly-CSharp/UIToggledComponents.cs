using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIToggle))]
[AddComponentMenu("NGUI/Interaction/Toggled Components")]
[ExecuteInEditMode]
public class UIToggledComponents : MonoBehaviour
{
	public List<MonoBehaviour> activate;

	public List<MonoBehaviour> deactivate;

	[HideInInspector]
	[SerializeField]
	private MonoBehaviour target;

	[HideInInspector]
	[SerializeField]
	private bool inverse;

	private void Awake()
	{
		if (target != null)
		{
			if (activate.Count == 0 && deactivate.Count == 0)
			{
				if (inverse)
				{
					deactivate.Add(target);
				}
				else
				{
					activate.Add(target);
				}
			}
			else
			{
				target = null;
			}
		}
		UIToggle component = GetComponent<UIToggle>();
		EventDelegate.Add(component.onChange, Toggle);
	}

	public void Toggle()
	{
		if (base.enabled)
		{
			for (int i = 0; i < activate.Count; i++)
			{
				MonoBehaviour monoBehaviour = activate[i];
				monoBehaviour.enabled = UIToggle.current.value;
			}
			for (int j = 0; j < deactivate.Count; j++)
			{
				MonoBehaviour monoBehaviour2 = deactivate[j];
				monoBehaviour2.enabled = !UIToggle.current.value;
			}
		}
	}
}
