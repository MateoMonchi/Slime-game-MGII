using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condicion 
{
    public CondicionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Func<Peleadores, bool> OneBeforeMove { get; set; }
    public Action<Peleadores> OnAfterTurn { get; set; }

}
