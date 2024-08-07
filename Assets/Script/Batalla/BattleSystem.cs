using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum BattleState { Start,ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver}
public enum BattleAction { Movimiento, SwitchPeleador, UseItem, Run}
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogeBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;

    int escapeAttempts;

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
        enemyUnit.Setup(peleadorMalo);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Peleadores.Movimientos);

        yield return dialogBox.TypeDialog($"Aparecio un {enemyUnit.Peleadores.Base.Name}");
        yield return new WaitForSeconds(1f);

        escapeAttempts = 0;
        ActionSelection();
    }

    
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        partyPeleador.peleadores.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
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
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if(playerAction == BattleAction.Movimiento)
        {
            playerUnit.Peleadores.CurrentMove = playerUnit.Peleadores.Movimientos[currentMove];
            enemyUnit.Peleadores.CurrentMove = enemyUnit.Peleadores.GetRandomMove();

            int playerMovePriority = playerUnit.Peleadores.CurrentMove.Base.Prioridad;
            int enemyMovePriority = enemyUnit.Peleadores.CurrentMove.Base.Prioridad;

            //Chequea quien va primero
            bool playerGoesFirst = true;
            if(enemyMovePriority > playerMovePriority)
                playerGoesFirst= false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Peleadores.Speed >= enemyUnit.Peleadores.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPeleador = secondUnit.Peleadores;

            //Primer turno
            yield return RunMove(firstUnit, secondUnit, firstUnit.Peleadores.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPeleador.HP > 0)
            {
                //Segundo turno
                yield return RunMove(secondUnit, firstUnit, secondUnit.Peleadores.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if(playerAction == BattleAction.SwitchPeleador)
            {
                var selectedPeleador = partyPeleador.peleadores[currentMember];
                state = BattleState.Busy;
                yield return CambioPeleador(selectedPeleador);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            //Enemy Turn
            var enemyMove = enemyUnit.Peleadores.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }
    IEnumerator PlayerMove()
    {
        state = BattleState.RunningTurn;

        var move = playerUnit.Peleadores.Movimientos[currentMember];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.RunningTurn)
            StartCoroutine(EnemyMove());
    }
    IEnumerator EnemyMove()
    {
        state = BattleState.RunningTurn;

        var move = enemyUnit.Peleadores.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.RunningTurn)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Movimiento movimiento)
    {
        bool canRunMove = sourceUnit.Peleadores.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Peleadores);
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Peleadores);

        movimiento.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Peleadores.Base.Name} uso {movimiento.Base.name}");

        if (CheckIfMoveHits(movimiento, sourceUnit.Peleadores, targetUnit.Peleadores))
        {

            yield return new WaitForSeconds(1f);

            if (movimiento.Base.Categoria == CategoriaMovimientos.Status)
            {
                yield return RunMoveEffects(movimiento.Base.Effects, sourceUnit.Peleadores, targetUnit.Peleadores, movimiento.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Peleadores.TakeDamage(movimiento, sourceUnit.Peleadores);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);

            }

            if(movimiento.Base.Secundarios != null && movimiento.Base.Secundarios.Count > 0 && targetUnit.Peleadores.HP > 0)
            {
                foreach (var secondary in movimiento.Base.Secundarios)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Peleadores, targetUnit.Peleadores, secondary.Target);
                }
            }

            if (targetUnit.Peleadores.HP <= 0)
            {
                yield return HandlePeleadorDerrotado(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Peleadores.Base.Name} fallo el ataque");
        }
    }

    IEnumerator RunMoveEffects(EffectosMovimientos effects, Peleadores source, Peleadores target, MoveTarget moveTarget)
    {
        //Bufeo de stats
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }
        //condicion de estado
        if(effects.Status != CondicionID.none)
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        sourceUnit.Peleadores.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Peleadores);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Peleadores.HP <= 0)
        {
            yield return HandlePeleadorDerrotado(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Movimiento movimiento, Peleadores source, Peleadores target)
    {
        if (movimiento.Base.AlwaysHits)
            return true;

        float movimientoPrecision = movimiento.Base.Precision;

        int precision = source.StatsBoosts[Stat.Precision];
        int evacion = target.StatsBoosts[Stat.Precision];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if(precision > 0)
            movimientoPrecision *= boostValues[precision];
        else
            movimientoPrecision /= boostValues[-precision];

        if (evacion > 0)
            movimientoPrecision /= boostValues[evacion];
        else
            movimientoPrecision *= boostValues[-evacion];

        return UnityEngine.Random.Range(1, 101) <= movimientoPrecision;
    }

    IEnumerator ShowStatusChanges (Peleadores peleadores)
    {
        while (peleadores.StatusChanges.Count > 0)
        {
            var message = peleadores.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePeleadorDerrotado(BattleUnit derrotaUnit)
    {
        yield return dialogBox.TypeDialog($"{derrotaUnit.Peleadores.Base.Name} Perecio");

        yield return new WaitForSeconds(2f);

        if (!derrotaUnit.IsPlayerUnit)
        {
            // exp ganada
            int expYield = derrotaUnit.Peleadores.Base.ExpYield;
            int enemyLevel = derrotaUnit.Peleadores.Level;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel) / 7);
            playerUnit.Peleadores.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Peleadores.Base.Name} gano {expGain} experiencia");
            yield return playerUnit.Hud.SetExpSmooth();

            //revisa si subiste de nivel 
            while (playerUnit.Peleadores.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Peleadores.Base.Name} subio al nivel {playerUnit.Peleadores.Level}");

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(derrotaUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var proximoPeleador = partyPeleador.GetHealyPeleadores();
            if (proximoPeleador != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critico > 1f)
            yield return dialogBox.TypeDialog("Realizo un critico");
        if (damageDetails.EfectividadesDeClases > 1f)
            yield return dialogBox.TypeDialog("Pego SuperEfectivo");
        else if (damageDetails.EfectividadesDeClases < 1f)
            yield return dialogBox.TypeDialog("No fue Efectivo");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelector();
        }
        else if (state == BattleState.MoveSelection)
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
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Correr
                StartCoroutine(RunTurns(BattleAction.Run));
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
            var movimiento = playerUnit.Peleadores.Movimientos[currentMove];
            if (movimiento.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Movimiento));
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

            if(prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPeleador));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(CambioPeleador(selectedMember));
            }
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
        dialogBox.SetMoveNames(nuevoPeleador.Movimientos);
        yield return dialogBox.TypeDialog($"Encargate {nuevoPeleador.Base.Name}!");

        state = BattleState.RunningTurn;
    }
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        ++escapeAttempts;

        int playerSpeed = playerUnit.Peleadores.Speed;
        int enemySpeed = enemyUnit.Peleadores.Speed;
        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Espacaste a salvo");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed *128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Espacaste a salvo");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"No pudiste escapar");
                state = BattleState.RunningTurn;
            }
        }
    }
}
