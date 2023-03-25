using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagerParent : MonoBehaviour
{
    private GameObject _loadingScreen = null;

    public void SetLoadingRef(GameObject _ref)
    {
        _loadingScreen = _ref;
    }

    public void SetLisForSceneLoad(Button _g, GameObject _mainMenuPanel,GameObject _loading, Image _progressBar,List<AsyncOperation> _scenesToLoad, bool _val, int index)
    {
        _g.onClick.AddListener(() => Onloading(_loading, _mainMenuPanel,_progressBar,_scenesToLoad, _val, index));
    }

    public void Onloading(GameObject _load, GameObject _mainMenuPanel,Image _progressBar,List<AsyncOperation> _scenesToLoad, bool _val, int index)
    {   
        StartCoroutine(LoadScene(index, _mainMenuPanel, _progressBar, _load, _val,_scenesToLoad));
    }
    IEnumerator LoadScene(int index, GameObject _mainMenuPanel, Image _loadingProgressBar, GameObject _load, bool _val,List<AsyncOperation> _scenesToLoad)
    {
        _load.SetActive(_val);
        _mainMenuPanel.SetActive(!_val);
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(index));
        float totalProgress = 0;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < _scenesToLoad.Count; ++i)
        {
            while (!_scenesToLoad[i].isDone)
            {
                totalProgress += _scenesToLoad[i].progress;
                _loadingProgressBar.fillAmount = totalProgress / _scenesToLoad.Count;
                yield return null;
            }

        }
    }


    public void InitiateLoad(float _dur)
    {
        StartCoroutine(Load(_dur));
    }

    IEnumerator Load(float _dur)
    {
        _loadingScreen.SetActive(true);
        yield return new WaitForSeconds(_dur);    
        _loadingScreen.SetActive(false);
    }

    public void Toggle(GameObject _obj ,bool val, float _dur = 0)
    {
        StartCoroutine(ToggleObject(_obj, val, _dur));
    }

    IEnumerator ToggleObject(GameObject _obj, bool val, float _dur)
    {
        yield return new WaitForSeconds(_dur);
        _obj.SetActive(val);
    }
}
