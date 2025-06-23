using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class StartScenes_Control : MonoBehaviour
{
    // Start is called before the first frame update
    public Button _StartBtn;
    public Button _OptionBtn;
    public Button _ExitBtn;


    public GameObject _Main;
    public GameObject _Option;

    public Button _BackBtn;

    public Slider _BGM_Bar;
    public Slider _Effect_Bar;

    public TextMeshProUGUI _BGMAMOUNT;
    public TextMeshProUGUI _EFFECTAMOUNT;

    void Start()
    {
        _StartBtn.onClick.AddListener(StartButton);
        _OptionBtn.onClick.AddListener(OptionButton);
        _ExitBtn.onClick.AddListener(ExitButton);

        _BackBtn.onClick.AddListener(BackButton);

        _BGM_Bar.onValueChanged.AddListener(BGMChange);
        _Effect_Bar.onValueChanged.AddListener(EffectChange);

        SoundManager._instance.PlayBGM();
    }

    void BGMChange(float x)
    {
        SoundManager._instance._Player.volume = x;
        MasterControl._Instance._BGMVolume = x;
        _BGMAMOUNT.text = Math.Floor(x * 100).ToString();
    }
    void EffectChange(float x)
    {
        MasterControl._Instance._EffectVolume = x;
        _EFFECTAMOUNT.text = Math.Floor(x * 100).ToString();
    }

    void BackButton()
    {
        _Option.gameObject.SetActive(false);
        _Main.gameObject.SetActive(true);
    }

    
    void StartButton()
    {
        MasterControl._Instance.SceneChange("GameScene", MasterControl.GameState.start);
    }

    void OptionButton()
    {
        _Main.gameObject.SetActive(false);
        _Option.gameObject.SetActive(true);
    }

    void ExitButton()
    {
        Application.Quit();
    }
}
