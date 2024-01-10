using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookDirection { Up, Down, Left, Right }
public class CharacterAnimator : MonoBehaviour
{
    public static CharacterAnimator Instance;

    [SerializeField] CharacterMovementController playerMovement;

    Animator anim;

    private void Awake()
    {
        Instance = this;

        anim = GetComponent<Animator>();

        if (playerMovement == null)
        {
            Debug.LogWarning("Reference to player movement not found, can't animate player.");
            Destroy(this);
        }

        if (anim == null)
        {
            Debug.LogWarning("Reference to animator not found, can't animate player.");
            Destroy(this);
        }
    }

    public void ForceLookDirection(LookDirection lookDirection)
    {
        switch (lookDirection)
        {
            case LookDirection.Up:
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 1);
                break;
            case LookDirection.Down:
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 0);
                break;
            case LookDirection.Left:
                anim.SetFloat("Horizontal", -1);
                anim.SetFloat("Vertical", 0);
                break;
            case LookDirection.Right:
                anim.SetFloat("Horizontal", 1);
                anim.SetFloat("Vertical", 0);
                break;
        }
    }

    public void PlayAnimation(string name)
    {
        anim.Play(name);
    }

    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    private void Update()
    {
        if (Time.timeScale <= 0 || !playerMovement.CanMove)
        {
            anim.SetFloat("Speed", 0);
            return;
        }

        if (playerMovement.movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", playerMovement.movement.x);
            anim.SetFloat("Vertical", playerMovement.movement.y);
        }
        anim.SetFloat("Speed", playerMovement.movement.sqrMagnitude);
    }
}