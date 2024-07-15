using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientos 
{
   public MovimientosBase Base { get; set; }
    public int PP { get; set; }

    public Movimientos(MovimientosBase pBase, int pp)
    {
        Base = pBase;
        PP = pp;
    }
}
