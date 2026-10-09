using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;


public class SystemInfo : MonoBehaviour
{
    private Text txt;
    private Button SystemInfoButton;

    // Use this for initialization
    void Start()
    {
        // find Button 'SystemInfo' and attach method to onClick listener
        SystemInfoButton = GameObject.Find("SystemInfo").GetComponent<Button>();

        SystemInfoButton.GetComponent<Button>().onClick.AddListener(delegate { GetSystemInfo(); });

        // find object 'Info' for output
        txt = GameObject.Find("/Canvas/Info").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    public void GetSystemInfo()
    {
        txt.text = "Application.dataPath: " + Application.dataPath + "\n" + "Application.streamingAssetsPath: " + Application.streamingAssetsPath + "\n" +
            "Environment.OSVersion.Platform: " + Environment.OSVersion.Platform + "\n" +
                "Application.persistentDataPath: " + Application.persistentDataPath + "\n" +
                    "System.AppDomain.CurrentDomain.BaseDirectory: " + System.AppDomain.CurrentDomain.BaseDirectory + "\n" +
                        "Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory() + "\n" +
                            "Application.ExecutablePath(): " + Application.absoluteURL;

    }

}


