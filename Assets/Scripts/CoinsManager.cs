using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static CoinsManager _instance;
    public static CoinsManager Instance { get { return _instance; } }

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

    [SerializeField] private GameObject CoinPrefab = default;

    [SerializeField] private GameObject CollectCoinParticle = default;

    public void SpawnCoinPrefab(Vector3 pos, Transform parent)
    {
        Lean.Pool.LeanPool.Spawn(CoinPrefab, pos, Quaternion.identity, parent);
    }

    public void SpawnCoinCollectParticle(Vector3 pos)
    {
        Lean.Pool.LeanPool.Spawn(CollectCoinParticle, pos, Quaternion.identity, TileManager.Instance.World_T);
    }
}