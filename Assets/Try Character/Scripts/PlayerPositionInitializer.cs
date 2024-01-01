using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionInitializer : MonoBehaviour
{
    static Vector2 savedPosition;
    static LookDirection savedLookDirection = LookDirection.Down;

    [SerializeField] CharacterMovementController characterMovementController;
    [SerializeField] Animator animator;
    [SerializeField] CharacterAnimator characterAnimator;
    private void Start()
    {
        if (savedPosition != Vector2.zero)
            transform.position = savedPosition;

        characterAnimator.ForceLookDirection(savedLookDirection);

        TryCharacterLeaveManager.OnBeforeSceneChanged += TryCharacterLeaveManager_OnBeforeSceneChanged;
    }

    private void TryCharacterLeaveManager_OnBeforeSceneChanged(object sender, float time)
    {
        characterMovementController.CanMove = false;

        savedPosition = transform.position;

        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        //Debug.Log(horizontal + " | " + vertical);

        if (horizontal == 0 && vertical == 1)
        {
            savedLookDirection = LookDirection.Up;
            //Debug.Log("Set Up");
        }
        else if (horizontal == 0 && vertical == -1)
        {
            savedLookDirection = LookDirection.Down;
            //Debug.Log("Set Down");
        }
        else if (horizontal == -1 && vertical == 0)
        {
            savedLookDirection = LookDirection.Left;
            //Debug.Log("Set Left");
        }
        else if (horizontal == 1 && vertical == 0)
        {
            savedLookDirection = LookDirection.Right;
            //Debug.Log("Set Right");
        }

        //Debug.Log(savedLookDirection);
    }

    private void OnDestroy()
    {
        TryCharacterLeaveManager.OnBeforeSceneChanged -= TryCharacterLeaveManager_OnBeforeSceneChanged;
    }
}
