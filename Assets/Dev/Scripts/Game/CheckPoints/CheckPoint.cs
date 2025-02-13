using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public CheckPointManager manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CheckPoint"+other.CompareTag("Player"));
        if(other.CompareTag("Player"))
        {
            manager.NextCheckPoint(this);
        }
    }
}
