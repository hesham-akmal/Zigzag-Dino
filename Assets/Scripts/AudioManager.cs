using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion Singleton

    [SerializeField]
    private AudioSource[] AudioSources;

    [SerializeField]
    private AudioClip AudioClip_CactusHit;

    [SerializeField]
    private AudioClip AudioClip_ChangeDir;

    [SerializeField]
    private AudioClip AudioClip_FallDefeat;

    [SerializeField]
    private AudioClip AudioClip_BtnClick;

    [SerializeField]
    private AudioClip AudioClip_CoinCollect;

    [SerializeField]
    private AudioClip AudioClip_Jump;

    [SerializeField]
    private AudioClip AudioClip_Buy;

    [SerializeField]
    private AudioClip AudioClip_Error;

    ////// SFX //////////////////////////////////////////

    private bool isSfxEnabled()
    {
        return PlayerPrefs.GetInt("SfxEnabled", 1) == 1;
    }

    private void PlaySfx(AudioClip ac)
    {
        if (isSfxEnabled())
            AudioSources[0].PlayOneShot(ac);
    }

    public void PlayCactusHitSfx()
    {
        PlaySfx(AudioClip_CactusHit);
    }

    public void PlayChangeDirSfx()
    {
        PlaySfx(AudioClip_ChangeDir);
    }

    public void PlayFallDefeatSfx()
    {
        PlaySfx(AudioClip_FallDefeat);
    }

    public void PlayBtnClickSfx()
    {
        PlaySfx(AudioClip_BtnClick);
    }

    public void PlayCoinCollectSfx()
    {
        PlaySfx(AudioClip_CoinCollect);
    }

    public void PlayJumpSfx()
    {
        PlaySfx(AudioClip_Jump);
    }

    public void PlayBuySfx()
    {
        PlaySfx(AudioClip_Buy);
    }

    public void PlayErrorSfx()
    {
        PlaySfx(AudioClip_Error);
    }
}