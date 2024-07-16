using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Peleadores
{
    public PeleadoresBase Base {get; set;}
    public int Level { get; set; }
    public int HP { get;set; }
    public List<Movimientos> Movimientos { get; set; }

    public Peleadores(PeleadoresBase pBase, int plevel)
    {
        Base = pBase;
        Level = plevel;
        HP = MaxHp;
       
        //Genera movimientos
        Movimientos = new List<Movimientos>();
        foreach(var movimiento in Base.Aprendermovimientos)
        {
            if (movimiento.Level <= Level)
                Movimientos.Add(new Movimientos(movimiento.Base));

            if (Movimientos.Count >= 4)
                break;
        }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }
    public int MagicAttack
    {
        get { return Mathf.FloorToInt((Base.MagicAttack * Level) / 100f) + 5; }
    }
    public int MagicDefense
    {
        get { return Mathf.FloorToInt((Base.MagicDefense * Level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }
    
    public bool TakeDamage(Movimientos movimiento, Peleadores agresor)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * agresor.Level + 10) / 250f;
        float d = a * movimiento.Base.Poder * ((float)agresor.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            return true;
        }
        return false;
    }
    public Movimientos GetRandomMove()
    {
        int r = Random.Range(0, Movimientos.Count);
        return Movimientos[r];
    }
}
