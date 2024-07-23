using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text mensageText;

    PartyMemeberUI[] slotMiembros;
    List<Peleadores> peleadores;

    public void Init()
    {
        slotMiembros = GetComponentsInChildren<PartyMemeberUI>();
    }
    public void SetPartyData(List<Peleadores> peleadores)
    {
        this.peleadores = peleadores;

        for(int i = 0; i < slotMiembros.Length; i++) 
        { 
           if(i < peleadores.Count)
                slotMiembros[i].SetData(peleadores[i]);
           else
                slotMiembros[i].gameObject.SetActive(false);
        }
        mensageText.text = "Elige al Peleador";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for(int i = 0;i < peleadores.Count;i++)
        {
            if (i == selectedMember)
                slotMiembros[i].SetSelected(true);
            else
                slotMiembros[i].SetSelected(false);
        }
    }

    public void SetMessageText(string mensage)
    {
        mensageText.text = mensage;
    }
}
