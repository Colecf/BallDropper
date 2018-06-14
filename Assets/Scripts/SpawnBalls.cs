using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour {

    public GameObject prefab;
    public GameObject increaseButton = null;
    public GameObject decreaseButton = null;
    public float tempoMin = 0.2f;
    public float tempoMax = 10f;
    private Vector3 spawnPosition;
    private bool isMasterDropper = false;
    private float spawnSpeed = 3.0f;

	// Use this for initialization
    void Start () {
        Invoke("spawnBalls", spawnSpeed);
        spawnPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
	}

    // Update is called once per frameu
    void Update()
    {
        spawnPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
    }

    public bool myClick()
    {
        bool i = increaseButton && increaseButton.GetComponent<RateButton>().isPressed();
        bool d = decreaseButton && decreaseButton.GetComponent<RateButton>().isPressed();
        if (i)
        {
            spawnSpeed *= 0.8f;
        }
        if(d)
        {
            spawnSpeed *= 1.2f;
        }
        if (isMasterDropper) {
            if (spawnSpeed < tempoMin) spawnSpeed = tempoMin;
            if (spawnSpeed > tempoMax) spawnSpeed = tempoMax;
            OSCHandler.Instance.SendMessageToClient("PD", "/unity/Tempo", spawnSpeed);
        }
        return i || d;
    }
    void spawnBalls(){
        Instantiate(prefab, spawnPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
        Invoke("spawnBalls", spawnSpeed);
    }
}
