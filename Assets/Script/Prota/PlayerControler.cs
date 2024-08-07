using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    const float offsetY = 0.3f;

    private Vector2 input;
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.isMoving)
        {
            HandleMovementInput();
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, CheckForEncounters));
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    private void HandleMovementInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.x != 0)
        {
            input.y = 0; 
        }
    }

    private void Interact()
    {
        Vector3 interactPos = transform.position;

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayer.Instance.InteractuableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }


    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayer.Instance.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                OnEncountered?.Invoke();
            }
        }
    }
   
}

