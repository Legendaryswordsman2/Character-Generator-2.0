using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewAnimationManager : MonoBehaviour
{
    public CharacterPreviewAnimationSetSO CurrentCharacterAnimation { get; private set; }

    //[SerializeField] CharacterPreviewAnimationSetSO defaultCharacterAnimation;

    [Space]

    [SerializeField] Animator characterRightAnimator;
    [SerializeField] Animator characterDownAnimator;
    [SerializeField] Animator characterUpAnimator;
    [SerializeField] Animator characterLeftAnimator;

    [Space]

    [SerializeField] GameObject characterRightGO;
    [SerializeField] GameObject characterDownGO;
    [SerializeField] GameObject characterUpGO;
    [SerializeField] GameObject characterLeftGO;

    public event EventHandler<CharacterPreviewAnimationSetSO> OnAnimationChanged;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        TryCharacterButtonManager.MoveAllUIOffScreen += TryCharacterButtonManager_MoveAllUIOffScreen;
    }

    private void TryCharacterButtonManager_MoveAllUIOffScreen(object sender, float time)
    {
        LeanTween.scale(gameObject, Vector2.zero, time);
        LeanTween.alphaCanvas(canvasGroup, 0, time / 2);
    }

    public void SetCharacterAnimation(CharacterPreviewAnimationSetSO newAnimation)
    {
        CurrentCharacterAnimation = newAnimation;

        characterRightAnimator.runtimeAnimatorController = CurrentCharacterAnimation.CharacterRightController;
        characterRightGO.SetActive(characterRightAnimator.runtimeAnimatorController != null);

        characterDownAnimator.runtimeAnimatorController = CurrentCharacterAnimation.CharacterDownController;
        characterDownGO.SetActive(characterDownAnimator.runtimeAnimatorController != null);

        characterUpAnimator.runtimeAnimatorController = CurrentCharacterAnimation.CharacterUpController;
        characterUpGO.SetActive(characterUpAnimator.runtimeAnimatorController != null);

        characterLeftAnimator.runtimeAnimatorController = CurrentCharacterAnimation.CharacterLeftController;
        characterLeftGO.SetActive(characterLeftAnimator.runtimeAnimatorController != null);

        OnAnimationChanged?.Invoke(this, CurrentCharacterAnimation);
    }

    private void OnDestroy()
    {
        TryCharacterButtonManager.MoveAllUIOffScreen -= TryCharacterButtonManager_MoveAllUIOffScreen;
    }
}