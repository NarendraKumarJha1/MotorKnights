using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] public GameObject _targetCube;
    [SerializeField] public GameObject _explodeVFX;
    [SerializeField] public bool _isAI = true;
    private Message _message;
    public int _botIndex;
    private void Start()
    {
        this.gameObject.tag = "Enemy";
        _targetCube.SetActive(false);
        _message = FindFirstObjectByType<Message>();
    }

    private void OnApplicationQuit()
    {
        Debug.LogWarning("Assigning EnemyTag");
        this.gameObject.tag = "Enemy";
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarShoot>())
        {
            Debug.Log("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);
            Debug.LogWarning("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);
            Debug.LogError("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);

            StartCoroutine(Popup(other.gameObject));

            //Check if spell is teleportation spell
            switch (other.gameObject.GetComponent<CarShoot>()._spellType)
            {
                case SpellType.Tp:
                    Debug.LogWarning("##Teleport Init##");
                    StartCoroutine(TeleportPlayer(other.gameObject));
                    break;
                case SpellType.Freeze:
                    Debug.LogWarning("##Freeze## Init");
                    StartCoroutine(Freeze(other.gameObject));
                    break;
                case SpellType.Invisible:
                    Debug.LogWarning("##Invisible Init##");
                    StartCoroutine(Invisible(other.gameObject));
                    break;
                case SpellType.Inverse:
                    Debug.LogWarning("##Inverse Init##");
                    StartCoroutine(Inverse(other.gameObject));
                    break;
            }
        }
    }

    IEnumerator Inverse(GameObject gameObject)
    {
        //Under Spell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        if (_isAI)
        {
            GetComponent<RCC_CarControllerV3>().isAi = true;
            GetComponent<RCC_CarControllerV3>().Uncontrollable();
        }else if(!_isAI)
        {
            GetComponent<RCC_CarControllerV3>().shouldInverse = true;
        }
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        yield return new WaitForSeconds(5f);
        //Spell released
        GetComponent<RCC_CarControllerV3>().shouldInverse = false;
        GetComponent<RCC_CarControllerV3>().isAi = true;
    }

    IEnumerator TeleportPlayer(GameObject gameObject)
    {
        //Under Spell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        Transform _player = gameObject.GetComponent<CarShoot>()._playerRef.transform; //player transform
        Transform _bot = transform; //Bot Transform
        
        Vector3 _posTo = _bot.position; //Bot position or Player Target postion
        Vector3 _posFrom = _player.position; //Player position or bot Target position

        Quaternion _rotTo = _bot.rotation; //Bot Rotation or Player Target rotation
        Quaternion _rotFrom = _player.rotation; //Player rotation or bot target rotation

        yield return new WaitForSeconds(0.01f); 
        //Spell Released
        //Changing player postion with the target bot
        _player.position = _posTo;
        _player.rotation = _rotTo;
        _bot.position = _posFrom;
        _bot.rotation = _rotFrom;
    }

    IEnumerator Freeze(GameObject gameObject)
    {
        //Under Spell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        Rigidbody _bot = this.gameObject.GetComponent<Rigidbody>();
        this.gameObject.GetComponent<RCC_CarControllerV3>().canControl = false;
        _bot.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        yield return new WaitForSeconds(3f);
        //Spell Released
        this.gameObject.GetComponent<RCC_CarControllerV3>().canControl = true;
        _bot.constraints = RigidbodyConstraints.None;
        Debug.Log("Freeze called");
    }

    IEnumerator Invisible(GameObject gameObject)
    {
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        yield return new WaitForSeconds(8f);
    }

    IEnumerator Popup(GameObject _other)
    {
        _message.gameObject.SetActive(true);
        _message._message.text = "Hit by Spell " + _other.gameObject;
        yield return new WaitForSeconds(3f);
        _message.gameObject.SetActive(false);
    }

/*    IEnumerator StartExplodeVFX(GameObject _obj)
    {
        _obj.GetComponent<BotManager>()._explodeVFX.SetActive(true);
        yield return new WaitForSeconds(3f);
        _obj.GetComponent<BotManager>()._explodeVFX.SetActive(false);
    }*/
}
