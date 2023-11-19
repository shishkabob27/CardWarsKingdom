using UnityEngine;

public class FontManager : Singleton<FontManager>
{
	private UIFont mJapaneseUIFont;

	public static string JapaneseFontName = "Font/JPFont";

	private UIFont mChineseUIFont;

	public static string ChineseFontName = "Font/CNSimp8";

	private UIFont mTurkishUIFont;

	public static string TurkishFontName = "Font/TRFont";

	public UIFont GetJPFont()
	{
		return mJapaneseUIFont;
	}

	public UIFont GetZHFont()
	{
		return mChineseUIFont;
	}

	public UIFont GetTRFont()
	{
		return mTurkishUIFont;
	}

	public void LoadLanguages()
	{
		if (Language.CurrentLanguage() == LanguageCode.JA)
		{
			GameObject gameObject = Singleton<SLOTResourceManager>.Instance.LoadResource(JapaneseFontName) as GameObject;
			if (gameObject != null)
			{
				UIFont uIFont = (mJapaneseUIFont = gameObject.GetComponent<UIFont>());
			}
		}
		if (Language.CurrentLanguage() == LanguageCode.TR)
		{
			GameObject gameObject2 = Singleton<SLOTResourceManager>.Instance.LoadResource(TurkishFontName) as GameObject;
			if (gameObject2 != null)
			{
				UIFont uIFont2 = (mTurkishUIFont = gameObject2.GetComponent<UIFont>());
			}
		}
	}
}
