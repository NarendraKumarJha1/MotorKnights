using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    #region Freeze spell variables
    [Header("Car Mesh")]
    [SerializeField]
    public MeshRenderer[] childMeshRenderers;


    [Header("Car Freeze Material")]
    [SerializeField]
    public Material _freezeMaterial;

    [Header("Car Original Material")]
    [SerializeField]
    private Material[] _originalMaterial;
    #endregion

    #region Trans spell variables
    [Header("Translucent Material")]
    [SerializeField]
    public Material _transMaterial;
    #endregion

    #region Inverse spell variables
    [Header("Inverse Material")]
    [SerializeField]
    public Material _inverseMaterial;
    #endregion


    CarShoot _car = null;
    [SerializeField]
    public GameObject _target;
    private void Start()
    {
        childMeshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Save the original materials
        _originalMaterial = new Material[childMeshRenderers.Length];
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            _originalMaterial[i] = childMeshRenderers[i].material;
        }

    }

    public void SpellFunc(SpellType _spellType)
    {
        Debug.Log("Spell Function called with spell type In Spell Func " + _spellType);

        switch (_spellType)
        {
            case SpellType.Freeze:
                Debug.Log("Freeze called");
                foreach (MeshRenderer mesh in childMeshRenderers)
                {
                    mesh.material = _freezeMaterial;
                }
                StartCoroutine(RetreiveOriginalMaterials(3f));
               break;
            case SpellType.Inverse:
                Debug.Log("Inverse called");
                foreach (MeshRenderer mesh in childMeshRenderers)
                {
                    mesh.material = _inverseMaterial;
                }
                StartCoroutine(RetreiveOriginalMaterials(5f));
                break;
            case SpellType.Invisible:
                Debug.Log("Invisible called");
                foreach (MeshRenderer mesh in childMeshRenderers)
                {
                    mesh.material = _transMaterial;
                }
                StartCoroutine(InvisibleEffect());
                StartCoroutine(RetreiveOriginalMaterials(8f));
                break;
        }
    }

    IEnumerator RetreiveOriginalMaterials(float _dur)
    {
        yield return new WaitForSeconds(_dur);

        // Replace their materials with the original materials
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            childMeshRenderers[i].material = _originalMaterial[i];
            Debug.Log("Restoring default materials " + _originalMaterial[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //On hit Try to get carshoot script to analyze spell type
        try
        {
            _car = other.gameObject.GetComponent<CarShoot>();
        }catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        if (_car != null)
        {
            if(_car._spellType == SpellType.Invisible){
                SpellFunc(_car._spellType);
            }
        }

    }

    IEnumerator InvisibleEffect()
    {
        Physics.IgnoreLayerCollision(8, 8, true);
        yield return new WaitForSeconds(8f);
        Physics.IgnoreLayerCollision(8, 8, false);
    }
}
