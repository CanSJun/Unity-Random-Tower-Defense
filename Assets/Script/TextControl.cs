using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextControl : MonoBehaviour
{

    TextMeshPro _Textcontrol;
    public string _Text;
    public Color _color;
    public int _Size;
    void Start()
    {
        _Textcontrol = GetComponent<TextMeshPro>();
        _Textcontrol.text = _Text.ToString();
        _Textcontrol.fontSize = _Size;
        Invoke("DeleteText", 2f);
    }

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        transform.Translate(new Vector3(0, 2f * Time.deltaTime, 0));
        _color.a = Mathf.Lerp(_color.a, 0, Time.deltaTime * 2f);
        _Textcontrol.color = _color;
    }

    public void DeleteText()
    {
        Destroy(gameObject);
    }
}
