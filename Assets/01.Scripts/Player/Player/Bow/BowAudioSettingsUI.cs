using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 활게임의 오디오 설정을 관리하는 UI 스크립트
/// 볼륨 조절, 3D 사운드 설정 등을 제공합니다.
/// </summary>
public class BowAudioSettingsUI : MonoBehaviour
{
    [Header("볼륨 슬라이더")]
    [Tooltip("마스터 볼륨 슬라이더")]
    public Slider masterVolumeSlider;
    
    [Tooltip("효과음 볼륨 슬라이더")]
    public Slider sfxVolumeSlider;
    
    [Tooltip("배경음악 볼륨 슬라이더")]
    public Slider musicVolumeSlider;
    
    [Header("볼륨 텍스트")]
    [Tooltip("마스터 볼륨 표시 텍스트")]
    public TextMeshProUGUI masterVolumeText;
    
    [Tooltip("효과음 볼륨 표시 텍스트")]
    public TextMeshProUGUI sfxVolumeText;
    
    [Tooltip("배경음악 볼륨 표시 텍스트")]
    public TextMeshProUGUI musicVolumeText;
    
    [Header("3D 사운드 설정")]
    [Tooltip("3D 사운드 토글 버튼")]
    public Toggle use3DSoundToggle;
    
    [Header("사운드 거리 설정")]
    [Tooltip("사운드 거리 슬라이더")]
    public Slider soundDistanceSlider;
    
    [Tooltip("사운드 거리 표시 텍스트")]
    public TextMeshProUGUI soundDistanceText;
    
    [Header("오디오 제어 버튼")]
    [Tooltip("모든 사운드 일시정지 버튼")]
    public Button pauseAllButton;
    
    [Tooltip("모든 사운드 재개 버튼")]
    public Button resumeAllButton;
    
    [Tooltip("모든 사운드 중지 버튼")]
    public Button stopAllButton;
    
    [Header("UI 설정")]
    [Tooltip("오디오 설정 패널")]
    public GameObject audioSettingsPanel;
    
    [Tooltip("설정 토글 버튼")]
    public Button settingsToggleButton;
    
    private bool _isSettingsPanelOpen = false;
    
    void Start()
    {
        // UI 초기화
        InitializeUI();
        
        // 이벤트 리스너 등록
        RegisterEventListeners();
        
        // 현재 오디오 설정으로 UI 업데이트
        UpdateUIFromAudioController();
    }
    
    /// <summary>
    /// UI 컴포넌트들을 초기화합니다.
    /// </summary>
    void InitializeUI()
    {
        // 슬라이더 초기값 설정
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = 1f;
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = 0.8f;
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = 0.6f;
        }
        
        if (soundDistanceSlider != null)
        {
            soundDistanceSlider.minValue = 1f;
            soundDistanceSlider.maxValue = 50f;
            soundDistanceSlider.value = 10f;
        }
        
        // 토글 초기값 설정
        if (use3DSoundToggle != null)
        {
            use3DSoundToggle.isOn = true;
        }
        
        // 설정 패널 초기 상태
        if (audioSettingsPanel != null)
        {
            audioSettingsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// UI 이벤트 리스너들을 등록합니다.
    /// </summary>
    void RegisterEventListeners()
    {
        // 볼륨 슬라이더 이벤트
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (soundDistanceSlider != null)
        {
            soundDistanceSlider.onValueChanged.AddListener(OnSoundDistanceChanged);
        }
        
        // 3D 사운드 토글 이벤트
        if (use3DSoundToggle != null)
        {
            use3DSoundToggle.onValueChanged.AddListener(On3DSoundToggled);
        }
        
        // 오디오 제어 버튼 이벤트
        if (pauseAllButton != null)
        {
            pauseAllButton.onClick.AddListener(OnPauseAllClicked);
        }
        
        if (resumeAllButton != null)
        {
            resumeAllButton.onClick.AddListener(OnResumeAllClicked);
        }
        
        if (stopAllButton != null)
        {
            stopAllButton.onClick.AddListener(OnStopAllClicked);
        }
        
        // 설정 토글 버튼 이벤트
        if (settingsToggleButton != null)
        {
            settingsToggleButton.onClick.AddListener(OnSettingsToggleClicked);
        }
    }
    
    /// <summary>
    /// BowAudioController의 현재 설정으로 UI를 업데이트합니다.
    /// </summary>
    void UpdateUIFromAudioController()
    {
        if (BowAudioController.Instance == null) return;
        
        // 볼륨 슬라이더 업데이트
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = BowAudioController.Instance.masterVolume;
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = BowAudioController.Instance.sfxVolume;
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = BowAudioController.Instance.musicVolume;
        }
        
        if (soundDistanceSlider != null)
        {
            soundDistanceSlider.value = BowAudioController.Instance.soundDistance;
        }
        
        // 3D 사운드 토글 업데이트
        if (use3DSoundToggle != null)
        {
            use3DSoundToggle.isOn = BowAudioController.Instance.use3DSound;
        }
        
        // 텍스트 업데이트
        UpdateVolumeTexts();
    }
    
