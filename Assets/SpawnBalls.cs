using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour {

    public GameObject prefab;
    private Vector3 spawnPosition;

	// Use this for initialization
    void Start () {
        InvokeRepeating("spawnBalls", 0.0f, 3.0f);
        spawnPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
	}
	
	// Update is called once per frameu
	void Update () {
	}

    void spawnBalls(){
        Instantiate(prefab, spawnPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
    }
}
