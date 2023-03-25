using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public enum Status
{
    Locked,
    Unclocked
}
[Serializable]
public class Car
{
    public GameObject _car;
    public string Name;
    public float _acceleration;
    public float _topSpeed;
    public float _handling;
    public float _nitro;
    public float _price;
    public Button _button;
    public Sprite _carSelected;
    public Sprite _carNormal;
    public Status _status;
}

public class MMPlayerSelectionManager : MonoBehaviour
{
    public Car[] _cars;

    [Header("Car properties variable")]
    [SerializeField]
    private TextMeshProUGUI _name, _acceleration, _topSpeed, _handling, _nitro, _price;

    [Header("Car Spawn point variable")]
    [SerializeField]
    private Transform _spawnPoint;

    //var for storing instantiated car
    private GameObject _carContainer;
    private int _index = 0;

    [Header("Effects")]
    [SerializeField]
    public GameObject _spawnEffect;

    private bool _isInitiated = false;
    private void Start()
    {
        foreach (Car car in _cars)
        {
            if(car._status == Status.Locked)
            {
                car._button.GetComponent<Onhover>().Locked.gameObject.SetActive(true);
            }
        }
        OnNextClick();
    }
    public void OnNextClick()
    {
        DeactivateAllButton();
        if (_index < 0)
        {
            _index = _cars.Length - 1;
            Debug.Log("Catching Exception " + _index + " index" + _cars.Length);
        }
        else if (_index > _cars.Length-1)
        {
            _index = 0;
            Debug.Log("Catching Exception " + _index);
        }

        ZoomCuurentButton(_index);
        Destroy(_carContainer != null ? _carContainer : null);
        StartCoroutine(InitiateSpawnEffect());
        _carContainer = Instantiate(_cars[_index]._car, _spawnPoint.position,_spawnPoint.rotation,_spawnPoint);
        _carContainer.AddComponent<ObjectMover>();
        _carContainer.gameObject.GetComponent<ObjectMover>().objectToMove = _carContainer;
        LeanTween.rotate(_carContainer, new Vector3(0, 800, 0), 5f);
        LeanTween.scale(_carContainer, new Vector3(7.5f, 7.5f, 7.5f),5f);
        UpdateCarProperties(true);
    }

    IEnumerator InitiateSpawnEffect()
    {
        _spawnEffect.SetActive(true);
        yield return new WaitForSeconds(3f);
        _spawnEffect.SetActive(false);
    }

    private void UpdateCarProperties(bool val, bool _ignore = false)
    {
        //true for increment and false for decrement
        _name.text = _cars[_index].Name.ToString();
        _acceleration.text = _cars[_index]._acceleration.ToString();
        _topSpeed.text = _cars[_index]._topSpeed.ToString();
        _handling.text = _cars[_index]._handling.ToString();
        _nitro.text = _cars[_index]._nitro.ToString();
        _price.text = _cars[_index]._price.ToString();
        //LeanTween.scale(_cars[_index]._button.gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.2f).setEaseInElastic();
        Debug.Log("Updating Index " + _index + " val " + val);
        if (!_ignore)
        {
            Debug.LogWarning("Cant Ignore");
            if (val)
            {
                _index++;
            }
            else
            {
                _index--;
            }
        }
        else
        {
            Debug.LogWarning("Ignore True ");
        }
        Debug.Log("Updated Index " + _index + " val " + val);
    }

    public void Activate(int index)
    {
        Destroy(_carContainer != null ? _carContainer : null);
        int i = 0;
        foreach (Car car in _cars)
        {
            if (i++ == index)
            {
                car._button.image.sprite = car._carSelected;
                StartCoroutine(InitiateSpawnEffect());
                _carContainer = Instantiate(_cars[index]._car, _spawnPoint.position, _spawnPoint.rotation, _spawnPoint);
                _carContainer.AddComponent<ObjectMover>();
                _carContainer.gameObject.GetComponent<ObjectMover>().objectToMove = _carContainer;
                LeanTween.rotate(_carContainer, new Vector3(0, 800, 0), 5f);
                LeanTween.scale(_carContainer, new Vector3(7.5f, 7.5f, 7.5f), 5f);
                UpdateCarProperties(false, true);
               
            }
            else
            {
                car._button.image.sprite = car._carNormal;
            }
        }
        _index = index;
        Debug.Log("Updated Index " + index);
    }

    public  void OnPreviousClick() {
        DeactivateAllButton();
        if (_index < 0)
        {
            _index = _cars.Length-1;
            Debug.Log("Catching Exception "+ _index + " index"+_cars.Length);
        }
        else if(_index > _cars.Length - 1)
        {
            _index = 0;
            Debug.Log("Catching Exception " + _index);
        }
        ZoomCuurentButton(_index);
        Destroy(_carContainer != null ? _carContainer : null);
        StartCoroutine(InitiateSpawnEffect());
        _carContainer = Instantiate(_cars[_index]._car, _spawnPoint.position, _spawnPoint.rotation, _spawnPoint);
        _carContainer.AddComponent<ObjectMover>();
        _carContainer.gameObject.GetComponent<ObjectMover>().objectToMove = _carContainer;

        LeanTween.rotate(_carContainer, new Vector3(0, 800, 0), 5f);
        LeanTween.scale(_carContainer, new Vector3(7.5f, 7.5f, 7.5f), 5f);
        UpdateCarProperties(false);
    }

    private void DeactivateAllButton()
    {
        foreach (Car car in _cars)
        {
            car._button.image.sprite = car._carNormal;
        }
    }
    private void ZoomCuurentButton(int index)
    {
        int i = 0;
       foreach(Car car in _cars)
        {
            if(i++ == index)
            {
                car._button.image.sprite = car._carSelected;
            }
            else
            {
                car._button.image.sprite = car._carNormal;
            }
        }
    }

}
