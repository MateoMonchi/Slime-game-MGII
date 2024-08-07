using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;

[System.Serializable]

public class Peleadores
{
    [SerializeField] PeleadoresBase _base;
    [SerializeField] int level;

    public PeleadoresBase Base
    { get { return _base; } }

    public int Level
    {

        get { return level; }
    }

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Movimiento> Movimientos { get; set; }
    public Movimiento CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatsBoosts { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condicion Status { get; private set; }
    public int StatusTime { get; set; }
    public Condicion VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        //Genera movimientos
        Movimientos = new List<Movimiento>();
        foreach (var movimiento in Base.Aprendermovimientos)
        {
            if (movimiento.Level <= Level)
                Movimientos.Add(new Movimiento(movimiento.Base));

            if (Movimientos.Count >= 4)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStats();

        HP = MaxHp;

        ResetStatsBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.MagicAttack, Mathf.FloorToInt((Base.MagicAttack * Level) / 100f) + 5);
        Stats.Add(Stat.MagicDefense, Mathf.FloorToInt((Base.MagicDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;

    }

    void ResetStatsBoost()
    {
        StatsBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.MagicAttack, 0 },
            {Stat.MagicDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.Precision, 0 },
            {Stat.Evasion, 0 },
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        // aplica el boost de stats
        int boost = StatsBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)

            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);


        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatsBoosts[stat] = Mathf.Clamp(StatsBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}´s {stat} subio!");
            else
                StatusChanges.Enqueue($"{Base.Name}´s {stat} bajo!");

            Debug.Log($"{stat} acaba de buffear {StatsBoosts[stat]}");
        }
    } 

    public bool CheckForLevelUp()
    {
        if(Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
  
    public int MagicAttack
    {
        get { return GetStat(Stat.MagicAttack); }
    }
    public int MagicDefense
    {
        get { return GetStat(Stat.MagicDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int MaxHp { get; private set; }

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
        float ataque = (movimiento.Base.Categoria == CategoriaMovimientos.Special) ? agresor.MagicAttack : agresor.Attack;
        float defensa = (movimiento.Base.Categoria == CategoriaMovimientos.Special) ? MagicDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * Clase * critico;
        float a = (2 * agresor.Level + 10) / 250f;
        float d = a * movimiento.Base.Poder * ((float)ataque / defensa) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(CondicionID condicionId)
    {
        if (Status != null) return;
        Status = ConditionDB.Condiciones[condicionId];
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public Movimiento GetRandomMove()
    {
        var moveWithPP = Movimientos.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, moveWithPP.Count);
        return moveWithPP[r];
    }

    public bool OnBeforeMove()
    {
        if(Status?.OneBeforeMove != null)
        {
            return Status.OneBeforeMove(this);
        }
        return true;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        ResetStatsBoost();
    }
}
public class DamageDetails
{
    public bool Perecer { get; set;} 
    public float Critico { get; set;}
    public float EfectividadesDeClases { get; set;}

}
