using UnityEngine;

public class HockeyAppIOS : MonoBehaviour
{
	public enum AuthenticatorType
	{
		Anonymous = 0,
		Device = 1,
		HockeyAppUser = 2,
		HockeyAppEmail = 3,
		WebAuth = 4,
	}

	public string appID;
	public AuthenticatorType authenticatorType;
	public string secret;
	public string serverURL;
	public bool autoUpload;
	public bool exceptionLogging;
	public bool updateManager;
}
