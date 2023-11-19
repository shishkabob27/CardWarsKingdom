using UnityEngine;

public class PanelStateNotifier : MonoBehaviour
{
	private void OnEnable()
	{
		SendMessageUpwards("OnPanelEnable", this, SendMessageOptions.DontRequireReceiver);
	}

	private void OnDisable()
	{
		SendMessageUpwards("OnPanelDisable", this, SendMessageOptions.DontRequireReceiver);
	}
}
