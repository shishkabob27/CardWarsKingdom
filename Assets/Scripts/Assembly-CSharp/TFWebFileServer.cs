using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zlib;
using JsonFx.Json;
using UnityEngine;

public class TFWebFileServer
{
	private class CallbackInfo
	{
		public FileCallbackHandler Callback { get; set; }

		public object UserData { get; set; }
	}

	public delegate void FileCallbackHandler(TFWebFileResponse response);

	public delegate void SaveFileCallbackHandler(TFWebFileResponse response);

	protected CookieContainer cookies;

	protected byte[] encodedOriginal;

	public TFWebFileServer()
	{
	}

	public TFWebFileServer(CookieContainer cookies)
	{
		this.cookies = cookies;
	}

	public void GetFile(string uri, WebHeaderCollection headers, FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting file " + uri);
		using (TFWebClient tFWebClient = new TFWebClient(cookies))
		{
			tFWebClient.NetworkError += OnNetworkError;
			headers.Add("Age", DateTime.UtcNow.UnixTimestamp().ToString("X"));
			tFWebClient.Headers = headers;
			foreach (string header in tFWebClient.Headers)
			{
				Debug.Log("Outbound:" + header + "-->" + tFWebClient.Headers[header]);
			}
			try
			{
				tFWebClient.DownloadDataCompleted += OnGetComplete;
				CallbackInfo callbackInfo = new CallbackInfo();
				callbackInfo.Callback = callback;
				callbackInfo.UserData = userData;
				tFWebClient.DownloadDataAsync(new Uri(uri), callbackInfo);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}

	protected void OnGetComplete(object sender, DownloadDataCompletedEventArgs e)
	{
		OnCallComplete(sender, e, true);
	}

	private void PopulateResponse(TFWebFileResponse response, HttpWebResponse httpRes)
	{
		response.StatusCode = httpRes.StatusCode;
		response.headers = httpRes.Headers;
		response.URI = httpRes.ResponseUri.ToString();
		try
		{
			byte[] array = new byte[4096];
			Stream responseStream = httpRes.GetResponseStream();
			int num = 0;
			MemoryStream memoryStream = new MemoryStream();
			while ((num = responseStream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			Encoding uTF = Encoding.UTF8;
			response.Data = uTF.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void SaveFile(string uri, string contents, WebHeaderCollection headers, FileCallbackHandler callback, object userdata = null)
	{
		using (TFWebClient tFWebClient = new TFWebClient(cookies))
		{
			tFWebClient.NetworkError += OnNetworkError;
			Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(contents);
			string text = string.Empty;
			if (dictionary.ContainsKey("PlayerName"))
			{
				text = text + "username=" + (string)dictionary["PlayerName"] + "&";
			}
			if (dictionary.ContainsKey("GP"))
			{
				text = text + "score=" + (int)dictionary["GP"] + "&";
			}
			if (dictionary.ContainsKey("Guild"))
			{
				text = text + "guild=" + (string)dictionary["Guild"] + "&";
			}
			text += "data=";
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] inArray = TFUtils.Zip(contents);
			byte[] bytes2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(inArray));
			byte[] array = new byte[bytes.Length + bytes2.Length];
			string s = "a7489a81.f68a9#dae5_44#fciuee87ta11b7555a";
			using (HMACSHA256 hMACSHA = new HMACSHA256())
			{
				hMACSHA.Key = Encoding.UTF8.GetBytes(s);
				byte[] array2 = hMACSHA.ComputeHash(bytes2);
				encodedOriginal = bytes2;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array2.Length; i++)
				{
					stringBuilder.AppendFormat("{0:X2}", array2[i]);
				}
				headers.Add("Age", stringBuilder.ToString().Substring(16, 16));
			}
			tFWebClient.Headers = headers;
			foreach (string header in tFWebClient.Headers)
			{
				Debug.Log("Outbound:" + header + "-->" + tFWebClient.Headers[header]);
			}
			bytes.CopyTo(array, 0);
			bytes2.CopyTo(array, bytes.Length);
			tFWebClient.UploadDataCompleted += OnSaveComplete;
			CallbackInfo callbackInfo = new CallbackInfo();
			callbackInfo.Callback = callback;
			callbackInfo.UserData = userdata;
			tFWebClient.UploadDataAsync(new Uri(uri), "PUT", array, callbackInfo);
		}
	}

	protected void OnSaveComplete(object sender, UploadDataCompletedEventArgs e)
	{
		OnCallComplete(sender, e, false);
	}

	protected string DecodeZippedData(byte[] input)
	{
		byte[] array = null;
		string text = null;
		if ((input[0] == 117 && input[1] == 115 && input[2] == 101 && input[3] == 114) || (input[0] == 72 && input[1] == 52 && input[2] == 115))
		{
			string @string = Encoding.UTF8.GetString(input);
			int num = @string.IndexOf("&data=");
			string s = @string.Remove(0, num + 6);
			array = Convert.FromBase64String(s);
		}
		else
		{
			array = input;
		}
		try
		{
			return TFUtils.Unzip(array);
		}
		catch (ZlibException)
		{
			Debug.Log("Error uncompressing data. Returning result directly.");
			return Encoding.UTF8.GetString(array);
		}
	}

	protected string OriginalData(byte[] input)
	{
		byte[] array = null;
		string text = null;
		string @string = Encoding.UTF8.GetString(input);
		int num = @string.IndexOf("&data=");
		return @string.Remove(0, num + 6);
	}

	protected bool NeedToCheckHash(string uriaddress)
	{
		if (uriaddress.Contains("persist") && uriaddress.Contains("game"))
		{
			return true;
		}
		return false;
	}

	protected void OnCallComplete(object sender, AsyncCompletedEventArgs e, bool uncompress)
	{
		try
		{
			TFWebClient tFWebClient = (TFWebClient)sender;
			TFWebFileResponse tFWebFileResponse = new TFWebFileResponse();
			tFWebFileResponse.URI = tFWebClient.BaseAddress;
			if (e.Error == null)
			{
				bool flag = false;
				tFWebFileResponse.StatusCode = HttpStatusCode.OK;
				if (e is DownloadDataCompletedEventArgs)
				{
					DownloadDataCompletedEventArgs downloadDataCompletedEventArgs = (DownloadDataCompletedEventArgs)e;
					tFWebFileResponse.Data = DecodeZippedData(downloadDataCompletedEventArgs.Result);
					flag = NeedToCheckHash(tFWebClient.uriaddress);
				}
				else if (e is UploadDataCompletedEventArgs)
				{
					UploadDataCompletedEventArgs uploadDataCompletedEventArgs = (UploadDataCompletedEventArgs)e;
					tFWebFileResponse.Data = Encoding.UTF8.GetString(uploadDataCompletedEventArgs.Result);
				}
				tFWebFileResponse.headers = tFWebClient.ResponseHeaders;
				bool flag2 = false;
				bool flag3 = false;
				foreach (string header in tFWebFileResponse.headers)
				{
					Debug.Log(header + "-->" + tFWebFileResponse.headers[header]);
					if (!(header == "Age"))
					{
						continue;
					}
					flag2 = true;
					byte[] bytes;
					int startIndex;
					int length;
					if (flag)
					{
						DownloadDataCompletedEventArgs downloadDataCompletedEventArgs2 = (DownloadDataCompletedEventArgs)e;
						bytes = Encoding.UTF8.GetBytes(OriginalData(downloadDataCompletedEventArgs2.Result));
						startIndex = 40;
						length = 16;
					}
					else
					{
						bytes = encodedOriginal;
						startIndex = 32;
						length = 16;
					}
					if (bytes == null)
					{
						continue;
					}
					string s = "ff689bd#41e5_44fabae87theb7ea530815fa";
					using (HMACSHA256 hMACSHA = new HMACSHA256())
					{
						hMACSHA.Key = Encoding.UTF8.GetBytes(s);
						byte[] array = hMACSHA.ComputeHash(bytes);
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < array.Length; i++)
						{
							stringBuilder.AppendFormat("{0:X2}", array[i]);
						}
						if (string.Compare(tFWebFileResponse.headers[header], stringBuilder.ToString().Substring(startIndex, length), true) == 0)
						{
							flag3 = true;
						}
					}
				}
				if (flag2 && flag && !flag3)
				{
					tFWebFileResponse.StatusCode = HttpStatusCode.NotAcceptable;
					tFWebFileResponse.Data = string.Empty;
					tFWebFileResponse.NetworkDown = true;
				}
				try
				{
					string value = tFWebFileResponse.headers["Date"];
					DateTime serverTime = Convert.ToDateTime(value);
					TFUtils.UpdateServerTime(serverTime);
				}
				catch (Exception)
				{
				}
			}
			else if (e.Error.GetType().Name == "WebException")
			{
				WebException ex2 = (WebException)e.Error;
				Debug.LogError(ex2);
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
				if (httpWebResponse != null)
				{
					PopulateResponse(tFWebFileResponse, httpWebResponse);
				}
				else
				{
					tFWebFileResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
					tFWebFileResponse.NetworkDown = true;
				}
			}
			else
			{
				Debug.Log("Server returned error");
				Debug.Log(e.Error);
				tFWebFileResponse.NetworkDown = true;
				tFWebFileResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
			}
			CallbackInfo callbackInfo = (CallbackInfo)e.UserState;
			tFWebFileResponse.UserData = callbackInfo.UserData;
			callbackInfo.Callback(tFWebFileResponse);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void DeleteFile(string uri, WebHeaderCollection headers, FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Deleting file " + uri);
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(uri));
		httpWebRequest.Method = "DELETE";
		httpWebRequest.CookieContainer = cookies;
		httpWebRequest.Headers = headers;
		foreach (string header in httpWebRequest.Headers)
		{
			Debug.Log("Outbound:" + header + "-->" + httpWebRequest.Headers[header]);
		}
		try
		{
			httpWebRequest.GetResponse();
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	private void OnNetworkError(object sender, WebException e)
	{
		Debug.Log("Got webException !!!!" + e);
	}
}
