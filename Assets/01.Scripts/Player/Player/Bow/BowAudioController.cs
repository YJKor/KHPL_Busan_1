using UnityEngine;

/// <summary>
/// 활게임의 모든 오디오 효과를 관리하는 컨트롤러
/// 활 시위 당김, 화살 발사, 타겟 히트 등의 사운드를 처리합니다.
/// </summary>
public class BowAudioController : MonoBehaviour
{
    [Header("활 시위 사운드")]
    [Tooltip("활 시위를 당길 때 재생되는 사운드")]
    public AudioClip bowStringPullSound;
    
    [Tooltip("활 시위를 놓을 때 재생되는 사운드")]
    public AudioClip bowStringReleaseSound;
    
    [Header("화살 사운드")]
    [Tooltip("화살이 발사될 때 재생되는 사운드")]
    public AudioClip arrowShootSound;
    
    [Tooltip("화살이 날아가는 바람 소리")]
    public AudioClip arrowFlightSound;
    
    [Header("타겟 히트 사운드")]
    [Tooltip("타겟에 화살이 맞았을 때 재생되는 사운드")]
    public AudioClip targetHitSound;
    
    [Tooltip("타겟이 파괴되었을 때 재생되는 사운드")]
    public AudioClip targetDestroySound;
    
    [Header("배경 음악")]
    [Tooltip("게임 배경 음악")]
    public AudioClip backgroundMusic;
    
    [Header("오디오 설정")]
    [Tooltip("마스터 볼륨 (0.0 ~ 1.0)")]
    [Range(0.0f, 1.0f)]
    public float masterVolume = 1.0f;
    
    [Tooltip("효과음 볼륨 (0.0 ~ 1.0)")]
    [Range(0.0f, 1.0f)]
    public float sfxVolume = 0.8f;
    
    [Tooltip("배경음악 볼륨 (0.0 ~ 1.0)")]
    [Range(0.0f, 1.0f)]
    public float musicVolume = 0.6f;
    
    [Tooltip("3D 사운드 사용 여부")]
    public bool use3DSound = true;
    
    [Tooltip("사운드 재생 거리")]
    public float soundDistance = 10f;
    
    // 오디오 소스 컴포넌트들
    private AudioSource _masterAudioSource;
    private AudioSource _sfxAudioSource;
    private AudioSource _musicAudioSource;
    private AudioSource _bowAudioSource;
    
