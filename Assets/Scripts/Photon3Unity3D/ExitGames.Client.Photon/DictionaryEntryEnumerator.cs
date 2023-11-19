using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class DictionaryEntryEnumerator : IEnumerator<DictionaryEntry>, IDisposable, IEnumerator
	{
		private IDictionaryEnumerator enumerator;

		object IEnumerator.Current => (DictionaryEntry)enumerator.Current;

		public DictionaryEntry Current => (DictionaryEntry)enumerator.Current;

		public object Key => enumerator.Key;

		public object Value => enumerator.Value;

		public DictionaryEntry Entry => enumerator.Entry;

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
}
