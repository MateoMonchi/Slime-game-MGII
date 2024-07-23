using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
   
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }
    public BattleHud Hud { get { return hud; } }

    public Peleadores Peleadores { get; set; }
    public void Setup(Peleadores peleador)
    {
        Peleadores = peleador;
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Peleadores.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Peleadores.Base.FrontSprite;

        hud.SetData(Peleadores);
       
    }
}
