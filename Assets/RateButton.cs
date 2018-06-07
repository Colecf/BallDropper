using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateButton : MonoBehaviour {

    private List<GameObject> intersectedObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool isPressed()
    {
        return intersectedObjects.Count > 0;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        intersectedObjects.Add(other.gameObject);
        Debug.Log("Enter");
        gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
    }

    public void OnTriggerExit(Collider other)
    {
        intersectedObjects.Remove(other.gameObject);
        if(intersectedObjects.Count == 0)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
        }
    }
}
