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

        characterPieceDatabase.OnActiveCharacterTypeChanged += CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }

    private void CharacterPieceDatabase_OnActiveCharacterTypeChanged(object sender, CharacterTypeSO e)
    {
        RefreshSection();
    }

    void RefreshSection()
    {
        foreach (AnimationSection section in characterTypeAnimationSections)
        {
            section.Section.SetActive(section.SectionCharacterType == characterPieceDatabase.ActiveCharacterType);
            if (section.SectionCharacterType == characterPieceDatabase.ActiveCharacterType) activeSection = section;
        }

        if (activeSection != null) characterPreviewAnimationManager.SetCharacterAnimation(activeSection.DefaultAnimationSet);
    }

    private void OnDestroy()
    {
        characterPieceDatabase.OnActiveCharacterTypeChanged -= CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }

    [System.Serializable]
    class AnimationSection
    {
        public CharacterTypeSO SectionCharacterType;
        public CharacterPreviewAnimationSetSO DefaultAnimationSet;
        public GameObject Section;
    }
}