    // 싱글톤 패턴
    public static BowAudioController Instance { get; private set; }
    
    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 오디오 소스 컴포넌트들 초기화
        InitializeAudioSources();
    }
    
    void Start()
    {
        // 배경 음악 재생
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }
    
    /// <summary>
    /// 오디오 소스 컴포넌트들을 초기화합니다.
    /// </summary>
    void InitializeAudioSources()
    {
        // 마스터 오디오 소스 (전체 볼륨 제어용)
        _masterAudioSource = gameObject.AddComponent<AudioSource>();
        _masterAudioSource.playOnAwake = false;
        _masterAudioSource.volume = masterVolume;
        
        // 효과음 전용 오디오 소스
        GameObject sfxObject = new GameObject("SFX Audio Source");
        sfxObject.transform.SetParent(transform);
        _sfxAudioSource = sfxObject.AddComponent<AudioSource>();
        _sfxAudioSource.playOnAwake = false;
        _sfxAudioSource.volume = sfxVolume;
        _sfxAudioSource.spatialBlend = use3DSound ? 1.0f : 0.0f;
        _sfxAudioSource.maxDistance = soundDistance;
        
        // 배경음악 전용 오디오 소스
        GameObject musicObject = new GameObject("Music Audio Source");
        musicObject.transform.SetParent(transform);
        _musicAudioSource = musicObject.AddComponent<AudioSource>();
        _musicAudioSource.playOnAwake = false;
        _musicAudioSource.volume = musicVolume;
        _musicAudioSource.loop = true;
        _musicAudioSource.spatialBlend = 0.0f; // 2D 사운드
        
        // 활 전용 오디오 소스 (3D 사운드용)
        GameObject bowObject = new GameObject("Bow Audio Source");
        bowObject.transform.SetParent(transform);
        _bowAudioSource = bowObject.AddComponent<AudioSource>();
        _bowAudioSource.playOnAwake = false;
        _bowAudioSource.volume = sfxVolume;
        _bowAudioSource.spatialBlend = use3DSound ? 1.0f : 0.0f;
        _bowAudioSource.maxDistance = soundDistance;
    }
    
    /// <summary>
    /// 배경 음악을 재생합니다.
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (_musicAudioSource != null && backgroundMusic != null)
        {
            _musicAudioSource.clip = backgroundMusic;
            _musicAudioSource.Play();
        }
    }
    
    /// <summary>
    /// 활 시위를 당길 때 사운드를 재생합니다.
    /// </summary>
    public void PlayBowStringPullSound()
    {
        if (_bowAudioSource != null && bowStringPullSound != null)
        {
            _bowAudioSource.PlayOneShot(bowStringPullSound);
        }
    }
    
    /// <summary>
    /// 활 시위를 놓을 때 사운드를 재생합니다.
    /// </summary>
    public void PlayBowStringReleaseSound()
    {
        if (_bowAudioSource != null && bowStringReleaseSound != null)
        {
            _bowAudioSource.PlayOneShot(bowStringReleaseSound);
        }
    }
    
    /// <summary>
    /// 화살 발사 사운드를 재생합니다.
    /// </summary>
    public void PlayArrowShootSound()
    {
        if (_sfxAudioSource != null && arrowShootSound != null)
        {
            _sfxAudioSource.PlayOneShot(arrowShootSound);
        }
    }
    
    /// <summary>
    /// 화살 비행 사운드를 재생합니다.
    /// </summary>
    public void PlayArrowFlightSound()
    {
        if (_sfxAudioSource != null && arrowFlightSound != null)
        {
            _sfxAudioSource.PlayOneShot(arrowFlightSound);
        }
    }
    
    /// <summary>
    /// 타겟 히트 사운드를 재생합니다.
    /// </summary>
    public void PlayTargetHitSound()
    {
        if (_sfxAudioSource != null && targetHitSound != null)
        {
            _sfxAudioSource.PlayOneShot(targetHitSound);
        }
    }
    
    /// <summary>
    /// 타겟 파괴 사운드를 재생합니다.
    /// </summary>
    public void PlayTargetDestroySound()
    {
        if (_sfxAudioSource != null && targetDestroySound != null)
        {
            _sfxAudioSource.PlayOneShot(targetDestroySound);
        }
    }
    
    /// <summary>
    /// 마스터 볼륨을 설정합니다.
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (_masterAudioSource != null)
        {
            _masterAudioSource.volume = masterVolume;
        }
    }
    
    /// <summary>
    /// 효과음 볼륨을 설정합니다.
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.volume = sfxVolume;
        }
        if (_bowAudioSource != null)
        {
            _bowAudioSource.volume = sfxVolume;
        }
    }
    
    /// <summary>
    /// 배경음악 볼륨을 설정합니다.
    /// </summary>
    /// <param name="volume">볼륨 값 (0.0 ~ 1.0)</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (_musicAudioSource != null)
        {
            _musicAudioSource.volume = musicVolume;
        }
    }
    
    /// <summary>
    /// 3D 사운드 사용 여부를 설정합니다.
    /// </summary>
    /// <param name="use3D">3D 사운드 사용 여부</param>
    public void Set3DSound(bool use3D)
    {
        use3DSound = use3D;
        float spatialBlend = use3DSound ? 1.0f : 0.0f;
        
        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.spatialBlend = spatialBlend;
        }
        if (_bowAudioSource != null)
        {
            _bowAudioSource.spatialBlend = spatialBlend;
        }
    }
    
    /// <summary>
    /// 활 오디오 소스의 위치를 설정합니다 (3D 사운드용).
    /// </summary>
    /// <param name="position">위치</param>
    public void SetBowAudioPosition(Vector3 position)
    {
        if (_bowAudioSource != null)
        {
            _bowAudioSource.transform.position = position;
        }
    }
    
    /// <summary>
    /// 모든 사운드를 일시정지합니다.
    /// </summary>
    public void PauseAllSounds()
    {
        if (_sfxAudioSource != null) _sfxAudioSource.Pause();
        if (_musicAudioSource != null) _musicAudioSource.Pause();
        if (_bowAudioSource != null) _bowAudioSource.Pause();
    }
    
    /// <summary>
    /// 모든 사운드를 재개합니다.
    /// </summary>
    public void ResumeAllSounds()
    {
        if (_sfxAudioSource != null) _sfxAudioSource.UnPause();
        if (_musicAudioSource != null) _musicAudioSource.UnPause();
        if (_bowAudioSource != null) _bowAudioSource.UnPause();
    }
    
    /// <summary>
    /// 모든 사운드를 중지합니다.
    /// </summary>
    public void StopAllSounds()
    {
        if (_sfxAudioSource != null) _sfxAudioSource.Stop();
        if (_musicAudioSource != null) _musicAudioSource.Stop();
        if (_bowAudioSource != null) _bowAudioSource.Stop();
    }
}
