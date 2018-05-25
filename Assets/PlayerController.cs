using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;
    public float moveSpeed = 1.0f;
    float rotationY = 0.0f;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Cursor.lockState == CursorLockMode.Locked) {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0, Input.GetAxis("Vertical") * moveSpeed));

            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, -89, 89);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
	}
}
