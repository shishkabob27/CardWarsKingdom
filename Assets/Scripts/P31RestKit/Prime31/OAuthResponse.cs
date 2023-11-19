using System.Collections.Generic;

namespace Prime31
{
	public class OAuthResponse
	{
		private Dictionary<string, string> _params;

		public string responseText { get; set; }

		public string this[string ix] => _params[ix];

		public OAuthResponse(string alltext)
		{
			responseText = alltext;
			_params = new Dictionary<string, string>();
			string[] array = alltext.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				_params.Add(array3[0], array3[1]);
			}
		}
	}
}
