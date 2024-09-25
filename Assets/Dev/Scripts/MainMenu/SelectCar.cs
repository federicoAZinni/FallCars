using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCar : MonoBehaviour
{
    [SerializeField] GameObject[] carPrefabs;
    [SerializeField] float radio_;

    [SerializeField] List<Vector3> posList;

    [SerializeField] int currentCarSelectedIndex;


    Coroutine rotCar;

    void Start()
    {
        MakeCircleWithCarsAcount(radio_);
        RotCarAnimation();
    }

    private void MakeCircleWithCarsAcount(float radio)
    {
        float constAngle = 360 / carPrefabs.Length;

        float angle = 0;

        //el primer coche decide la posicion del auto selecionado
        Vector3 pos = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radio,0 ,Mathf.Cos(angle * Mathf.Deg2Rad) * radio);
        posList.Add(carPrefabs[0].transform.localPosition);
        
        angle += constAngle;
        //--------------------------------------------


        for (int i = 1; i < carPrefabs.Length; i++)
        {
            pos = new Vector3( Mathf.Sin(angle * Mathf.Deg2Rad) * radio, 0, Mathf.Cos(angle * Mathf.Deg2Rad) * radio);
            posList.Add(pos);
            carPrefabs[i].transform.localPosition = pos;

            angle += constAngle;
        }
    }


    public void ChangeCurrentCar(int index)// -1 or 1
    {
        LeanTween.cancel(carPrefabs[currentCarSelectedIndex]);

        foreach (var item in carPrefabs)
            item.transform.localEulerAngles = Vector3.zero;


        currentCarSelectedIndex += -index;
        if (currentCarSelectedIndex < 0) currentCarSelectedIndex = carPrefabs.Length - 1;
        if (currentCarSelectedIndex >= carPrefabs.Length) currentCarSelectedIndex = 0;


        if (index > 0)
        {
            Vector3 temp = posList[0];
            posList.RemoveAt(0);
            posList.Add(temp);
        }
        else
        {
            Vector3 temp = posList[posList.Count - 1];
            posList.RemoveAt(posList.Count - 1);
            posList.Insert(0, temp);
        }

        for (int i = 0; i < posList.Count; i++)
            LeanTween.moveLocal(carPrefabs[i], posList[i], 0.3f);

        RotCarAnimation();
    }

    private void RotCarAnimation()
    {
        LeanTween.rotateAround(carPrefabs[currentCarSelectedIndex], Vector3.up, 360, 3).setRepeat(-1);
    }
}
