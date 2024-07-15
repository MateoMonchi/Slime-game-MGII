using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peleadores
{
    PeleadoresBase _base;
    int level;

    public Peleadores(PeleadoresBase pBase, int plevel)
    {
        _base = pBase;
        level = plevel;
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * level) / 100f) + 5; }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.MaxHp * level) / 100f) + 10; }
    }
    public int MagicAttack
    {
        get { return Mathf.FloorToInt((_base.MagicAttack * level) / 100f) + 5; }
    }
    public int MagicDefense
    {
        get { return Mathf.FloorToInt((_base.MagicDefense * level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f) + 5; }
    }
}
