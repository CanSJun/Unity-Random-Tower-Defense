using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MasterControl : MonoBehaviour
{

    static MasterControl _unique;
    public GameObject _obj;
    public Slider _ProgressBar;
    public TextMeshProUGUI _Tip;
    public string[] _text;

    public float _BGMVolume = 1.0f;
    public float _EffectVolume = 1.0f;

    public static MasterControl _Instance
    {
        get { return _unique; }



    }
    public enum GameState
    {
        none = 0,
        start,
        loading,
        end
    }

    AsyncOperation _loadProc;

    GameState _nowState = GameState.none;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _unique = this;
        SceneChange("StartScene", GameState.start);
    }

    public void SceneChange(string ScenName, GameState type) {
        _obj.gameObject.SetActive(true);
        _ProgressBar.value = 0;
        _nowState = type;
        _Tip.text = _text[Random.Range(0, _text.Length)];
        _loadProc = SceneManager.LoadSceneAsync(ScenName);
    }


    void Update()
    {
        if(_loadProc != null)
        {
            _ProgressBar.value = _loadProc.progress;
            if (!_loadProc.isDone)
            {
                _nowState = GameState.loading;
            }
            else
            {
                _nowState = GameState.end;
                _loadProc = null;
                _obj.gameObject.SetActive(false);
            }

        }
    }
}
