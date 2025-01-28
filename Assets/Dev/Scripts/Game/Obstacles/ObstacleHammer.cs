using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHammer : MonoBehaviour
{
    [Range(0, 1000)]
    [SerializeField] float hitStrength = 500;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rbPlayer))
            {
                
                Vector3 hitPoint = collision.contacts[0].point;
                rbPlayer.AddForceAtPosition(Vector3.up * hitStrength, hitPoint, ForceMode.Impulse);
               
            }
        }
    }
}
