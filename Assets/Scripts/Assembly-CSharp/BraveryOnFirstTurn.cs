public class BraveryOnFirstTurn : OnFirstTurn
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val1);
		return true;
	}
}
