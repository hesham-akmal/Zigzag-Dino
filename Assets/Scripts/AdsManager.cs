using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    #region SingletonAndAwake

    private static AdsManager _instance;
    public static AdsManager Instance { get { return _instance; } }

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

    private const string gameId = "3911149";
    private string myPlacementId = "rewardedVideo";
    [SerializeField] private GameObject WatchAdPlusSign = default;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Advertisement.Initialize(gameId);
        Advertisement.AddListener(this);
    }

    private void ShowOrHideWatchAdPlusSign()
    {
        if (Advertisement.IsReady(myPlacementId))
            WatchAdPlusSign.SetActive(true);
        else
            WatchAdPlusSign.SetActive(false);
    }

    public void GameSessionEnd()
    {
        ShowOrHideWatchAdPlusSign();
    }

    public void ShowRewardedVideo()
    {
        if (Advertisement.IsReady(myPlacementId))
            Advertisement.Show(myPlacementId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            BankManager.Instance.AddCoins(MainManager.Instance.gameSettings.CoinsAddedFromRewardAd);
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
        ShowOrHideWatchAdPlusSign();
    }

    public void OnUnityAdsDidError(string message)
    {
        print(message);
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
        // Pause game (for ios version)
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (myPlacementId == placementId)
            ShowOrHideWatchAdPlusSign();
    }
}