using RGSK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RaceManagerBehaviour : MonoBehaviour
{
    public void SetPointer(GameObject _opponentPointer, GameObject _playerPointer, List<MMLapStats> _list)
    {
        foreach (var item in _list)
        {
            if (item.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Found " + item);
                GameObject _g = Instantiate(_playerPointer);
                _g.GetComponent<RacerPointer>().target = item.gameObject.transform;
            }
            else
            {
                Debug.Log("Enemy Found " + item);
                GameObject _g = Instantiate(_opponentPointer);
                _g.GetComponent<RacerPointer>().target = item.gameObject.transform;
            }

        }
    }
    public void SetListeners(Button _pauseButton, Button _playButton, GameObject _panel)
    {
        _pauseButton.onClick.AddListener(() => Pause(_panel));
        _playButton.onClick.AddListener(() => Play(_panel));
    }

    public void Play(GameObject _pausePanel)
    {
        Debug.Log("Play Pressed");
        Time.timeScale = 1; // Pause game
        _pausePanel.SetActive(false);
    }

    public void Pause(GameObject _pausePanel)
    {
        Debug.Log("Pause Pressed");
        Time.timeScale = 0; // Pause game
        _pausePanel.SetActive(true);
    }
}
