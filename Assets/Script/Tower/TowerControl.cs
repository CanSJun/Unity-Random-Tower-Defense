using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;




public class TowerControl : MonoBehaviour
{
    [SerializeField] GameObject _BuildingEffect;
    [SerializeField] GameObject _BuffEffect;
    public float _AttackSpeed;
    float _OriginAttackSpeed;
    public LayerMask _MobLayer; // �ֺ������� ���ؼ�
    public LayerMask _TowerLayer; // ������ ���ؼ�
    float _attacktime = 0;

    Animator _Action;


    float _SeeAreaTime = 1.5f;
    GameObject _Target = null;
    [SerializeField] GameObject _Area;
    [SerializeField] GameObject _Head;
    [SerializeField] ParticleSystem _LArm;
    [SerializeField] ParticleSystem _RArm;
    [SerializeField] TextMeshProUGUI _LevelText;

    public int _Level = 1;
    public int _Damage;
    float _Buff = 0f;
    float _BuffTime = 0f;
    float _GiveBuffTime = 0f;
    
    public bool _upgraded = false;

    void Awake()
    {
        _Action = GetComponent<Animator>();
    }
    void Start()
    {
        GameObject effect = Instantiate(_BuildingEffect, transform.position, _BuildingEffect.transform.rotation);
        Destroy(effect, 3);
        _Area.SetActive(true);
        _OriginAttackSpeed = _AttackSpeed;


    }

    void LateUpdate()
    {
        if (_upgraded)
        {
            return;
        }
        if(transform.position.x >= 50)
        {
            return;
        }
        if (transform.CompareTag("BuffTower") && _GiveBuffTime <= 0)
        {
            
            int rnd = Random.Range(1, 101);
            if (rnd < (30 + (2 * _Level)))
            {
                float Splash = 8;
                Collider[] _AreaTower = Physics.OverlapSphere(transform.position, Splash, _TowerLayer);
                foreach (var _in in _AreaTower)
                {
                    TowerControl tower = _in.GetComponent<TowerControl>();
                    if (tower._BuffTime <= 0)
                    {
                        if (tower._Buff < (15 + (5 * _Level)))
                        {
                            tower._Buff = (15 + (5 * _Level));
                            tower._AttackSpeed = tower._OriginAttackSpeed;
                            tower._AttackSpeed -= (tower._OriginAttackSpeed / 100) * (15 + (5 * _Level));
                            tower._BuffTime = 6 + _Level;
                            GameObject effect = Instantiate(_BuffEffect, _in.transform.position, _BuffEffect.transform.rotation);
                            Destroy(effect, 6 + _Level);
                        }
                    }
                }
                SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Buff);
            }
            _GiveBuffTime = 15 - _Level;
        }
    }
    void Update()
    {
        if (GamePlay._instance._CurrentState == GamePlay.PlayerState.Wait || GamePlay._instance._CurrentState == GamePlay.PlayerState.END)
        {
            return;
        }

        if (_upgraded)
        {
            return;
        }
        if(_SeeAreaTime > 0)
        {
            _SeeAreaTime -= Time.deltaTime;
            if(_SeeAreaTime <= 0)
            {
                _Area.SetActive(false);
            }
        }
        if (_Target)
        {
            _Head.transform.LookAt(_Target.transform.position);
        }


        if (_BuffTime > 0)
        {
            _BuffTime -= Time.deltaTime;
            if (_BuffTime <= 0)
            {
                _AttackSpeed = _OriginAttackSpeed;
                _Buff = 0;
            }
        }

        if (_GiveBuffTime > 0)
        {
            _GiveBuffTime -= Time.deltaTime;
        }

        if (_attacktime > 0)
        {
            _attacktime -= Time.deltaTime;
        }
        if (_Target && _Target.GetComponent<MonsterMoveControl>()._IsDead)
        {
            _Target = null;
        }else if (_Target && _attacktime <= 0) {

                //_Action.SetBool("IsAttack", true);
                //_Action.SetFloat("AttackSpeed", 1/_AttackSpeed);

                if (!_LArm.gameObject.activeSelf)
                {
                    //�� ����� ������ �ϳ��� �ϳ��� üũ!
                    _LArm.gameObject.SetActive(true);
                    _RArm.gameObject.SetActive(true);

                }
                else
                {
                    _LArm.Play();
                    if (!_LArm.gameObject.name.Equals(_RArm.gameObject.name))
                    {
                        //�ѱ��� �ϳ��� �ָ� ���ؼ�
                        _RArm.Play();
                    }
                }
            
            SoundManager._instance.PlaySound(SoundManager.SOUNDTYPE.Tower_Attack);
            _attacktime = _AttackSpeed;
            // �� ���� ���Ͱ� ���� ���� ���.

            if (transform.CompareTag("MegaTower") || transform.CompareTag("SlowTower"))
            {
                float Splash = 8.0f;
                Collider[] _HitMob = Physics.OverlapSphere(_Target.transform.position, Splash, _MobLayer);
                int count = 0;
                foreach (var Hit in _HitMob)
                {
                    if (Hit.transform != _Target.transform)
                    {
                        if (transform.CompareTag("MegaTower")) Hit.GetComponent<MonsterMoveControl>()._Hitted(_Damage / 2);
                        else
                        {
                            if (count < 3 + _Level)
                            {
                                count++;
                                Hit.GetComponent<MonsterMoveControl>()._Hitted(_Damage / 5);

                                if(_Level > 1)Hit.GetComponent<MonsterMoveControl>()._MoveSpeed = _Level; // ���� �ܰ躰�� �پ���
                                else Hit.GetComponent<MonsterMoveControl>()._MoveSpeed = 1;
                            }
                            else break;
                        }
                    }
                }
                _Target.GetComponent<MonsterMoveControl>()._Hitted(_Damage);
                if (transform.CompareTag("SlowTower")) _Target.GetComponent<MonsterMoveControl>()._MoveSpeed = 3;
                
            }
            else _Target.GetComponent<MonsterMoveControl>()._Hitted(_Damage);
        }

        
    }


    public void _ChangeLevel()
    {
        _Level++;
        _LevelText.text = "Lev : "+_Level;
        _Damage += _Damage;

    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Mob") && !_Target && !other.gameObject.GetComponent<MonsterMoveControl>()._IsDead)
        {
            //Target�� ���ų� ���� ���Ͱ� ���� �ʾ��� ��� Target���� ����
            _Target = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mob") && _Target && _Target == other.gameObject && !other.gameObject.GetComponent<MonsterMoveControl>()._IsDead)
        {
            //Target�� �ְ�, �� Target�� �� ������ ������ ���� ���°� �ƴ� ��� Target�� �ʱ�ȭ
            _Target = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mob") && !_Target && !other.gameObject.GetComponent<MonsterMoveControl>()._IsDead)
        {
            //Target�� ���� ���� �ȿ� �ִ� ���� �� ���� ���� ���� Target���� ����
            _Target = other.gameObject;
        }
    }
}
