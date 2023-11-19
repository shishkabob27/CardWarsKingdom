using UnityEngine;

public class CreatureInfoEffectPrefabScript : MonoBehaviour
{
	public int id;

	public void OnEffectPressed()
	{
		if (Singleton<CreatureInfoPopup>.Instance.mShowingId == id)
		{
			Singleton<CreatureInfoPopup>.Instance.OnEffectReleased();
			return;
		}
		Singleton<CreatureInfoPopup>.Instance.mShowingId = id;
		Singleton<CreatureInfoPopup>.Instance.OnEffectPressed();
	}

	public void OnEffectReleased()
	{
	}
}
