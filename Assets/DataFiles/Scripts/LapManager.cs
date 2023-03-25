using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LapManager : MonoBehaviour
{
    public static LapManager instance;

    public MMLeaderBoardManager _LeaderBoardInstance;


    [SerializeField] private Transform _node;
    [SerializeField] private Transform _racerLookAt;
    [SerializeField] private Transform _racer;

    [SerializeField] private GameObject _PlayerCar;
    [SerializeField] private GameObject _startText;
    [SerializeField] private GameObject _finishText;
    [SerializeField] private GameObject _recentPlayer;

/*    [Header("UI")]
    [SerializeField] Text _currLapT;
    [SerializeField] Text _bestLapT;
    [SerializeField] Text _lastLapT;
    [SerializeField] Text _lapT;*/

    [Header("String Lap stats")]
    [SerializeField] string _currLap;
    [SerializeField] string _bestLap;
    [SerializeField] string _lastLap;
    [SerializeField] string _lap;

    [Header("Target Lap")]
    private int _totalLaps = 3;

    private float _currTime;
    private float _prevTime;
    private float _bestTime;
    private float _totalTime;
    private float _timeOfReach;
   
    private Vector3 _pos;
    private Quaternion _rot;
    private int _lapCountLocal=0;
    bool _isFirstLap = false;


    #region INT Variables

    private int _rankCount = 0;

    #endregion
    private void Awake()
    {
        _pos = _node.position;
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Debug.LogWarning(" ## Total Lap ## " + _totalLaps + " Local lap " + _lapCountLocal);
        _pos = _node.position;
        _rot = _node.rotation;
      /*  _pos = new Vector3(-100, 10, 335);
        _rot = Quaternion.Euler(0, 120, 0);*/
        IncLap();
        _currTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetButton("Fire3")&&Input.GetButton("Fire2"))
        {
            ReSpawm(_racer.gameObject);
        }


        float _t = Time.time - _currTime;
        _totalTime = _t;
        string _min = ((int)_t / 60).ToString();
        string _sec = (_t % 60).ToString("f0");
        string _miliSec = ((_t % 1)*100).ToString("f0");
        //_currLapT.text ="TOTAL " + _min + " : " + _sec + " : " + _miliSec;
        _currLap ="TOTAL " + _min + " : " + _sec + " : " + _miliSec;
    }

    public void IncLap()
    {
        _lapCountLocal++;
        SetLap(_lapCountLocal);
    }

    public void DecLap()
    {
        if (_lapCountLocal < 1)
        {
            _lapCountLocal--;
            SetLap(_lapCountLocal);
        }
    }

    public void SetLap(int _lapcount)
    {
        //_lapT.text = "LAP " + _lapcount;
        _lap = "LAP " + _lapcount;
    }

    private void CheckForRaceFinished(int _lapCountLocal)
    {
        Debug.LogWarning(" ## Total Lap ## " + _totalLaps + " Local lap " + _lapCountLocal);

        if (_lapCountLocal == _totalLaps)
        {
            _recentPlayer.GetComponent<MMLapStats>()._rank = ++_rankCount;
            _recentPlayer.GetComponent<MMLapStats>().RaceFinished();
            _recentPlayer.GetComponent<RCC_CarControllerV3>().canControl = false;
            _recentPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            StartCoroutine(TogglePlayer(_recentPlayer));
            Debug.LogWarning("## Calling RaceFinished ## " + _totalLaps);
        }
    }

    IEnumerator TogglePlayer(GameObject _recentPlayerRef)
    {
        yield return new WaitForSeconds(0.01f);
        _recentPlayerRef.SetActive(false);
    }

    public void SetPrevLap()
    {
        if(_isFirstLap == false)
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
        //_lastLapT.text = "Last " + _min + " : " + _sec + " : " + _miliSec;
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
        //_bestLapT.text = "Best " + _min + " : " + _sec + " : " + _miliSec;
        _bestLap = "Best " + _min + " : " + _sec + " : " + _miliSec;
    }

    public void ReSpawm(GameObject _object)
    {
        if (_object.CompareTag("Enemy"))
        {
            _object.SetActive(false);
            _object.GetComponent<MMLapStats>().ResetLapCounter();
            _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _object.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            _object.transform.position = _pos;
            _object.SetActive(true);
        }
        else{

        }
        _object.SetActive(false);
        _object.GetComponent<MMLapStats>().ResetLapCounter();
        Debug.Log("Called Respawn");
        Debug.Log("Prev pos " + _object.transform.position);
        Debug.Log("Prev rotation " + _object.transform.rotation);
        _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _object.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        _object.transform.position = _pos;
        StartCoroutine(EnablePlayer(_object));
        Debug.LogWarning("Look at set");
        Debug.Log("Prev pos " + _object.transform.position);
        Debug.Log("Prev rotation " + _object.transform.rotation);
        RCC_Skidmarks._instance.ClearMesh();
    }

    IEnumerator EnablePlayer(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        _gameObject.transform.LookAt(_racerLookAt);
        _gameObject.SetActive(true);
    }

    public void ToggleText(int _suc)
    {
        if (_suc == 0)
        {
            _startText.SetActive(true);
            _finishText.SetActive(false);
        }
        else
        {
            _startText.SetActive(false);
            _finishText.SetActive(true);
        }
    }

    public void RaceFinished()
    {
        _LeaderBoardInstance.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RCC_CarControllerV3>())
        {
            _recentPlayer = other.gameObject;
            CheckForRaceFinished(_lapCountLocal);
        }
    }

}
