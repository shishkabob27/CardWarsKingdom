namespace Facebook.Unity.Canvas
{
	internal interface ICanvasJSWrapper
	{
		string IntegrationMethodJs { get; }

		string GetSDKVersion();

		void ExternalCall(string functionName, params object[] args);

		void DisableFullScreen();

		void ExternalEval(string script);
	}
}
