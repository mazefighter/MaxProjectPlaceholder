using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class M_TextEffects : MonoBehaviour
{

    private TextMeshProUGUI _textMeshProUGUI;
    
   public float AppearStartValue = 0;
   public int characterAppear;
   public float AppearCurrentValue = 0;
   public float AppearUsedValue;
   public float AppearEndvalue = 1;

   private bool ValidCharacter;
   private float defaulSpeed;

   private M_Text mText;
   private void Awake()
   {
       mText = GetComponent<M_Text>();
       defaulSpeed = mText.AllEffectsSpeed;
   }

   public IEnumerator CoUpdate(TextMeshProUGUI _textMeshProUGUI, List<M_TextCharacter> parsedText)
   {
       while (true)
         {
             _textMeshProUGUI.ForceMeshUpdate();
             TMP_TextInfo textInfo = _textMeshProUGUI.textInfo;
             int position = 0;

             try
             {
                 foreach (M_TextCharacter character in parsedText)
                 {
                     if (mText.FadeIn && ! mText.skipAppear)
                     {
                         if (position > characterAppear)
                         {
                             AppearUsedValue = 0;
                         }

                         if (position == characterAppear)
                         {
                             AppearUsedValue = AppearCurrentValue;
                         }
                         if(position < characterAppear)
                         {
                             AppearUsedValue = AppearEndvalue;
                         }
                     }
                     else
                     {
                         AppearUsedValue = 1;
                     }
                     switch (character.effectEnum)
                     {
                         case M_EffectEnum.wobble:
                             Wobble(position, textInfo, character._color, AppearUsedValue, character.speed);
                             position++;
                             break;
                         case M_EffectEnum.shake:
                             Shake(position, textInfo, character._color, AppearUsedValue, character.speed);
                             position++;
                             break;
                         case M_EffectEnum.windy:
                             Windy(position, textInfo, character._color, AppearUsedValue, character.speed);
                             position++;
                             break;
                         case M_EffectEnum.old:
                             Old(position, textInfo, character._color, AppearUsedValue, character.speed);
                             position++;
                             break;
                         case M_EffectEnum.colorswoosh:
                             ColorSwoosh(position, textInfo, character._color, AppearUsedValue, character.speed);
                             position++;
                             break;
                         default:
                             
                             
                             TMP_CharacterInfo charInfo = textInfo.characterInfo[position];

                             TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

                             if (charInfo.character is ' ' or '\n')
                             {
                                 position++;
                                 break;
                             }

                             for (int j = 0; j < 4; j++)
                             {
                                 meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)character._color.x,
                                     (byte)character._color.y, (byte)character._color.z, (byte)(character._color.w * AppearUsedValue));
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
                 
                 if (mText.FadeIn && !mText.skipAppear)
                 {
                     if (AppearCurrentValue + mText.FadeSpeed < AppearEndvalue)
                     {
                         AppearCurrentValue += mText.FadeSpeed;
                     }
                     else if (characterAppear < parsedText.Count)
                     {
                         AppearCurrentValue = AppearStartValue;
                         characterAppear++;
                         if (parsedText[characterAppear]._character == ' ')
                         {
                             characterAppear++;
                         }
                     }
                     else
                     {
                         mText.skipAppear = true;
                     }
                 }
                 
             }
             catch 
             {
                 
             }
             
             if (mText != null)
             {
                 yield return new WaitForSeconds(mText.AllEffectsSpeed);
             }
             else
             {
                 yield return new WaitForSeconds(GetComponent<M_Text>().AllEffectsSpeed);
             }
             
         }
     }

     
     

    

    private void Shake(int position, TMP_TextInfo textInfo, int4 color, float appearPercent, float speed)
    {
        
                TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

                if (charInfo.character is ' ' or '\n')
                {
                    return;
                }

                float randomShakeX = Random.Range(-2, 2);
                float randomShakeY = Random.Range(-2, 2);
                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = meshInfo.vertices[charInfo.vertexIndex + j];
                    meshInfo.vertices[charInfo.vertexIndex + j] =
                        orig + new Vector3(randomShakeX, randomShakeY , 0);
                    meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)(color.w * appearPercent));
                }
    }

    private void Wobble(int position, TMP_TextInfo textInfo, int4 color, float appearPercent, float speed)
    {
        
                TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                
                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                
                if (charInfo.character is ' ' or '\n')
                {
                    return;
                }

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = meshInfo.vertices[charInfo.vertexIndex + j];
                    meshInfo.vertices[charInfo.vertexIndex+ j] =
                        orig + new Vector3(0, Mathf.Sin(Time.time * 4f + orig.x * 0.01f) * 5f, 0);
                    meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)(color.w * appearPercent));
                }
                
    }

    private void Windy(int position, TMP_TextInfo textInfo, int4 color, float appearPercent, float speed)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                
        TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                
        if (charInfo.character is ' ' or '\n')
        {
            return;
        }

        for (int j = 0; j < 4; j++)
        {
            Vector3 orig = meshInfo.vertices[charInfo.vertexIndex + j];
            meshInfo.vertices[charInfo.vertexIndex+ j] =
                orig + new Vector3(Mathf.Sin(Time.time * 4f + orig.x * 0.01f) * 5f,0 , 0);
            meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)(color.w * appearPercent));
        }
                
    }
    
    private void Old(int position, TMP_TextInfo textInfo, int4 color, float appearPercent, float speed)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                

        TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];

        if (charInfo.character is ' ' or '\n')
        {
            return;
        }

        for (int j = 0; j < 4; j++)
        {
            meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)(color.w * appearPercent));
        }
    }
    
    private void ColorSwoosh(int position, TMP_TextInfo textInfo, int4 color, float appearPercent, float speed)
    {

        
        TMP_CharacterInfo charInfo = textInfo.characterInfo[position];
                
        TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                
        if (charInfo.character is ' ' or '\n')
        {
            return;
        }

        for (int j = 0; j < 4; j++)
        {
            
            float theta = Mathf.Sin(Time.time / 5f * 2 * Mathf.PI);
            
                Color sinusColor = Color.Lerp(new Color(color.x,color.y,color.z,color.w), new Color(255,255,255,255), (Mathf.Sin(theta) + 1) / 2);
                meshInfo.colors32[charInfo.vertexIndex + j] = new Color32((byte)sinusColor.r,(byte)sinusColor.g,(byte)sinusColor.b,(byte)(color.w*appearPercent));
        }
                
    }
}
