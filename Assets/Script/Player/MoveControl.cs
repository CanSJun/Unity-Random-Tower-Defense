using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GamePlay;

public class MoveControl : MonoBehaviour
{
    public float _MoveSpeed = 20;
    public float _RotationSpeed = 360;

    public GameObject _ClickEffect;
    public GameObject _SelectEffect;
    GameObject _CurrentSelectEffect;
    public GameObject[] _Tower;

    [SerializeField] GameObject _Text;
    public TextMeshProUGUI _Gold_Text;
    public GameObject _Bottom_Text;
    public TextMeshProUGUI _Stat_Text;
    public TextMeshProUGUI _Chr_Text;
    Vector3 _GoalPosition;
    Quaternion _GoalRotation;


    [SerializeField] GameObject _Cam;
    [SerializeField] Camera[] _TowerCam;
    [SerializeField] string[] _TowerDescript;

    public Button _UpgradeBtn;
    public Button _DeleteBtn;


    GameObject _CurrentObj; // 지금 내 타워가 무엇인지 받아오기 위해서

    public GameObject _Option;

    enum TowerCamType
    {
        Fast = 0,
        Slow,
        Buff,
        Mega
    }

    Animator _aniControl;

    void Awake()
    {
       _aniControl = GetComponent<Animator>();
        _GoalPosition = transform.position;

        
    }

    void Start()
    {
        _UpgradeBtn.onClick.AddListener(UpgradeClicked);
        _DeleteBtn.onClick.AddListener(DeleteClicked);
    }
    void UpgradeClicked()
    {

        GamePlay._instance._CurrentState = GamePlay.PlayerState.Upgrade;
    }

    void DeleteClicked()
    {
        GamePlay._instance._CurrentState = GamePlay.PlayerState.None;
        int gold = _CurrentObj.GetComponentInParent<TowerControl>()._Level * 15;
        CameraControl._instance._SetMoney(CameraControl._instance._GetMoney() + gold);
        _Gold_Text.text = (CameraControl._instance._GetMoney()).ToString();
        SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Delete);

        GameObject T = Instantiate(_Text);
        Vector3 pos = _CurrentObj.transform.parent.gameObject.transform.position;
        pos.y += 13f;
        pos.x += 5f;

        T.transform.position = pos;
        T.GetComponent<TextControl>()._color = Color.yellow;
        T.GetComponent<TextControl>()._Size = 75;
        T.GetComponent<TextControl>()._Text = gold.ToString();
        _Cam.SetActive(false);
        for (int i = 0; i < _TowerCam.Length; i++)
        {
            if (_TowerCam[i] && _TowerCam[i].gameObject.activeSelf)
            {
                _TowerCam[i].gameObject.SetActive(false);
            }
        }
        Destroy(_CurrentSelectEffect);
        Destroy(_CurrentObj.transform.parent.gameObject);

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
        if(GamePlay._instance._CurrentState == GamePlay.PlayerState.Wait || GamePlay._instance._CurrentState == GamePlay.PlayerState.END)
        {
            return;
        }
        if (GamePlay._instance.State == GamePlay.PlayerState.None)
        {

            if (Input.GetButtonDown("Fire1"))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int Imask = 1 << LayerMask.NameToLayer("Tower");
                if (Physics.Raycast(r, out hit, Mathf.Infinity, Imask))
                {
                    GameObject _tower = hit.collider.gameObject;
                    if (!_Cam.gameObject.activeSelf)
                    {
                        _Cam.gameObject.SetActive(true);
                    }
                    for (int i = 0; i < _TowerCam.Length; i++)
                    {
                        if (_TowerCam[i] && _TowerCam[i].gameObject.activeSelf)
                        {
                            _TowerCam[i].gameObject.SetActive(false);
                        }
                    }
                    switch (_tower.gameObject.tag)
                    {
                        case "FastTower":
                            _TowerCam[0].gameObject.SetActive(true);
                            _Chr_Text.text = _TowerDescript[0];
                            break;
                        case "SlowTower":
                            _TowerCam[1].gameObject.SetActive(true);
                            _Chr_Text.text = _TowerDescript[1];
                            break;
                        case "BuffTower":
                            _TowerCam[2].gameObject.SetActive(true);
                            _Chr_Text.text = _TowerDescript[2];
                            break;
                        case "MegaTower":
                            _TowerCam[3].gameObject.SetActive(true);
                            _Chr_Text.text = _TowerDescript[3];
                            break;
                    }

                    //이렇게 난눈이유는 CurrentObj가 없을 때 첫 지정 해주기 위해서.
                    if (_CurrentObj == null)
                    {
                        _CurrentObj = _tower;
                        _CurrentSelectEffect = Instantiate(_SelectEffect, _tower.transform.position, _SelectEffect.transform.rotation);
                    }
                    else
                    {
                        Destroy(_CurrentSelectEffect);
                        _CurrentObj = _tower;
                        _CurrentSelectEffect = Instantiate(_SelectEffect, _CurrentObj.transform.position, _SelectEffect.transform.rotation);
                    }
                    _Stat_Text.text = "Lev : "+ _tower.GetComponentInParent<TowerControl>()._Level +" Dam : "+ _tower.GetComponentInParent<TowerControl>()._Damage+" Speed : "+ _tower.GetComponentInParent<TowerControl>()._AttackSpeed;


                }
            }else if (Input.GetKeyDown(KeyCode.P) && _Cam.gameObject.activeSelf)
            {
                GamePlay._instance._CurrentState = GamePlay.PlayerState.Upgrade;
            }
            else if (Input.GetKeyDown(KeyCode.D) && _Cam.gameObject.activeSelf)
            {
                int gold = _CurrentObj.GetComponentInParent<TowerControl>()._Level * 15;
                CameraControl._instance._SetMoney(CameraControl._instance._GetMoney() + gold);
                _Gold_Text.text = (CameraControl._instance._GetMoney()).ToString();
                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Delete);

