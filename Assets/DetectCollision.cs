using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class DetectCollision : MonoBehaviour {
    public Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 vel = rb.velocity;
        Vector3 normal = collision.contacts[0].normal;
        float angle = Vector3.Angle(vel, normal);
        float velMag = vel.magnitude;
        Vector3 contactPoint = collision.contacts[0].point;

        List<float> angleContactPointVelocity = new List<float>();

        angleContactPointVelocity.Add(angle);
        angleContactPointVelocity.Add(contactPoint.y);
        angleContactPointVelocity.Add(velMag);
        string other = collision.contacts[0].otherCollider.name;
        float platformType = -1.0f;
        if(other.Contains("PlatformBase2")){
            platformType = 2.0f;
        }
        else if(other.Contains("PlatformBase3")){
            platformType = 3.0f;
        }
        else{
            platformType = 1.0f;
        }

        angleContactPointVelocity.Add(platformType);
        //OSCHandler.Instance.SendMessageToClient("PD", "/unity/AngleContactPointVelocity", angleContactPointVelocity);
    }
}
