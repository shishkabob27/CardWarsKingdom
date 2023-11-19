using UnityEngine;

public class HelpButton : MonoBehaviour
{
	private void OnClick()
	{
		Singleton<HelpShiftManager>.Instance.ShowConversation(HelpshiftConfig.Instance.getApiConfig());
	}
}
