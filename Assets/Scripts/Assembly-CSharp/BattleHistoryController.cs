using System.Collections.Generic;
using Allies;
using UnityEngine;

public class BattleHistoryController : MonoBehaviour
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UILabel playerNameLabel;

	public UILabel NoMatchesLabel;

	public UIStreamingGrid currentSeasonGrid;

	private UIStreamingGridDataSource<BattleHistory> mCurrentSeasonGridDataSource = new UIStreamingGridDataSource<BattleHistory>();

	public UIStreamingGrid previousSeasonsGrid;

	private UIStreamingGridDataSource<BattleHistory> mPreviousSeasonsGridDataSource = new UIStreamingGridDataSource<BattleHistory>();

	public GameObject historyEntryTemplate;

	public UIToggle globalToggle;

	public LeagueBadge PlayerBadge;

	public UITexture playerFlag;

	private static BattleHistoryController _instance;

	public List<AllyData> alliesList = new List<AllyData>();

	public static BattleHistoryController Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Start()
	{
		_instance = this;
	}

	public void Show()
	{
		playerNameLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName;
		PlayerBadge.PopulateMyData();
		Session theSession = SessionManager.Instance.theSession;
		Ally.GetAlliesList(theSession, AlliesListCallback);
		ShowTween.Play();
	}

	private void AlliesListCallback(List<AllyData> allieslist, ResponseFlag flag)
	{
		if (flag == ResponseFlag.Success)
		{
			alliesList = allieslist;
		}
		else
		{
			alliesList = new List<AllyData>();
		}
		OnGlobalValueChanged();
	}

	public void BuildRankedMatchesList()
	{
		List<BattleHistory> rankedMatches = BattleHistoryLocalSavesManager.Instance.getRankedMatches();
		InventoryTile.ClearDelegates(false);
		mCurrentSeasonGridDataSource.Init(currentSeasonGrid, historyEntryTemplate, rankedMatches);
		NoMatchesLabel.gameObject.SetActive(rankedMatches.Count == 0);
	}

	public void BuildUnrankedMatchesList()
	{
		List<BattleHistory> unrankedMatches = BattleHistoryLocalSavesManager.Instance.getUnrankedMatches();
		InventoryTile.ClearDelegates(false);
		mCurrentSeasonGridDataSource.Init(currentSeasonGrid, historyEntryTemplate, unrankedMatches);
		NoMatchesLabel.gameObject.SetActive(unrankedMatches.Count == 0);
	}

	public void HideHistoryUI()
	{
		HideTween.Play();
		Singleton<PvPModeSelectController>.Instance.ShowElements();
	}

	public void OnCloseButtonClick()
	{
		HideTween.Play();
		Singleton<PvPModeSelectController>.Instance.Hide();
	}

	public void Unload()
	{
		mCurrentSeasonGridDataSource.Clear();
		mPreviousSeasonsGridDataSource.Clear();
	}

	public void OnGlobalValueChanged()
	{
		UIToggle activeToggle = UIToggle.GetActiveToggle(6);
		if (activeToggle != globalToggle)
		{
			BuildUnrankedMatchesList();
		}
		else
		{
			BuildRankedMatchesList();
		}
	}

	public void RefreshInviteButtons()
	{
		for (int i = 0; i < currentSeasonGrid.transform.childCount; i++)
		{
			Transform child = currentSeasonGrid.transform.GetChild(i);
			if (child.gameObject.activeInHierarchy)
			{
				BattleHistoryItemController component = child.GetComponent<BattleHistoryItemController>();
				component.RefreshInviteButton();
			}
		}
		for (int j = 0; j < previousSeasonsGrid.transform.childCount; j++)
		{
			Transform child2 = previousSeasonsGrid.transform.GetChild(j);
			if (child2.gameObject.activeInHierarchy)
			{
				BattleHistoryItemController component2 = child2.GetComponent<BattleHistoryItemController>();
				component2.RefreshInviteButton();
			}
		}
	}
}
