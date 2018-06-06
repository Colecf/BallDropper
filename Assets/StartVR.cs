using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
