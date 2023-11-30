using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupMessage : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text detailsText;

    string[] textFrames = new string[4];

    int index;

    bool isActive = false;

    Animator anim;

    private void Awake()
    {
        textFrames[0] = titleText.text;

        textFrames[1] = titleText.text + ".";

        textFrames[2] = titleText.text + "..";

        textFrames[3] = titleText.text + "...";

        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isActive = true;
        CharacterPieceGrabber.OnNewSpriteLoaded += CharacterPieceGrabber_OnNewSpriteLoaded;
        CycleText();
    }

    private void CharacterPieceGrabber_OnNewSpriteLoaded(object sender, CharacterPieceGrabber.OnNewSpriteLoadedEventArgs e)
    {
        detailsText.text = e.Sprite.name + e.Extention;
    }

    private void OnDisable()
    {
        isActive = false;
        CharacterPieceGrabber.OnNewSpriteLoaded -= CharacterPieceGrabber_OnNewSpriteLoaded;
    }

    public void OnFinishedSettingUp()
    {
        isActive = false;
        titleText.text = "";
        detailsText.text = "";
        anim.SetTrigger("Trigger");
    }

    async void CycleText()
    {
        if (index == 4)
            index = 0;

        titleText.text = textFrames[index];
        index++;


        await UniTask.Delay(500);

        if (!isActive) return;

        CycleText();
    }

    public void OnSetupBackgroundAnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
