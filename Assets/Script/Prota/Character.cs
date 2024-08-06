using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    public bool isMoving { get; private set; }

    public IEnumerator Move(Vector2 moveVect, Action OnMoveOver = null)
    {
        Vector3 targetPos = transform.position + new Vector3(moveVect.x, moveVect.y);

        if (!IsPathClear(targetPos))
            yield break;

        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        OnMoveOver?.Invoke();
    }
    
    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractuableLayer | GameLayer.Instance.PlayerLayer) == true)
            return false;

        return true;
    }


    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.2f, GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractuableLayer) == null)
        {
            return false;
        }
        return true;
    }
    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {

        }
        else
            Debug.LogError("Error al mirar hacia: no puedes pedirle al personaje que mire en diagonal");
    }
}

