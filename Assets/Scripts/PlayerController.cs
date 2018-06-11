using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class PlayerController : MonoBehaviour {

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;
    public float moveSpeed = 1.0f;

    private float rotationY = 0.0f;
    private GameObject theCamera;

    public GameObject platformTemplate;
    public GameObject platformTemplate2;
    public GameObject platformTemplate3;

    public Material material1;
    public Material material2;
    public Material material3;
    public Material selectedMaterial;

    private GameObject currentPlatform = null;
    private GameObject selectedPlatform = null;

    private Vector3 platformStart;
    private float platformWidth = 1.0f;

    private GameObject lookedAtObject = null;

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.enabled = false;
        UnityEngine.XR.XRSettings.LoadDeviceByName("");

        Cursor.lockState = CursorLockMode.Locked;
        theCamera = GameObject.Find("Main Camera");
        //yield return new WaitForEndOfFrame();
        selectedPlatform = platformTemplate;
        //OSCHandler.Instance.Init();
        //OSCHandler.Instance.SendMessageToClient("PD", "/Unity/Tempo", 3.0f);
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 lookedAtPoint = transform.position + theCamera.transform.rotation * new Vector3(0, 0, 3);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;

            
            if (currentPlatform == null)
            {
                bool absorbed = false;
                foreach (GameObject o in GameObject.FindGameObjectsWithTag("Dropper"))
                {
                    if (o.GetComponent<SpawnBalls>())
                    {
                        absorbed = absorbed || o.GetComponent<SpawnBalls>().myClick();
                    }
                }
                if (!absorbed)
                {
                    platformStart = lookedAtPoint;
                    currentPlatform = Instantiate(selectedPlatform);
                    currentPlatform.transform.position = lookedAtPoint;
                    platformWidth = 1.0f;
                }
            } else {
                currentPlatform = null;
            }
        }

        if (Cursor.lockState == CursorLockMode.Locked) {
            platformWidth += Input.GetAxis("Mouse ScrollWheel");
            if(platformWidth < 0.1f) {
                platformWidth = 0.1f;
            }
            if(platformWidth > 20) {
                platformWidth = 20;
            }
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Flight") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed));

            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, -89, 89);

            transform.localEulerAngles = new Vector3(-0, rotationX, 0);
            theCamera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);

            if(!currentPlatform) {
                RaycastHit hit;
                Vector3 direction = theCamera.transform.rotation * new Vector3(0, 0, 1);
                if (Physics.Raycast(transform.position, direction, out hit, 10, LayerMask.GetMask("Platform")) &&
                    hit.collider.gameObject.CompareTag("Platform"))
                {
                    if (lookedAtObject != hit.collider.gameObject)
                    {
                        unhighlightPlatform();
                        lookedAtObject = hit.collider.gameObject;
                        lookedAtObject.GetComponent<Renderer>().material = selectedMaterial;
                    }
                } else {
                    unhighlightPlatform();
                }

                if(Input.GetMouseButtonDown(1)) {
                    if (lookedAtObject)
                    {
                        Destroy(lookedAtObject);
                        lookedAtObject = null;
                    } else
                    {
                        if (selectedPlatform == platformTemplate)
                        {
                            selectedPlatform = platformTemplate2;
                        }
                        else if (selectedPlatform == platformTemplate2)
                        {
                            selectedPlatform = platformTemplate3;
                        }
                        else
                        {
                            selectedPlatform = platformTemplate;
                        }
                    }
                }

            } else {
                currentPlatform.transform.position = (platformStart + lookedAtPoint) / 2;
                Vector3 temp = currentPlatform.transform.localScale;
                temp.z = (lookedAtPoint - platformStart).magnitude;
                temp.x = platformWidth;
                currentPlatform.transform.localScale = temp;

                if ((lookedAtPoint - platformStart).magnitude > 0)
                {
                    currentPlatform.transform.rotation = vectorRotationQ(new Vector3(0, 0, 1), (lookedAtPoint - platformStart).normalized);
                }

                unhighlightPlatform();
            }
        }
	}

    private Quaternion vectorRotationQ(Vector3 from, Vector3 target)
    {
        return Quaternion.Euler(Quaternion.LookRotation(target).eulerAngles - Quaternion.LookRotation(from).eulerAngles);
    }

    private void unhighlightPlatform()
    {
        if (lookedAtObject)
        {
            if(lookedAtObject.name.Contains("PlatformBase3")){
                lookedAtObject.GetComponent<Renderer>().material = material3;
            }
            else if(lookedAtObject.name.Contains("PlatformBase2")){
                lookedAtObject.GetComponent<Renderer>().material = material2;
            }
            else{
                lookedAtObject.GetComponent<Renderer>().material = material1;
            }
        }
        lookedAtObject = null;
    }
}
