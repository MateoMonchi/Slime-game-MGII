using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HpBar HPBar;

    [SerializeField] Color psnColor;

    Peleadores _peleadores;
    Dictionary<CondicionID, Color> statusColors;

    public void SetData(Peleadores peleadores)
    {
        _peleadores = peleadores;

        nameText.text = peleadores.Base.Name;
        levelText.text = "Lv" + peleadores.Level;
        HPBar.SetHp((float) peleadores.HP/peleadores.MaxHp);

        statusColors = new Dictionary<CondicionID, Color>() 
        {
            {CondicionID.psn, psnColor},
        };

        SetStatusText();
        _peleadores.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if(_peleadores.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _peleadores.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_peleadores.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_peleadores.HpChanged)
        {
            yield return HPBar.SetHPSmooth((float)_peleadores.HP / _peleadores.MaxHp);
            _peleadores.HpChanged = false;
        }
    }
}
