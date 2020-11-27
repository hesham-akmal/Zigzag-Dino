using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static DebugManager _instance;
    public static DebugManager Instance { get { return _instance; } }

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

    [NonSerialized] public bool isEnabled = true;

    public bool ForceAutoNaviagte;
    public bool ForceNoObstacles;
    public bool EnterGameInstantOnPlay;

    private void Start()
    {
        if (isEnabled)
        {
            if (EnterGameInstantOnPlay)
            {
                MainManager.Instance.ReseteGameElements(false);
                MainManager.Instance.gameState = MainManager.GameState.GameSession;
                TileManager.Instance.StartMovement();
            }
        }
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (ForceAutoNaviagte)
            PlayerManager.Instance.AutoNavigate = true;

        if (ForceNoObstacles)
            TileManager.Instance.tilesType = TileManager.TilesType.NoObstacleTiles;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            TileManager.Instance.WorldCurrentMovementSpeed += 10;

        if (Input.GetKeyDown(KeyCode.S))
            PlayerManager.Instance.Jump();

        if (Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}