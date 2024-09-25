using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHammer : MonoBehaviour
{

    Rigidbody r;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        r.AddTorque(transform.forward*50, ForceMode.Force);
        r.maxLinearVelocity = 500;
    }
}
