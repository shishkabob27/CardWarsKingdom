using UnityEngine;

public class BackButtonTarget : MonoBehaviour
{
	private void OnClick()
	{
		MenuStackManager.OnBackButtonTargetClicked(this);
		Object.Destroy(this);
	}
}
