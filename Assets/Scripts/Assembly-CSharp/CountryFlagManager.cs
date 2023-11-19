public class CountryFlagManager : Singleton<CountryFlagManager>
{
	private const string DEFAULT_CODE = "";

	public string GetMyCountryCode()
	{
		string text = Singleton<TBPvPManager>.Instance.CountryCode;
		if (text == string.Empty)
		{
			text = Singleton<PlayerInfoScript>.Instance.SaveData.LastKnownLocation;
		}
		else if (Singleton<PlayerInfoScript>.Instance.SaveData.LastKnownLocation != text)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.LastKnownLocation = text;
			Singleton<PlayerInfoScript>.Instance.Save();
		}
		return text;
	}

	public string GetMyFlag()
	{
		return TextureForCountryCode(GetMyCountryCode());
	}

	public string TextureForCountryCode(string code)
	{
		if (code == null || code == string.Empty)
		{
			return string.Empty;
		}
		return "Flags/" + code.ToLower();
	}
}
