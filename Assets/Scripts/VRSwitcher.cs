using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSwitcher : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //UnityEngine.XR.XRSettings.enabled = false;
        //UnityEngine.XR.XRSettings.LoadDeviceByName("");
        string[] args = System.Environment.GetCommandLineArgs();
        //string[] args = { "mygame.exe", "-vr"};
        bool inVr = false;
        foreach(string arg in args)
        {
            if(arg == "-vr")
            {
                inVr = true;
                SceneManager.LoadScene("VR");
                break;
            }
        }
        if(!inVr)
        {
            SceneManager.LoadScene("SampleScene");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
