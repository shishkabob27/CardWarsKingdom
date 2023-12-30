using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour {

	int wait;
	
	// Update is called once per frame
	void FixedUpdate(){
		if (Application.isPlaying) wait++;
		if (wait == 60 || Application.platform != RuntimePlatform.Android) SceneManager.LoadScene("StartupScene");
	}
}
