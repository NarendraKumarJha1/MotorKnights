using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class RaceManagement : RaceManagerBehaviour
{
    [SerializeField] private GameObject _opponentPointer;
    [SerializeField] private GameObject _playerPointer;
    List<MMLapStats> list = new List<MMLapStats>();

    [Header("Pause")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private GameObject _pausePanel;

    private void Start()
    {
        Debug.Log("Race management Initiated");
        StartCoroutine(SetPointerDelay());
        SetListeners(_pauseButton, _playButton, _pausePanel);
    }

    IEnumerator SetPointerDelay()
    {
        yield return new WaitForSeconds(3f);
        list = FindObjectsOfType<MMLapStats>().ToList();
        SetPointer(_opponentPointer, _playerPointer, list);
    }

    private void Update()
    {
        PlayPause();
    }

    private void PlayPause()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            Pause(_pausePanel);
        }
        else if(Input.GetKeyUp(KeyCode.P))
        {
            Play(_pausePanel);
        }
    }
}
