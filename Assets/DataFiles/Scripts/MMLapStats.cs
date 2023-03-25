using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMLapStats : MonoBehaviour
{
    public bool _isPlayer;

    //static instance
    public static MMLapStats instance;

    //Rank for leaderboard
    [SerializeField] public int _rankSetter;
    [SerializeField] public int _rank
    {
        get
        {
            return _rankSetter;
        }
        set { _rankSetter = value; }
    }

    [SerializeField] public string _racerName;
    [SerializeField] public string _vehicleName;

    private MMLeaderBoardManager _leaderBoardManager;


    [Header("Lap Time")]
    [SerializeField]
    private float _currTime;
    [SerializeField]
    private float _prevTime;
    [SerializeField]
    private float _bestTime;
    [SerializeField]
    private float _totalTime;
    [SerializeField]
    private float _timeOfReach;


    //Collider collision tag counter
    [Header("Lap Counter")]
    [SerializeField]
    private int _count = 0;
    [SerializeField]
    private int _Fcount = 0;
    [SerializeField]
    private int _PreFcount = 0;
    [SerializeField]
    private int _PostCount = 0;
    [SerializeField]
    private int _MidCount = 0;


    [Header("String For Lap Stats")]
    [SerializeField] private string _currLap;
    [SerializeField] private string _bestLap;
    [SerializeField] private string _lastLap;
    [SerializeField] private string _lap;

    [Header("String For Lap Stats")]
    [SerializeField] private Text _currLapT;
    [SerializeField] private Text _bestLapT;
    [SerializeField] private Text _lastLapT;
    [SerializeField] private Text _lapT;

    [Header("LapCount")]
    [SerializeField]
    private int _lapCountLocal = 1;


    bool _isFirstLap = false;

    private void Start()
    {
        _lapCountLocal = 1;
        instance = this;
        if (instance != null)
        {
            instance = this;
        }
        _currTime = Time.time;
    }

    private void Update()
    {
        float _t = Time.time - _currTime;
        _totalTime = _t;
        string _min = ((int)_t / 60).ToString();
        string _sec = (_t % 60).ToString("f0");
        string _miliSec = ((_t % 1) * 100).ToString("f0");
        if (_isPlayer)
        {
            _currLapT.text = "TOTAL " + _min + " : " + _sec + " : " + _miliSec;
        }
        _currLap = "TOTAL " + _min + " : " + _sec + " : " + _miliSec;


        //Check for lap updation by counters
        CheckForLapUpdation();
    }

    private void CheckForLapUpdation()
    {
        if (_count >= 6 && _Fcount >= 1 && _PreFcount >= 1 && _PostCount >= 1 && _MidCount >= 1)
        {
            UpdatePrevLap();
            IncLap();
            StartCoroutine(ResetCounters());
        }
    }

    IEnumerator ResetCounters()
    {
        _count = 0;
        yield return new WaitForSeconds(0.5f);
        _Fcount = 0;
        _PreFcount = 0;
        _PostCount = 0;
        _MidCount = 0;
    }

    public void ResetLapCounter()
    {
        _count = 0;
        _Fcount = 0;
        _PreFcount = 0;
        _PostCount = 0;
        _MidCount = 0;
    }

    public void RaceFinished() 
    {
        if(_isPlayer)
        {
            LapManager.instance.RaceFinished();
        }
        GameObject _g = Instantiate(LapManager.instance._LeaderBoardInstance._playerStats, LapManager.instance._LeaderBoardInstance._spawnPoint.transform);
        ShowPlayerDetails(_g);
        Debug.LogWarning(" ## Race finished Executed ## ");
    }

    private void ShowPlayerDetails(GameObject _g)
    {
        MMPlayerStatsManager _mmPlayerStatsManager = _g.GetComponent<MMPlayerStatsManager>();
        _mmPlayerStatsManager._name.text = _racerName;
        _mmPlayerStatsManager._rank.text = _rank.ToString();
    }

    public void UpdatePrevLap()
    {
        if (_isFirstLap == false)
        {
            _timeOfReach = _totalTime;
            _prevTime = _totalTime - _currTime;
            _bestTime = _totalTime - _currTime;
            SetBestLap();
            _isFirstLap = true;
        }
        else
        {
            _prevTime = _totalTime - _timeOfReach;
            _timeOfReach = _totalTime;
        }

        string _min = ((int)_prevTime / 60).ToString();
        string _sec = (_prevTime % 60).ToString("f0");
        string _miliSec = ((_prevTime % 1) * 100).ToString("f0");
        if (_isPlayer)
        {
            _lastLapT.text = "Last " + _min + " : " + _sec + " : " + _miliSec;
        }
        _lastLap = "Last " + _min + " : " + _sec + " : " + _miliSec;
        if (_prevTime < _bestTime)
        {
            _bestTime = _prevTime;
            SetBestLap();
        }
    }
    public void SetBestLap()
    {
        string _min = ((int)_bestTime / 60).ToString();
        string _sec = (_bestTime % 60).ToString("f0");
        string _miliSec = ((_bestTime % 1) * 100).ToString("f0");
        if (_isPlayer)
        {
            _bestLapT.text = "Best " + _min + " : " + _sec + " : " + _miliSec;
        }
        _bestLap = "Best " + _min + " : " + _sec + " : " + _miliSec;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            _count++;
        }
        if (other.gameObject.CompareTag("FinalCheckPoint"))
        {
            _Fcount++;

        }
        if (other.gameObject.CompareTag("PreFinalCheckPoint"))
        {
            _PreFcount++;
        }
        if (other.gameObject.CompareTag("PostFinalCheckPoint"))
        {
            _PostCount++;
        }
        if (other.gameObject.CompareTag("MidFinalCheckPoint"))
        {
            _MidCount++;
        }
    }

    public void ReduceCount()
    {
        _count = 1;
        if (_lapCountLocal < 1)
        {
            _lapCountLocal--;
            SetLap(_lapCountLocal);
        }
    }
    public void SetLap(int _lapcount)
    {
        if (_isPlayer)
        {
            _lapT.text = "LAP " + _lapcount.ToString();
        }
    }
    public void IncLap()
    {
        _lapCountLocal++;
        SetLap(_lapCountLocal);
    }
}
