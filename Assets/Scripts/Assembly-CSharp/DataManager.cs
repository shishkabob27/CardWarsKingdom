using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MiniJSON;
using UnityEngine;

public abstract class DataManager<T> : IDataManager where T : ILoadableData
{
	private const string parttwo = "210429q";

	private const string partfour = "bmmfzDb";

	private bool isLoaded;

	private string partone = "_ijmMW";

	private string partthree = "m201510";

	protected Dictionary<string, T> Database = new Dictionary<string, T>();

	protected List<T> DatabaseArray = new List<T>();

	protected List<IDataManager> Dependencies = new List<IDataManager>();

	protected static object threadLock;

	private bool doneLoadingAndParsingJsonData;

	private bool donePostLoad;

	private Exception ex;

	public string FilePath { get; set; }

	public bool IsLoaded
	{
		get
		{
			return isLoaded;
		}
		set
		{
			isLoaded = value;
		}
	}

	private bool ExceptionThrown
	{
		get
		{
			return null != ex;
		}
	}

	private void PrintException(Exception ex)
	{
		if (ex == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(ex.ToString());
		for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
		{
			stringBuilder.AppendLine(string.Format("\tInnerException: {0}", innerException.ToString()));
		}
		foreach (object key in ex.Data.Keys)
		{
			stringBuilder.AppendLine("****** Extra Data ******");
			stringBuilder.AppendLine(string.Format("[{0}] = {1}", key, ex.Data[key]));
		}
	}

	private void CheckAndThrowExeptions()
	{
		if (ex != null)
		{
			PrintException(ex);
			throw ex;
		}
	}

	public IEnumerator Load()
	{
		IsLoaded = false;
		doneLoadingAndParsingJsonData = false;
		donePostLoad = false;
		ex = null;
		if (threadLock == null)
		{
			threadLock = new object();
		}
		IDataManager manager2 = default(IDataManager);
		foreach (IDataManager manager in Dependencies)
		{
			new Thread((ThreadStart)delegate
			{
				loadDependencyThread(manager2);
			}).Start();
			while (!manager.IsLoaded && !ExceptionThrown)
			{
				yield return null;
			}
			CheckAndThrowExeptions();
		}
		string appliedFilePath = SessionManager.Instance.GetStreamingAssetsPath(FilePath);
		if (appliedFilePath.Contains("://"))
		{
			WWW www = new WWW(appliedFilePath);
			while (!www.isDone)
			{
				yield return null;
			}
			string jsonText = www.text;
			LoadAndParseJsonDataThread(appliedFilePath, jsonText);
		}
		else
		{
			LoadAndParseJsonDataThread(appliedFilePath);
		}
		while (!doneLoadingAndParsingJsonData && !ExceptionThrown)
		{
			yield return null;
		}
		CheckAndThrowExeptions();
		IsLoaded = true;
		new Thread((ThreadStart)delegate
		{
			PostLoadThread();
		}).Start();
		while (!donePostLoad && !ExceptionThrown)
		{
			yield return null;
		}
		CheckAndThrowExeptions();
	}

	private void loadDependencyThread(IDataManager manager)
	{
		lock (threadLock)
		{
			try
			{
				if (manager != null) manager.Load();
			}
			catch (Exception ex)
			{
				if (manager != null)
				{
					ex.Data.Add("Manager", manager.ToString());
				}
				this.ex = ex;
			}
		}
	}

	private void LoadAndParseJsonDataThread(string appliedFilePath)
	{
		LoadAndParseJsonDataThread(appliedFilePath, null);
	}

	private void LoadAndParseJsonDataThread(string appliedFilePath, string wwwText)
	{
		lock (threadLock)
		{
			try
			{
				doneLoadingAndParsingJsonData = false;
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (!appliedFilePath.Contains("://"))
				{
					empty = ((!appliedFilePath.EndsWith("asset_bundle_settings.json")) ? RijndaelCrypto.DecryptBase64(appliedFilePath, "210429qbmmfzDb" + partone + partthree) : File.ReadAllText(appliedFilePath));
				}
				else
				{
					empty2 = ((wwwText == null) ? TFUtils.getJsonTextFromWWW(appliedFilePath) : wwwText);
					empty = ((!appliedFilePath.EndsWith("asset_bundle_settings.json")) ? RijndaelCrypto.DecryptBase64Text(empty2, "210429qbmmfzDb" + partone + partthree) : empty2);
				}
				try
				{
					List<object> jlist = (List<object>)Json.Deserialize(empty);
					ParseRows(jlist);
					doneLoadingAndParsingJsonData = true;
				}
				catch
				{
					List<object> list = new List<object>();
					object item = Json.Deserialize(empty);
					list.Add(item);
					ParseRows(list);
					doneLoadingAndParsingJsonData = true;
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("Filename", appliedFilePath);
				this.ex = ex;
			}
		}
	}

	private void PostLoadThread()
	{
		lock (threadLock)
		{
			try
			{
				donePostLoad = false;
				PostLoad();
				donePostLoad = true;
			}
			catch (Exception ex)
			{
				Exception ex2 = (this.ex = ex);
			}
		}
	}

	protected virtual void ParseRows(List<object> jlist)
	{
		foreach (object item in jlist)
		{
			if (item == null)
			{
				Debug.LogError("Item was Null");
                continue;
            }
			if (item is Dictionary<string, object>)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)item;
				T val = (T)Activator.CreateInstance(typeof(T));
				val.Populate(dict);
				if (val.ID != string.Empty && !Database.ContainsKey(val.ID))
				{
					Database.Add(val.ID, val);
				}
				DatabaseArray.Add(val);
			}
			else{
                Debug.LogError("Unexpected data type: " + item.GetType().FullName);
            }
        }
			
	}

	protected virtual void PostLoad()
	{
	}

	public void AddDependency(IDataManager manager)
	{
		if (Dependencies == null)
		{
			Dependencies = new List<IDataManager>();
		}
		Dependencies.Add(manager);
	}

	public T GetData(string ID)
	{
		if (Database.ContainsKey(ID))
		{
			return Database[ID];
		}
		return default(T);
	}

	public T GetData(int index)
	{
		if (index < 0 || index >= DatabaseArray.Count)
		{
			return default(T);
		}
		return DatabaseArray[index];
	}

	public int GetIndex(T data)
	{
		return DatabaseArray.IndexOf(data);
	}

	public T Find(Predicate<T> match)
	{
		return DatabaseArray.Find(match);
	}

	public List<T> GetDatabase()
	{
		return DatabaseArray;
	}

	public virtual void Unload()
	{
		Database.Clear();
		DatabaseArray.Clear();
		IsLoaded = false;
		doneLoadingAndParsingJsonData = false;
		donePostLoad = false;
		ex = null;
	}
}
