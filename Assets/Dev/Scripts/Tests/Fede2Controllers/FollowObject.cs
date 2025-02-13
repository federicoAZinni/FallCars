using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool followRotation = true;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.position;
        if (!followRotation) return;
        transform.rotation = Quaternion.Euler(0, target.rotation.eulerAngles.y, 0);
    }
}
