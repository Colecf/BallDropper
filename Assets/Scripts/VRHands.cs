using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHands : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public static GameObject platformTemplate = null;
    private static GameObject currentPlatform = null;
    private static Vector3 platformStart;
    private static Vector3 platformEnd;
    private static float platformWidth = 1.0f;
    private bool isStartHand = false;
    private bool drawing = false;

    private GameObject lookedAtObject = null;

    // Use this for initialization
    void Start () {
        platformTemplate = Resources.Load("PlatformBase") as GameObject;
        Debug.Log("Start");
        
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            platformWidth = (Controller.GetAxis().y + 1) * 10;

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (lookedAtObject)
            {
                Destroy(lookedAtObject);
                lookedAtObject = null;
            }
            else
            {
                Debug.Log("Clicked");
                if (currentPlatform == null)
                {
                    platformStart = platformEnd = trackedObj.transform.position;
                    currentPlatform = Instantiate(platformTemplate);
                    currentPlatform.transform.position = platformStart;
                    platformWidth = 1.0f;
                    isStartHand = false;
                    setScale(currentPlatform, 0.2f, 0.2f, 0.2f);
                }
                else
                {
                    isStartHand = true;
                }
                drawing = true;
            }
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (!isStartHand)
            {
                currentPlatform = null;
            }
            drawing = false;
        }

        if (currentPlatform)
        {
            if (isStartHand && drawing)
            {
                platformStart = trackedObj.transform.position;
            }
            else if (!isStartHand && drawing)
            {
                platformEnd = trackedObj.transform.position;
            }

            if (!isStartHand)
            {
                currentPlatform.transform.position = (platformStart + platformEnd) / 2;
                setScale(currentPlatform, platformWidth*0.2f, 0.2f, (platformEnd - platformStart).magnitude);

                currentPlatform.transform.rotation = vectorRotationQ(new Vector3(0, 0, 1), (platformEnd - platformStart).normalized);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!drawing && lookedAtObject == null && other.gameObject.CompareTag("Platform"))
        {
            lookedAtObject = other.gameObject;
            lookedAtObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!drawing && lookedAtObject == null && other.gameObject.CompareTag("Platform"))
        {
            lookedAtObject = other.gameObject;
            lookedAtObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lookedAtObject)
        {
            lookedAtObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            lookedAtObject = null;
        }
    }

    private Quaternion vectorRotationQ(Vector3 from, Vector3 target)
    {
        return Quaternion.Euler(Quaternion.LookRotation(target).eulerAngles - Quaternion.LookRotation(from).eulerAngles);
    }

    private void setScale(GameObject o, float x, float y, float z)
    {
        Vector3 temp = o.transform.localScale;
        temp.z = z;
        temp.x = x;
        temp.y = y;
        o.transform.localScale = temp;
    }

    private void unhighlightPlatform()
    {
        if (lookedAtObject)
        {
            lookedAtObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        lookedAtObject = null;
    }
}
