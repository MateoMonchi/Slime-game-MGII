using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemeberUI : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar HPBar;

    [SerializeField] Color highlightedColor;

    Peleadores _peleadores;


    public void SetData(Peleadores peleadores)
    {
        _peleadores = peleadores;

        nameText.text = peleadores.Base.Name;
        levelText.text = "Lv" + peleadores.Level;
        HPBar.SetHp((float)peleadores.HP / peleadores.MaxHp);

    }

    public void SetSelected(bool selected)
    {
        if(selected)
        nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }

}
