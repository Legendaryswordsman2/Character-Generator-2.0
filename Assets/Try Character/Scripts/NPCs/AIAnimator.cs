using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SideDirection { Left, Right }
public class AIAnimator : MonoBehaviour
{
    [SerializeField] AIPath aiPath;
    [SerializeField] Animator animator;
    private void Update()
    {
        if (aiPath == null || aiPath.canMove == false || animator == null || Time.timeScale == 0) return;

        if (!aiPath.reachedDestination)
        {
            animator.SetFloat("Horizontal", aiPath.velocity.x);
            animator.SetFloat("Vertical", aiPath.velocity.y);
        }

        animator.SetFloat("Speed", aiPath.velocity.sqrMagnitude);
    }

    public void ForceLookDirection(LookDirection lookDirection)
    {
        switch (lookDirection)
        {
            case LookDirection.Up:
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 1);
                break;
            case LookDirection.Down:
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);
                break;
            case LookDirection.Left:
                animator.SetFloat("Horizontal", -1);
                animator.SetFloat("Vertical", 0);
                break;
            case LookDirection.Right:
                animator.SetFloat("Horizontal", 1);
                animator.SetFloat("Vertical", 0);
                break;
        }
    }
}
