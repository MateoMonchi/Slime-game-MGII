using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Peleador", menuName = "Peleador/Crear nuevos peleadores")]

public class PeleadoresBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PeleadorClase clase1;
    [SerializeField] PeleadorClase clase2;

    //Stats Bases
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int magicAttack;
    [SerializeField] int magicDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] List<AprenderMovimientos> aprenderMovimientos;

    public int GetExpForLevel(int level)
    {
        if(growthRate == GrowthRate.Rapido)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MedioRapido)
        {
            return level * level * level    ;
        }
        return -1;
    }
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public PeleadorClase Clase1
    {
        get { return clase1; }
    }
    public PeleadorClase Clase2
    {
        get { return clase2; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int MagicAttack
    {
        get { return magicAttack; }
    }
    public int MagicDefense
    {
        get { return magicDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public List<AprenderMovimientos> Aprendermovimientos
    {
        get { return aprenderMovimientos; }
    }
    public int ExpYield => expYield;

    public GrowthRate GrowthRate => growthRate;
}
[System.Serializable]

public class AprenderMovimientos
{
    [SerializeField] MovimientosBase movimientosBase;
    [SerializeField] int level;

    public MovimientosBase Base
    {
        get { return movimientosBase; }
    }
    public int Level
    {
        get { return level; }
    }
}
public enum PeleadorClase
{
    None,
    Normal,
    Fuego,
    Agua,
    Rayo,
    Tierra,
    Viento,
    Maquina,
    Hielo
}

public enum GrowthRate
{
    Rapido, MedioRapido
}

public enum Stat
{
    Attack,
    Defense,
    MagicAttack,
    MagicDefense,
    Speed,

    Precision,
    Evasion
}

public class ClaseChart
{
    static float[][] chart =
    {
        //                   NOR  FIR   WAT   ELE   GRO   WIN   MAQ
        /*NOR*/ new float[] {1f,  1f,   1f,   1f,   1f,   1f,   0.5f},
        /*FIR*/ new float[] {1f,  0.5f, 0.5f, 1f,   2f,   2f,   1f},
        /*WAT*/ new float[] {1f,  2f,   0.5f, 0.5f,  2f, 1f,   1f},
        /*ELE*/ new float[] {1f,  1f,   2f,   0.5f, 0.5f,   2f, 0.5f},
        /*GRO*/ new float[] {1f,  2f,   1f,   2f, 0.5f, 1f,   0.5f},
        /*WIN*/ new float[] {1f,  0.5f, 1f,   2f,   1f,   0.5f, 1f},
        /*MAQ*/ new float[] {1f,  1f,   2f,   1f,   0.5f, 2f,   1f},
        /*ICE*/ new float[] {1f,  0.5f,   2f,   1f,   1f, 2f,   1f},
    };

    public static float GetEffectiveness(PeleadorClase attackType, PeleadorClase defenseType)
    {
        if (attackType == PeleadorClase.None || defenseType == PeleadorClase.None)
            return 1;
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }
}
