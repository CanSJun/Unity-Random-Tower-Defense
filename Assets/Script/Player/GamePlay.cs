using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{

    public enum PlayerState
    {
        None       = 0,
        Building,
        Upgrade,
        Wait,
        END
    }
    
    public PlayerState _CurrentState = PlayerState.None;
    public PlayerState State { get { return _CurrentState; } }

    public int _BuildingCost = 30;





    public Button _ReplayBtn;
    public Button _OptionBtn;
    public Button _ExitBtn;
    public Button _ExitBtn_sub;
    public GameObject _Option;
    public GameObject _Option_Sub;
    public GameObject _Setting;
    public Slider _BGM_Bar;
    public Slider _Effect_Bar;

    public TextMeshProUGUI _BGMAMOUNT;
    public TextMeshProUGUI _EFFECTAMOUNT;

    [SerializeField] GameObject _Text;
    static GamePlay _unique;
    public static GamePlay _instance
    {
        get { return _unique; }
    }


    void Awake()
    {
        _unique = this;

    }


    void Start()
    {
        GameReady();

        _ExitBtn.onClick.AddListener(ExitButton);
        _OptionBtn.onClick.AddListener(OptionButton);
        _ReplayBtn.onClick.AddListener(RePlayButton);
        _ExitBtn_sub.onClick.AddListener(ExitButtonSub);

        _BGM_Bar.onValueChanged.AddListener(BGMChange);
        _Effect_Bar.onValueChanged.AddListener(EffectChange);


        _BGM_Bar.value = MasterControl._Instance._BGMVolume;
        _Effect_Bar.value = MasterControl._Instance._EffectVolume;
        _BGMAMOUNT.text = Math.Floor(MasterControl._Instance._BGMVolume * 100).ToString();
        _EFFECTAMOUNT.text = Math.Floor(MasterControl._Instance._EffectVolume * 100).ToString();
    }

    void _TextChange(string txt, Color color)
    {
        GameObject T = Instantiate(_Text);
        Vector3 pos = transform.position;
        pos.x -= 3f;
        pos.y += 5f;
        T.transform.position = pos;
        T.GetComponent<TextControl>()._color = color;
        T.GetComponent<TextControl>()._Size = 75;
        T.GetComponent<TextControl>()._Text = txt;
    }
    void Update()
    {
        if (_CurrentState == PlayerState.END)
        {
            return;
        }
            if (_CurrentState == PlayerState.Wait)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _Setting.gameObject.activeSelf)
            {
                _Option_Sub.SetActive(true);
                _Setting.SetActive(false);
            }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.B) && _CurrentState != PlayerState.Building)
            {
                //�� ���°� Bulding ���°� �ƴ� ��� B�� ������ Building ����

                if (CameraControl._instance._GetMoney() >= _BuildingCost)
                {
                    _CurrentState = PlayerState.Building;
                    PaintingTile();
                }
                else
                {
                    _TextChange("�Ǽ� ����� �����մϴ�.", Color.red);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_CurrentState == PlayerState.Building)
                {
                    //���� �� ���°� Building ������ �� Esc�� ������ ���� ���·�.
                    _CurrentState = PlayerState.None;
                    PaintingTile();
                }

            }
        }
    }

    public void PaintingTile()
    {
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Tile");

        switch (_CurrentState)
        {
            case PlayerState.Building:
                foreach (GameObject plane in planes)
                {
                    if (plane.transform.childCount <= 0)
                    {
                        plane.GetComponent<Renderer>().material.color = Color.green; // Ÿ���� ������ �ʷϻ�
                    }
                    else
                    {
                        plane.GetComponent<Renderer>().material.color = Color.red; // Ÿ���� ������ ����������
                    }
                }
                break;
            case PlayerState.None:
                foreach (GameObject plane in planes)
                {
                    plane.GetComponent<Renderer>().material.color = Color.white; // �ʱ�ȭ
                }
                break;
        }
    }


    public void GameReady()
    {
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

    void RePlayButton()
    {
        _CurrentState = PlayerState.None;
        _Option.SetActive(false);
    }

    void OptionButton()
    {
        _Option_Sub.SetActive(false);
        _Setting.SetActive(true);
    }

    void ExitButton()
    {
        _Option.SetActive(false);
        GamePlay._instance._CurrentState = PlayerState.END;
    }
    void ExitButtonSub()
    {
        _Option_Sub.SetActive(true);
        _Setting.SetActive(false);
    }
}
