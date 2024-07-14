using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving;
    private Vector2 input;

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Para evitar movimiento diagonal
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                var targetPos = new Vector3(transform.position.x + input.x, transform.position.y + input.y, transform.position.z);
                StartCoroutine(Move(targetPos));
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}

