public class BraveryBreakDefenseOnManyEnemyDragAttacks : OnEnemyAttack
{
	public override bool OnEnable()
	{
		if (Attacker.DragAttacksThisTurn == 2 && IsDragAttack)
		{
			ApplyStatus(Attacker, StatusEnum.Bravery, base.Val1);
			ApplyStatus(Attacker, StatusEnum.BreakDefense, base.Val2);
			return true;
		}
		return false;
	}
}
