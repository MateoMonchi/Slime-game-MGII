using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogeBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }
    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }
    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }
    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }
    public void UpdateMoveSelection(int selectedMove, Movimiento movimientos)
    {
        for(int i=0; i < moveTexts.Count; ++i)
        {
            if(i== selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
        ppText.text = $"PP {movimientos.PP}/{movimientos.Base.PP}";
        typeText.text = movimientos.Base.Clase.ToString() ;

        if (movimientos.PP == 0)
            ppText.color = Color.red;
        else 
            ppText.color = Color.black;
    }

    public void SetMoveNames(List<Movimiento> movimientos)
    {
        for(int i= 0; i < moveTexts.Count; ++i)
        {
            if (i < movimientos.Count)
                moveTexts[i].text = movimientos[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }
}
