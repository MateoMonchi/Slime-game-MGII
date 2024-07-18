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
    [SerializeField] int pp;


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
    public int Poder
    {
        get { return poder; }
    }
    public int PP
    {
        get { return pp; }
    }

    public bool IsSpecial
    {
        get
        {
            if (clase == PeleadorClase.Fuego || clase == PeleadorClase.Viento || clase == PeleadorClase.Agua || clase == PeleadorClase.Rayo)
            {

                return true;
            }
            else
            {
                return false;
            }

        }
    }
}

