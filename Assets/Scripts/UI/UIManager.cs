using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            InitVars();
        }
    }

    #endregion SingletonAndAwake

    [SerializeField] private List<ScreenGroup> allScreenGroups;

    [SerializeField] public ScreenGroup MenuSG;
    [SerializeField] public ScreenGroup InGameSG;
    [SerializeField] public ScreenGroup DeathSG;

    private void InitVars()
    {
        allScreenGroups = new List<ScreenGroup>();
        allScreenGroups.Add(MenuSG);
        allScreenGroups.Add(InGameSG);
        allScreenGroups.Add(DeathSG);
        HideAllSGInstant();
    }

    public void Init()
    {
        ActivateSuitableSG();
    }

    public void GameSessionEnd()
    {
        ((DeathSG)DeathSG).GameSessionEnd();
        ActivateSuitableSG();
    }

    public void ActivateSuitableSG()
    {
        switch (MainManager.Instance.gameState)
        {
            case MainManager.GameState.Menu:
                HideEnabledSGAnim();
                ShowSG(MenuSG);
                break;

            case MainManager.GameState.GameSession:
                HideEnabledSGAnim();
                ShowSG(InGameSG);
                break;

            case MainManager.GameState.DeathMenu:
                HideEnabledSGAnim();
                ShowSG(DeathSG);
                break;
        }
    }

    private void HideEnabledSGAnim()
    {
        LeanTween.reset();
        foreach (ScreenGroup sg in allScreenGroups)
            if (sg.gameObject.activeSelf)
                sg.GetComponent<ScreenGroup>().AnimateMyChildrenOut();
    }

    private void HideAllSGInstant()
    {
        foreach (ScreenGroup sg in allScreenGroups)
            sg.gameObject.SetActive(false);
    }

    private void ShowSG(ScreenGroup sg)
    {
        sg.gameObject.SetActive(true);
        sg.AnimateMyChildrenIn();
    }

    public void UpdateInGameScore(int score)
    {
        InGameSG.GetComponent<InGameSG>().UpdateInGameScore(score);
    }
}