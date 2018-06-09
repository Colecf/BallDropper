using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHands : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public bool isLeftHand = false;

    public static GameObject platformTemplate = null;
    private static GameObject currentPlatform = null;
    private static Vector3 platformStart;
    private static Vector3 platformEnd;
    private static float platformWidth = 1.0f;
    private bool isStartHand = false;
    private bool drawing = false;

    public GameObject parentObj;
    private Vector3 displacementBase;
    private Vector3 scaleBase;

    class GripState
    {
        public Vector3 start;
        public Vector3 current;
        public bool active = false;
        public Vector3 delta
        {
            get { return current - start; }
        }
        public float getScaleDelta(GripState other)
        {
            return (current - other.current).magnitude - (start - other.start).magnitude;
        }
    }

    private static GripState[] gripStates = { new GripState(), new GripState() };

    private GameObject lookedAtObject = null;

    // Use this for initialization
    void Start () {
        platformTemplate = Resources.Load("PlatformBase") as GameObject;
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
                        platformStart = platformEnd = trackedObj.transform.position;
                        currentPlatform = Instantiate(platformTemplate);
                        currentPlatform.transform.position = platformStart;
                        platformWidth = 1.0f;
                        isStartHand = false;
                    }
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
                setScale(currentPlatform, platformWidth, 1f, (platformEnd - platformStart).magnitude);

                if((platformEnd - platformStart).magnitude > 0)
                    currentPlatform.transform.rotation = vectorRotationQ(new Vector3(0, 0, 1), (platformEnd - platformStart).normalized);
            }
        }

        GripState myGripState;
        GripState otherGripState;
        if (isLeftHand)
        {
            myGripState = gripStates[1];
            otherGripState = gripStates[0];
        }
        else
        {
            myGripState = gripStates[0];
            otherGripState = gripStates[1];
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            myGripState.active = true;
            myGripState.start = trackedObj.transform.localPosition;
            displacementBase = parentObj.transform.position;
            scaleBase = parentObj.transform.localScale;
        }
        if(Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            myGripState.active = false;
        }

        if(myGripState.active)
        {
            myGripState.current = trackedObj.transform.localPosition;
            if (!otherGripState.active)
            {
                parentObj.transform.position = displacementBase + -scaleBase.x*myGripState.delta;
            }
            if (otherGripState.active && isLeftHand)
            {
                parentObj.transform.localScale = scaleBase + -scaleBase.x*new Vector3(myGripState.getScaleDelta(otherGripState), myGripState.getScaleDelta(otherGripState), myGripState.getScaleDelta(otherGripState));
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
