using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movimiento", menuName = "Peleador/Crear nuevo movimiento")]
public class MovimientosBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] int poder;
    [SerializeField] int precision;
    [SerializeField] int pp;

    [SerializeField] List<AprenderMovimientos> aprenderMovimientos;
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public int Precision
    {
        get { return precision; }
    }
    public int Poder
    {
        get { return poder; }
    }
    public int PP
    {
        get { return pp; }
    }
    public List<AprenderMovimientos> aprendermovimientos
    {
        get { return aprenderMovimientos; }
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
}
