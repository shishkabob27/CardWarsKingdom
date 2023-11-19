public class ChatMetaData
{
	public string UserId = string.Empty;

	public string Name = string.Empty;

	public int Level;

	public string FacebookId = string.Empty;

	public string CountryCode = string.Empty;

	public int CurrentLeague;

	public int BestLeague;

	public string Leader = string.Empty;

	public string ZapUserId = string.Empty;

	public string PortraitId = string.Empty;

	public string GachaCreature;

	public string Dungeon;

	public string InviteData;

	public override string ToString()
	{
		if (GachaCreature != null)
		{
			return string.Format("[ChatMetaData from={0}, GachaCreature={1}]", UserId, GachaCreature);
		}
		if (Dungeon != null)
		{
			return string.Format("[ChatMetaData from={0}, Dungeon={1}]", UserId, Dungeon);
		}
		return string.Format("[ChatMetaData from={0}]", UserId);
	}
}
