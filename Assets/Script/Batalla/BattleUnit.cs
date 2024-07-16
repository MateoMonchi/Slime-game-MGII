using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PeleadoresBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Peleadores Peleadores { get; set; }
    public void Setup()
    {
        Peleadores = new Peleadores(_base, level);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Peleadores.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Peleadores.Base.FrontSprite;
       
    }
}
