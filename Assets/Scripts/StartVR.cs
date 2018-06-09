using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
        UnityEngine.XR.XRSettings.enabled = true;

        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("PD", "/Unity/Tempo", 3.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
