using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class PvPGlobalLeaderboardController : MonoBehaviour
{
	public GameObject PlayerLeaderboardTemplate;

	private int LeaderBoardMaxSize = 25;

	private AsyncData<List<LeaderboardData>> mAsyncGlobalLeaderboardData = new AsyncData<List<LeaderboardData>>();

	private AsyncData<List<LeaderboardData>> mAsyncLocalLeaderboardData = new AsyncData<List<LeaderboardData>>();

	private void OnEnable()
	{
		if (mAsyncGlobalLeaderboardData.processed)
		{
			global::Multiplayer.Multiplayer.TopLeaderboard(SessionManager.Instance.theSession, LeaderboardCallback);
		}
		if (mAsyncLocalLeaderboardData.processed)
		{
			global::Multiplayer.Multiplayer.NearbyLeaderboard(SessionManager.Instance.theSession, LocalLeaderboardCallback);
		}
	}

	public void LeaderboardCallback(List<LeaderboardData> data)
	{
		ResponseFlag a_flag = ResponseFlag.Success;
		mAsyncGlobalLeaderboardData.Set(a_flag, data);
	}

	public void LocalLeaderboardCallback(List<LeaderboardData> data)
	{
		ResponseFlag a_flag = ResponseFlag.Success;
		mAsyncLocalLeaderboardData.Set(a_flag, data);
	}

	private void Update()
	{
		if (mAsyncGlobalLeaderboardData.processed || mAsyncLocalLeaderboardData.processed)
		{
			return;
		}
		mAsyncGlobalLeaderboardData.processed = true;
		mAsyncLocalLeaderboardData.processed = true;
		if (mAsyncGlobalLeaderboardData.MP_Data == null)
		{
			return;
		}
		UIGrid component = base.gameObject.GetComponent<UIGrid>();
		int num = 0;
		ClearList(component);
		foreach (LeaderboardData mP_Datum in mAsyncGlobalLeaderboardData.MP_Data)
		{
			GameObject aLeaderboardItemObj = NGUITools.AddChild(component.gameObject, PlayerLeaderboardTemplate);
			fillItem(aLeaderboardItemObj, mP_Datum);
			if (++num == LeaderBoardMaxSize)
			{
				break;
			}
		}
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		if (Convert.ToInt32(instance.PvPData.PlayerRank) > LeaderBoardMaxSize)
		{
			GameObject gameObject = NGUITools.AddChild(component.gameObject, PlayerLeaderboardTemplate);
			PvPLeaderboardItem pvPLeaderboardItem = gameObject.GetComponentsInChildren<PvPLeaderboardItem>(true)[0];
			pvPLeaderboardItem.gameObject.SetActive(true);
			foreach (LeaderboardData mP_Datum2 in mAsyncLocalLeaderboardData.MP_Data)
			{
				LeaderboardData leaderboardData = mP_Datum2;
				if (leaderboardData.trophies == instance.PvPData.TotalTrophies && leaderboardData.name == instance.PvPData.PlayerName)
				{
					fillItem(gameObject, mP_Datum2);
					break;
				}
			}
		}
		component.Reposition();
	}

	private void ClearList(UIGrid list)
	{
		Transform transform = list.transform;
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = transform.GetChild(num).gameObject;
			gameObject.SetActive(false);
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void fillItem(GameObject aLeaderboardItemObj, object data)
	{
		PvPLeaderboardItem pvPLeaderboardItem = aLeaderboardItemObj.GetComponentsInChildren<PvPLeaderboardItem>(true)[0];
		pvPLeaderboardItem.gameObject.SetActive(true);
		LeaderboardData leaderboardData = data as LeaderboardData;
		pvPLeaderboardItem.Name.text = leaderboardData.name;
		pvPLeaderboardItem.Rank.text = leaderboardData.rank.ToString();
		pvPLeaderboardItem.Score.text = leaderboardData.trophies.ToString();
		pvPLeaderboardItem.Icone.ReplaceTexture(leaderboardData.icon);
		pvPLeaderboardItem.UserRecord.text = leaderboardData.wins + " - " + leaderboardData.losses;
		pvPLeaderboardItem.UserRecord.gameObject.SetActive(true);
	}
}
