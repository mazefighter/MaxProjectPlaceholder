using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_TextEffects : MonoBehaviour
{
    private static M_TextEffects instance;

    [SerializeField]  private string displayString;
    private TextMeshProUGUI textField;
    public bool submitPressed;
    private Dictionary<string, List<Vector2Int>> commands = new Dictionary<string, List<Vector2Int>>();
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
        foreach (M_TextCharacter character in test)
        {
            print(character._character);
        }

        /*submitPressed = false;
        commands.Clear();
        this.displayString = displayString;
        this.textField = textField;
        if (runningEffect != null)
        {
            StopCoroutine(runningEffect);
        }
        if (runningAppear == null)
        {
            runningAppear= StartCoroutine(Appear());
        }
        else
        {
            StopCoroutine(runningAppear);
            runningAppear= StartCoroutine(Appear());
        }*/
    }

    private IEnumerator Appear()
    {
        string prefix = "<color=#00000000>";
        string postfix = "</color>";
        string tmpText;
        string commandString = "";
        int commandStart = 0;
        int commandEnd = 0;
        bool getCommandString;

        for (int i = 0; i < displayString.Length; i++)
        {
            if (displayString[i].Equals('<'))
            {
                //if not a statement to change text
                if (displayString[i+1].Equals('<'))
                {
                    displayString = displayString.Remove(i + 1, 1);
                    i++;
                    continue;
                }

                if (!displayString[i+1].Equals('/'))
                {
                    commandString = "";
                    commandStart = i;
                    getCommandString = true;
                }
                else
                {
                    commandEnd = i;
                    getCommandString = false;
                    if (commands != null)
                    {
                        if (commands.ContainsKey(commandString))
                        {
                            List<Vector2Int> tempList = commands[commandString];
                            tempList.Add(new(commandStart, commandEnd));
                            commands[commandString] = tempList;
                        }
                        else
                        {
                            commands.Add(commandString, new List<Vector2Int>{new(commandStart, commandEnd)});
                        }
                    }
                    else
                    {
                        commands.Add(commandString, new List<Vector2Int>{new(commandStart, commandEnd)});
                    }
                }
                //if a statement to change text
                commandString += displayString[i];
                int startForDelete = i;
                while (!displayString[i].Equals('>'))
                {
                    i++;
                    if (getCommandString)
                    {
                        commandString += displayString[i];
                    }
                }

                displayString = displayString.Remove(startForDelete, i + 1 - startForDelete);
                i = startForDelete - 1;
                continue;
            }
            
            tmpText = displayString.Insert(i + 1, prefix);
            tmpText += postfix;
            textField.text = tmpText;
            yield return new WaitForSeconds(0.02f);
        }

        runningEffect = StartCoroutine(PlayEffect());

    }
    
    private IEnumerator PlayEffect()
    {
        while (!submitPressed)
        {
            textField.ForceMeshUpdate();
            TMP_TextInfo textInfo = textField.textInfo;
            
            foreach (KeyValuePair<string, List<Vector2Int>> key in commands)
            {
                switch (key.Key)
                {
                    case "<wobble>":
                        Wobble(key.Value, textInfo);
                        break;
                    case "<shake>":
                        Shake(key.Value, textInfo);
                        break;
                }
            }
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textField.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Shake(List<Vector2Int> startEnd, TMP_TextInfo textInfo)
    {
        foreach (Vector2Int Indexes in startEnd)
        {
            for (int i = Indexes.x; i <= Indexes.y; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] =
                        orig + new Vector3(Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0 , 0);
                }
            }
        }
    }

    private void Wobble(List<Vector2Int> startEnd, TMP_TextInfo textInfo)
    {
        foreach (Vector2Int Indexes in startEnd)
        {
            for (int i = Indexes.x; i <= Indexes.y; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] =
                        orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0);
                }
            }
        }
    }
    
}
