using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMystic : MonoBehaviour
{
    public static GameManagerMystic _instance; 

    [Header("UI")]
    [SerializeField]
    GameObject _spellDameDescription;
    [SerializeField]
    GameObject _shortcutsPanel;
    [SerializeField] 
    GameObject _InGameDebugger;
    [SerializeField] 
    TextMeshProUGUI _prompt;
    [SerializeField]
    Button _back;

    bool _ingameDebugToggle = false;
    public bool _enable = true;
    public bool _debug = false;

    [Header("Cars")]
    [SerializeField]
    GameObject[] _botCars;
    [SerializeField]
    List<GameObject> _botCarsList;
    [SerializeField]
    GameObject[] _spawnPoints;

    [Header("Panel")]
    [SerializeField]
    GameObject _loadingPanel;
    [SerializeField]

    public string modelPath; // Path to the model file on disk
    public GameObject targetObject; // Object to assign the loaded model to

    void Start()
    {
        _instance = this;
        _InGameDebugger.SetActive(false);
        StartCoroutine(InstantiateCars());
        StartCoroutine(StartInitialScreen());
    }

    IEnumerator InstantiateCars()
    {
        int i = _botCars.Length;
        yield return new WaitForSeconds(2f);
        for(int index = 0; index<i;index++ )
        {
            GameObject _g =  Instantiate(_botCars[index], _spawnPoints[index].transform.position, _spawnPoints[index].transform.rotation);
            
            _botCarsList.Add(_g);
            try
            {
            _g.GetComponent<BotManager>()._botIndex = index;

            }catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            _g.GetComponent<RCC_CarControllerV3>().canControl = false;
        }
    }

    IEnumerator StartInitialScreen()
    {
        _loadingPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitForSeconds(2f);
        foreach (GameObject _rc in _botCarsList)
        {
            Debug.Log("Turning on car control");

            _rc.GetComponent<RCC_CarControllerV3>().canControl = !_debug;
        }
        _loadingPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.F1))
        {
            _enable = !_enable;
            _prompt.gameObject.SetActive(true);
            _prompt.text = _enable == true ? "Devloper mode Activated" : "Devloper mode Deactivated";
            StartCoroutine(DisablePromt());
        }

        if (_enable)
            CheckForInGameDebuggerToggle();

        StartCoroutine(CheckForDesPanel());
        StartCoroutine(CheckForShortCutsPanel());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(LoadHomeScreen());
        }
        _back.onClick.AddListener(InitiateLoadHomeScreen);
    }

    public void InitiateLoadHomeScreen()
    {
        StartCoroutine(LoadHomeScreen());
    }

    IEnumerator LoadHomeScreen()
    {
        _loadingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        Back();
    }

    IEnumerator CheckForShortCutsPanel()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _shortcutsPanel.SetActive(true);
            LeanTween.moveLocalY(_shortcutsPanel.gameObject, -400f, 0.8f).setEaseOutBack();
        }
        else if (Input.GetKeyUp(KeyCode.F1))
        {
            LeanTween.moveLocalY(_shortcutsPanel.gameObject, -900f, 0.5f).setEaseInSine();
            yield return new WaitForSeconds(0.5f);
            _shortcutsPanel.SetActive(false);
        }
    }
    IEnumerator CheckForDesPanel()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            _spellDameDescription.SetActive(true);
            LeanTween.scale(_spellDameDescription.gameObject,new Vector3(1, 1, 1), 1f).setEaseOutBack();
        }else if (Input.GetKeyUp(KeyCode.F5))
        {
            LeanTween.scale(_spellDameDescription.gameObject, new Vector3(0, 0, 0), 0.5f).setEaseInSine();
            yield return new WaitForSeconds(0.5f);
            _spellDameDescription.SetActive(false);
        }
    }
    public void Restart()
    {
        _spellDameDescription.transform.localScale = Vector3.zero;
    }
    IEnumerator DisablePromt()
    {
        yield return new WaitForSeconds(1f);
        _prompt.gameObject.SetActive(false);
    }

    private void CheckForInGameDebuggerToggle()
    {
        if(Input.GetKeyDown(KeyCode.K) && Input.GetKeyDown(KeyCode.J) && Input.GetKeyDown(KeyCode.H))
        {
            _ingameDebugToggle = !_ingameDebugToggle;
            _InGameDebugger.SetActive(_ingameDebugToggle);
        }
    }


/*    IEnumerator LoadModelAsync()
    {
        var requests = new List<AsyncOperation>();
        foreach (GameObject model in _botCars)
        {
            var request = Resources.LoadAsync<GameObject>(model.name);
            requests.Add(request);
        }

        // Wait for all models to be loaded
        foreach (var request in requests)
        {
            yield return request;
        }

        // Once all models are loaded, instantiate them at the spawn locations
        for (int i = 0; i < _botCars.Length; i++)
        {
            // Get the next spawn location
            GameObject spawnLocation = _spawnPoints[i % _spawnPoints.Length];

            // Instantiate the model at the spawn location
            GameObject loadedModel = Instantiate(requests[i % requests.Count].result, spawnLocation.transform.position, Quaternion.identity);

            // Parent the loaded model to the target object
            loadedModel.transform.SetParent(targetObject.transform, false);
        }
    }*/

    public void Back()
    {
        SceneManager.LoadScene(1);
    }
}
