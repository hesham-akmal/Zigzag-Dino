using UnityEngine;
using Lean.Pool;
using System.Collections.Generic;
using System.Collections;

public class TileManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static TileManager _instance;
    public static TileManager Instance { get { return _instance; } }

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

    public float WorldCurrentMovementSpeed;

    [SerializeField] public float WorldMovementSpeedAcceleration;

    [SerializeField] private int TilesNumSpawnAtStart = default;

    [SerializeField] private int TilesDespawnAtZaxis = default;

    ////////////////////////////////////////////////////////////////////////////////////

    public enum TilesType { NoObstacleTiles, LowObstacleTiles, CactusTilesOnly, JumpTilesOnly, TrickTilesOnly };

    public TilesType tilesType;

    public enum WorldMovementState { Idle, Right, Left };

    public WorldMovementState worldMovementState = WorldMovementState.Idle;

    ////////////////////////////////////////////////////////////////////////////////////

    [SerializeField] private GameObject BlankTilePrefab = default;
    [SerializeField] private GameObject JumpTilePrefab = default;
    [SerializeField] private GameObject CactusTilePrefab = default;
    [SerializeField] private GameObject TrickTilePrefab = default;
    [SerializeField] private GameObject StartTilePrefab = default;

    //Latest spawned tile
    private GameObject YoungestTile;

    ////////////////////////////////////////////////////////////////////////////////////

    [SerializeField] public Transform World_T = default;

    private Quaternion LeftDirQuaternion = Quaternion.Euler(0, -45, 0);
    private Quaternion RightDirQuaternion = Quaternion.Euler(0, 45, 180);

    private Vector3 CoinsOnTileOffset = new Vector3(0, 10, 0);

    public void Init()
    {
        CancelInvoke();
        StopAllCoroutines();

        DespawnAllWorldObjects();

        SpawnStartTilePrefab();

        AddTiles(TilesNumSpawnAtStart);

        SetWorldMovementToRight();

        StartMovement();

        InvokeRepeating("CheckDespawnGOsUpdate", 0, 0.2f);
    }

    private void Update()
    {
        MovementSpeedIncrementUpdate();
    }

    private void FixedUpdate()
    {
        WorldMovementUpdate();
    }

    private void SpawnStartTilePrefab()
    {
        YoungestTile = LeanPool.Spawn(StartTilePrefab, Vector3.zero, LeftDirQuaternion, World_T);
        YoungestTile.transform.localPosition = Vector3.zero;
    }

    private void AddTile()
    {
        //Get first or second tile child
        Vector3 newTilePos = YoungestTile.transform.GetChild(Random.Range(0, 2)).position;

        // The tile that will be spawned
        GameObject RandTilePrefab = default;

        switch (tilesType)
        {
            case TilesType.LowObstacleTiles:

                if (Random.Range(1f, 100f) <= MainManager.Instance.gameSettings.LowObstacleTilesPercentage)
                {
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            RandTilePrefab = JumpTilePrefab;
                            break;

                        case 1:
                            RandTilePrefab = CactusTilePrefab;
                            break;

                        case 2:
                            RandTilePrefab = TrickTilePrefab;
                            break;
                    }
                }
                else
                {
                    RandTilePrefab = BlankTilePrefab;
                }

                break;

            case TilesType.NoObstacleTiles:
                RandTilePrefab = BlankTilePrefab;
                break;

            case TilesType.CactusTilesOnly:
                RandTilePrefab = CactusTilePrefab;
                break;

            case TilesType.JumpTilesOnly:
                RandTilePrefab = JumpTilePrefab;
                break;

            case TilesType.TrickTilesOnly:
                RandTilePrefab = TrickTilePrefab;
                break;
        }

        // Random rotation for the following tile types
        Quaternion rotation = LeftDirQuaternion;

        if (RandTilePrefab == BlankTilePrefab ||
            RandTilePrefab == TrickTilePrefab ||
            RandTilePrefab == JumpTilePrefab)
        {
            if (GameSettings.RandomBool())
                rotation = RightDirQuaternion;
            else
                rotation = LeftDirQuaternion;
        }

        //Add a new tile to the end of latest tile (linked list kind of situation)
        YoungestTile = LeanPool.Spawn(RandTilePrefab, newTilePos, rotation, World_T);

        //Cactus tiles have special randomization technique
        if (RandTilePrefab == CactusTilePrefab)
        {
            YoungestTile.GetComponent<CactusTile>().RandomizeDir();
        }
        // Coins generation
        else if (RandTilePrefab == BlankTilePrefab)
        {
            if (MainManager.Instance.gameState == MainManager.GameState.GameSession)
                SpawnCoinRandomly(YoungestTile.transform.position + CoinsOnTileOffset);
        }
    }

    private void AddTiles(int num)
    {
        for (int i = 0; i < num; i++)
            AddTile();
    }

    private void CheckDespawnGOsUpdate()
    {
        for (int i = 0; i < World_T.childCount; i++)
        {
            if (World_T.GetChild(i).transform.position.z < TilesDespawnAtZaxis)
            {
                LeanPool.Despawn(World_T.GetChild(i));

                if (World_T.GetChild(i).CompareTag("Tile"))
                    AddTile();
            }
        }
    }

    private void DespawnAllWorldObjects()
    {
        for (int i = World_T.childCount - 1; i >= 0; i--)
            LeanPool.Despawn(World_T.GetChild(i));
    }

    private void SetWorldMovementToRight()
    {
        worldMovementState = WorldMovementState.Right;
        SyncPlayerDir();
    }

    public void ToggleWorldMovementDir()
    {
        switch (worldMovementState)
        {
            case WorldMovementState.Right:
                worldMovementState = WorldMovementState.Left;
                break;

            case WorldMovementState.Left:
                worldMovementState = WorldMovementState.Right;
                break;
        }
        SyncPlayerDir();
    }

    private void SyncPlayerDir()
    {
        switch (worldMovementState)
        {
            case WorldMovementState.Right:
                PlayerManager.Instance.SetDinoDirection(true);
                break;

            case WorldMovementState.Left:
                PlayerManager.Instance.SetDinoDirection(false);
                break;
        }
    }

    /// <summary>
    /// Moves all world transforms, runs in FixedUpdate.
    /// </summary>
    private void WorldMovementUpdate()
    {
        Vector3 direction;

        switch (worldMovementState)
        {
            case WorldMovementState.Right:
                direction = -World_T.transform.right;
                break;

            case WorldMovementState.Left:
                direction = -World_T.transform.forward;
                break;

            default:
                direction = Vector3.zero;
                break;
        }

        foreach (Transform t in World_T)
            t.position += (direction * WorldCurrentMovementSpeed * 0.01f);
    }

    public IEnumerator StopMovementSmooth(float timeToEnd)
    {
        float timer = 0f;
        float StartSpeed = WorldCurrentMovementSpeed;

        while (timer < timeToEnd)
        {
            float lerpFactor = timer / timeToEnd;
            WorldCurrentMovementSpeed = Mathf.Lerp(StartSpeed, 0, lerpFactor);
            timer += Time.deltaTime;
            yield return null;
        }
        WorldCurrentMovementSpeed = 0;
    }

    private void SpawnCoinRandomly(Vector3 pos)
    {
        if (Random.Range(1, 101) <= MainManager.Instance.gameSettings.CoinsPercentage)
            CoinsManager.Instance.SpawnCoinPrefab(pos, World_T);
    }

    /// <summary>
    /// Starts tiles movement
    /// </summary>
    public void StartMovement()
    {
        if (MainManager.Instance.RevivedGameSession)
            WorldCurrentMovementSpeed = PlayerManager.Instance.MovementSpeedBeforeDeath;
        else
            WorldCurrentMovementSpeed = MainManager.Instance.gameSettings.WorldStartMovementSpeed;
    }

    private void MovementSpeedIncrementUpdate()
    {
        if (MainManager.Instance.gameState != MainManager.GameState.GameSession)
            return;

        float NewMovementSpeed = WorldCurrentMovementSpeed + WorldMovementSpeedAcceleration * 0.1f * Time.deltaTime;

        WorldCurrentMovementSpeed = Mathf.Clamp(NewMovementSpeed,
            MainManager.Instance.gameSettings.WorldStartMovementSpeed,
            MainManager.Instance.gameSettings.WorldFinalMovementSpeed);
    }
}