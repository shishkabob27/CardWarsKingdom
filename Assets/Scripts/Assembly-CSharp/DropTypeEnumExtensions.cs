public static class DropTypeEnumExtensions
{
	public static bool IsCurrency(this DropTypeEnum type)
	{
		return type == DropTypeEnum.SoftCurrency || type == DropTypeEnum.SocialCurrency || type == DropTypeEnum.HardCurrency || type == DropTypeEnum.Tickets || type == DropTypeEnum.RankXP;
	}
}
