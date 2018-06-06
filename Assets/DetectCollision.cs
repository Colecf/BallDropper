using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        print("angle: " + Vector3.Angle(vel, normal));
        print("contact point: " + collision.contacts[0].point);
        print("velocity: " + vel);
    }
}
