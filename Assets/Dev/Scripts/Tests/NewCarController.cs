using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCarController : MonoBehaviour
{
    #region variables
    float currentspeed;
    [SerializeField] float speed;
    [SerializeField] float speedRot;
    [SerializeField] float speedRotLerp;
    [Range(0, 10)]
    [SerializeField] float rotSpeed;
    [SerializeField] Rigidbody sphereRb;
    public Rigidbody boxRb;
    [SerializeField] Transform carMesh; 
    [SerializeField] Transform sphereCollider; 
    [SerializeField] Transform carTop;

    [SerializeField] float rayDist;
    float currentRotate;
    [SerializeField] LayerMask normalInteractMask;
    public Transform carNormal;
    bool isOnGround;
    RaycastHit hit;
    Vector3 extents;
    float hasBeenTurned = 0; 


    private delegate void FollowElement();

    private FollowElement followElement;

    #endregion

    private void Start()
    {
        Renderer rend = carTop.GetComponent<Renderer>();
        Bounds bounds = rend.bounds;
        extents = bounds.extents;
    }
    private void Update()
    {
        currentspeed = Input.GetAxis("Vertical") * speed;

        currentRotate =  Input.GetAxis("Horizontal")* speedRot;
 
        if (isOnGround)
        {
            EnableSphereCollider();
        }
        else
        {
            EnableBoxCollider();
        }

        followElement();

    }

    private void FixedUpdate()
    {
        sphereRb.AddForce(carMesh.forward * currentspeed, ForceMode.Acceleration);
        carTop.transform.Rotate(0, currentRotate, 0);
        AlignWithGround();   
    }
    private void EnableBoxCollider()
    {
        sphereRb.velocity = Vector3.zero;

        if (sphereCollider.GetComponent<Collider>().enabled || !boxRb.GetComponent<Collider>().enabled)
        {
            boxRb.GetComponent<Collider>().enabled = true;
            boxRb.constraints = RigidbodyConstraints.None;
            sphereCollider.GetComponent<Collider>().enabled = false;
        }
        followElement = BallFollowsCar;
    }
    private void EnableSphereCollider()
    {
        if (!sphereCollider.GetComponent<Collider>().enabled || boxRb.GetComponent<Collider>().enabled)
        {
            boxRb.GetComponent<Collider>().enabled = false;
            boxRb.constraints = RigidbodyConstraints.FreezeRotation;
            sphereRb.GetComponent<Collider>().enabled = true;
        }

        followElement = CarFollowsBall;
    }
    private void BallFollowsCar()
    { 
        sphereCollider.position = carTop.transform.position;
        sphereRb.velocity = boxRb.velocity;
    }
    private void CarFollowsBall()
    {
        carTop.transform.position = sphereCollider.position;
        boxRb.velocity = sphereRb.velocity;
    }
    void AlignWithGround()
    {
        List<Vector3> corners = new List<Vector3>{ 
        carTop.transform.TransformPoint(new Vector3(0.5f, 0, 0.5f)),
        carTop.transform.TransformPoint(new Vector3(-0.5f, 0, 0.5f)),
        carTop.transform.TransformPoint(new Vector3(-0.5f, 0, -0.5f)),
        carTop.transform.TransformPoint(new Vector3(0.5f, 0, -0.5f))
        };
        // List<Quaternion> rotations = new List<Quaternion>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> hits = new List<Vector3>();
        for (int i =0; i<corners.Count; i++)
        {
            if (Physics.Raycast(corners[i], -carTop.transform.up , out hit, rayDist, normalInteractMask))
            {
                // rotations.Add(Quaternion.FromToRotation(carTop.transform.up, hit.normal) * carTop.transform.localRotation);
                normals.Add(hit.normal);
                hits.Add(corners[i]);
                Debug.DrawRay(corners[i], -carTop.transform.up * rayDist, Color.green);
                
            }
        }
        if (normals.Count > 2)
        {
            isOnGround = true;
            carTop.transform.localRotation = Quaternion.Lerp(carTop.transform.localRotation, Quaternion.FromToRotation(carTop.transform.up, 
                                             AverageQuaternions(normals)) * carTop.transform.localRotation, Time.deltaTime * rotSpeed);
        
        }else if (hits.Count == 1)
        {
            
            boxRb.AddForceAtPosition(Vector3.up * 0.1f,hits[0], ForceMode.Impulse);
            
            isOnGround = false;

        }else
        {
            isOnGround = false;
              
            if (Physics.Raycast(boxRb.transform.position, carTop.transform.up , out hit, rayDist, normalInteractMask))
            {
              
                hasBeenTurned += Time.deltaTime;
                
            }else 
            {
                hasBeenTurned=0;
            }

            if (hasBeenTurned > 1)
            {
                boxRb.AddForce(Vector3.up * 10, ForceMode.Impulse);
                StartCoroutine(RotateToZero());
            }


        }
        
        
    }
    Vector3 AverageQuaternions(List<Vector3> normals)
    {
        Vector3 average = Vector3.zero;
        for (int i = 0; i < normals.Count; i++)
        {
           average += normals[i];
        }
        return average;
        

    }


    private IEnumerator RotateToZero()
    {
        
       
        while (carTop.transform.localEulerAngles.x > 10f || carTop.transform.localEulerAngles.z > 10f)
        {
            if (isOnGround)
            {
                yield break;
            }
            carTop.transform.localRotation = Quaternion.Lerp(carTop.transform.localRotation, Quaternion.identity, Time.deltaTime * speedRotLerp);
            yield return null;
        }
       
    }

        
}
