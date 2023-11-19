using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class InvocationCache
	{
		private class CachedOperation
		{
			public int InvocationId { get; set; }

			public Action Action { get; set; }
		}

		private readonly LinkedList<CachedOperation> cache = new LinkedList<CachedOperation>();

		private int nextInvocationId = 1;

		public int NextInvocationId => nextInvocationId;

		public int Count => cache.Count;

		public void Reset()
		{
			lock (cache)
			{
				nextInvocationId = 1;
				cache.Clear();
			}
		}

		public void Invoke(int invocationId, Action action)
		{
			lock (cache)
			{
				if (invocationId < nextInvocationId)
				{
					return;
				}
				if (invocationId == nextInvocationId)
				{
					nextInvocationId++;
					action();
					if (cache.Count > 0)
					{
						LinkedListNode<CachedOperation> linkedListNode = cache.First;
						while (linkedListNode != null && linkedListNode.Value.InvocationId == nextInvocationId)
						{
							nextInvocationId++;
							linkedListNode.Value.Action();
							linkedListNode = linkedListNode.Next;
							cache.RemoveFirst();
						}
					}
					return;
				}
				CachedOperation value = new CachedOperation
				{
					InvocationId = invocationId,
					Action = action
				};
				if (cache.Count == 0)
				{
					cache.AddLast(value);
					return;
				}
				for (LinkedListNode<CachedOperation> linkedListNode2 = cache.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
				{
					if (linkedListNode2.Value.InvocationId > invocationId)
					{
						cache.AddBefore(linkedListNode2, value);
						return;
					}
				}
				cache.AddLast(value);
			}
		}
	}
}
