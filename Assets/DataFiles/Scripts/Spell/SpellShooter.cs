using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System;

public class SpellShooter : MonoBehaviour
{
    Vector3 origin;
    Vector3 direction = Vector3.forward; // Set direction vector
    Message _message;
    public Transform _targetObject = null;

    RaycastHit _tempHit;
    private bool _hasPrevHit = false;
    private bool _isAnimating = false;
    private int _tempBotIndex = -1;
    private void Start()
    {
        origin = this.transform.position;
        // Create a new line renderer component
        _message = FindFirstObjectByType<Message>();
        _message.gameObject.SetActive(false);
    }

    void Update()
    {
        origin = this.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.TransformDirection(direction), out hit, 40f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
            Debug.LogWarning("Target Object " + hit.transform.gameObject.name);
            if (hit.collider.gameObject.GetComponent<BotManager>() || hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.LogWarning("Turning Cube on");
                //StartCoroutine(Popup(hit.collider.gameObject));// Showing message that target is locked
                StartCoroutine(ShowTargetCube(hit.collider.gameObject));
                _targetObject = hit.transform;
            }
            // Draw a line to visualize the raycast
        }
        else
        {
            // Disable the line renderer if the raycast doesn't hit anything
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 20f, Color.green);
        }

        if(_targetObject != null)
        {
            PopUpMessage(_targetObject.gameObject);
        }
        else
        {
            _message.gameObject.SetActive(false);
        }
    }

    public void PopUpMessage(GameObject _other)
    {
        _message.gameObject.SetActive(true);
        string var = _other.gameObject.name;
        var = var.Replace("(Clone)", "");
        _message._message.text = "Target Locked " + var;
       
    }
/*
    IEnumerator Popup(GameObject _other)
    {
        _message.gameObject.SetActive(true);
        string var = _other.gameObject.name;
        var = var.Replace("(Clone)", "");
        _message._message.text = "Target Locked " + var;
        yield return new WaitForSeconds(1);
        _message.gameObject.SetActive(false);
    }*/

    IEnumerator ShowTargetCube(GameObject _gO)
    {
        _isAnimating = true;
        _gO.GetComponent<BotManager>()._targetCube.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _gO.GetComponent<BotManager>()._targetCube.SetActive(false);
        _isAnimating = false;
    }
}
