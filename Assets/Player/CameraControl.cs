using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] _Effect = new GameObject[10];

    public GameObject _Monster;
    public GameObject _Boss;
    public static GameObject _MobEndEffect;
    public GameObject _SpawnPoint;

    public int _level = 1; // STAGE
    [SerializeField] static int life = 100; // 생명
    [SerializeField] int money = 0; // 돈

    float currenttime = 0;
    int _SpawnAmount = 8;
    int _TotalAmount;
    float _SpawnTime = 0.55f;

    public int _kill = 0;
    static TextMeshProUGUI _HP_Text;
    public TextMeshProUGUI _Gold_Text;
    public TextMeshProUGUI _Stage_Text;
    public TextMeshProUGUI _Time_Text;

    GameObject _MonsterParent;


    float _GameTime;
    bool _GameCheck = false;
    // 결과 창 //
    public int Result_Kill_Amount = 0;
    public int Result_Stage_Amount = 1;
    public GameObject _Result;
    public TextMeshProUGUI _RKillText;
    public TextMeshProUGUI _RStageText;
    public Button _ReTryBtn;
    public Button _RExitButton;

    bool _ResultCheck = false;
    
    enum GameState
    {
        Stop = 0,
        Spawn,
        Start,
        End
    }
    GameState _NowState = GameState.Stop;

    static CameraControl _unique;
    public static CameraControl _instance
    {
        get { return _unique; }
    }
    void Awake()
    {
        _GameTime = 10;

        _MobEndEffect = _Effect[0];
        currenttime = _SpawnTime;
        _MonsterParent = GameObject.FindGameObjectWithTag("Monster");
        _unique = this;

    }
    public static void _MobEnd(Vector3 obj, int x)
    {
       GameObject effectInstance = Instantiate(_MobEndEffect, obj, _MobEndEffect.transform.rotation);
       Destroy(effectInstance, 3);
       life -= x;
       _HP_Text.text = life.ToString();
    }

    void Start()
    {

        _HP_Text = GameObject.Find("HP_TEXT").GetComponent<TextMeshProUGUI>();

        
        life = 100;
        Result_Kill_Amount = 0;
        Result_Stage_Amount = 1;
        money = 120;
        currenttime = 0;
        _SpawnAmount = 8;
        _SpawnTime = 0.55f;
        _TotalAmount = _SpawnAmount;
        _HP_Text.text = life.ToString();
        _Gold_Text.text = money.ToString();

        _ReTryBtn.onClick.AddListener(ReTryButton);
        _RExitButton.onClick.AddListener(ExitButton);

        PrintTime(0);
    }
    void ReTryButton()
    {
        MasterControl._Instance.SceneChange("GameScene", MasterControl.GameState.start);
    }

    void PrintTime(float deltatime)
    {
        if (!_GameCheck)
        {
            _GameTime -= deltatime;

            int minutes = Mathf.FloorToInt(_GameTime / 60); // 분 계산
            int seconds = Mathf.FloorToInt(_GameTime % 60); // 초 계산


            _Time_Text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    void ExitButton()
    {
        Application.Quit();
    }
    void Update()
    {
        if (GamePlay._instance._CurrentState == GamePlay.PlayerState.Wait)
        {
            return;
        }
        if (GamePlay._instance._CurrentState == GamePlay.PlayerState.END && !_ResultCheck)
        {
            _ResultCheck = true;
            _Result.SetActive(true);
            _RKillText.text = "킬 수 : " + Result_Kill_Amount.ToString();
            _RStageText.text = "스테이지 : " + Result_Stage_Amount.ToString();
            return;
        }else if (GamePlay._instance._CurrentState == GamePlay.PlayerState.END && _ResultCheck)
        {
            return;
        }


        PrintTime(Time.deltaTime);
        if(_GameTime <= 1) {
            _GameCheck = true;
            switch (_NowState)
            {
                case GameState.Stop:
                        _NowState = GameState.Spawn;
                        MonsterSpawn();
                    break;

                case GameState.Spawn:

                    if (_SpawnAmount > 0)
                    {
                        if (currenttime <= 0)
                        {
                            MonsterSpawn();
                            currenttime = _SpawnTime;
                        }
                        else
                        {
                            currenttime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        _NowState = GameState.Start;
                    }
                    break;
                case GameState.Start:

                    if (_kill == _TotalAmount)
                    {
                        money += 30;
                        _Gold_Text.text = money.ToString();
                        _level++;
                        _GameTime = 7;
                        _GameCheck = false;
                        CameraControl._instance.Result_Stage_Amount++;
                        _Stage_Text.text = _level.ToString();
                        _kill = 0;
                        if (_level % 10 == 0) _SpawnAmount = _TotalAmount = 1;
                        
                        else
                        {
                            int amount = 8 + (2 * _level);
                            if (amount > 100) amount = 100;
                            _SpawnAmount = _TotalAmount = amount;
                        }
                    }
                    else
                    {
                        if (_SpawnAmount > 0)
                        {
                            if (currenttime <= 0)
                            {
                                MonsterSpawn();
                                currenttime = _SpawnTime;
                            }
                            else currenttime -= Time.deltaTime;
                            
                        }
                    }

                    break;
            }
        }
    }

    void MonsterSpawn()
    {

        if(_level % 10 == 0)
        {
            // 10단계마다 보스 몬스터 1마리 소환
            _SpawnAmount -= _SpawnAmount;
            GameObject obj = Instantiate(_Boss, _SpawnPoint.transform.position, Quaternion.Euler(0, 180, 0), _MonsterParent.transform);
            obj.gameObject.SetActive(true);
        }
        else
        {
            // 일반 단계는 일반 좀비 소환
            _SpawnAmount--;
            GameObject obj = Instantiate(_Monster, _SpawnPoint.transform.position, Quaternion.Euler(0, 180, 0), _MonsterParent.transform);
            obj.gameObject.SetActive(true);
        }
    }



    public int _GetLevel()
    {
        return _level;
    }
    public void _SetLevel(int x)
    {
        _level = x;
    }
    public int _GetLife()
    {
        return life;
    }
    public void _SetLife(int x)
    {
        life = x;
    }

    public int _GetMoney()
    {
        return money;
    }
    public void _SetMoney(int x)
    {
        money = x;
    }
}
