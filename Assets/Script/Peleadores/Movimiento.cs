using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento 
{
   public MovimientosBase Base { get; set; }
    public int PP { get; set; }

    public Movimiento(MovimientosBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
