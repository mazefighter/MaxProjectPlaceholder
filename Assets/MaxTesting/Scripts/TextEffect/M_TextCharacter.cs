using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class M_TextCharacter
{
    public M_EffectEnum effectEnum { get; }
    public int4 _color { get; }
    public char _character { get; }

    public float AppearPercent;

    public M_TextCharacter(char _character, M_EffectEnum effectEnum, int4 _color)
    {
        this.effectEnum = effectEnum ;
        this._color = _color;
        this._character = _character;
    }
}
