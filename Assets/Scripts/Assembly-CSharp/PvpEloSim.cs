using UnityEngine;

public class PvpEloSim : MonoBehaviour
{
	private class Player
	{
		public float PowerRating;

		public int GamesPlayed;

		public int GamesToPlay;

		public int TotalWins;

		public int WinStreak;

		public float TotalPowerDiff;

		public float WinRatio;

		public int Points;
	}

	public int PlayerCount;

	public int MinGamesPerPlayer;

	public int MaxGamesPerPlayer;

	public float FewerGamesPlayedFactor;

	public int MatchRange;

	public bool RngWins;

	public Vector2 PlayerPowerRange;

	public int KFactor;

	public float PointScale;

	public float PointLostRatio;

	public bool Execute;

	private void Update()
	{
	}
}
