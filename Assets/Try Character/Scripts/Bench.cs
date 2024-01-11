using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] CharacterTypeSO benchActiveCharacterType;

    [Space]

    [SerializeField] Transform sitPOS;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D collisionCollider;
    [SerializeField] CanvasGroup canvasGroup;

    [Space]

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material outlineMaterial;

    [Space]

    [SerializeField] string animationName;

    bool isInRange = false;
    bool isSitting = false;

    CharacterMovementController characterMovementController;
    CharacterAnimator characterAnimator;

    Vector2 playerPosition;

    private void Start()
    {
        characterMovementController = CharacterMovementController.Instance;
        characterAnimator = CharacterAnimator.Instance;

        if(CharacterPieceDatabase.Instance.ActiveCharacterType != benchActiveCharacterType)
        {
            canvasGroup.gameObject.SetActive(false);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !enabled) return;

        isInRange = true;
        spriteRenderer.material = outlineMaterial;

        LeanTween.cancel(canvasGroup.gameObject);
        LeanTween.alphaCanvas(canvasGroup, 1, 0.1f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !enabled) return;

        isInRange = false;
        spriteRenderer.material = defaultMaterial;

        LeanTween.cancel(canvasGroup.gameObject);
        LeanTween.alphaCanvas(canvasGroup, 0.85f, 0.1f);
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isSitting)
                SitDown();
            else
                GetUp();
        }
    }

    void SitDown()
    {
        isSitting = true;
        characterMovementController.CanMove = false;
        spriteRenderer.material = defaultMaterial;

        playerPosition = characterMovementController.transform.position;

        collisionCollider.enabled = false;


        characterAnimator.PlayAnimation(animationName);

        characterMovementController.transform.position = sitPOS.position;
    }

    void GetUp()
    {
        isSitting = false;

        characterAnimator.SetTrigger("Stop Sitting");
        spriteRenderer.material = outlineMaterial;

        characterMovementController.transform.position = playerPosition;

        collisionCollider.enabled = true;

        characterMovementController.CanMove = true;
    }
}