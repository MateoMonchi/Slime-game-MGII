using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum BattleState { Start,PlayerAction, PlayerMove,EnemyMove,Busy}
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogeBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    public void StartBattle()
    {
        StartCoroutine(SetUpBattle());
    }

    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Peleadores);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Peleadores);

        dialogBox.SetMoveNames(playerUnit.Peleadores.Movimientos);

        yield return dialogBox.TypeDialog($"Aparecio un {enemyUnit.Peleadores.Base.Name}");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Elige una acción"));
        dialogBox.EnableActionSelector(true);
    }
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var movimiento = playerUnit.Peleadores.Movimientos[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Peleadores.Base.Name} uso {movimiento.Base.name}");
        yield return new WaitForSeconds(1f);
        var damageDetails = enemyUnit.Peleadores.TakeDamage(movimiento, playerUnit.Peleadores);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if(damageDetails.Perecer)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Peleadores.Base.Name} Perecio");

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        var movimiento = enemyUnit.Peleadores.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Peleadores.Base.Name} uso {movimiento.Base.name}");
        yield return new WaitForSeconds(1f);
        var damageDetails = playerUnit.Peleadores.TakeDamage(movimiento, playerUnit.Peleadores);
       yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Perecer)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Peleadores.Base.Name} Perecio");

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critico > 1f)
            yield return dialogBox.TypeDialog("Black Flash");
        if (damageDetails.EfectividadesDeClases > 1f)
            yield return dialogBox.TypeDialog("Pego SuperEfectivo");
        else if (damageDetails.EfectividadesDeClases < 1f)
            yield return dialogBox.TypeDialog("No fue Efectivo");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelector();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelector()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }
        dialogBox.UpdateActionSelection(currentAction);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Pelear
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Correr
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Peleadores.Movimientos.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
       else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Peleadores.Movimientos.Count - 2)
                currentMove += 2; 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 1)
                currentMove -= 2;
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Peleadores.Movimientos[currentMove]);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
