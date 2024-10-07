using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFallGround : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float timeToFall;


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            //LeanTween.scaleZ(gameObject, transform.localScale.z * 1.3f, 0.5f).setLoopPingPong(1).setEaseInBounce();
            //LeanTween.scaleX(gameObject, transform.localScale.x * 1.3f, 0.5f).setLoopPingPong(1).setEaseInBounce();
            //LeanTween.delayedCall(timeToFall, () => { rb.constraints = RigidbodyConstraints.None; });


            LeanTween.scaleZ(gameObject, transform.localScale.z *1.2f, 0.25f).setLoopPingPong(1).setEaseOutBack();
            LeanTween.scaleX(gameObject, transform.localScale.x * 1.2f, 0.25f).setLoopPingPong(1).setEaseOutBack();
            LeanTween.delayedCall(timeToFall, () => {
                LeanTween.scaleZ(gameObject, transform.localScale.z * 0, 0.5f).setEaseInBack();
                LeanTween.scaleX(gameObject, transform.localScale.x * 0, 0.5f).setEaseInBack();
                rb.constraints = RigidbodyConstraints.None; });
        }
    }
}
