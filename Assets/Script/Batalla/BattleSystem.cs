using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum BattleState { Start,PlayerAction, PlayerMove,EnemyMove,Busy, PartyScreen}
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogeBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PartyPeleadores partyPeleador;
    Peleadores peleadorMalo;

    public void StartBattle(PartyPeleadores partyPeleador, Peleadores peleleadorMalo)
    {
        this.partyPeleador = partyPeleador;
        this.peleadorMalo = peleleadorMalo;
        StartCoroutine(SetUpBattle());
    }

    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup(partyPeleador.GetHealyPeleadores());
        playerHud.SetData(playerUnit.Peleadores);
        enemyUnit.Setup(peleadorMalo);
        enemyHud.SetData(enemyUnit.Peleadores);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Peleadores.Movimientos);

        yield return dialogBox.TypeDialog($"Aparecio un {enemyUnit.Peleadores.Base.Name}");
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }
    void ActionSelection()
    {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("Elige una acción");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(partyPeleador.peleadores);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.Busy;

        var movimiento = playerUnit.Peleadores.Movimientos[currentMove];
        movimiento.PP--;
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
        movimiento.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Peleadores.Base.Name} uso {movimiento.Base.name}");
        yield return new WaitForSeconds(1f);
        var damageDetails = playerUnit.Peleadores.TakeDamage(movimiento, playerUnit.Peleadores);
       yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Perecer)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Peleadores.Base.Name} Perecio");

            yield return new WaitForSeconds(2f);

            var proximoPeleador = partyPeleador.GetHealyPeleadores();
            if( proximoPeleador != null )
            {
                OpenPartyScreen();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            ActionSelection();
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
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelector();
        }
    }

    void HandleActionSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

            dialogBox.UpdateActionSelection(currentAction);
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Pelear
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Mochila
            }
            else if (currentAction == 2)
            {
                //Peleadores
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Correr
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Peleadores.Movimientos.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Peleadores.Movimientos[currentMove]);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }
    void HandlePartySelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, partyPeleador.peleadores.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = partyPeleador.peleadores[currentMember];
            if(selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("No puede pelear porque esta derrotado");
                return;
            }
            if(selectedMember == playerUnit.Peleadores)
            {
                partyScreen.SetMessageText("No se puede cambiar por este");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(CambioPeleador(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator CambioPeleador(Peleadores nuevoPeleador)
    {
        if (playerUnit.Peleadores.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Vuelve {playerUnit.Peleadores.Base.Name}");
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(nuevoPeleador);
        playerHud.SetData(nuevoPeleador);
        dialogBox.SetMoveNames(nuevoPeleador.Movimientos);
        yield return dialogBox.TypeDialog($"Encargate {nuevoPeleador.Base.Name}!");

        StartCoroutine(EnemyMove());
    }
}
