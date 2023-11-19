public class Push : StatusState
{
	protected override void OnEnable()
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.PUSH_CREATURE_TO_HAND;
		gameMessage.WhichPlayer = base.Target.Owner;
		gameMessage.Creature = base.Target;
		base.Target.Owner.Game.AddMessage(gameMessage);
		PlayerState owner = base.Target.Owner;
		CreatureItem data = base.Target.Data;
		base.Target.Owner.RemoveCreature(base.Target.Lane);
		base.Target.Clean();
		base.Target.Init(owner, data);
		base.Target.Owner.DeploymentList.Add(base.Target);
	}
}
