using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class M_TextEffects : MonoBehaviour
{
    private static M_TextEffects instance;
    private int appearIndex;

    [SerializeField]  private string displayString;
    
    public bool submitPressed;
    private Coroutine runningAppear;
    private Coroutine runningEffect;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Text Effect Manager");
        }
        instance = this;
    }
    public static M_TextEffects GetInstance()
    {
        return instance;
    }

    public void WriteText(string displayString, TextMeshProUGUI textField)
    {
        M_TextParser textParser = new M_TextParser();
        List<M_TextCharacter> test = textParser.ParseText(displayString);
        this.displayString = displayString;
        submitPressed = false;
        if (runningAppear == null)
        {
           runningAppear= StartCoroutine(Appear(test, textField));
        }
        else
        {
            StopCoroutine(runningAppear);
            runningAppear= StartCoroutine(Appear(test, textField));
        }
        
        if (runningEffect == null)
        {
            runningEffect = StartCoroutine(PlayEffect(test, textField));
        }
        else
        {
            StopCoroutine(runningEffect);
            runningEffect= StartCoroutine(PlayEffect(test, textField));
        }
        
    }

    private IEnumerator Appear(List<M_TextCharacter> parsedText, TextMeshProUGUI textField)
    {
        textField.text = "";
        appearIndex = 0;
        string testString = "";
        foreach (M_TextCharacter test in parsedText)
        {
            testString += test._character;
        }
        textField.text = testString;
        yield return new WaitForSeconds(0.05f);
    }
    
    private IEnumerator PlayEffect(List<M_TextCharacter> parsedText,TextMeshProUGUI textField)
    {
        while (!submitPressed)
        {
            textField.ForceMeshUpdate();
            TMP_TextInfo textInfo = textField.textInfo;
            int position = 0;
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
                            meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)character._color.x, (byte)character._color.y, (byte)character._color.z, (byte)character._color.w);
                        }
                        position++;
                        break;
                }
            }
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                textField.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return new WaitForSeconds(0.01f);
        }
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
