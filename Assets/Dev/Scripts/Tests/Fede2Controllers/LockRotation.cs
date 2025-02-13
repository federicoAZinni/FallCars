using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
   [SerializeField] SuspensionCarController car;

    
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,car.transform.rotation.eulerAngles.y,0);
        // transform.rotation = Quaternion.Euler(0, 0, car.transform.rotation.eulerAngles.z);
    }
}
