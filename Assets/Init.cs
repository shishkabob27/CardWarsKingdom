using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour {

	int wait;
	
	// Update is called once per frame
	void FixedUpdate(){
		if (Application.isPlaying) wait++;
		if (Application.platform == RuntimePlatform.Android) Application.targetFrameRate = 60;
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		if (wait == 60 || Application.platform != RuntimePlatform.Android) SceneManager.LoadScene("BootVideoScene");
	}
}
