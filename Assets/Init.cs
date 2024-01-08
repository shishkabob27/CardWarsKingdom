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
		#if UNITY_EDITOR
		return;
		#endif
		if (Application.isPlaying) wait++;
		if (wait >= 60 || Application.platform != RuntimePlatform.Android) Proceed();
	}

	void Proceed(){
		SceneManager.LoadScene("BootVideoScene");
	}

	#if UNITY_EDITOR
    private void OnGUI()
    {
        // Set up button dimensions and positions
        int buttonWidth = 100;
        int buttonHeight = 50;
        int spacing = 10;

        // Calculate positions for buttons
        int xPosition = (Screen.width / 2) - (buttonWidth + spacing);
        int yPosition = Screen.height / 2;

        // Draw Local button
        if (GUI.Button(new Rect(xPosition, yPosition, buttonWidth, buttonHeight), "Local"))
        {
            // Code to execute when Local button is clicked
            SQSettings.useLocalServer = true;
			Proceed();
        }

        // Draw Remote button
        xPosition += buttonWidth + spacing;
        if (GUI.Button(new Rect(xPosition, yPosition, buttonWidth, buttonHeight), "Remote"))
        {
            // Code to execute when Remote button is clicked
            SQSettings.useLocalServer = false;
			Proceed();
        }
    }
	#endif
}
