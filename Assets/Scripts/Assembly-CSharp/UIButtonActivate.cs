using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate : MonoBehaviour
{
	public GameObject target;

	public bool state = true;

	private void OnClick()
	{
		Trigger();
	}

	public void Trigger()
	{
		if (target != null)
		{
			NGUITools.SetActive(target, state);
		}
	}
}
