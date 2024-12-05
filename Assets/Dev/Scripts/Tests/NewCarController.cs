using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.iOS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NewCarController : MonoBehaviour
{
    float currentspeed;
    [SerializeField] float speed;
    [SerializeField] float speedRot;
    [SerializeField] float speedRotLerp;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform carMesh; 
    [SerializeField] Transform sphereCollider; 
    float currentRotate;
    [SerializeField] LayerMask normalInteractMask;
    public Transform carNormal;
    bool isOnGround;


    private void Update()
    {
        currentspeed = Input.GetAxis("Vertical") * speed;

        currentRotate =  Input.GetAxis("Horizontal")* speedRot;

        carMesh.transform.position = sphereCollider.position - new Vector3(0, 1, 0);

        if (Input.GetKey(KeyCode.Space)) rb.drag = 0.5f;
        else rb.drag = 2f;




    }


    void AlignWithGround()
    {
        if (Physics.Raycast(carMesh.transform.position, Vector3.down, out RaycastHit hit, 0.2f, normalInteractMask))
        {
            isOnGround = true;
            Quaternion targetRotation = Quaternion.FromToRotation(carMesh.transform.up, hit.normal) * carMesh.transform.localRotation;
            carMesh.transform.localRotation = Quaternion.Lerp(carMesh.transform.localRotation, targetRotation,Time.deltaTime*10) ;
        }
        else
        {
            isOnGround = false;
        }

    }

    private void FixedUpdate()
    {
        rb.AddForce(carMesh.forward * currentspeed, ForceMode.Acceleration);
        carMesh.transform.Rotate(0, currentRotate, 0);
        AlignWithGround();

    }
}
