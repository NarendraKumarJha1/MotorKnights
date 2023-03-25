using System;
using UnityEngine;
using HWRWeaponSystem;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class PowerUpsHandler : MonoBehaviour
{
    #region Enum
    public enum PowerUpType { None, Boost, Missile, Conversion }
    public enum CurrentState { Car, Robot }
    #endregion

    #region Local Classes
    [Serializable]
    public class PowersData
    {
        public PowerUpType powerUpType = PowerUpType.Boost;
        public int powerCount = 1000;
    }
    #endregion

    /*    public 
    */
    public static PowerUpsHandler instance;
    public bool isAI = false;
    public bool _heldDown = false;
    public bool _engine = true;
    public CurrentState currentState = CurrentState.Car;
    public List<PowersData> powersDatas = new List<PowersData>();

    public float currentTime = 0;
    public float powerAfterTime = 6;

    public float backToRobotTime = 15;
    public float remainingTime = 0;
    public KeyCode robotConversionKey = KeyCode.C;
    public KeyCode fireKey = KeyCode.F;
    public GameObject speedparticle;
    public bool isSpeedParticle;

    float robotConvertionTime = 0;
    Rigidbody rigidbody;
    public ProgressBar PbC;
    PhotonView photonView;

    private void OnEnable()
    {
        currentTime = Time.time;
/*        for (int i = 0; i < weaponLaunchers.Length; i++)
        {
            //weaponLaunchers[i].gameObject.SetActive(true);
            *//*weaponLaunchers[i].OnActive = HasPower(PowerUpType.Missile);*//*
        }

        photonView = GetComponent<PhotonView>();*/
    }

    private void Awake()
    {

        _engine = true;
        if (!isAI)
            SetUi();

        if (!rigidbody)
            rigidbody = this.GetComponent<Rigidbody>();
        if (instance == null)
        {
            instance = this;
        }

    }

    void SetUi()
    {
        if (!isAI)
            if (GameController.instance)
            {
                GameController.instance.SetMissileUi(GetPowerCount(PowerUpType.Missile), currentState == CurrentState.Car);
                GameController.instance.SetConversionSprite(currentState == CurrentState.Car, GetPowerCount(PowerUpType.Conversion));
            }
    }

    public void KillEngine()
    {
        _engine = false;
    }

    public void StartEngine()
    {
        _engine = true;
    }
    private void Start()
    {
        SetupBoostBar();
    }

    private void SetupBoostBar()
    {
        if (!isAI)
        {
            PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);
            PbC.BarValue = _pd.powerCount;
        }
    }

    private void Update()
    {
        CheckForNitro();
        IncreaseNitro();
        if (!isAI)
        {
            if (Constants.IsWindowEditor())
            {
                if (HasPower(PowerUpType.Conversion) && Input.GetKeyDown(robotConversionKey) && currentState == CurrentState.Car)
                    Convert(true);
                if (Input.GetKeyDown(fireKey))
                {
                    //FireMissile();
                    photonView.RPC("FireMissile", RpcTarget.All, transform.name);
                }
            }

            if (currentState == CurrentState.Robot)
            {
                if (convertingBack)
                    return;
                if (backToRobotTime - remainingTime <= 0)
                {
                    Convert(false);
                    convertingBack = true;
                    Debug.Log("Here");
                }
                remainingTime = (Time.time - robotConvertionTime) + Time.deltaTime;
                if (GameController.instance)
                    GameController.instance.uiController.BackToCarFiller(remainingTime, backToRobotTime);
            }
        }

        CheckSpeedForparticle();
        PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);

        //Debug.Log("Powercount" + _pd.powerCount);
    }

    private void IncreaseNitro()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);

            if (_pd.powerCount < 1000)
            {
                StartCoroutine(IncreasePowerOfBoost());
            }
        }
    }
    IEnumerator IncreasePowerOfBoost()
    {
        PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);
        while (_pd.powerCount < 1000)
        {
            //Debug.Log("Increasing nitro");
            yield return new WaitForSeconds(0.01f);
            _pd.powerCount = _pd.powerCount + 1;
            PbC.BarValue = _pd.powerCount;
        }


    }
    private void CheckForNitro()
    {
        //Debug.Log("Engine status "+ _engine);
        if (Input.GetKey(KeyCode.LeftShift) && _engine == true && !isAI)
        {
            //Debug.Log("Boost");
            Boost();
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            Debug.Log("Disabling Nitro!");
            VehicleNitro.instance.DisableNitro();
        }

        MangageHeldDown();
    }

    private void MangageHeldDown()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _heldDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _heldDown = false;
        }
    }

    void CheckSpeedForparticle()
    {
        if (isSpeedParticle)
            speedparticle.SetActive(this.GetComponent<RCC_CarControllerV3>().speed > 180 && currentState != CurrentState.Robot);
    }

    bool convertingBack = false;
    int val = 0;

    public VehicleNitro aiNitro = null;
    public float nitorDuration = 5, extraForceAmount = 1000;
    void Boost()
    {
        if (HasPower(PowerUpType.Boost))
        {
            DecrementPower(PowerUpType.Boost);
            aiNitro.ActivateNitro(nitorDuration, extraForceAmount, 5000);
        }
        else
        {
            VehicleNitro.instance.DisableNitro();
        }
    }
    public WeaponLauncher[] weaponLaunchers = null;

    [PunRPC]
    public void FireMissile(string carname)
    {
        if (HasPower(PowerUpType.Missile))
        {
            DecrementPower(PowerUpType.Missile);

            Debug.LogError("Missile used here");
            object[] _data = new object[] { true, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpType.Missile };
            PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);

            for (int i = 0; i < weaponLaunchers.Length; i++)
            {
                weaponLaunchers[i].Owner = gameObject;
                weaponLaunchers[i].Shoot(carname);
                weaponLaunchers[i].OnActive = HasPower(PowerUpType.Missile);
            }
        }

        //freemode
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.LogError("Missile used here");
            object[] _data = new object[] { true, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpType.Missile };
            //PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);

            for (int i = 0; i < weaponLaunchers.Length; i++)
            {
                weaponLaunchers[i].Shoot(transform.name);
                weaponLaunchers[i].OnActive = HasPower(PowerUpType.Missile);
            }
        }


        SetUi();
    }

    public void MissileUsed()
    {
        for (int i = 0; i < weaponLaunchers.Length; i++)
        {
            weaponLaunchers[i].Shoot(transform.name);
            weaponLaunchers[i].OnActive = HasPower(PowerUpType.Missile);
        }
    }

    public void Convert(bool active)
    {
        if (HasPower(PowerUpType.Conversion) && currentState == CurrentState.Robot)
            DecrementPower(PowerUpType.Conversion);

        object[] _data = new object[] { active, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpType.Conversion };
        PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);

        StartConversion(Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public MeshRenderer[] carMainMeshes = null;
    public Collider mainCollider = null;
    public GameObject robotMainMesh = null;
    public float animationTime = 2.0f;
    public Animator[] conversionAnimators = null;

    Coroutine conversionCoroutine = null;
    public void StartConversion(int acNo)
    {
        conversionCoroutine = StartCoroutine(StartConversionCoroutine(acNo));
    }

    IEnumerator StartConversionCoroutine(int acno)
    {
        //StopCar(true);
        for (int i = 0; i < carMainMeshes.Length; i++)
            carMainMeshes[i].enabled = false;
        robotMainMesh.SetActive(false);
        for (int i = 0; i < conversionAnimators.Length; i++)
        {
            conversionAnimators[i].gameObject.SetActive(true);
            conversionAnimators[i].SetTrigger(currentState == CurrentState.Car ? "ToRobot" : "ToCar");
        }
        yield return new WaitForSeconds(animationTime);
        for (int i = 0; i < conversionAnimators.Length; i++)
            conversionAnimators[i].gameObject.SetActive(false);
        robotConvertionTime = Time.time;
        currentState = (currentState == CurrentState.Car) ? CurrentState.Robot : CurrentState.Car;
        convertingBack = false;
        remainingTime = 0;
        for (int i = 0; i < carMainMeshes.Length; i++)
            carMainMeshes[i].enabled = currentState == CurrentState.Car;
        robotMainMesh.SetActive(currentState == CurrentState.Robot);

        if (acno == Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber)
            SetUi();

        yield return new WaitForSeconds(0.1f);

        if (conversionCoroutine != null)
            StopCoroutine(conversionCoroutine);
    }

    bool HasPower(PowerUpType power)
    {

        PowersData _pd = powersDatas.Find(x => x.powerUpType == power);
        //Debug.Log("Powercount" + _pd.powerCount);
        if (_pd != null)
            return _pd.powerCount > 0;

        return false;
        /*        return true;*/
    }

    void DecrementPower(PowerUpType powerUpType)
    {
        PowersData _pd = powersDatas.Find(x => x.powerUpType == powerUpType);

        if (_pd != null)
        {
            _pd.powerCount = _pd.powerCount - 3;
            if (_pd.powerCount < 0)
                _pd.powerCount = 0;

            PbC.BarValue = _pd.powerCount;
        }
    }

    int GetPowerCount(PowerUpType powerUpType)
    {
        PowersData _pd = powersDatas.Find(x => x.powerUpType == powerUpType);

        if (_pd != null)
            return _pd.powerCount;

        return 0;
    }

    public GameObject explosionEffect = null;
    public void DestroyCar(string name)
    {
        explosionEffect.SetActive(true);
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        mainCollider.isTrigger = true;
        //Debug.LogError("car destroyed");
        destroyCoroutine = StartCoroutine(StartAgain(name));
    }

    [SerializeField] float startAgainTime = 5;
    Coroutine destroyCoroutine;
    IEnumerator StartAgain(string name)
    {
        if (PhotonNetwork.LocalPlayer.NickName == name)
        {
            RGSK.RaceUI.instance.VehicleDestroyedMessage.SetActive(true);
        }
        yield return new WaitForSeconds(5);

        explosionEffect.SetActive(false);
        //  Debug.LogError("stopping the damage");
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;

        mainCollider.isTrigger = false;
        if (GetComponent<DamageReceiver>())
            GetComponent<DamageReceiver>().health = 100;

        if (PhotonNetwork.LocalPlayer.NickName == name)
        {
            RGSK.RaceUI.instance.VehicleDestroyedMessage.SetActive(false);
        }
        if (destroyCoroutine != null)
            StopCoroutine(destroyCoroutine);
        destroyCoroutine = null;
    }

    void StopCar(bool isStop)
    {
        rigidbody.isKinematic = isStop;
    }

    public GameObject particleEffectParent = null;
    public void ShowParticleEffect()
    {
        if (!isAI)
        {
            particleEffectParent.SetActive(true);
            disableCoroutine = StartCoroutine(DisableEffect());
        }
    }
    Coroutine disableCoroutine;
    IEnumerator DisableEffect()
    {
        yield return new WaitForSeconds(3.0f);

        particleEffectParent.SetActive(false);
        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine);
        disableCoroutine = null;
    }

    public void AddPowerUp(PowerUpType pickUpType)
    {
        AddPower(pickUpType);
        SetUi();
    }
    void AddPower(PowerUpType pickUpType)
    {
        for (int i = 0; i < powersDatas.Count; i++)
        {
            if (pickUpType == powersDatas[i].powerUpType)
            {
                powersDatas[i].powerCount++;
                break;
            }
        }
    }
    public bool IsCar()
    {
        return currentState == CurrentState.Car;
    }
}