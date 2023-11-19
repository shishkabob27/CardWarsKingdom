using System.Collections.Generic;

public interface ILoadableData
{
	string ID { get; }

	void Populate(Dictionary<string, object> dict);
}
