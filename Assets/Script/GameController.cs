using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;

    private GameState state;

    private void Awake()
    {
        ConditionDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if(trainer != null)
            {
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogManager.i.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.i.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var partyPeleadores = playerController.GetComponent<PartyPeleadores>();
        var peleadoresMalos = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomPeleadoresMalos();

        battleSystem.StartBattle(partyPeleadores, peleadoresMalos);
    }

    private void EndBattle(bool ganar)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.Dialog:
                DialogManager.i.HandleUpdate();
                break;
        }
    }
}
