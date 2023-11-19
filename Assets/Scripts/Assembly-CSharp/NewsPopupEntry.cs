using UnityEngine;

public class NewsPopupEntry : MonoBehaviour
{
	public UILabel Header;

	public UILabel Body;

	public GameObject Separator;

	public float SeparatorSpacing;

	public void Populate(MailData data, bool lastEntry)
	{
		Header.text = data.Title;
		Body.text = data.BodyText;
		if (lastEntry)
		{
			Separator.SetActive(false);
			return;
		}
		Separator.SetActive(true);
		Vector3 localPosition = Separator.transform.localPosition;
		localPosition.y = Body.transform.localPosition.y - (float)Body.height - SeparatorSpacing;
		Separator.transform.localPosition = localPosition;
	}

	public float GetTotalHeight()
	{
		return 0f - Body.transform.localPosition.y + (float)Body.height + SeparatorSpacing * 2f;
	}
}
