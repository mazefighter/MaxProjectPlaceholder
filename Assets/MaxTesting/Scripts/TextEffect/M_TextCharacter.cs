using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class M_TextCharacter
{
    

    public M_Effects _effect { get; }
    public int4 _color { get; }
    public char _character { get; }

    public M_TextCharacter(char _character, M_Effects _effect, int4 _color)
    {
        this._effect = _effect ;
        this._color = _color;
        this._character = _character;
    }
}
