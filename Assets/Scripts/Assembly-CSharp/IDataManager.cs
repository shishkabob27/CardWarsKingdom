using System.Collections;

public interface IDataManager
{
	bool IsLoaded { get; set; }

	IEnumerator Load();

	void Unload();
}
