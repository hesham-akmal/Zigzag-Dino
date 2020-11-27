using TMPro;
using UnityEngine;

public class InGameSG : ScreenGroup
{
    [SerializeField] private TextMeshProUGUI InGameScoreTMP = default;

    public void UpdateInGameScore(int score)
    {
        InGameScoreTMP.text = string.Format("{0:D1}", score);
    }
}