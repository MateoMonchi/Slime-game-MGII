using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    private void Start()
    {
        SetUpBattle();
    }
    
    public void SetUpBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Peleadores);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Peleadores);
    }

}
