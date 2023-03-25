using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpell : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RCC_CarControllerV3>() && !other.gameObject.CompareTag("Player"))
        {

            StartCoroutine(TractionOff(other.gameObject));
            GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator TractionOff(GameObject _other)
    {
        RCC_CarControllerV3 _car = _other.GetComponent<RCC_CarControllerV3>();
        bool _tempTractionHelper = _car.tractionHelper;
        float _tempTractionHelperStrenth = _car.tractionHelperStrength;
        bool _tempSteerHelper = _car.steeringHelper;
        float _tempSteerHelperLinear = _car.steerHelperLinearVelStrength;
        float _tempSteerHelperAngular = _car.steerHelperAngularVelStrength;
        _car.tractionHelper = true;
        _car.tractionHelperStrength = 0;
        _car.steeringHelper = true;
        _car.steerHelperLinearVelStrength = 0;
        _car.steerHelperAngularVelStrength = 0;
        _car.steerInput = 1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = -1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = 1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = -1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = 1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = -1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = 1;
        yield return new WaitForSeconds(0.5f);
        _car.steerInput = -1;
        yield return new WaitForSeconds(0.5f);
        _car.tractionHelper = _tempTractionHelper;
        _car.tractionHelperStrength = _tempTractionHelperStrenth;
        _car.steeringHelper = _tempSteerHelper;
        _car.steerHelperLinearVelStrength = _tempSteerHelperLinear;
        _car.steerHelperAngularVelStrength = _tempSteerHelperAngular;
        Destroy(this);
    }
}
