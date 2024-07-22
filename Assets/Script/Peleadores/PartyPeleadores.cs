using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyPeleadores : MonoBehaviour
{
   [SerializeField]List<Peleadores> peleads;

    public List<Peleadores> peleadores
    {
        get 
        {
            return peleads; 
        }
    }

    private void Start()
    {
        foreach (var pelead in peleads)
        {
            pelead.Init();
        }
    }
    public Peleadores GetHealyPeleadores()
    {
        return peleads.Where(x => x.HP > 0).FirstOrDefault();
    }
}
