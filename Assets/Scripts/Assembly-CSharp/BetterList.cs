using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BetterList<T> : IEnumerable, ICollection, ICollection<T>, IEnumerable<T>
{
	public delegate int CompareFunc(T left, T right);

	public T[] buffer;

	public int size;

	public int Count
	{
		get
		{
			return size;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public bool IsSynchronized
	{
		get
		{
			return false;
		}
	}

	public object SyncRoot
	{
		get
		{
			return this;
		}
	}

	[DebuggerHidden]
	public T this[int i]
	{
		get
		{
			return buffer[i];
		}
		set
		{
			buffer[i] = value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.CopyTo(T[] myArr, int index)
	{
		T[] array = buffer;
		foreach (T val in array)
		{
			myArr.SetValue(val, index);
			index++;
		}
	}

	void ICollection.CopyTo(Array myArr, int index)
	{
		T[] array = buffer;
		foreach (T val in array)
		{
			myArr.SetValue(val, index);
			index++;
		}
	}

	[DebuggerStepThrough]
	public IEnumerator<T> GetEnumerator()
	{
		if (buffer != null)
		{
			for (int i = 0; i < size; i++)
			{
				yield return buffer[i];
			}
		}
	}

	private void AllocateMore()
	{
		T[] array = ((buffer == null) ? new T[32] : new T[Mathf.Max(buffer.Length << 1, 32)]);
		if (buffer != null && size > 0)
		{
			buffer.CopyTo(array, 0);
		}
		buffer = array;
	}

	private void Trim()
	{
		if (size > 0)
		{
			if (size < buffer.Length)
			{
				T[] array = new T[size];
				for (int i = 0; i < size; i++)
				{
					array[i] = buffer[i];
				}
				buffer = array;
			}
		}
		else
		{
			buffer = null;
		}
	}

	public void Clear()
	{
		size = 0;
	}

	public void Release()
	{
		size = 0;
		buffer = null;
	}

	public void Add(T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore();
		}
		buffer[size++] = item;
	}

	public void Insert(int index, T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore();
		}
		if (index > -1 && index < size)
		{
			for (int num = size; num > index; num--)
			{
				buffer[num] = buffer[num - 1];
			}
			buffer[index] = item;
			size++;
		}
		else
		{
			Add(item);
		}
	}

	public bool Contains(T item)
	{
		if (buffer == null)
		{
			return false;
		}
		for (int i = 0; i < size; i++)
		{
			if (buffer[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public int IndexOf(T item)
	{
		if (buffer == null)
		{
			return -1;
		}
		for (int i = 0; i < size; i++)
		{
			if (buffer[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public bool Remove(T item)
	{
		if (buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < size; i++)
			{
				if (@default.Equals(buffer[i], item))
				{
					size--;
					buffer[i] = default(T);
					for (int j = i; j < size; j++)
					{
						buffer[j] = buffer[j + 1];
					}
					buffer[size] = default(T);
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		if (buffer != null && index > -1 && index < size)
		{
			size--;
			buffer[index] = default(T);
			for (int i = index; i < size; i++)
			{
				buffer[i] = buffer[i + 1];
			}
			buffer[size] = default(T);
		}
	}

	public T Pop()
	{
		if (buffer != null && size != 0)
		{
			T result = buffer[--size];
			buffer[size] = default(T);
			return result;
		}
		return default(T);
	}

	public T[] ToArray()
	{
		Trim();
		return buffer;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public void Sort(CompareFunc comparer)
	{
		int num = 0;
		int num2 = size - 1;
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = num; i < num2; i++)
			{
				if (comparer(buffer[i], buffer[i + 1]) > 0)
				{
					T val = buffer[i];
					buffer[i] = buffer[i + 1];
					buffer[i + 1] = val;
					flag = true;
				}
				else if (!flag)
				{
					num = ((i != 0) ? (i - 1) : 0);
				}
			}
		}
	}
}
