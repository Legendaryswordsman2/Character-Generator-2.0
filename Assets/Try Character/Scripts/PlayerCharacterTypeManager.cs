using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterTypeManager : MonoBehaviour
{
    [SerializeField] CharacterMovementController characterMovementController;
    [SerializeField] Animator playerAnimator;

    CharacterPieceDatabase characterPieceDatabase;
    private void Start()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        playerAnimator.runtimeAnimatorController = characterPieceDatabase.ActiveCharacterType.CharacterController;
        characterMovementController.MoveSpeed = characterPieceDatabase.ActiveCharacterType.PlayerCharacterSpeed;
    }
}