using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : UIManagerParent
{
    [Header("Buttons")]
    [SerializeField] private Button _lavaMode;
    [SerializeField] private Button _cityRace;
    [SerializeField] private Button _beachMode;
    [SerializeField] private Button _garage;
    [SerializeField] private Button _homeO;
    [SerializeField] private Button _homeT;
    [SerializeField] private Button _leaderBoard;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenupanel;
    [SerializeField] private GameObject _garagePanel;
    [SerializeField] public GameObject _leaderBoardPanel;

    public GameObject _loadingScreen;
    public Image _loadingProgressBar;
    List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();
    private void Start()
    {
        SetLisForSceneLoad(_lavaMode, _mainMenupanel, _loadingScreen, _loadingProgressBar, _scenesToLoad, true, 2);
        SetLisForSceneLoad(_beachMode, _mainMenupanel, _loadingScreen, _loadingProgressBar, _scenesToLoad, true, 3);
        SetLoadingRef(_loadingScreen);
        SetListener();
    }


    #region

  

    #endregion




    private void SetListener()
    {
        _garage.onClick.AddListener(LoadGarage);
        _homeO.onClick.AddListener(LoadMainMenuFromGarage);
        _homeT.onClick.AddListener(LoadMainMenuFromLeaderBoard);
        _leaderBoard.onClick.AddListener(LoadLeaderBoard);
    }

    private void LoadLeaderBoard()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel, false, 1.1f);
        Toggle(_leaderBoardPanel, true, 0.9f);
    }

    private void LoadMainMenuFromGarage()
    {
        InitiateLoad(1f);
        Toggle(_garagePanel, false, 0.9f);
        Toggle(_mainMenupanel, true, 1.1f);
    }private void LoadMainMenuFromLeaderBoard()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel, true, 0.9f);
        Toggle(_leaderBoardPanel, false, 1f);
    }

    private void LoadGarage()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel,false,1.1f);
        Toggle(_garagePanel,true,1f);
    }
}
