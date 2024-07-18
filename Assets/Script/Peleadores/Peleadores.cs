using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Peleadores
{
    public PeleadoresBase Base {get; set;}
    public int Level { get; set; }
    public int HP { get;set; }
    public List<Movimiento> Movimientos { get; set; }

    public Peleadores(PeleadoresBase pBase, int plevel)
    {
        Base = pBase;
        Level = plevel;
        HP = MaxHp;
       
        //Genera movimientos
        Movimientos = new List<Movimiento>();
        foreach(var movimiento in Base.Aprendermovimientos)
        {
            if (movimiento.Level <= Level)
                Movimientos.Add(new Movimiento(movimiento.Base));

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
    
    public DamageDetails TakeDamage(Movimiento movimiento, Peleadores agresor)
    {
        float critico = 1f;
        if (Random.value * 100f <= 6.25f)
            critico = 2f;

        float Clase = ClaseChart.GetEffectiveness(movimiento.Base.Clase, this.Base.Clase1) * ClaseChart.GetEffectiveness(movimiento.Base.Clase, this.Base.Clase2);

        var damageDetails = new DamageDetails()
        {
            EfectividadesDeClases = Clase,
            Critico = critico,
            Perecer = false
        };
        float modifiers = Random.Range(0.85f, 1f) * Clase * critico;
        float a = (2 * agresor.Level + 10) / 250f;
        float d = a * movimiento.Base.Poder * ((float)agresor.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Perecer = true;
        }
        return damageDetails;
    }
    public Movimiento GetRandomMove()
    {
        int r = Random.Range(0, Movimientos.Count);
        return Movimientos[r];
    }
}
public class DamageDetails
{
    public bool Perecer { get; set;} 
    public float Critico { get; set;}
    public float EfectividadesDeClases { get; set;}

}
