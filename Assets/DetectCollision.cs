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

        OSCHandler.Instance.SendMessageToClient("PD", "/unity/AngleContactPointVelocity", angleContactPointVelocity);
    }
}
