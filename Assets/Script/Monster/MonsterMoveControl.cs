using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterMoveControl : MonoBehaviour
{

    public float _MoveSpeed;
    float _OriginSpeed;

    public GameObject[] _Points = new GameObject[12];

    [SerializeField] GameObject _HP_Bar;
    [SerializeField] GameObject _Text;
    [SerializeField] Transform _Text_pos;

    public bool _IsDead;
    int _HP;

    
    private enum MobState
    {
        Start       = 0,
        Point_1,
        Point_2,
        Point_3,
        Point_4,
        Point_5, 
        Point_6,
        Point_7,
        Point_8, 
        Point_9, 
        Point_10, 
        Point_11,
        Point_12,
        Point_13,
        Point_14,
        Point_15,
        Point_16,
        Point_17,
        Point_18,
    }
    MobState _CurrentState = MobState.Start;
    Vector3 _GoalPosition;

    public TextMeshProUGUI _Gold_Text;

    Animator _Action;
    void Awake()
    {
        _GoalPosition = transform.position;
        _IsDead = false;
        _Action = GetComponent<Animator>();
    
    }

    void Start()
    {
        if (CameraControl._instance._level % 10 == 0)
        {
            _HP = (CameraControl._instance._level * 5) + 30;
        }
        else
        {
            _HP = (CameraControl._instance._level * 2) + 5 ;
        }
        if (CameraControl._instance._level == 1) _HP = 5;
        _HP_Bar.GetComponent<Slider>().maxValue = _HP;
        _HP_Bar.GetComponent<Slider>().value = _HP;
        _OriginSpeed = _MoveSpeed;
        //레벨마다 최대 체력이 다르니깐 설정!
    }
    void Update()
    {
        if (GamePlay._instance._CurrentState == GamePlay.PlayerState.Wait || GamePlay._instance._CurrentState == GamePlay.PlayerState.END)
        {
            return;
        }

        if (_IsDead)
        {
            return;
        }
        if(_MoveSpeed < _OriginSpeed)
        {
            _MoveSpeed += 1 * Time.deltaTime;
        }
        if (Vector3.Distance(transform.position, _GoalPosition) < 0.1f)
        {
            //만약에 내 위치와 목표 위치의 차이가 없으면? 그 위치라는 뜻!
            if (_CurrentState == MobState.Start)
            {
                //만약에 내 상태가 Start라면?
                _CurrentState = (MobState)1;
                _GoalPosition = _Points[1].transform.position;
                //현재 상태를 MobState의 1번째로 즉 point1으로 바꾸어 주고 목표 지점을 첫번째를 향하게 해준다.

            }
            else
            {
                //Start가 아닌 경우는 11까지 봐야함.
                int CurrentIndex = (int)_CurrentState;
                // 현재 내 상태를 int로 가져온다 만약에 point-3이라면 3을 가져 올 것임.
                if (CurrentIndex < _Points.Length) // 만약에 배열 크기 안이라면 즉 내가 선언해준게 12개인데 그 안일 경우만 실행
                {
                    _CurrentState = (MobState)(CurrentIndex + 1); // Start가 아니니깐 point2부터 시작 하여야 하기 때문에 + 1
                    if (CurrentIndex < _Points.Length - 1) // -1을 해준 이유는 배열의 마지막이 End 지점이기 때문
                    {
                        _GoalPosition = _Points[CurrentIndex + 1].transform.position;
                        // 내 목표 지점은 현재 point 지점에서 + 1한 point로 지정해줌
                    }
                    else
                    {
                        CameraControl._instance._kill++;

                        if (CameraControl._instance._level % 10 == 0)
                        {
                            CameraControl._MobEnd(_GoalPosition, 50);
                        }
                        else
                        {
                            CameraControl._MobEnd(_GoalPosition, 1);
                        }

                        
                        Destroy(gameObject);
                        if(CameraControl._instance._GetLife() <= 0)
                        {
                            GamePlay._instance._CurrentState = GamePlay.PlayerState.END;
                        }
                        return;
                    }
                }
            }
        }

        transform.LookAt(_GoalPosition); // 해당 방향을 바라보게
        transform.position = Vector3.MoveTowards(transform.position, _GoalPosition , _MoveSpeed * Time.deltaTime);
        // 이동
    }

    public void _Hitted(int Damage)
    {
        if (_HP <= Damage)
        {
            if (_IsDead) return;
            _IsDead = true;
            //만약에 현재 체력이 데미지 보다 작다면?
            CameraControl._instance._kill++;
            //  HP바를 꺼준다.
            _HP_Bar.SetActive(false);
            _Action.SetBool("IsDeath", true);
            if (CameraControl._instance._level % 10 == 0)
            {
                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Boss_Die);
            }
            else
            {
                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Zomebie_Die);
            }
            if (CameraControl._instance._level % 10 == 0)
            {
                GameObject T = Instantiate(_Text);
                T.transform.position = _Text_pos.position;
                T.GetComponent<TextControl>()._color = Color.yellow;
                T.GetComponent<TextControl>()._Size = 50;
                T.GetComponent<TextControl>()._Text = "50";
                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Coin);
                CameraControl._instance._SetMoney(CameraControl._instance._GetMoney() + 50);
                _Gold_Text.text = CameraControl._instance._GetMoney().ToString();
            }
            else
            {
                int rnd = Random.Range(1, 101);
                if (rnd <= 65)
                {
                    CameraControl._instance._SetMoney(CameraControl._instance._GetMoney() + 5);
                    _Gold_Text.text = CameraControl._instance._GetMoney().ToString();
                    GameObject T = Instantiate(_Text);
                    T.transform.position = _Text_pos.position;
                    T.GetComponent<TextControl>()._color = Color.yellow;
                    T.GetComponent<TextControl>()._Size = 50;
                    T.GetComponent<TextControl>()._Text = "5";
                    SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Coin);
                }
            }
            CameraControl._instance.Result_Kill_Amount++;
            Destroy(gameObject, 3);
            // 그리고 object를 삭제
        }
        else
        {
            _HP_Bar.GetComponent<Slider>().value -= Damage;
            _HP -= Damage;
            //체력 감소
        }
    }
}
