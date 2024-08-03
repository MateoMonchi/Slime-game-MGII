using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movimiento", menuName = "Peleador/Crear nuevo movimiento")]
public class MovimientosBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] PeleadorClase clase;
    [SerializeField] int poder;
    [SerializeField] int precision;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int prioridad;
    [SerializeField] CategoriaMovimientos categoria;
    [SerializeField] EffectosMovimientos effects;
    [SerializeField] List<EffectosSecundarios> secundarios;
    [SerializeField] MoveTarget target;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public PeleadorClase Clase
    {
        get { return clase; }
    }
    public int Precision
    {
        get { return precision; }
    }
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int Poder
    {
        get { return poder; }
    }
    public int PP
    {
        get { return pp; }
    }

    public int Prioridad
    {
        get { return prioridad; }
    }

    public CategoriaMovimientos Categoria
    {
        get { return categoria;}
    }

    public EffectosMovimientos Effects
    {
        get { return effects; }
    }

    public List<EffectosSecundarios> Secundarios
    {
        get { return secundarios; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class EffectosMovimientos
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] CondicionID status;
    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public CondicionID Status
    {
        get { return status; }
    }
}

[System.Serializable]
public class EffectosSecundarios : EffectosMovimientos
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
public enum CategoriaMovimientos
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}

