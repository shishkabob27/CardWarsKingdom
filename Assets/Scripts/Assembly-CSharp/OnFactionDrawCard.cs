public abstract class OnFactionDrawCard : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DRAW_CARD && Message.WhichPlayer == base.Owner.Owner && Message.Card.Faction == (CreatureFaction)base.Val2)
		{
			return OnEnable();
		}
		return false;
	}
}
