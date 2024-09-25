using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TestTaskWebGL : MonoBehaviour
{
    
    void Start()
    {
        Test();
    }

    async void Test()
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("asd");
            await UniTask.Delay(2000);
        }
    }
}
