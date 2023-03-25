using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerUpsHandler;

public class VehicleNitro : MonoBehaviour
{
    public static VehicleNitro instance;
    public enum Mode
    {
        Acceleration,
        Impulse
    };
    public AudioSource _nitroSound;
    public Mode mode = Mode.Acceleration;
    public float forceValue = 100f;
    public float maxVelocity = 6000f;
    public float forceValueforAI = 100f;
    public float maxVelocityforAI = 1000f;
    bool isNitroPlaying = false;
    public bool _isAi = false;
    public bool _heldDown = false;
    public ParticleSystem[] AfterBurnerEffects;

    public float minFOV;
    public float maxFOV;

    public bool NOSBool;

    Rigidbody m_rigidBody;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Noss();
        }
    }
    private void Update()
    {
        if(_isAi)
        {
            StartCoroutine(NitroForAI());
        }
        Debug.Log("NOS "+ NOSBool);
        MangageHeldDown();
    }
    public void SetNOS(bool val)
    {
        NOSBool = val;
        Debug.Log("Setnos");
    }
    IEnumerator NitroForAI()
    {
        int i = UnityEngine.Random.Range(0, 3);
        int k = UnityEngine.Random.Range(2, 8);
        if(i == 1)
        {
            NOSBool = true;
            yield return new WaitForSeconds(k);
            NOSBool  = false;
        }

    }
    private void MangageHeldDown()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _heldDown = true;
            NOSBool = _heldDown;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _heldDown = false;
            NOSBool = _heldDown;    
        }
    }
    private void Noss()
    {

        if (NOSBool)
        {
            if (m_rigidBody.velocity.magnitude < maxVelocity)
            {
                m_rigidBody.AddRelativeForce(Vector3.forward * forceValue, mode == Mode.Acceleration ? ForceMode.Acceleration : ForceMode.Impulse);
                Debug.Log("Nitro");
                if (!AfterBurnerEffects[0].gameObject.activeInHierarchy)
                {
/*                    object[] _data = new object[] { true, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpsHandler.PowerUpType.Boost };
                    PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);*/
                    foreach (ParticleSystem ps in AfterBurnerEffects)
                        ps.gameObject.SetActive(true);
                }
            }
            else
                NOSBool = false;
        }
        else
        {
            if (AfterBurnerEffects[0].gameObject.activeInHierarchy)
            {
/*                object[] _data = new object[] { false, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpsHandler.PowerUpType.Boost };
                PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);*/

                foreach (ParticleSystem ps in AfterBurnerEffects)
                    ps.gameObject.SetActive(false);
            }
        }
    }

    public void AI_ActivateNitro()
    {
        Debug.Log("Activating AI Nitro "+ this.gameObject);
        ActiveNosEffect(true);
        ActivateNitro(5f, forceValueforAI, maxVelocityforAI);
    }
    public void ActiveNosEffect(bool active)
    {
        Debug.Log("BreakZone detected Activating effects "+ this.gameObject);
        foreach (ParticleSystem ps in AfterBurnerEffects)
            ps.gameObject.SetActive(active);
    }

    public void ActivateNitro(float duration, float extraForceAmount, float maxVelocityToMaintain)
    {
        NOSBool = true;
        CancelInvoke("DisableNitro");
        Invoke(nameof(DisableNitro), duration);
        if (isNitroPlaying == false && !_nitroSound)
        {
            _nitroSound.Play();
            isNitroPlaying = true;
        }
    }

    public void DisableNitro()
    {
        NOSBool = false;
        ActiveNosEffect(false);
        if (isNitroPlaying == true)
        {
            _nitroSound.Stop();
            isNitroPlaying = false;
        }
        Debug.Log("##Disabling nitro## "+ NOSBool);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("NitroZone") && _isAi)
        {
            InitiateBoostforAI();
        }
    }
    public void InitiateBoostforAI()
    {
        StartCoroutine(BoostForAI());
    }
    IEnumerator BoostForAI()
    {
        yield return new WaitForSeconds(0.1f);
        AI_ActivateNitro();
        /* NOSBool = true;
        ActiveNosEffect(true);
        //AI_ActivateNitro();
        yield return new WaitForSeconds(3f);
        //DisableNitro();
        NOSBool = false;
        ActiveNosEffect(false);*/
    }
}