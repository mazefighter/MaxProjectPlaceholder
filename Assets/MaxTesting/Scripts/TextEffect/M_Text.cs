using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_Text : MonoBehaviour
{
    [SerializeField, TextArea(3,5)] private string text;
    private void OnValidate()
    {
        TMP_Text tmpText= GetComponent<TMP_Text>();
        tmpText.text = text;
        
    }
}
