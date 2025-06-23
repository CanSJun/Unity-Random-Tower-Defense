using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update

    
    [SerializeField] AudioClip _BMG;

    public AudioSource _Player;

    [SerializeField] AudioClip[] _Effect;

    List<AudioSource> _ltEffectPlayers;

    public enum SOUNDTYPE
    {
        Click,
        Nope,
        Damaged,
        Building,
        Upgrade,
        Delete,
        Zomebie_Die,
        Boss_Die,
        Tower_Attack,
        Buff,
        Coin,
    }


    static SoundManager _unique;
    public static SoundManager _instance
    {
        get { return _unique; }
    }

    void Awake()
    {
        _unique = this;
        _ltEffectPlayers = new List<AudioSource>();
        _Player = GetComponent<AudioSource>();
    }
    void Start()
    {
        
    }


    void LateUpdate()
    {
        foreach(AudioSource source in _ltEffectPlayers)
        {
            if(source.isPlaying == false)
            {
                _ltEffectPlayers.Remove(source);
                Destroy(source.gameObject);
                break;
            }
        }
        //플레이가 끝난 이펙트는 없엔다
    }

    public void PlayBGM(bool loop = true)
    {
        _Player.clip = _BMG;
        _Player.loop = loop;
        _Player.volume = MasterControl._Instance._BGMVolume;
        _Player.Play();
    }

    public void PlaySound(SOUNDTYPE type,  bool loop = false)
    {
        GameObject snd = new GameObject("EffectSound");
        snd.transform.SetParent(transform);
        AudioSource Sound = snd.AddComponent<AudioSource>();
        Sound.clip = _Effect[(int)type];
        Sound.loop = loop;
        Sound.volume = MasterControl._Instance._EffectVolume;
        Sound.Play();

        _ltEffectPlayers.Add(Sound);
    }

}
