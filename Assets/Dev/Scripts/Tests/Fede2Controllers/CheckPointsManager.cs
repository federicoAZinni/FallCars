using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointsManager : MonoBehaviour
{
    [SerializeField] List<CheckPoints> checkPoints;
    [SerializeField] int currentCheckPoint = 0;

    void Start()
    {
        foreach (CheckPoints checkPoint in checkPoints)
        {
            checkPoint.OnPlayerEnterCheckPoint += SetCheckPoint;
            if (checkPoint.index != currentCheckPoint)
            {
                checkPoint.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        foreach (CheckPoints checkPoint in checkPoints)
        {
            checkPoint.OnPlayerEnterCheckPoint -= SetCheckPoint;
        }
    }

    public void SetCheckPoint(int index, SuspensionCarController player)
    {
        if (index != currentCheckPoint)
        {
            Debug.LogError($"Index {index} does not correspond with current checkpoint {currentCheckPoint}");
            return;
        }

        foreach (CheckPoints checkPoint in checkPoints)
        {
            
            if (checkPoint.index == index)
            {   
                checkPoint.gameObject.SetActive(false);
            }
        }

        currentCheckPoint = index + 1;
        
        if (currentCheckPoint < checkPoints.Count)
        {
            Debug.Log( checkPoints.Count);
            foreach (CheckPoints checkPoint in checkPoints)
            {
                Debug.Log($"Comparing event index {currentCheckPoint} with checkpoint {checkPoint.index}");
                if (checkPoint.index == currentCheckPoint)
                {
                    checkPoint.gameObject.SetActive(true);
                    Debug.Log($"Checkpoint {checkPoint.index} is now active: {checkPoint.gameObject.activeSelf}");
                }
            }
        }
        else
        {
            Debug.Log($"{player} has reached the last checkpoint");
        }
    }
}