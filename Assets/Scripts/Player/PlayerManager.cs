using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

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

        InitComponents();
    }

    #endregion SingletonAndAwake

    //////////////////////////////////////////////////////////////
    ///LegsMovement
    private float LegsTimer = 0;

    [SerializeField] private float FootstepsTime = default;

    private bool LegsFlipFlop = false;

    [SerializeField] private GameObject RightLeg = default;
    [SerializeField] private GameObject LeftLeg = default;

    [SerializeField] private Transform RightLegUpPos = default;
    [SerializeField] private Transform RightLegBottomPos = default;

    [SerializeField] private Transform LeftLegUpPos = default;
    [SerializeField] private Transform LeftLegBottomPos = default;

    //////////////////////////////////////////////////////////////
    /// Player States
    public enum PlayerState { Running, Jumping, FallingFromJump, FallingToDeath };

    public PlayerState playerState;

    private bool isAlive = true;

    //////////////////////////////////////////////////////////////
    /// Direction

    [SerializeField] private GameObject Dino = default;

    [SerializeField] private Transform DinoRightDir_T = default;
    [SerializeField] private Transform DinoLeftDir_T = default;

    private MeshRenderer DinoMR;
    private Rigidbody DinoRB;
    private BoxCollider DinoBoxCollider;
    private SphereCollider DinoSphereCollider;

    [SerializeField] private Material DinoRightDir_Mat = default;
    [SerializeField] private Material DinoLeftDir_Mat = default;

    //////////////////////////////////////////////////////////////

    [NonSerialized] public bool AutoNavigate;

    // Used for when player is reviving, setting start movement speed to this.
    [NonSerialized] public float MovementSpeedBeforeDeath = 0;

    //////////////////////////////////////////////////////////////

    private void InitComponents()
    {
        DinoMR = Dino.GetComponent<MeshRenderer>();
        DinoBoxCollider = Dino.GetComponent<BoxCollider>();
        DinoSphereCollider = Dino.GetComponent<SphereCollider>();
        DinoRB = Dino.GetComponent<Rigidbody>();
        //Disable rigid body sleeping
        DinoRB.sleepThreshold = 0;
    }

    public void Init()
    {
        CancelInvoke();
        StopAllCoroutines();

        Dino.transform.localPosition = Vector3.zero;
        DinoBoxCollider.enabled = true;
        DinoSphereCollider.enabled = true;

        //Freeze all except Y pos, for falling mechanism
        DinoRB.constraints = RigidbodyConstraints.FreezeAll;
        DinoRB.constraints = ~RigidbodyConstraints.FreezePositionY;

        DinoRB.isKinematic = false;
        isAlive = true;
        playerState = PlayerState.Running;
    }

    private void Update()
    {
        if (isAlive)
        {
            DinoLegsMovementUpdate();
            AutoNavigateUpdate();
            CheckPlayerTapInput();
        }

        StateMachineUpdate();

        //Debug.DrawRay(Dino.transform.position, -Dino.transform.right * 100, Color.red);//
    }

    private void LateUpdate()
    {
        GravityUpdate();
    }

    /// <summary>
    /// Toggles one leg upwards and the other downwards, for legs movement animation
    /// </summary>
    private void DinoLegsMovementUpdate()
    {
        LegsTimer += Time.deltaTime;
        if (LegsTimer > FootstepsTime && playerState == PlayerState.Running)
        {
            LegsFlipFlop = !LegsFlipFlop;
            if (LegsFlipFlop)
            {
                RightLeg.transform.localPosition = RightLegUpPos.localPosition;
                LeftLeg.transform.localPosition = LeftLegBottomPos.localPosition;
            }
            else
            {
                RightLeg.transform.localPosition = RightLegBottomPos.localPosition;
                LeftLeg.transform.localPosition = LeftLegUpPos.localPosition;
            }
            LegsTimer = 0;
        }
    }

    private void AutoNavigateUpdate()
    {
        // Bit shift the index of the layer 8 to get a bit mask
        // This would cast rays only against colliders in layer 8 (Tile Layer).
        int layerMask = 1 << 8;

        // If true Check future fall event and avoid it.
        if (AutoNavigate && playerState == PlayerState.Running)
            if (
                (TileManager.Instance.worldMovementState == TileManager.WorldMovementState.Left && !Physics.Raycast(Dino.transform.position + new Vector3(-5, 0, 5), -transform.up, 50, layerMask))
                ||
                (TileManager.Instance.worldMovementState == TileManager.WorldMovementState.Right && !Physics.Raycast(Dino.transform.position + new Vector3(5, 0, 5), -transform.up, 50, layerMask))
                )
            {
                TileManager.Instance.ToggleWorldMovementDir();
            }
    }

    private void CheckPlayerTapInput()
    {
        if (MainManager.Instance.gameState != MainManager.GameState.GameSession
            || AutoNavigate
            || playerState == PlayerState.Jumping
            || playerState == PlayerState.FallingToDeath)
            return;

        if (InputManager.Instance.Tap)
        {
            if (playerState == PlayerState.Running && isTapJumpForward())
            {
                Jump();
            }
            else
            {
                TileManager.Instance.ToggleWorldMovementDir();
                ScoreManager.Instance.AddScoreDistance(1);
                AudioManager.Instance.PlayChangeDirSfx();
            }
        }
    }

    private void StateMachineUpdate()
    {
        // State Machine Transition :: From <Jumping> to <FallingFromJump>
        if (playerState == PlayerState.Jumping && DinoRB.velocity.y < -5f)
            playerState = PlayerState.FallingFromJump;

        // State Machine Transition :: From <FallingFromJump> to <Running>
        else if (playerState == PlayerState.FallingFromJump && DinoRB.velocity.y == 0 && Dino.transform.localPosition.y < 1)
            playerState = PlayerState.Running;
    }

    private void GravityUpdate()
    {
        //Starts falling downwards to death
        if (Dino.transform.localPosition.y < -0.5f && isAlive)
        {
            StartFallToDeath();
        }

        PredictEarlyFallDeath();

        //Stops player from falling out of Y axis bounds (downward)
        if (Dino.transform.position.y < -1000)
            DinoRB.isKinematic = true;
    }

    /// <summary>
    /// This method runs in update. Used for when the player jumps in a jump tile.
    /// While the player is falling from the jump, it starts predicting if player won't make it to the
    /// other tile, if true then it starts fall death early on, this is to prevent player mesh intersecting with tiles (as much as possible).
    /// </summary>
    private void PredictEarlyFallDeath()
    {
        const int FutureDistance = 2;
        const int FutureHeight = 4;

        if (playerState == PlayerState.FallingFromJump)
        {
            bool isGroundBelowFuturePosition = false;
            if ((TileManager.Instance.worldMovementState == TileManager.WorldMovementState.Left &&
                    !isGroundUnder(new Vector3(Dino.transform.position.x - FutureDistance, Dino.transform.position.y, Dino.transform.position.z + FutureDistance)))
                    ||
                    (TileManager.Instance.worldMovementState == TileManager.WorldMovementState.Right &&
                    !isGroundUnder(new Vector3(Dino.transform.position.x + FutureDistance, Dino.transform.position.y, Dino.transform.position.z + FutureDistance)))
                 )
                isGroundBelowFuturePosition = true;

            if (Dino.transform.localPosition.y < FutureHeight
            && isGroundBelowFuturePosition
            && !isTapJumpForward())
            {
                StartFallToDeath();
            }
        }
    }

    public void SetDinoDirection(bool Right)
    {
        if (Right)
        {
            DinoMR.material = DinoRightDir_Mat;
            Dino.transform.rotation = DinoRightDir_T.rotation;
        }
        else
        {
            DinoMR.material = DinoLeftDir_Mat;
            Dino.transform.rotation = DinoLeftDir_T.rotation;
        }
    }

    private void StartFallToDeath()
    {
        if (!isAlive) return;

        isAlive = false;

        // State Machine Transition :: From <Any> to <FallingToDeath>
        playerState = PlayerState.FallingToDeath;

        DinoBoxCollider.enabled = false;
        DinoSphereCollider.enabled = false;

        print("Dead");

        MovementSpeedBeforeDeath = TileManager.Instance.WorldCurrentMovementSpeed;

        StartCoroutine(TileManager.Instance.StopMovementSmooth(1));

        AudioManager.Instance.PlayFallDefeatSfx();

        MainManager.Instance.StartDeathMenuSession();
    }

    /// <summary>
    /// Detects if an object with layer "TapJump" is in front of player
    /// </summary>
    /// <returns></returns>
    private bool isTapJumpForward()
    {
        // Bit shift the index of the layer 9 to get a bit mask, This would cast rays only against colliders in TapJump Layer. Cactus is an example of a TampJump Obstacle.
        int layerMask = 1 << 9;

        // Does the ray intersect a cactus
        if (Physics.Raycast(Dino.transform.position, -Dino.transform.right, 100, layerMask))
            return true;

        return false;
    }

    private bool isGroundUnder(Vector3 pointPos)
    {
        // Bit shift the index of the layer 8 to get a bit mask
        // This would cast rays only against colliders in layer 8 (Tile Layer).
        int layerMask = 1 << 8;

        //RaycastHit hit;
        // Does the ray intersect any tiles
        Debug.DrawRay(pointPos, -transform.up * 50, Color.red);
        if (Physics.Raycast(pointPos, -transform.up, 50, layerMask))
            return true;

        return false;
    }

    public void OnTriggerWithCactus()
    {
        StopAllCoroutines();
        StartFallToDeath();
        DinoRB.constraints = ~RigidbodyConstraints.FreezePositionZ;
        DinoRB.AddForce(0, 100, -75, ForceMode.Impulse);
        AudioManager.Instance.PlayCactusHitSfx();
    }

    public void Jump()
    {
        if (playerState == PlayerState.Running)
        {
            // State Machine Transition :: From <Running> to <Jumping>
            playerState = PlayerState.Jumping;
            DinoRB.AddForce(0, 120, 0, ForceMode.Impulse);
            AudioManager.Instance.PlayJumpSfx();
        }
    }

    public void CollectCoin(GameObject CoinCollected)
    {
        CoinsManager.Instance.SpawnCoinCollectParticle(CoinCollected.transform.position);
        Lean.Pool.LeanPool.Despawn(CoinCollected);
        BankManager.Instance.AddCoins(1);
        AudioManager.Instance.PlayCoinCollectSfx();
    }
}