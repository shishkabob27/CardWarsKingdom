using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleJSON
{
	public class JSONArray : JSONNode, IEnumerable
	{
		private List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return m_List[aIndex];
			}
			set
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					m_List.Add(value);
				}
				else
				{
					m_List[aIndex] = value;
				}
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				m_List.Add(value);
			}
		}

		public override int Count
		{
			get
			{
				return m_List.Count;
			}
		}

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				foreach (JSONNode item in m_List)
				{
					yield return item;
				}
			}
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			m_List.Add(aItem);
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
			{
				return null;
			}
			JSONNode result = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return result;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode item in m_List)
			{
				yield return item;
			}
		}

		public override string ToString()
		{
			string text = "[ ";
			foreach (JSONNode item in m_List)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				text += item.ToString();
			}
			return text + " ]";
		}

		public override string ToString(string aPrefix)
		{
			string text = "[ ";
			foreach (JSONNode item in m_List)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text += item.ToString(aPrefix + "   ");
			}
			return text + "\n" + aPrefix + "]";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)1);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}
	}
}
