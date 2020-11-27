using UnityEngine;
using System;
using SaveSystem;

public class ScoreManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }

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

    #endregion SingletonAndAwake

    [NonSerialized] public int score;

    private int highScore;

    private Cryptography crypto;

    public void Init()
    {
        crypto = new Cryptography();

        score = 0;

        if (EasySave.HasKey<string>("highscore"))
            highScore = getHighscore();
        else
            highScore = 0;

        UpdateInGameScoreUI(); // Set initial score in UI
    }

    private int getHighscore()
    {
        return crypto.Decrypt<int>(EasySave.Load<string>("highscore"));
    }

    private void UpdateInGameScoreUI()
    {
        UIManager.Instance.UpdateInGameScore(score); // Post score to text field
    }

    public void AddScoreDistance(int pointsToAdd)
    {
        score += pointsToAdd; // Add points to current score

        UpdateInGameScoreUI();
    }

    /// <summary>
    /// Checks score against high score and saves if higher
    /// </summary>
    private void CheckHigherScore()
    {
        // If new high score
        if (score > highScore)
        {
            highScore = score;
            EasySave.Save("highscore", crypto.Encrypt(highScore)); // Store high score
            ((DeathSG)UIManager.Instance.DeathSG).UpdateScoresTMP("new highscore\n" + score, "");
        }
        else
            ((DeathSG)UIManager.Instance.DeathSG).UpdateScoresTMP("score " + score, "high score " + highScore);
    }

    public void GameSessionEnd()
    {
        CheckHigherScore();
    }
}