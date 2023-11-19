using System;

namespace Prime31
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class P31DeserializeableFieldAttribute : Attribute
	{
		public readonly string key;

		public readonly bool isCollection;

		public Type type;

		public P31DeserializeableFieldAttribute(string key)
		{
			this.key = key;
		}

		public P31DeserializeableFieldAttribute(string key, Type type)
			: this(key)
		{
			this.type = type;
		}

		public P31DeserializeableFieldAttribute(string key, Type type, bool isCollection)
			: this(key, type)
		{
			this.isCollection = isCollection;
		}
	}
}
