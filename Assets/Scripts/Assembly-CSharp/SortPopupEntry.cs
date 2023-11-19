using UnityEngine;

public class SortPopupEntry : MonoBehaviour
{
	public UILabel NumberLabel;

	public UILabel NameLabel;

	public GameObject DirectionGroup;

	public UILabel DirectionLabel;

	public GameObject DeleteButton;

	public SortEntry Entry { get; set; }

	public void OnClickName()
	{
		Singleton<SortPopupController>.Instance.OnClickName(this);
	}

	public void OnClickDirection()
	{
		Singleton<SortPopupController>.Instance.OnClickDirection(this);
	}

	public void OnClickDelete()
	{
		Singleton<SortPopupController>.Instance.OnClickDelete(this);
	}

	public void Populate(SortEntry sortEntry, int index, bool showDelete)
	{
		Entry = sortEntry;
		DeleteButton.SetActive(showDelete);
		if (sortEntry != null)
		{
			NameLabel.text = sortEntry.GetName();
			NumberLabel.text = (index + 1).ToString();
			DirectionGroup.SetActive(true);
			DirectionLabel.text = sortEntry.GetDirectionLabel();
		}
		else
		{
			NameLabel.text = KFFLocalization.Get("!!ADD_ANOTHER_SORT");
			NumberLabel.text = string.Empty;
			DirectionGroup.SetActive(false);
		}
	}
}
