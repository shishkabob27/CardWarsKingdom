using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Prime31
{
	public class P31RestKit
	{
		protected string _baseUrl;

		public bool debugRequests = false;

		protected bool forceJsonResponse;

		private GameObject _surrogateGameObject;

		private MonoBehaviour _surrogateMonobehaviour;

		protected virtual GameObject surrogateGameObject
		{
			get
			{
				if (_surrogateGameObject == null)
				{
					_surrogateGameObject = GameObject.Find("P31CoroutineSurrogate");
					if (_surrogateGameObject == null)
					{
						_surrogateGameObject = new GameObject("P31CoroutineSurrogate");
						UnityEngine.Object.DontDestroyOnLoad(_surrogateGameObject);
					}
				}
				return _surrogateGameObject;
			}
			set
			{
				_surrogateGameObject = value;
			}
		}

		protected MonoBehaviour surrogateMonobehaviour
		{
			get
			{
				if (_surrogateMonobehaviour == null)
				{
					_surrogateMonobehaviour = surrogateGameObject.AddComponent<MonoBehaviour>();
				}
				return _surrogateMonobehaviour;
			}
			set
			{
				_surrogateMonobehaviour = value;
			}
		}

		protected virtual IEnumerator send(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters, Action<string, object> onComplete)
		{
			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			WWW www = processRequest(path, httpVerb, parameters);
			yield return www;
			if (debugRequests)
			{
				Debug.Log("response error: " + www.error);
				Debug.Log("response text: " + www.text);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Response Headers:\n");
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", responseHeader.Key, responseHeader.Value);
				}
				Debug.Log(stringBuilder.ToString());
			}
			if (onComplete != null)
			{
				processResponse(www, onComplete);
			}
			www.Dispose();
		}

		protected virtual WWW processRequest(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!path.StartsWith("http"))
			{
				stringBuilder.Append(_baseUrl).Append(path);
			}
			else
			{
				stringBuilder.Append(path);
			}
			bool flag = httpVerb != HTTPVerb.GET;
			WWWForm wWWForm = ((!flag) ? null : new WWWForm());
			if (parameters != null && parameters.Count > 0)
			{
				if (flag)
				{
					foreach (KeyValuePair<string, object> parameter in parameters)
					{
						if (parameter.Value is string)
						{
							wWWForm.AddField(parameter.Key, parameter.Value as string);
						}
						else if (parameter.Value is byte[])
						{
							wWWForm.AddBinaryData(parameter.Key, parameter.Value as byte[]);
						}
					}
				}
				else
				{
					bool flag2 = true;
					if (path.Contains("?"))
					{
						flag2 = false;
					}
					foreach (KeyValuePair<string, object> parameter2 in parameters)
					{
						if (parameter2.Value is string)
						{
							stringBuilder.AppendFormat("{0}{1}={2}", (!flag2) ? "&" : "?", WWW.EscapeURL(parameter2.Key), WWW.EscapeURL(parameter2.Value as string));
							flag2 = false;
						}
					}
				}
			}
			if (debugRequests)
			{
				Debug.Log("url: " + stringBuilder.ToString());
			}
			Dictionary<string, string> dictionary = null;
			if (flag)
			{
				IDictionary headersFromForm = getHeadersFromForm(wWWForm);
				if (headersFromForm != null)
				{
					dictionary = new Dictionary<string, string>();
					if (headersFromForm.Contains("Content-Type"))
					{
						dictionary.Add("Content-Type", headersFromForm["Content-Type"].ToString());
					}
					if (debugRequests)
					{
						Debug.Log("Found a POST request. Fetching headers from WWWForm and starting with these as a base: ");
						Utils.logObject(dictionary);
					}
				}
			}
			return (!flag) ? new WWW(stringBuilder.ToString()) : new WWW(stringBuilder.ToString(), wWWForm.data, headersForRequest(httpVerb, dictionary));
		}

		protected virtual Dictionary<string, string> headersForRequest(HTTPVerb httpVerb, Dictionary<string, string> headers = null)
		{
			headers = headers ?? new Dictionary<string, string>();
			switch (httpVerb)
			{
			case HTTPVerb.GET:
				headers.Add("METHOD", "GET");
				break;
			case HTTPVerb.POST:
				headers.Add("METHOD", "POST");
				break;
			case HTTPVerb.PUT:
				headers.Add("METHOD", "PUT");
				headers.Add("X-HTTP-Method-Override", "PUT");
				break;
			case HTTPVerb.DELETE:
				headers.Add("METHOD", "DELETE");
				headers.Add("X-HTTP-Method-Override", "DELETE");
				break;
			}
			return headers;
		}

		protected virtual void processResponse(WWW www, Action<string, object> onComplete)
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				onComplete(www.error, null);
			}
			else if (isResponseJson(www))
			{
				object obj = Json.decode(www.text);
				if (obj == null)
				{
					obj = www.text;
				}
				onComplete(null, obj);
			}
			else
			{
				onComplete(null, www.text);
			}
		}

		protected bool isResponseJson(WWW www)
		{
			bool flag = false;
			if (forceJsonResponse)
			{
				flag = true;
			}
			if (!flag)
			{
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					if (responseHeader.Key.ToLower() == "content-type" && (responseHeader.Value.ToLower().Contains("/json") || responseHeader.Value.ToLower().Contains("/javascript")))
					{
						flag = true;
					}
				}
			}
			if (flag && !www.text.StartsWith("[") && !www.text.StartsWith("{"))
			{
				return false;
			}
			return flag;
		}

		protected virtual IDictionary getHeadersFromForm(WWWForm form)
		{
			try
			{
				PropertyInfo property = form.GetType().GetProperty("headers");
				if (property != null)
				{
					return property.GetValue(form, null) as IDictionary;
				}
				Debug.Log("couldnt find the 'headers' property on the WWWForm object: " + form);
			}
			catch (Exception ex)
			{
				Debug.Log("ran into a problem transferring headers from WWWForm to the WWW request: " + ex);
			}
			return null;
		}

		public void setBaseUrl(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		public void get(string path, Action<string, object> completionHandler)
		{
			get(path, null, completionHandler);
		}

		public void get(string path, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.GET, parameters, completionHandler));
		}

		public void post(string path, Action<string, object> completionHandler)
		{
			post(path, null, completionHandler);
		}

		public void post(string path, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.POST, parameters, completionHandler));
		}

		public void put(string path, Action<string, object> completionHandler)
		{
			put(path, null, completionHandler);
		}

		public void put(string path, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.PUT, parameters, completionHandler));
		}
	}
}
