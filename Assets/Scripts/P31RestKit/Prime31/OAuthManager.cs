using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Prime31
{
	public class OAuthManager
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		private SortedDictionary<string, string> _params;

		private Random _random;

		private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		public string this[string ix]
		{
			get
			{
				if (_params.ContainsKey(ix))
				{
					return _params[ix];
				}
				throw new ArgumentException(ix);
			}
			set
			{
				if (!_params.ContainsKey(ix))
				{
					throw new ArgumentException(ix);
				}
				_params[ix] = value;
			}
		}

		public OAuthManager()
		{
			_random = new Random();
			_params = new SortedDictionary<string, string>();
			_params["consumer_key"] = "";
			_params["consumer_secret"] = "";
			_params["timestamp"] = generateTimeStamp();
			_params["nonce"] = generateNonce();
			_params["signature_method"] = "HMAC-SHA1";
			_params["signature"] = "";
			_params["token"] = "";
			_params["token_secret"] = "";
			_params["version"] = "1.0";
		}

		public OAuthManager(string consumerKey, string consumerSecret, string token, string tokenSecret)
			: this()
		{
			_params["consumer_key"] = consumerKey;
			_params["consumer_secret"] = consumerSecret;
			_params["token"] = token;
			_params["token_secret"] = tokenSecret;
		}

		private string generateTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - _epoch).TotalSeconds).ToString();
		}

		private void prepareNewRequest()
		{
			_params["nonce"] = generateNonce();
			_params["timestamp"] = generateTimeStamp();
		}

		private string generateNonce()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 8; i++)
			{
				if (_random.Next(3) == 0)
				{
					stringBuilder.Append((char)(_random.Next(26) + 97), 1);
				}
				else
				{
					stringBuilder.Append((char)(_random.Next(10) + 48), 1);
				}
			}
			return stringBuilder.ToString();
		}

		private SortedDictionary<string, string> extractQueryParameters(string queryString)
		{
			if (queryString.StartsWith("?"))
			{
				queryString = queryString.Remove(0, 1);
			}
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			if (string.IsNullOrEmpty(queryString))
			{
				return sortedDictionary;
			}
			string[] array = queryString.Split('&');
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text) && !text.StartsWith("oauth_"))
				{
					if (text.IndexOf('=') > -1)
					{
						string[] array2 = text.Split('=');
						sortedDictionary.Add(array2[0], array2[1]);
					}
					else
					{
						sortedDictionary.Add(text, string.Empty);
					}
				}
			}
			return sortedDictionary;
		}

		public static string urlEncode(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				if (unreservedChars.IndexOf(c) != -1)
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('%' + $"{(int)c:X2}");
				}
			}
			return stringBuilder.ToString();
		}

		private static SortedDictionary<string, string> mergePostParamsWithOauthParams(SortedDictionary<string, string> postParams, SortedDictionary<string, string> oAuthParams)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (KeyValuePair<string, string> postParam in postParams)
			{
				sortedDictionary.Add(postParam.Key, postParam.Value);
			}
			foreach (KeyValuePair<string, string> oAuthParam in oAuthParams)
			{
				if (!string.IsNullOrEmpty(oAuthParam.Value) && !oAuthParam.Key.EndsWith("secret"))
				{
					sortedDictionary.Add("oauth_" + oAuthParam.Key, oAuthParam.Value);
				}
			}
			return sortedDictionary;
		}

		private static string encodeRequestParameters(SortedDictionary<string, string> p)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in p)
			{
				if (!string.IsNullOrEmpty(item.Value) && !item.Key.EndsWith("secret"))
				{
					stringBuilder.AppendFormat("oauth_{0}=\"{1}\", ", item.Key, urlEncode(item.Value));
				}
			}
			return stringBuilder.ToString().TrimEnd(' ').TrimEnd(',');
		}

		public static byte[] encodePostParameters(SortedDictionary<string, string> p)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in p)
			{
				if (!string.IsNullOrEmpty(item.Value))
				{
					stringBuilder.AppendFormat("{0}={1}, ", urlEncode(item.Key), urlEncode(item.Value));
				}
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString().TrimEnd(' ').TrimEnd(','));
		}

		public OAuthResponse acquireRequestToken(string uri, string method)
		{
			prepareNewRequest();
			string authorizationHeader = getAuthorizationHeader(uri, method);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Headers.Add("Authorization", authorizationHeader);
			httpWebRequest.Method = method;
			using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			OAuthResponse oAuthResponse = new OAuthResponse(streamReader.ReadToEnd());
			this["token"] = oAuthResponse["oauth_token"];
			try
			{
				if (oAuthResponse["oauth_token_secret"] != null)
				{
					this["token_secret"] = oAuthResponse["oauth_token_secret"];
				}
			}
			catch
			{
			}
			return oAuthResponse;
		}

		public OAuthResponse acquireAccessToken(string uri, string method, string verifier)
		{
			prepareNewRequest();
			_params["verifier"] = verifier;
			string authorizationHeader = getAuthorizationHeader(uri, method);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Headers.Add("Authorization", authorizationHeader);
			httpWebRequest.Method = method;
			using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			OAuthResponse oAuthResponse = new OAuthResponse(streamReader.ReadToEnd());
			this["token"] = oAuthResponse["oauth_token"];
			this["token_secret"] = oAuthResponse["oauth_token_secret"];
			return oAuthResponse;
		}

		public string generateCredsHeader(string uri, string method, string realm)
		{
			prepareNewRequest();
			return getAuthorizationHeader(uri, method, realm);
		}

		public string generateAuthzHeader(string uri, string method)
		{
			prepareNewRequest();
			return getAuthorizationHeader(uri, method, null);
		}

		private string getAuthorizationHeader(string uri, string method)
		{
			return getAuthorizationHeader(uri, method, null);
		}

		private string getAuthorizationHeader(string uri, string method, string realm)
		{
			if (string.IsNullOrEmpty(_params["consumer_key"]))
			{
				throw new ArgumentNullException("consumer_key");
			}
			if (string.IsNullOrEmpty(_params["signature_method"]))
			{
				throw new ArgumentNullException("signature_method");
			}
			sign(uri, method);
			string text = encodeRequestParameters(_params);
			return (!string.IsNullOrEmpty(realm)) ? ($"OAuth realm=\"{realm}\", " + text) : ("OAuth " + text);
		}

		private void sign(string uri, string method)
		{
			string signatureBase = getSignatureBase(uri, method);
			HashAlgorithm hash = getHash();
			byte[] bytes = Encoding.ASCII.GetBytes(signatureBase);
			byte[] inArray = hash.ComputeHash(bytes);
			this["signature"] = Convert.ToBase64String(inArray);
		}

		private string getSignatureBase(string url, string method)
		{
			Uri uri = new Uri(url);
			string text = $"{uri.Scheme}://{uri.Host}";
			if ((!(uri.Scheme == "http") || uri.Port != 80) && (!(uri.Scheme == "https") || uri.Port != 443))
			{
				text = text + ":" + uri.Port;
			}
			text += uri.AbsolutePath;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(method).Append('&').Append(urlEncode(text))
				.Append('&');
			SortedDictionary<string, string> sortedDictionary = extractQueryParameters(uri.Query);
			foreach (KeyValuePair<string, string> param in _params)
			{
				if (!string.IsNullOrEmpty(_params[param.Key]) && !param.Key.EndsWith("_secret") && !param.Key.EndsWith("signature"))
				{
					sortedDictionary.Add("oauth_" + param.Key, param.Value);
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (KeyValuePair<string, string> item in sortedDictionary)
			{
				stringBuilder2.AppendFormat("{0}={1}&", item.Key, item.Value);
			}
			stringBuilder.Append(urlEncode(stringBuilder2.ToString().TrimEnd('&')));
			return stringBuilder.ToString();
		}

		private HashAlgorithm getHash()
		{
			if (this["signature_method"] != "HMAC-SHA1")
			{
				throw new NotImplementedException();
			}
			string s = string.Format("{0}&{1}", urlEncode(this["consumer_secret"]), urlEncode(this["token_secret"]));
			HMACSHA1 hMACSHA = new HMACSHA1();
			hMACSHA.Key = Encoding.ASCII.GetBytes(s);
			return hMACSHA;
		}
	}
}
