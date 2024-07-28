using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB 
{
    public static void Init()
    {
        foreach (var kvp in Condiciones)
        {
            var condicionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = condicionId;
        }
    } 
    public static Dictionary<CondicionID, Condicion> Condiciones { get; set; } = new Dictionary<CondicionID, Condicion>()
    {
        {
            CondicionID.psn, 
            new Condicion()
            {
                Name = "Veneno",
                StartMessage = "ha sido envenendado",
                OnAfterTurn = (Peleadores peleadores) =>
                {
                    peleadores.UpdateHP(peleadores.MaxHp / 8);
                    peleadores.StatusChanges.Enqueue($"{peleadores.Base.Name} sufre por el veneno");
                }
            }
        },
        {
           CondicionID.brn, 
            new Condicion()
            {
                Name = "Quemado",
                StartMessage = "ha sido quemado",
                OnAfterTurn = (Peleadores peleadores) =>
                {
                    peleadores.UpdateHP(peleadores.MaxHp / 16);
                    peleadores.StatusChanges.Enqueue($"{peleadores.Base.Name} sufre por la quemadura");
                }
            }
        },/*
        {
            CondicionID.brn, 
            new Condicion()
            {
                Name = "Paralizado",
                StartMessage = "ha sido Paralizado",
                OneBeforeMove = (Peleadores peleadores) =>
                {
                   if(Random.Range(1,5) == 1)
                   {
                       peleadores.StatusChanges.Enqueue($"{peleadores.Base.Name} esta paralizado y no puede moverse");
                       return false;
                   }
                   return true;
                }
            }
        }*/
    };
}
public enum CondicionID
{
   none, psn, brn, par, frz
}
