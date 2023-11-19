using UnityEngine;

public class PvpEloSim : MonoBehaviour
{
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
}
