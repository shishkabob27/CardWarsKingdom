using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWClient
{
	public delegate void FinishedDelegate(WWW www);

	public delegate void DisposedDelegate();

	private MonoBehaviour mMonoBehaviour;

	private string mUrl;

	private WWW mWww;

	private WWWForm mForm;

	private Dictionary<string, string> mHeaders;

	private float mTimeout;

	private FinishedDelegate mOnDone;

	private FinishedDelegate mOnFail;

	private DisposedDelegate mOnDisposed;

	private bool mDisposed;

	public Dictionary<string, string> Headers
	{
		get
		{
			return mHeaders;
		}
		set
		{
			mHeaders = value;
		}
	}

	public float Timeout
	{
		get
		{
			return mTimeout;
		}
		set
		{
			mTimeout = value;
		}
	}

	public FinishedDelegate OnDone
	{
		set
		{
			mOnDone = value;
		}
	}

	public FinishedDelegate OnFail
	{
		set
		{
			mOnFail = value;
		}
	}

	public DisposedDelegate OnDisposed
	{
		set
		{
			mOnDisposed = value;
		}
	}

	public WWWClient(MonoBehaviour monoBehaviour, string url)
	{
		mMonoBehaviour = monoBehaviour;
		mUrl = url;
		mHeaders = new Dictionary<string, string>();
		mForm = new WWWForm();
		mTimeout = -1f;
		mDisposed = false;
	}

	public void AddHeader(string headerName, string value)
	{
		mHeaders.Add(headerName, value);
	}

	public void AddData(string fieldName, string value)
	{
		mForm.AddField(fieldName, value);
	}

	public void AddBinaryData(string fieldName, byte[] contents)
	{
		mForm.AddBinaryData(fieldName, contents);
	}

	public void AddBinaryData(string fieldName, byte[] contents, string fileName)
	{
		mForm.AddBinaryData(fieldName, contents, fileName);
	}

	public void AddBinaryData(string fieldName, byte[] contents, string fileName, string mimeType)
	{
		mForm.AddBinaryData(fieldName, contents, fileName, mimeType);
	}

	public void Request()
	{
		mMonoBehaviour.StartCoroutine(RequestCoroutine());
	}

	public void Dispose()
	{
		if (mWww != null && !mDisposed)
		{
			mWww.Dispose();
			mDisposed = true;
		}
	}

	private IEnumerator RequestCoroutine()
	{
		if (mForm.data.Length > 0)
		{
			mWww = new WWW(mUrl, mForm.data, mHeaders);
		}
		else
		{
			mWww = new WWW(mUrl, null, mHeaders);
		}
		yield return mMonoBehaviour.StartCoroutine(CheckTimeout());
		if (mDisposed)
		{
			if (mOnDisposed != null)
			{
				mOnDisposed();
			}
		}
		else if (string.IsNullOrEmpty(mWww.error))
		{
			if (mOnDone != null)
			{
				mOnDone(mWww);
			}
		}
		else if (mOnFail != null)
		{
			mOnFail(mWww);
		}
	}

	private IEnumerator CheckTimeout()
	{
		float startTime = Time.time;
		while (!mDisposed && !mWww.isDone)
		{
			if (mTimeout > 0f && Time.time - startTime >= mTimeout)
			{
				Dispose();
				break;
			}
			yield return null;
		}
		yield return null;
	}
}