    /// <summary>
    /// 볼륨 텍스트들을 업데이트합니다.
    /// </summary>
    void UpdateVolumeTexts()
    {
        if (masterVolumeText != null && masterVolumeSlider != null)
        {
            masterVolumeText.text = $"마스터 볼륨: {(masterVolumeSlider.value * 100):F0}%";
        }
        
        if (sfxVolumeText != null && sfxVolumeSlider != null)
        {
            sfxVolumeText.text = $"효과음 볼륨: {(sfxVolumeSlider.value * 100):F0}%";
        }
        
        if (musicVolumeText != null && musicVolumeSlider != null)
        {
            musicVolumeText.text = $"배경음악 볼륨: {(musicVolumeSlider.value * 100):F0}%";
        }
        
        if (soundDistanceText != null && soundDistanceSlider != null)
        {
            soundDistanceText.text = $"사운드 거리: {soundDistanceSlider.value:F1}m";
        }
    }
    
    // 이벤트 핸들러들
    void OnMasterVolumeChanged(float value)
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.SetMasterVolume(value);
        }
        UpdateVolumeTexts();
    }
    
    void OnSFXVolumeChanged(float value)
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.SetSFXVolume(value);
        }
        UpdateVolumeTexts();
    }
    
    void OnMusicVolumeChanged(float value)
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.SetMusicVolume(value);
        }
        UpdateVolumeTexts();
    }
    
    void OnSoundDistanceChanged(float value)
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.soundDistance = value;
        }
        UpdateVolumeTexts();
    }
    
    void On3DSoundToggled(bool isOn)
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.Set3DSound(isOn);
        }
    }
    
    void OnPauseAllClicked()
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.PauseAllSounds();
        }
    }
    
    void OnResumeAllClicked()
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.ResumeAllSounds();
        }
    }
    
    void OnStopAllClicked()
    {
        if (BowAudioController.Instance != null)
        {
            BowAudioController.Instance.StopAllSounds();
        }
    }
    
    void OnSettingsToggleClicked()
    {
        _isSettingsPanelOpen = !_isSettingsPanelOpen;
        
        if (audioSettingsPanel != null)
        {
            audioSettingsPanel.SetActive(_isSettingsPanelOpen);
        }
    }
    
    /// <summary>
    /// 설정을 PlayerPrefs에 저장합니다.
    /// </summary>
    public void SaveSettings()
    {
        if (BowAudioController.Instance == null) return;
        
        PlayerPrefs.SetFloat("MasterVolume", BowAudioController.Instance.masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", BowAudioController.Instance.sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", BowAudioController.Instance.musicVolume);
        PlayerPrefs.SetFloat("SoundDistance", BowAudioController.Instance.soundDistance);
        PlayerPrefs.SetInt("Use3DSound", BowAudioController.Instance.use3DSound ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// PlayerPrefs에서 설정을 불러옵니다.
    /// </summary>
    public void LoadSettings()
    {
        if (BowAudioController.Instance == null) return;
        
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        float soundDistance = PlayerPrefs.GetFloat("SoundDistance", 10f);
        bool use3DSound = PlayerPrefs.GetInt("Use3DSound", 1) == 1;
        
        BowAudioController.Instance.SetMasterVolume(masterVolume);
        BowAudioController.Instance.SetSFXVolume(sfxVolume);
        BowAudioController.Instance.SetMusicVolume(musicVolume);
        BowAudioController.Instance.soundDistance = soundDistance;
        BowAudioController.Instance.Set3DSound(use3DSound);
        
        UpdateUIFromAudioController();
    }
    
    void OnDestroy()
    {
        // 설정 저장
        SaveSettings();
    }
}
