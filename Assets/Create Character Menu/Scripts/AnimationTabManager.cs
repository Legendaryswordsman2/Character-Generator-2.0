using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTabManager : MonoBehaviour
{
    [SerializeField] CharacterPreviewAnimationManager characterPreviewAnimationManager;

    [Space]

    [SerializeField] AnimationSection[] characterTypeAnimationSections;

    CharacterPieceDatabase characterPieceDatabase;

    AnimationSection activeSection;

    private void Start()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        RefreshSection();

        if (activeSection != null) characterPreviewAnimationManager.SetCharacterAnimation(activeSection.DefaultAnimationSet);
    }
    void RefreshSection()
    {
        foreach (AnimationSection section in characterTypeAnimationSections)
        {
            section.Section.SetActive(section.SectionCharacterType == characterPieceDatabase.ActiveCharacterType);
            if (section.SectionCharacterType == characterPieceDatabase.ActiveCharacterType) activeSection = section;
        }
    }

    [System.Serializable]
    class AnimationSection
    {
        public CharacterTypeSO SectionCharacterType;
        public CharacterPreviewAnimationSetSO DefaultAnimationSet;
        public GameObject Section;
    }
}