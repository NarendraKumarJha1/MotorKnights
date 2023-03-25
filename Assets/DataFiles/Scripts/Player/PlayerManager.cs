using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    public GameObject _playerCanvas;

    [SerializeField]
    public bool _isPlayer = false;

    private void Start()
    {
        if (!_isPlayer)
        {
            _playerCanvas.SetActive(false);
        }
           
    }

}
