using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour {

	int wait;

	void Start(){
		if (Application.platform == RuntimePlatform.Android) Application.targetFrameRate = 60;
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
	}
	
	// Update is called once per frame
	void FixedUpdate(){
		if (Application.isPlaying) wait++;
		if (wait == 60 || Application.platform != RuntimePlatform.Android) SceneManager.LoadScene("BootVideoScene");
	}
}
