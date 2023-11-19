using System.Collections.Generic;

namespace Prime31
{
	public class JsonArray : List<object>
	{
		public JsonArray()
		{
		}

		public JsonArray(int capacity)
			: base(capacity)
		{
		}

		public override string ToString()
		{
			return JsonFormatter.prettyPrint(SimpleJson.encode(this)) ?? string.Empty;
		}
	}
}
