using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BoostController : MonoBehaviour
{
    [Header ("Boost Settings")]
    [Range(0, 10)]
    [SerializeField] float boostForceOnGround = 5;
    [Range(0, 10)]
    [SerializeField] float boostForceOnAir = 5;
    [Range(0, 100)]
    [SerializeField] float boostConsumptionSpeed = 10;
    [SerializeField] float boostCurrentAmount = 0;

    [Header("References")]
    [SerializeField] Transform boostPoint;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] SuspensionCarController carController;

    float boostMaximunAmotunt = 100;


    // Start is called before the first frame update
   void OnEnable()
   {
        carController.OnBoost += Boost;
   }
   void OnDisable()
   {
        carController.OnBoost -= Boost;
   }

    

    void Boost()
    {
        if (boostCurrentAmount <= 0) 
        {
            carController.isBoosting = false; 
            return;
        }
        if (carController.isGrounded)
        {
            boostCurrentAmount -= Time.deltaTime * boostConsumptionSpeed;
            playerRb.AddForceAtPosition(transform.forward*boostForceOnGround * Time.deltaTime*1000, boostPoint.position, ForceMode.Acceleration);
            carController.isBoosting=true;

        }else
        {
            boostCurrentAmount -= Time.deltaTime*10;
            playerRb.AddForceAtPosition(transform.forward*boostForceOnAir * Time.deltaTime*1000, boostPoint.position, ForceMode.Acceleration);
            carController.isBoosting=true;
        }
        
        
    }

    public bool AddBoost(float amount)
    {
        if (boostCurrentAmount >= boostMaximunAmotunt)return false;
        if (boostCurrentAmount + amount > boostMaximunAmotunt)
        {
            boostCurrentAmount = boostMaximunAmotunt;
            return true;
        }
        boostCurrentAmount += amount;

        return true;
       
    }
}
