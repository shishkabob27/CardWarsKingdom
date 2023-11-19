public static class GameStateExtensions
{
	public static bool IsP1Turn(this GameState state)
	{
		return state == GameState.P1StartTurn || state == GameState.P1Turn || state == GameState.P1EndTurn;
	}

	public static bool IsP2Turn(this GameState state)
	{
		return state == GameState.P2StartTurn || state == GameState.P2Turn || state == GameState.P2EndTurn;
	}
}
