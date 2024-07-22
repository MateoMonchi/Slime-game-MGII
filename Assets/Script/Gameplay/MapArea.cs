using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Peleadores> peleadoresMalos;
    public Peleadores GetRandomPeleadoresMalos()
    {
        var peleadoreMalo = peleadoresMalos[Random.Range(0, peleadoresMalos.Count)];
        peleadoreMalo.Init();
        return peleadoreMalo;
    }
}
