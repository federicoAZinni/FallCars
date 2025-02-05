using UnityEngine;
using System;

public class CheckPoints : MonoBehaviour
{
    
    [SerializeField] public int index;
    public Transform visuals;
    public event Action<int, SuspensionCarController> OnPlayerEnterCheckPoint;


    void Start()
    {
        LeanTween.moveY(visuals.gameObject, visuals.position.y + 0.5f, 1f).setEaseInOutSine().setLoopPingPong();
        LeanTween.rotateAround(visuals.gameObject, Vector3.up, 360f, 2f).setEaseLinear().setRepeat(-1);
    }

    void OnTriggerEnter(Collider collision)
    {
        
        
        if (collision.TryGetComponent<SuspensionCarController>(out SuspensionCarController player))
        {
            OnPlayerEnterCheckPoint?.Invoke(index, player);
        }
    }

}
