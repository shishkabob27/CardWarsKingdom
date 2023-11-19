public static class AmountComparisonExtensions
{
	public static bool Compare(this CardConditions.AmountComparison comparison, int lhs, int rhs)
	{
		switch (comparison)
		{
		case CardConditions.AmountComparison.Exactly:
			return lhs == rhs;
		case CardConditions.AmountComparison.MoreThan:
			return lhs > rhs;
		case CardConditions.AmountComparison.LessThan:
			return lhs < rhs;
		default:
			return false;
		}
	}

	public static bool Compare(this CardConditions.AmountComparison comparison, float lhs, float rhs)
	{
		switch (comparison)
		{
		case CardConditions.AmountComparison.Exactly:
			return lhs == rhs;
		case CardConditions.AmountComparison.MoreThan:
			return lhs > rhs;
		case CardConditions.AmountComparison.LessThan:
			return lhs < rhs;
		default:
			return false;
		}
	}
}
