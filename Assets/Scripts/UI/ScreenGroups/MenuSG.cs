using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSG : ScreenGroup
{
    [SerializeField] private Image SfxImg;

    private void Start()
    {
        SfxImg.GetComponent<Button>().onClick.AddListener(ToggleSfx);
        SetSfxBehaviourAndIcons();
    }

    public void ToggleSfx()
    {
        if (PlayerPrefs.GetInt("SfxEnabled", 1) == 1)
            PlayerPrefs.SetInt("SfxEnabled", 0);
        else
        {
            PlayerPrefs.SetInt("SfxEnabled", 1);
            AudioManager.Instance.PlayBtnClickSfx();
        }

        SetSfxBehaviourAndIcons();
    }

    private void SetSfxBehaviourAndIcons()
    {
        if (PlayerPrefs.GetInt("SfxEnabled", 1) == 1)
            SfxImg.color = new Color(SfxImg.color.r, SfxImg.color.g, SfxImg.color.b, 1f);
        else
            SfxImg.color = new Color(SfxImg.color.r, SfxImg.color.g, SfxImg.color.b, 0.5f);
    }
}