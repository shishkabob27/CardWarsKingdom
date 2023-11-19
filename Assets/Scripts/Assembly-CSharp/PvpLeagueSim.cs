using System.Collections.Generic;
using UnityEngine;

public class PvpLeagueSim : MonoBehaviour
{
	private class Player
	{
		public float PowerRating;

		public int CurrentLeague;

		public int GamesPlayed;

		public int GamesToPlay;

		public bool QuitAtTop;

		public int TotalWins;

		public int PointsInLeague;

		public int WinStreak;

		public float TotalPowerDiff;

		public int TotalLeagueDiff;

		public float WinRatio;

		public int ReachedLeague1AtGame;
	}

	private class League
	{
		public PvpRankData RankData;

		public List<Player> Players = new List<Player>();
	}

	public int PlayerCount;

	public int MinGamesPerPlayer;

	public int MaxGamesPerPlayer;

	public int MatchRange;

	public bool RngWins;

	public float ChanceToQuitAtTop;

	public float LowSkillBias;

	public bool Execute;

	private void Update()
	{
	}
}
