using System;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static MainManager _instance;
    public static MainManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            UnlockFPS();
        }
    }

    #endregion SingletonAndAwake

    public enum GameState { Menu, GameSession, DeathMenu };

    public GameState gameState;

    [SerializeField] private bool PublishBuild;

    public GameSettings gameSettings = default;

    // True when player has revived once before
    [NonSerialized] public bool RevivedGameSession;

    private void Start()
    {
        LeanTween.reset();

        RevivedGameSession = false;

        StartMenuSession();

        if (DebugManager.Instance != null && DebugManager.Instance.enabled && DebugManager.Instance.EnterGameInstantOnPlay)
        {
            StartGameSession(false);
        }

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    private void UnlockFPS()
    {
        Application.targetFrameRate = gameSettings.TargetFrameRate;
    }

    public void ReseteGameElements(bool Revived)
    {
        print("ReseteGameElements()");

        if (PublishBuild)
            DebugManager.Instance.isEnabled = false;

        if (Revived)
            RevivedGameSession = true;

        TileManager.Instance.Init();
        PlayerManager.Instance.Init();
        ScoreManager.Instance.Init();
        UIManager.Instance.Init();
    }

    public void StartMenuSession()
    {
        TileManager.Instance.tilesType = TileManager.TilesType.NoObstacleTiles;
        PlayerManager.Instance.AutoNavigate = true;
        gameState = GameState.Menu;
        ReseteGameElements(false);
    }

    public void StartGameSession(bool Revived)
    {
        TileManager.Instance.tilesType = TileManager.TilesType.LowObstacleTiles;
        PlayerManager.Instance.AutoNavigate = false;
        gameState = GameState.GameSession;
        ReseteGameElements(Revived);
        PlayerManager.Instance.Jump();
    }

    public void StartDeathMenuSession()
    {
        // Scenario where the AutoNavigate fails and the player dies while in the Menu state.
        if (gameState == GameState.Menu)
            return;

        gameState = GameState.DeathMenu;
        ScoreManager.Instance.GameSessionEnd();
        AdsManager.Instance.GameSessionEnd();
        UIManager.Instance.GameSessionEnd();

        // Revive chance is only once per game session. If already revived then hide ReviveChanceGrp when 2nd death.
        ((DeathSG)UIManager.Instance.DeathSG).SetReviveChanceGrp(!RevivedGameSession);

        //Reset RevivedGameSession, for next Game Session
        RevivedGameSession = false;
    }

    public void ReviveIfCanAfford()
    {
        if (BankManager.Instance.DeductCoins(gameSettings.RevivePrice))
        {
            AudioManager.Instance.PlayBuySfx();
            Revive();
        }
        else
        {
            AudioManager.Instance.PlayErrorSfx();
            ((DeathSG)UIManager.Instance.DeathSG).BounceBankCoinsBtn();
        }
    }

    private void Revive()
    {
        // Set start score as last game session score
        int LastGameSessionScore = ScoreManager.Instance.score;
        StartGameSession(true);
        ScoreManager.Instance.AddScoreDistance(LastGameSessionScore);
    }
}