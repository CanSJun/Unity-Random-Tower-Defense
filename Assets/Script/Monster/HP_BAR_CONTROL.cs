using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_BAR_CONTROL : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            //ü�¹ٰ� ī�޶� ���⿡ ���߾ ���������� 
    }
}
