using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] Transform localCameraTransform;
    [SerializeField] Transform carTransform;
    [SerializeField] Transform arrow;
    [SerializeField] CheckPointManager checkPointManager;
    [SerializeField] float yOffset = 5;  

    void FixedUpdate()
    {
        arrow.position = carTransform.position;
        localCameraTransform.position = Camera.main.transform.position;
        localCameraTransform.LookAt(arrow.position);
        if (checkPointManager.currentCheckPointIndex >= checkPointManager.checkPointsList.Count)
        {
            Hide();
        }
        else
        {
        arrow.LookAt(new Vector3 (checkPointManager.checkPointsList[checkPointManager.currentCheckPointIndex].transform.position.x, 
                                  checkPointManager.checkPointsList[checkPointManager.currentCheckPointIndex].transform.position.y-yOffset, 
                                  checkPointManager.checkPointsList[checkPointManager.currentCheckPointIndex].transform.position.z));
        }
    }

    void Hide()
    {
        arrow.gameObject.SetActive(false);
    }
}
