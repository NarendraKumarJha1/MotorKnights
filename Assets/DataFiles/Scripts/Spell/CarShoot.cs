using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShoot : MonoBehaviour
{
    public static CarShoot _instance;
    private Rigidbody _rb;
    private float m_Thrust = 3000;
    private Vector3 _targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    public float timeRemaining = 5;
    public bool timerIsRunning = false;
    public bool _canFire = false;
    SpellShooter _spellShooter = null;
    Transform _targetObject = null;
    [SerializeField]
    public GameObject _playerRef = null;
    [SerializeField]
    public GameObject _OilVFX;
    private SpellType spellType;

    //If target is not loacked
    public bool _isTargetLocked = false;

    public SpellType _spellType
    {
        set
        {
            spellType = value;
            Debug.Log("Assigned Spell "+spellType);
        }
        get
        {
            return spellType;
        }
    }

    private void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        ScaleFireBall();
        _rb = GetComponent<Rigidbody>();
        _playerRef = FindObjectOfType<PlayerManager>().gameObject;
    }

    public void ScaleFireBall()
    {
        LeanTween.scale(this.gameObject, _targetScale, 5f);
    }

    public void Fire()
    {
        _spellShooter = FindObjectOfType<SpellShooter>();
        if (_spellShooter._targetObject!= null)
        {
            _targetObject = _spellShooter._targetObject.GetComponent<SpellEffect>()._target.transform;
            _isTargetLocked = true;
        }
        else
        {
            Debug.LogError(" Error getting targetObject");
            _targetObject = General._instance.GetTarget();

        }

        Debug.Log("Fired");
        Debug.LogWarning("Actual Thrust " + m_Thrust);
        Debug.LogWarning("100X Thrust " + m_Thrust * 100);
        var _force = Mathf.Abs(m_Thrust);
        _force = Convert.ToInt32(_force);
        if (m_Thrust < 50)
        {
            _force = 3500;
        }
        else
        {
            _force = _force * 110;
        }
        Debug.LogWarning(" After Actual Thrust " + m_Thrust + " Force :- " + _force);
        Debug.LogWarning("After 100X Thrust " + m_Thrust * 100);
        if(_spellType == SpellType.Freeze || _spellType == SpellType.Tp || _spellType == SpellType.Inverse)
        {
            LookAtTarget(4000);
        }
        else if(_spellType == SpellType.Invisible || _spellType == SpellType.Oil)
        {
            LookAtSelf(_force);
        }
    }

    private void Update()
    {
        if(_canFire && _isTargetLocked)
        {
            Debug.Log("resetting target "+_targetObject.transform.position + " spell " + this.gameObject);
            if (_targetObject != null)
            {
                this.gameObject.transform.LookAt(_targetObject.transform);
                _rb.AddForce(transform.forward * 4000);
            }
        }
    }

    //For absorbing spell make spell look at the car
    private void LookAtSelf(float _force)
    {
        _force = 700;
        this.gameObject.transform.LookAt(GetComponentInParent<RCC_CarControllerV3>().gameObject.transform);
        _rb.AddForce(transform.forward * _force);
    }

    //For Shooting spell make spell look at the target location in front or look at car locked
    private void LookAtTarget(float _force)
    {
        if (_targetObject != null)
        {
            _canFire = true;
            this.gameObject.transform.LookAt(_targetObject.transform);
            _rb.AddForce(transform.forward * _force);
            Debug.LogWarning("##TargetObject is not equal null");
        }
        else if(_targetObject == null)
        {
            this.gameObject.transform.LookAt(General._instance.GetTarget());
            Debug.LogWarning("##TargetObject is null");
            _rb.AddForce(transform.forward * 4000);
        }
    }

    public void SetThrust(float Val)
    {
        m_Thrust = Mathf.Abs(Val);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }

        else if(_spellType == SpellType.Invisible)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Destroy(this.gameObject);
            }
        }

        if(other.gameObject.CompareTag("Track") && _spellType == SpellType.Oil) {
            if (_OilVFX != null)
            {
                GameObject _oil = Instantiate(_OilVFX, new Vector3(transform.position.x, other.transform.position.y+0.1f, transform.position.z), Quaternion.identity);
            }
        }
    }
}
