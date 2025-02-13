using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] CheckPoint checkPointPrefab;
    [SerializeField] GameManager gameManager;
    public List<CheckPoint> checkPointsList;
    public int currentCheckPointIndex = 0;


    private void Start()
    {

        if (checkPointsList.Count <= 0) return;

        checkPointsList[0].gameObject.SetActive(true);

        for (int i = 1; i < checkPointsList.Count; i++)
        {
            checkPointsList[i].gameObject.SetActive(false);
        }
    }

    public void CreateCheckPoint()
    {
        CheckPoint checkPoint = Instantiate(checkPointPrefab, transform);
        checkPointsList.Add(checkPoint);
        checkPoint.manager = this;
    }

    public void NextCheckPoint(CheckPoint _checkPoint)
    {
        currentCheckPointIndex++;
        if (currentCheckPointIndex >= checkPointsList.Count)// Si pasa por aca quiere decir que ya llego al ultimo Checkpoint Osea que gano
        {
            gameManager.FinishRaceServerRpc(gameManager.localId);
            return;
        }
        
        _checkPoint.gameObject.SetActive(false);
        checkPointsList[currentCheckPointIndex].gameObject.SetActive(true);
    }
}

