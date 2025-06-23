using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_BAR_CONTROL : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            //체력바가 카메라 방향에 맞추어서 보여지도록 
    }
}
