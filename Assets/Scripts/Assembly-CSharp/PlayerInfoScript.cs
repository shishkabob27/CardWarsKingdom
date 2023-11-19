public class PlayerInfoScript : Singleton<PlayerInfoScript>
{
	public PlayerSaveData SaveData;
	public GameStateData StateData;
	public PvPGameStateData PvPData;
	public bool NewSession;
	public string crooz_loginkey;
	public float MinPauseResetTimeSec;
}
