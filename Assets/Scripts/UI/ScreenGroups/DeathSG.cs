using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathSG : ScreenGroup
{
    [SerializeField] private TextMeshProUGUI RevivePriceTMP = default;
    [SerializeField] private TextMeshProUGUI BankTMP = default;

    [SerializeField] private TextMeshProUGUI ScoreTMP = default;
    [SerializeField] private TextMeshProUGUI HighScoreTMP = default;

    [SerializeField] private GameObject ReviveChanceGrp = default;
    [SerializeField] private Button BankCoinsBtn = default;

    private void Start()
    {
        UpdateRevivePriceTMP();
    }

    public void UpdateRevivePriceTMP()
    {
        RevivePriceTMP.text = MainManager.Instance.gameSettings.RevivePrice.ToString();
    }

    public void UpdateBankTMP()
    {
        BankTMP.text = BankManager.Instance.getBankCoins().ToString();
    }

    public void GameSessionEnd()
    {
        UpdateBankTMP();
    }

    public void UpdateScoresTMP(string Score, string HighScore)
    {
        ScoreTMP.text = Score;
        HighScoreTMP.text = HighScore;
    }

    public void SetReviveChanceGrp(bool Enabled)
    {
        ReviveChanceGrp.SetActive(Enabled);
    }

    public void BounceBankCoinsBtn()
    {
        BankCoinsBtn.GetComponent<BtnBehaviour>().Bounce();
    }
}