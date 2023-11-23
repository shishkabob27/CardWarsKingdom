using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionListController : Singleton<MissionListController>
{
	public GameObject MissionGridPrefab;

	public UITweenController ShowTween;

	public UITweenController ShowGlobalTween;

	public UITweenController ShowDailyTween;

	public UIPanel DailyPanel;

	public UIPanel DailyScrollPanel;

	public UIPanel GlobalPanel;

	public UIPanel GlobalScrollPanel;

	public UIStreamingGrid MissionGrid;

	private UIStreamingGridDataSource<Mission> mMissionGridDataSource = new UIStreamingGridDataSource<Mission>();

	public UILabel MissionTimeLeft;

	public GameObject NoMissionsLabel;

	public UIStreamingGrid GlobalMissionGrid;

	private UIStreamingGridDataSource<Mission> mGlobalMissionGridDataSource = new UIStreamingGridDataSource<Mission>();

	public GameObject ShowDailyCollider;

	public GameObject ShowGlobalCollider;

	private bool mShowing;

	private bool mSwappingPanel;

	private int mFrontDepth;

	private int mBackDepth;

	private void Awake()
	{
		mFrontDepth = DailyPanel.depth;
		mBackDepth = GlobalPanel.depth;
		ShowDailyCollider.SetActive(false);
		ShowGlobalCollider.SetActive(true);
	}

	public void Show()
	{
		mShowing = true;
		ShowTween.Play();
		Repopulate();
	}

	private void Update()
	{
		if (mShowing)
		{
			MissionTimeLeft.text = PlayerInfoScript.FormatTimeString((int)DetachedSingleton<MissionManager>.Instance.GetMinutesUntilQuestRefresh() * 60) + " " + KFFLocalization.Get("!!REMAINING");
		}
	}

	public void Repopulate()
	{
		if (mShowing)
		{
			DetachedSingleton<MissionManager>.Instance.SortMissions();
			mMissionGridDataSource.Init(MissionGrid, MissionGridPrefab, DetachedSingleton<MissionManager>.Instance.DailyMissions);
			mGlobalMissionGridDataSource.Init(GlobalMissionGrid, MissionGridPrefab, DetachedSingleton<MissionManager>.Instance.GlobalMissions);
			NoMissionsLabel.SetActive(DetachedSingleton<MissionManager>.Instance.DailyMissions.Count == 0);
		}
	}

	public void Unload()
	{
		mShowing = false;
		mMissionGridDataSource.Clear();
		mGlobalMissionGridDataSource.Clear();
	}

	public void OnSwipeRight()
	{
		if (!mSwappingPanel)
		{
			if (ShowGlobalCollider.activeSelf)
			{
				OnClickGlobalList();
			}
			else if (ShowDailyCollider.activeSelf)
			{
				OnClickDailyList();
			}
		}
	}

	public void OnSwipeLeft()
	{
		if (!mSwappingPanel)
		{
			if (ShowGlobalCollider.activeSelf)
			{
				OnClickGlobalList();
			}
			else if (ShowDailyCollider.activeSelf)
			{
				OnClickDailyList();
			}
		}
	}

	public void OnClickGlobalList()
	{
		if (!mSwappingPanel)
		{
			mSwappingPanel = true;
			StartCoroutine(BringGlobalToFront());
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_DailyMissionsFlip");
		}
	}

	private IEnumerator BringGlobalToFront()
	{
		ShowGlobalTween.Play();
		ShowDailyCollider.SetActive(true);
		ShowGlobalCollider.SetActive(false);
		yield return new WaitForSeconds(ShowGlobalTween.LongestTweenDuration() / 2f);
		DailyPanel.depth = mBackDepth;
		GlobalPanel.depth = mFrontDepth;
		DailyScrollPanel.depth = DailyPanel.depth + 1;
		GlobalScrollPanel.depth = GlobalPanel.depth + 1;
		mSwappingPanel = false;
	}

	public void OnClickDailyList()
	{
		if (!mSwappingPanel)
		{
			mSwappingPanel = true;
			StartCoroutine(BringDailyToFront());
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_DailyMissionsFlip");
		}
	}

	private IEnumerator BringDailyToFront()
	{
		ShowDailyTween.Play();
		ShowDailyCollider.SetActive(false);
		ShowGlobalCollider.SetActive(true);
		yield return new WaitForSeconds(ShowDailyTween.LongestTweenDuration() / 2f);
		GlobalPanel.depth = mBackDepth;
		DailyPanel.depth = mFrontDepth;
		DailyScrollPanel.depth = DailyPanel.depth + 1;
		GlobalScrollPanel.depth = GlobalPanel.depth + 1;
		mSwappingPanel = false;
	}
}
