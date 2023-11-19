// ExitGames.Client.Photon.DictionaryEntryEnumerator
using System;
using System.Collections;
using System.Collections.Generic;

public class DictionaryEntryEnumerator : IEnumerator<DictionaryEntry>, IDisposable, IEnumerator
{
	private IDictionaryEnumerator enumerator;

	object IEnumerator.Current
	{
		get
		{
			return (DictionaryEntry)enumerator.Current;
		}
	}

	public DictionaryEntry Current
	{
		get
		{
			return (DictionaryEntry)enumerator.Current;
		}
	}

	public object Key
	{
		get
		{
			return enumerator.Key;
		}
	}

	public object Value
	{
		get
		{
			return enumerator.Value;
		}
	}

	public DictionaryEntry Entry
	{
		get
		{
			return enumerator.Entry;
		}
	}

	public DictionaryEntryEnumerator(IDictionaryEnumerator original)
	{
		enumerator = original;
	}

	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	public void Reset()
	{
		enumerator.Reset();
	}

	public void Dispose()
	{
		enumerator = null;
	}
}
