using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Peleadores> peleadoresDeArea;

    public Peleadores GetRandomPeleadoresDeArea()
    {
        return peleadoresDeArea[Random.Range(0, peleadoresDeArea.Count)];
    }
}
