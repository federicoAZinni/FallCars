using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boosters : MonoBehaviour
{
    [Header("Boost Settings")]
    [Range(0, 100)]
    [SerializeField] float boostAmount = 5;
    [SerializeField] bool automaticRespawn = false;
    [SerializeField] float respawnTime = 5;
    [Header("References")]
    [SerializeField] Transform visuals;


    void Start()
    {
        LeanTween.moveY(visuals.gameObject, visuals.position.y + 0.5f, 1f).setEaseInOutSine().setLoopPingPong();
         LeanTween.rotateAround(visuals.gameObject, Vector3.up, 360f, 2f).setEaseLinear().setRepeat(-1);
    }

    
    void OnTriggerEnter(Collider collision)
    {
        CheckBoost(collision);
    }

    void OnTriggerStay(Collider collision)
    {
        CheckBoost(collision);
    }

    void CheckBoost(Collider collision)
    {
        if (collision.TryGetComponent<BoostController>(out BoostController boostController))
        {
            if (!visuals.gameObject.activeSelf) return;
            if (boostController.AddBoost(boostAmount))
            {
                if (automaticRespawn)
                {
                    StartCoroutine(Respawn());
                }
                else
                {
                    visuals.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Boost is full");
            }
        }
    }

    
    IEnumerator Respawn()
    {
        visuals.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        visuals.gameObject.SetActive(true);
    }
}
