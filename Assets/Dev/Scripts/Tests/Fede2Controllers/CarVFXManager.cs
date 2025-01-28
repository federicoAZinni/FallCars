using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVFXManager : MonoBehaviour
{
   [SerializeField] TrailRenderer trailRendererRight;
   [SerializeField] TrailRenderer trailRendererLeft;
   [SerializeField] SuspensionCarController carController;

    void Update()
    {
        if (carController.isGrounded && carController.handBrake)
        {
            LeaveTrail();
        }
        else
        {
            StopLeaveTrail();
        }
    }
    
    public void LeaveTrail()
    {
        trailRendererRight.emitting = true;
        trailRendererLeft.emitting = true;
    }
    public void StopLeaveTrail()
    {
        trailRendererRight.emitting = false;
        trailRendererLeft.emitting = false;
    }
   

}
