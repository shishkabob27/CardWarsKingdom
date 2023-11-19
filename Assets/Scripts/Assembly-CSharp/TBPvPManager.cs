public class TBPvPManager : Singleton<TBPvPManager>
{
	public enum GameState
	{
		None = 0,
		Login = 1,
		InGame = 2,
	}

	public string AppId;
	public GameState CurrentState;
	public string PvPCompatibilityVersion;
}
