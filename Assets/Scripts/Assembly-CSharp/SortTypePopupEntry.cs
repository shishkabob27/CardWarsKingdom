using UnityEngine;

public class SortTypePopupEntry : MonoBehaviour
{
	public UILabel NameLabel;

	public SortTypeEnum SortType { get; set; }

	public void OnClickName()
	{
		Singleton<SortPopupController>.Instance.OnClickSortType(this);
	}

	public void Populate(SortTypeEnum sortType)
	{
		SortType = sortType;
		NameLabel.text = SortEntry.GetName(sortType);
	}
}
