using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	internal class AsyncRequestString : MonoBehaviour
	{
		private Uri url;

		private HttpMethod method;

		private IDictionary<string, string> formData;

		private WWWForm query;

		private FacebookDelegate<IGraphResult> callback;

		internal static void Post(Uri url, Dictionary<string, string> formData = null, FacebookDelegate<IGraphResult> callback = null)
		{
			Request(url, HttpMethod.POST, formData, callback);
		}

		internal static void Get(Uri url, Dictionary<string, string> formData = null, FacebookDelegate<IGraphResult> callback = null)
		{
			Request(url, HttpMethod.GET, formData, callback);
		}

		internal static void Request(Uri url, HttpMethod method, WWWForm query = null, FacebookDelegate<IGraphResult> callback = null)
		{
			ComponentFactory.AddComponent<AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetQuery(query)
				.SetCallback(callback);
		}

		internal static void Request(Uri url, HttpMethod method, IDictionary<string, string> formData = null, FacebookDelegate<IGraphResult> callback = null)
		{
			ComponentFactory.AddComponent<AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetFormData(formData)
				.SetCallback(callback);
		}

		internal IEnumerator Start()
		{
			WWW www;
			if (method == HttpMethod.GET)
			{
				string urlParams = ((!url.AbsoluteUri.Contains("?")) ? "?" : "&");
				if (formData != null)
				{
					foreach (KeyValuePair<string, string> pair2 in formData)
					{
						urlParams += string.Format("{0}={1}&", Uri.EscapeDataString(pair2.Key), Uri.EscapeDataString(pair2.Value));
					}
				}
				Dictionary<string, string> headers = new Dictionary<string, string>();
				headers["User-Agent"] = Constants.GraphApiUserAgent;
				www = new WWW(string.Concat(url, urlParams), null, headers);
			}
			else
			{
				if (query == null)
				{
					query = new WWWForm();
				}
				if (method == HttpMethod.DELETE)
				{
					query.AddField("method", "delete");
				}
				if (formData != null)
				{
					foreach (KeyValuePair<string, string> pair in formData)
					{
						query.AddField(pair.Key, pair.Value);
					}
				}
				query.headers["User-Agent"] = Constants.GraphApiUserAgent;
				www = new WWW(url.AbsoluteUri, query);
			}
			yield return www;
			if (callback != null)
			{
				callback(new GraphResult(www));
			}
			www.Dispose();
			UnityEngine.Object.Destroy(this);
		}

		internal AsyncRequestString SetUrl(Uri url)
		{
			this.url = url;
			return this;
		}

		internal AsyncRequestString SetMethod(HttpMethod method)
		{
			this.method = method;
			return this;
		}

		internal AsyncRequestString SetFormData(IDictionary<string, string> formData)
		{
			this.formData = formData;
			return this;
		}

		internal AsyncRequestString SetQuery(WWWForm query)
		{
			this.query = query;
			return this;
		}

		internal AsyncRequestString SetCallback(FacebookDelegate<IGraphResult> callback)
		{
			this.callback = callback;
			return this;
		}
	}
}
