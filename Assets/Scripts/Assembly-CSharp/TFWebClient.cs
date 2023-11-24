using System;
using System.Net;
using UnityEngine;

public class TFWebClient : WebClient
{
	public delegate void OnNetworkError(object sender, WebException e);

	private const int TIMEOUT = 30000;

	private const string USER_AGENT = "Innertube Explorer v0.1";

	private static int _maxConnections = 2;

	public string uriaddress;

	private CookieContainer cookies;

	public static int maxConnections
	{
		get
		{
			return maxConnections;
		}
		set
		{
			_maxConnections = Math.Max(1, value);
			ServicePointManager.DefaultConnectionLimit = _maxConnections;
		}
	}

	public event OnNetworkError NetworkError;

	public TFWebClient(CookieContainer cookieContainer)
	{
		cookies = cookieContainer;
	}

	protected override WebRequest GetWebRequest(Uri address)
	{
		uriaddress = address.AbsoluteUri;
		HttpWebRequest httpWebRequest = base.GetWebRequest(address) as HttpWebRequest;
		httpWebRequest.CookieContainer = cookies;
		httpWebRequest.Timeout = 30000;
		httpWebRequest.UserAgent = "Innertube Explorer v0.1";
		ServicePoint servicePoint = ServicePointManager.FindServicePoint(address);
		servicePoint.Expect100Continue = false;
		servicePoint.ConnectionLimit = _maxConnections;
		return httpWebRequest;
	}

	protected override WebResponse GetWebResponse(WebRequest request)
	{
		//Discarded unreachable code: IL_000d, IL_0051
		try
		{
			return base.GetWebResponse(request);
		}
		catch (WebException ex)
		{
			Debug.LogWarning(string.Concat(request.RequestUri, ", Exception status: ", Enum.GetName(typeof(WebExceptionStatus), ex.Status)));
			this.NetworkError(this, ex);
			throw ex;
		}
	}
}
