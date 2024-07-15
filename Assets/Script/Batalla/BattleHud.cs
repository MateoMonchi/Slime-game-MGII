using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar HPBar;

    public void SetData(Peleadores peleadores)
    {
        nameText.text = peleadores.Base.Name;
        levelText.text = "Lv" + peleadores.Level;
        HPBar.SetHp((float) peleadores.HP/peleadores.MaxHp);
    }
}
