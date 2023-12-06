using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDropdown : MonoBehaviour
{
    [SerializeField] CharacterDropdownManager dropdownManager;

    [Space]

    [SerializeField] int dropdownIndex = 0;

    public void OnDropdownChanged(int index)
    {
        dropdownManager.CharacterPiecesDropdownData[dropdownIndex].SetActiveSprite(index);

        //if (initialized)
        dropdownManager.OnDropdownUpdated(dropdownIndex);
    }
}