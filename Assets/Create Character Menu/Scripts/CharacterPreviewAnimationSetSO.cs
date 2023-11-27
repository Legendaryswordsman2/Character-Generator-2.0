using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Preview Animation Set", fileName = "New Character Preview Animation Set")]
public class CharacterPreviewAnimationSetSO : ScriptableObject
{
    public string characterAnimationName = "New Animation";

    [Space]

    public RuntimeAnimatorController CharacterLeftController;
    public RuntimeAnimatorController CharacterDownController;
    public RuntimeAnimatorController CharacterUpController;
    public RuntimeAnimatorController CharacterRightController;
}