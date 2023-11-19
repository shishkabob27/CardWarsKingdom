using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
[AddComponentMenu("NGUI/Interaction/Language Selection")]
public class LanguageSelection : MonoBehaviour
{
	private UIPopupList mList;

	private void Start()
	{
		mList = GetComponent<UIPopupList>();
		if (Localization.knownLanguages != null)
		{
			mList.items.Clear();
			int i = 0;
			for (int num = Localization.knownLanguages.Length; i < num; i++)
			{
				mList.items.Add(Localization.knownLanguages[i]);
			}
			mList.value = Localization.language;
		}
		EventDelegate.Add(mList.onChange, OnChange);
	}

	private void OnChange()
	{
		Localization.language = UIPopupList.current.value;
	}
}
