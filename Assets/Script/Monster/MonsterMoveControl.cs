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
        //�������� �ִ� ü���� �ٸ��ϱ� ����!
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
            //���࿡ �� ��ġ�� ��ǥ ��ġ�� ���̰� ������? �� ��ġ��� ��!
            if (_CurrentState == MobState.Start)
            {
                //���࿡ �� ���°� Start���?
                _CurrentState = (MobState)1;
                _GoalPosition = _Points[1].transform.position;
                //���� ���¸� MobState�� 1��°�� �� point1���� �ٲپ� �ְ� ��ǥ ������ ù��°�� ���ϰ� ���ش�.

            }
            else
            {
                //Start�� �ƴ� ���� 11���� ������.
                int CurrentIndex = (int)_CurrentState;
                // ���� �� ���¸� int�� �����´� ���࿡ point-3�̶�� 3�� ���� �� ����.
                if (CurrentIndex < _Points.Length) // ���࿡ �迭 ũ�� ���̶�� �� ���� �������ذ� 12���ε� �� ���� ��츸 ����
                {
                    _CurrentState = (MobState)(CurrentIndex + 1); // Start�� �ƴϴϱ� point2���� ���� �Ͽ��� �ϱ� ������ + 1
                    if (CurrentIndex < _Points.Length - 1) // -1�� ���� ������ �迭�� �������� End �����̱� ����
                    {
                        _GoalPosition = _Points[CurrentIndex + 1].transform.position;
                        // �� ��ǥ ������ ���� point �������� + 1�� point�� ��������
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

        transform.LookAt(_GoalPosition); // �ش� ������ �ٶ󺸰�
        transform.position = Vector3.MoveTowards(transform.position, _GoalPosition , _MoveSpeed * Time.deltaTime);
        // �̵�
    }

    public void _Hitted(int Damage)
    {
        if (_HP <= Damage)
        {
            if (_IsDead) return;
            _IsDead = true;
            //���࿡ ���� ü���� ������ ���� �۴ٸ�?
            CameraControl._instance._kill++;
            //  HP�ٸ� ���ش�.
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
            // �׸��� object�� ����
        }
        else
        {
            _HP_Bar.GetComponent<Slider>().value -= Damage;
            _HP -= Damage;
            //ü�� ����
        }
    }
}
