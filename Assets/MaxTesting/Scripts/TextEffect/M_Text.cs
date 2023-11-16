using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class M_Text : MonoBehaviour
{
    [SerializeField, TextArea(3,5)] private string text;
    private TMP_Text tmpText;
    private TextMeshProUGUI _textMeshProUGUI;
    private Coroutine runningAppear;
    private Coroutine runningEffect;
    private bool TextUpdate;
    private List<M_TextCharacter> parsedText;


    private void Awake()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private void OnValidate()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        tmpText= GetComponent<TMP_Text>();
        TextUpdate = true;
        TextUpdate = false;
        WriteText(); 
    }

    private void LogPlayModeState(PlayModeStateChange obj)
    {
        if (this != null)
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            tmpText= GetComponent<TMP_Text>();
            TextUpdate = true;
            TextUpdate = false;
            WriteText();
        }
        
    }


    public void WriteText()
    {
        M_TextParser textParser = new M_TextParser();
        parsedText = textParser.ParseText(text);
        if (runningAppear == null)
        {
            runningAppear= StartCoroutine(Appear(parsedText));
        }
        else
        {
            StopCoroutine(runningAppear);
            runningAppear= StartCoroutine(Appear(parsedText));
        }
        
        // if (runningEffect == null)
        // {
        //     runningEffect = StartCoroutine(PlayEffect(test));
        // }
        // else
        // {
        //     StopCoroutine(runningEffect);
        //     runningEffect= StartCoroutine(PlayEffect(test));
        // }
        
    }
    
    private IEnumerator Appear(List<M_TextCharacter> parsedText)
    {
        _textMeshProUGUI.text = "";
        string testString = "";
        foreach (M_TextCharacter test in parsedText)
        {
            testString += test._character;
        }
        _textMeshProUGUI.text = testString;
        _textMeshProUGUI.ForceMeshUpdate();
        yield return new WaitForSeconds(0.05f);
    }

   
    private void Update()
    {
        _textMeshProUGUI.ForceMeshUpdate();
            TMP_TextInfo textInfo = _textMeshProUGUI.textInfo;
            int position = 0;
            try
            {
                foreach (M_TextCharacter character in parsedText)
                {
                    switch (character.effectEnum)
                    {
                        case M_EffectEnum.wobble:
                            Wobble(position, textInfo, character._color);
                            position++;
                            break;
                        case M_EffectEnum.shake:
                            Shake(position, textInfo, character._color);
                            position++;
                            break;
                        default:
                            TMP_CharacterInfo charInfo = textInfo.characterInfo[position];

                            TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

                            if (charInfo.character == ' ')
                            {
                                position++;
                                break;
                            }

                            for (int j = 0; j < 4; j++)
                            {
                                meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)character._color.x,
                                    (byte)character._color.y, (byte)character._color.z, (byte)character._color.w);
                            }

                            position++;
                            break;
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        var meshInfo = textInfo.meshInfo[i];
                        meshInfo.mesh.vertices = meshInfo.vertices;
                        meshInfo.mesh.colors32 = meshInfo.colors32;
                        _textMeshProUGUI.UpdateGeometry(meshInfo.mesh, i);
                    }
                }
            }
            catch (Exception e)
            {
                    
            }
    }

    private IEnumerator PlayEffect(List<M_TextCharacter> parsedText)
    {
        /*while (!TextUpdate)
        {
            _textMeshProUGUI.ForceMeshUpdate();
            TMP_TextInfo textInfo = _textMeshProUGUI.textInfo;
            int position = 0;
            try
            {
                foreach (M_TextCharacter character in parsedText)
                {
                    switch (character.effectEnum)
                    {
                        case M_EffectEnum.wobble:
                            Wobble(position, textInfo, character._color);
                            position++;
                            break;
                        case M_EffectEnum.shake:
                            Shake(position, textInfo, character._color);
                            position++;
                            break;
                        default:
                            TMP_CharacterInfo charInfo = textInfo.characterInfo[position];

                            TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

                            if (charInfo.character == ' ')
                            {
                                position++;
                                break;
                            }

                            for (int j = 0; j < 4; j++)
                            {
                                meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)character._color.x,
                                    (byte)character._color.y, (byte)character._color.z, (byte)character._color.w);
                            }

                            position++;
                            break;
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        var meshInfo = textInfo.meshInfo[i];
                        meshInfo.mesh.vertices = meshInfo.vertices;
                        meshInfo.mesh.colors32 = meshInfo.colors32;
                        _textMeshProUGUI.UpdateGeometry(meshInfo.mesh, i);
                    }
                }
            }
            catch (Exception e)
            {
                    
            }

            
        }*/
        yield return new WaitForSeconds(0.01f);
    }

    private void Shake(int position, TMP_TextInfo textInfo, int4 color)
    {
        
                TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

                if (charInfo.character == ' ')
                {
                    return;
                }

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = meshInfo.vertices[charInfo.vertexIndex + j];
                    meshInfo.vertices[charInfo.vertexIndex + j] =
                        orig + new Vector3(Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0 , 0);
                    meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)color.w);
                }
    }

    private void Wobble(int position, TMP_TextInfo textInfo, int4 color)
    {
        
                TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                
                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                
                if (charInfo.character == ' ')
                {
                    return;
                }

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = meshInfo.vertices[charInfo.vertexIndex + j];
                    meshInfo.vertices[charInfo.vertexIndex+ j] =
                        orig + new Vector3(0, Mathf.Sin(Time.time * 4f + orig.x * 0.01f) * 30f, 0);
                    meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)color.w);
                }
                
    }
}
