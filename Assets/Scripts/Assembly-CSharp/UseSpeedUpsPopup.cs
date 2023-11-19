using System;
using UnityEngine;

public class UseSpeedUpsPopup : Singleton<UseSpeedUpsPopup>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public GameObject LinePrefab;

	public UIGrid LinesGrid;

	private Action<SpeedUpData> mCallback;

	private SpeedUpData mSelectedSpeedup;

	public void Show(Action<SpeedUpData> callback)
	{
		ShowTween.Play();
		mCallback = callback;
		mSelectedSpeedup = null;
		foreach (SpeedUpData item in SpeedUpDataManager.Instance.GetDatabase())
		{
			SpeedUpPrefab component = LinesGrid.transform.InstantiateAsChild(LinePrefab).GetComponent<SpeedUpPrefab>();
			component.gameObject.SetActive(true);
			component.Populate(item);
		}
		LinesGrid.Reposition();
	}

	public void Unload()
	{
		LinesGrid.transform.DestroyAllChildren();
		mCallback(mSelectedSpeedup);
	}

	public void ApplySpeedup(SpeedUpData data)
	{
		mSelectedSpeedup = data;
		HideTween.Play();
	}
}
