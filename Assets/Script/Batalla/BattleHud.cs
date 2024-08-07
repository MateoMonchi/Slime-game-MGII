using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HpBar HPBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;

    Peleadores _peleadores;
    Dictionary<CondicionID, Color> statusColors;

    public void SetData(Peleadores peleadores)
    {
        _peleadores = peleadores;

        nameText.text = peleadores.Base.Name;
        SetLevel();
        HPBar.SetHp((float) peleadores.HP/peleadores.MaxHp);
        SetExp();

        statusColors = new Dictionary<CondicionID, Color>() 
        {
            {CondicionID.psn, psnColor},
             {CondicionID.brn, brnColor},
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

    public void SetLevel()
    {
        levelText.text = "Lvl" + _peleadores.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _peleadores.Base.GetExpForLevel(_peleadores.Level);
        int nextLevelExp = _peleadores.Base.GetExpForLevel(_peleadores.Level + 1);

        float normalizedExp = (float)(_peleadores.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
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
