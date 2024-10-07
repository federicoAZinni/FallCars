using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHammer : MonoBehaviour
{

    //Rigidbody r;

    //private void Start()
    //{
    //    r = GetComponent<Rigidbody>();
    //}
    //private void Update()
    //{
    //    r.AddTorque(transform.forward*50, ForceMode.Force);
    //    r.maxLinearVelocity = 500;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rbPlayer))
            {
                rbPlayer.AddForce(Vector3.up * 25000, ForceMode.Impulse);
            }
        }
    }
}
