using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class M_TextParser
{
    private List<M_TextCharacter> finishedString = new List<M_TextCharacter>();
    public int4 defaulColor = new int4(255,255,255,255);
    public List<M_TextCharacter> ParseText(string parseString)
    {
        string effectString = "";
        M_Effects effects;
        int4 color = defaulColor;

        for (int i = 0; i < parseString.Length; i++)
        {
            if (parseString[i] == '<')
            {
                if (parseString[i + 1] == '\\')
                {
                    finishedString.Add(new M_TextCharacter(parseString[i], M_Effects.normal, color));
                    i++;
                    continue;
                }

                effectString += parseString[i];
                while (parseString[i] != '>')
                {
                    i++;
                    effectString += parseString[i];
                }

                if (effectString.StartsWith("<color="))
                {
                    //Get color code from command and then split the string and convert the hexadecimals to int. Save the int in an int4. This int4 is used as color
                    Regex regex = new Regex(@"\#(?:[0-9a-fA-F]{4}){2,3}");
                    Match output = regex.Match(effectString);
                    string regexMatch = output.Value;
                    regexMatch = regexMatch.Remove(0, 1);
                    string splitString = string.Join(string.Empty,regexMatch.Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(" {0}", x) : x.ToString()));
                    string[] stringArray = splitString.Split(' ');
                    color.x = int.Parse(stringArray[0], System.Globalization.NumberStyles.HexNumber);
                    color.y = int.Parse(stringArray[1], System.Globalization.NumberStyles.HexNumber);
                    color.z = int.Parse(stringArray[2], System.Globalization.NumberStyles.HexNumber);
                    color.w = int.Parse(stringArray[3], System.Globalization.NumberStyles.HexNumber);
                    
                }

                if (effectString.StartsWith("</color"))
                {
                    color = defaulColor;
                }
                switch (effectString)
                {
                    case "<wobble>":
                        while (parseString[i] != '<' || parseString[i+1] != '/' )
                        {
                            i++;
                            finishedString.Add(new M_TextCharacter(parseString[i], M_Effects.wobble, color));
                        }

                        finishedString.RemoveAt(finishedString.Count - 1);
                        break;
                    
                    case "<shake>":
                        while (parseString[i] != '<' || parseString[i+1] != '/')
                        {
                            i++;
                            finishedString.Add(new M_TextCharacter(parseString[i], M_Effects.shake, color));
                        }

                        finishedString.RemoveAt(finishedString.Count - 1);
                        break;
                }
                while (parseString[i] != '>')
                {
                    i++;
                }
            }
            else
            {
                finishedString.Add(new M_TextCharacter(parseString[i], M_Effects.normal, color));
            }
            effectString = "";
        }
        finishedString.RemoveAt(finishedString.Count - 1);
        return finishedString;
    }
}
