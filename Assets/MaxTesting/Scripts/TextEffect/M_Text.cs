using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteAlways]
[RequireComponent(typeof(M_TextEffects))]
public class M_Text : MonoBehaviour
{
    [SerializeField, TextArea(3, 5)] public string text;
    public bool FadeIn = true;
    public float AllEffectsSpeed = 0.02f;
    public float FadeSpeed = 0.3f;
    private TextMeshProUGUI _textMeshProUGUI;
    private Coroutine _coUpdate;
    public bool skipAppear;
    public List<M_TextCharacter> parsedText { get; private set; }


    private void Awake()
    {
        if (Application.isPlaying)
        {
            GetComponent<TextMeshProUGUI>().color = Color.clear;
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            WriteText(text, _textMeshProUGUI);
        }
    }

    private void OnValidate()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        WriteText(text, _textMeshProUGUI); 
    }

    public void TextOnCodeChange(string newText)
    {
        text = newText;
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        WriteText(text, _textMeshProUGUI); 
    }
    

    private void WriteText(string text, TextMeshProUGUI _textMeshProUGUI)
    {
        if (_coUpdate != null && Application.isPlaying)
        {
            M_TextEffects textEffects = GetComponent<M_TextEffects>();
           textEffects.AppearCurrentValue = textEffects.AppearStartValue;
           textEffects.characterAppear = 0;
            StopCoroutine(_coUpdate);
        }
        M_TextParser textParser = new M_TextParser();
        parsedText = textParser.ParseText(text);
        this._textMeshProUGUI = _textMeshProUGUI;
        string testString = "";
        foreach (M_TextCharacter test in parsedText)
        {
            testString += test._character;
        }
        _textMeshProUGUI.text = testString;
        if (Application.isPlaying)
        {
             _coUpdate = StartCoroutine(GetComponent<M_TextEffects>().CoUpdate(this._textMeshProUGUI, parsedText));
        }
       
    }










}
