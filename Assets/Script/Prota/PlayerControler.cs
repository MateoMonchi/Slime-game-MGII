using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public LayerMask SolidObjectsLayer;
    public LayerMask LongGrassLayer;
    public float moveSpeed = 5f;
    public event Action OnEncountered;
    private bool isMoving;
    private Vector2 input;

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                var targetPos = new Vector3(transform.position.x + input.x, transform.position.y + input.y, transform.position.z);
               if(IsWalkAble(targetPos)) 
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
        CheckForEncountres();
    }
    private bool IsWalkAble(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos,0.2f, SolidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }
    private void CheckForEncountres()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, LongGrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                OnEncountered();
            }
        }
    }
}

