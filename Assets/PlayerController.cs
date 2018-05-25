using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;
    public float moveSpeed = 1.0f;
    private float rotationY = 0.0f;
    public GameObject platformTemplate;
    private GameObject currentPlatform = null;
    private Vector3 platformStart;
    private bool drawingPlatform = false;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 lookedAtPoint = transform.position + transform.rotation * new Vector3(0, 0, 3);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (!drawingPlatform)
            {
                platformStart = lookedAtPoint;
                currentPlatform = Instantiate(platformTemplate);
                currentPlatform.transform.position = lookedAtPoint;
                drawingPlatform = true;
            } else {
                drawingPlatform = false;
            }
        }
        if(drawingPlatform) {
            currentPlatform.transform.position = new Vector3((platformStart.x + lookedAtPoint.x) / 2,
                                                             (platformStart.y + lookedAtPoint.y) / 2,
                                                             (platformStart.z + lookedAtPoint.z) / 2);
            Vector3 temp = currentPlatform.transform.localScale;
            temp.z = (lookedAtPoint - platformStart).magnitude;
            currentPlatform.transform.localScale = temp;

            currentPlatform.transform.rotation = vectorRotationQ(new Vector3(0, 0, 1), (lookedAtPoint - platformStart).normalized);
        }
        if (Cursor.lockState == CursorLockMode.Locked) {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0, Input.GetAxis("Vertical") * moveSpeed));

            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, -89, 89);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
	}
    public Quaternion vectorRotationQ(Vector3 from, Vector3 target)
    {
        return Quaternion.Euler(Quaternion.LookRotation(target).eulerAngles - Quaternion.LookRotation(from).eulerAngles);
    }
}
