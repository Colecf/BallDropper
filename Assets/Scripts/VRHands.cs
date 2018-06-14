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
    public static GameObject platformTemplate2 = null;
    public static GameObject platformTemplate3 = null;

    public GameObject dropper;

    private static GameObject currentObject = null;
    private static Vector3 platformStart;
    private static Vector3 platformEnd;
    private static float platformWidth = 1.0f;
    private bool isStartHand = false;
    private bool drawing = false;
    private static bool dropperMode = false;

    private bool wasActiveLastFrame = false;

    private bool isDropperHand = false;

    public Material material1;
    public Material material2;
    public Material material3;
    public Material material4;
    public Material selectedPlatformMaterial;
    public Material selectedDropperMaterial;

    private GameObject selectedPlatform = null;

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
        platformTemplate2 = Resources.Load("PlatformBase2") as GameObject;
        platformTemplate3 = Resources.Load("PlatformBase3") as GameObject;
        selectedPlatform = platformTemplate;
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
        if(Controller.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) && isLeftHand && !wasActiveLastFrame){
            dropperMode = !dropperMode;
            wasActiveLastFrame = true;
        }
        if (!Controller.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) && isLeftHand)
        {
            wasActiveLastFrame = false;
        }
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) && !isLeftHand && !wasActiveLastFrame && !dropperMode)
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
        if (!Controller.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) && !isLeftHand)
        {
            wasActiveLastFrame = false;
        }
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (lookedAtObject)
            {
                Destroy(lookedAtObject);
                lookedAtObject = null;
            }
            else
            {
                if (currentObject == null)
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
                        if(dropperMode){
                            currentObject = Instantiate(dropper);
                            isDropperHand = true;
                        }
                        else{
                            currentObject = Instantiate(platformTemplate);
                        }
                        currentObject.transform.position = platformStart;
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
            if (isDropperHand)
            {
                currentObject = null;
                isDropperHand = false;
            }
            if (!isStartHand)
            {
                currentObject = null;
            }
            drawing = false;
        }

        if (currentObject)
        {
            if (currentObject.name.Contains("Dropper") && isDropperHand) 
            {
                currentObject.transform.position = trackedObj.transform.position;
            }
            else if(currentObject.name.Contains("Platform")){
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
                    currentObject.transform.position = (platformStart + platformEnd) / 2;
                    setScale(currentObject, platformWidth, 1f, (platformEnd - platformStart).magnitude);

                    if ((platformEnd - platformStart).magnitude > 0)
                        currentObject.transform.rotation = vectorRotationQ(new Vector3(0, 0, 1), (platformEnd - platformStart).normalized);
                }
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
            lookedAtObject.GetComponent<Renderer>().material = selectedPlatformMaterial;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!drawing && lookedAtObject == null && other.gameObject.CompareTag("Platform"))
        {
            lookedAtObject = other.gameObject;
            lookedAtObject.GetComponent<Renderer>().material = selectedPlatformMaterial;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        unhighlight();
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

    private void unhighlight()
    {
        if (lookedAtObject)
        {
            if (lookedAtObject.name.Contains("PlatformBase3"))
            {
                lookedAtObject.GetComponent<Renderer>().material = material3;
            }
            else if (lookedAtObject.name.Contains("PlatformBase2"))
            {
                lookedAtObject.GetComponent<Renderer>().material = material2;
            }
            else if (lookedAtObject.name.Contains("Dropper"))
            {
                lookedAtObject.GetComponent<Renderer>().material = material4;
            }
            else
            {
                lookedAtObject.GetComponent<Renderer>().material = material1;
            }
        }
        lookedAtObject = null;
    }
}
