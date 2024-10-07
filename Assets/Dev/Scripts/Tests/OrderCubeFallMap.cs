using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCubeFallMap : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector2 dimention;
    [SerializeField] Vector2 separation;


    

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {

        //MeshRenderer mesh = prefab.GetComponent<MeshRenderer>();

        Vector2 tempPos = new Vector2(transform.position.x, transform.position.y);

        //separation.x = mesh.bounds.size.x;
        //separation.y = mesh.bounds.size.z;

        float tempOffsetX = 1.5f;
        for (int i = 0; i < dimention.y; i++)
        {
            for (int k = 0; k < dimention.x; k++)
            {
                GameObject clon =  Instantiate(prefab, transform);
                clon.transform.position = new Vector3(tempPos.x, transform.position.y, tempPos.y);
                tempPos.x += separation.x;
            }

            if(i%2!=0) tempPos.x = transform.position.x;
            else tempPos.x = transform.position.x + tempOffsetX;

            tempPos.y += separation.y;
            
        }
    }
}
