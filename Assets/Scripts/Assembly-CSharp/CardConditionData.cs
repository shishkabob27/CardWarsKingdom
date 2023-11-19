using System.Collections.Generic;

public class CardConditionData
{
	public PlayerType OwningPlayer;

	public PlayerType TargetPlayer;

	public BoardState Board;

	public int TargetLane;

	public int TargetLane2;

	public List<string> Parameters;

	public float TargetHPPercentBeforeAttack;

	public CardConditionData(PlayerType owningPlayer, PlayerType targetPlayer, BoardState board, int targetLane, int targetLane2 = -1, float targetHPPercentBeforeAttack = 0f)
	{
		OwningPlayer = owningPlayer;
		TargetPlayer = targetPlayer;
		Board = board;
		TargetLane = targetLane;
		TargetLane2 = targetLane2;
		TargetHPPercentBeforeAttack = targetHPPercentBeforeAttack;
	}

	public void SetToT2()
	{
		OwningPlayer = TargetPlayer;
		TargetLane = TargetLane2;
	}
}
