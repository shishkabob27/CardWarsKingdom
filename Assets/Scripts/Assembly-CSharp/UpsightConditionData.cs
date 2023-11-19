public class UpsightConditionData
{
	public enum UpsightMSConditionType
	{
		BuyHero,
		Feature,
		None
	}

	public string Suffix;

	public UpsightMSConditionType ConditionType = UpsightMSConditionType.None;

	public string ConditionValue;

	public int Priority;
}