                GameObject T = Instantiate(_Text);
                Vector3 pos = _CurrentObj.transform.parent.gameObject.transform.position;
                pos.y += 13f;
                pos.x += 5f;

                T.transform.position = pos;
                T.GetComponent<TextControl>()._color = Color.yellow;
                T.GetComponent<TextControl>()._Size = 75;
                T.GetComponent<TextControl>()._Text = gold.ToString();
                _Cam.SetActive(false);
                for (int i = 0; i < _TowerCam.Length; i++)
                {
                    if (_TowerCam[i] && _TowerCam[i].gameObject.activeSelf)
                    {
                        _TowerCam[i].gameObject.SetActive(false);
                    }
                }
                Destroy(_CurrentSelectEffect);
                Destroy(_CurrentObj.transform.parent.gameObject);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && _Cam.gameObject.activeSelf)
            {
                for (int i = 0; i < _TowerCam.Length; i++)
                {
                    if (_TowerCam[i] && _TowerCam[i].gameObject.activeSelf)
                    {
                        _TowerCam[i].gameObject.SetActive(false);
                    }
                }
                _Cam.gameObject.SetActive(false);
                if (_CurrentObj)
                {
                    Destroy(_CurrentSelectEffect);
                }
                //업그레이드 상태일땐 취소
                if (GamePlay._instance._CurrentState == GamePlay.PlayerState.Upgrade)
                {
                    GamePlay._instance._CurrentState = PlayerState.None;
                    _Bottom_Text.gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !_Cam.gameObject.activeSelf)
            {
                GamePlay._instance._CurrentState = GamePlay.PlayerState.Wait;
                _Option.gameObject.SetActive(true);
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int Imask = 1 << LayerMask.NameToLayer("Road_Mob") | 1 << LayerMask.NameToLayer("Road_User");
                if (Physics.Raycast(r, out hit, Mathf.Infinity, Imask))
                {
                    _GoalPosition = hit.point;
                    _GoalRotation = Quaternion.LookRotation(_GoalPosition - transform.position);
                    Instantiate(_ClickEffect, _GoalPosition, _ClickEffect.transform.rotation);
                    SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Click);
                }
            }
            if (transform.position != _GoalPosition)
            {
                _aniControl.SetBool("isMoveCheck", true);
                transform.position = Vector3.MoveTowards(transform.position, _GoalPosition, _MoveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _GoalRotation, _RotationSpeed * Time.deltaTime);
            }
            else
            {
                _aniControl.SetBool("isMoveCheck", false);
            }
        }

        else if (GamePlay._instance.State == GamePlay.PlayerState.Upgrade)
        {
            _Bottom_Text.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GamePlay._instance._CurrentState = GamePlay.PlayerState.None;
                _Bottom_Text.gameObject.SetActive(false);
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int Imask = 1 << LayerMask.NameToLayer("Tower");
                if (Physics.Raycast(r, out hit, Mathf.Infinity, Imask))
                {
                    GameObject _tower = hit.collider.gameObject;
                    if (_tower.gameObject == _CurrentObj)
                    {
                        _TextChange("다른 건물을 선택을 하여 주세요.", Color.red);
                    }
                    else
                    {
                        if (_tower.gameObject.tag == _CurrentObj.transform.tag)
                        {
                            if (_tower.GetComponentInParent<TowerControl>()._Level == _CurrentObj.GetComponentInParent<TowerControl>()._Level)
                            {
                                _tower.GetComponentInParent<TowerControl>()._upgraded = true;
                                Destroy(_tower.transform.parent.gameObject);

                                _CurrentObj.GetComponentInParent<TowerControl>()._ChangeLevel();
                                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Upgrade);
                                GamePlay._instance._CurrentState = GamePlay.PlayerState.None;
                                _Stat_Text.text = "Lev : " + _CurrentObj.GetComponentInParent<TowerControl>()._Level + " Dam : " + _CurrentObj.GetComponentInParent<TowerControl>()._Damage + " Speed : " + _CurrentObj.GetComponentInParent<TowerControl>()._AttackSpeed;
                                _Bottom_Text.gameObject.SetActive(false);
                                _TextChange("업그레이드 완료", Color.blue);
                            }
                            else
                            {
                                _TextChange("같은 레벨의 건물만 가능합니다.", Color.red);
                            }
                        }
                        else
                        {
                            _TextChange("동일한 건물만 가능합니다.", Color.red);
                        }

                    }
                }
            }
        }
        else if (GamePlay._instance.State == GamePlay.PlayerState.Building)
        {
            if (Input.GetButtonDown("Fire1"))
            {

                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int Imask = 1 << LayerMask.NameToLayer("Road_User");
                if (Physics.Raycast(r, out hit, Mathf.Infinity, Imask))
                {
                    GameObject Tileobj = hit.collider.gameObject; // 클릭한 타일 가져오기
                    

                    if (Tileobj.transform.childCount <= 0) // 만약에 이 위치에 타워가 없으면? 건설 가능
                    {
                        
                        Vector3 TileCenter = hit.collider.bounds.center; // 타일 중앙  
                        int rnd = Random.Range(0, _Tower.Length);

                        GameObject Build = Instantiate(_Tower[rnd].gameObject, TileCenter, Quaternion.identity, hit.collider.transform);
                        Build.transform.localScale = new Vector3(
                            _Tower[rnd].gameObject.transform.lossyScale.x / Tileobj.transform.lossyScale.x,
                            _Tower[rnd].gameObject.transform.lossyScale.y / Tileobj.transform.lossyScale.y,
                            _Tower[rnd].gameObject.transform.lossyScale.z / Tileobj.transform.lossyScale.z

                        ); // 부모 크기에 종속되지 않기 위해서 
                        GamePlay._instance._CurrentState = GamePlay.PlayerState.None; // 현재 상태 변경
                        GamePlay._instance.PaintingTile(); // 타일 색칠을 다시 초기화
                        
                        SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Building);
                        CameraControl._instance._SetMoney(CameraControl._instance._GetMoney() - GamePlay._instance._BuildingCost);
                        _Gold_Text.text = (CameraControl._instance._GetMoney()).ToString();

                        GameObject T = Instantiate(_Text);
                        Vector3 pos = TileCenter;
                        pos.y += 13f;
                        pos.x += 5f;
                        T.transform.position = pos;

                        T.GetComponent<TextControl>()._color = Color.gray;
                        T.GetComponent<TextControl>()._Size = 80;
                        T.GetComponent<TextControl>()._Text = GamePlay._instance._BuildingCost.ToString();
                    }
                    else
                    {

                        SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Nope);
                        _TextChange("현 위치에는 건설을 할 수 없습니다.", Color.red);
                    }
                }
            }
        }
     }

   
}
