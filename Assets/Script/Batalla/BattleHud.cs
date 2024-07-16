using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar HPBar;

    Peleadores _peleadores;
    public void SetData(Peleadores peleadores)
    {
        _peleadores = peleadores;

        nameText.text = peleadores.Base.Name;
        levelText.text = "Lv" + peleadores.Level;
        HPBar.SetHp((float) peleadores.HP/peleadores.MaxHp);
    }
    public IEnumerator UpdateHP()
    {
       yield return HPBar.SetHPSmooth((float)_peleadores.HP / _peleadores.MaxHp);
    }
}
