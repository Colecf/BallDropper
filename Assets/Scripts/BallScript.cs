using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
    private float initializationTime;
	// Use this for initialization
	void Start () {
        initializationTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        if(transform.position.y < 0 || timeSinceInitialization > 20){
            Destroy(gameObject);
        }

	}
}
