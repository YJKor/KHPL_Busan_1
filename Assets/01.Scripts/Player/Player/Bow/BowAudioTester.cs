using UnityEngine;

/// <summary>
/// 활게임 오디오 시스템을 테스트하기 위한 스크립트
/// 키보드 입력으로 각종 사운드를 테스트할 수 있습니다.
/// </summary>
public class BowAudioTester : MonoBehaviour
{
    [Header("테스트 설정")]
    [Tooltip("테스트 모드 활성화 여부")]
    public bool enableTestMode = true;
    
    [Tooltip("테스트 키 설정")]
    [Header("키 설정")]
    public KeyCode bowStringPullKey = KeyCode.Alpha1;
    public KeyCode bowStringReleaseKey = KeyCode.Alpha2;
    public KeyCode arrowShootKey = KeyCode.Alpha3;
    public KeyCode arrowFlightKey = KeyCode.Alpha4;
    public KeyCode targetHitKey = KeyCode.Alpha5;
    public KeyCode targetDestroyKey = KeyCode.Alpha6;
    public KeyCode toggleMusicKey = KeyCode.M;
    public KeyCode pauseAllKey = KeyCode.P;
    public KeyCode resumeAllKey = KeyCode.R;
    public KeyCode stopAllKey = KeyCode.S;
    
    [Header("볼륨 테스트")]
    public KeyCode volumeUpKey = KeyCode.UpArrow;
    public KeyCode volumeDownKey = KeyCode.DownArrow;
    public KeyCode sfxVolumeUpKey = KeyCode.RightArrow;
    public KeyCode sfxVolumeDownKey = KeyCode.LeftArrow;
    
    private bool _isMusicPlaying = false;
    
    void Start()
    {
        if (!enableTestMode)
        {
            Debug.Log("오디오 테스트 모드가 비활성화되어 있습니다. Inspector에서 enableTestMode를 체크하세요.");
            return;
        }
        
        Debug.Log("=== 활게임 오디오 테스트 모드 활성화 ===");
        Debug.Log("1: 활 시위 당김 사운드");
        Debug.Log("2: 활 시위 놓기 사운드");
        Debug.Log("3: 화살 발사 사운드");
        Debug.Log("4: 화살 비행 사운드");
        Debug.Log("5: 타겟 히트 사운드");
        Debug.Log("6: 타겟 파괴 사운드");
        Debug.Log("M: 배경 음악 토글");
        Debug.Log("P: 모든 사운드 일시정지");
        Debug.Log("R: 모든 사운드 재개");
        Debug.Log("S: 모든 사운드 중지");
        Debug.Log("↑/↓: 마스터 볼륨 조절");
        Debug.Log("←/→: 효과음 볼륨 조절");
    }
    
    void Update()
    {
        if (!enableTestMode || BowAudioController.Instance == null) return;
        
        // 활 시위 사운드 테스트
        if (Input.GetKeyDown(bowStringPullKey))
        {
            Debug.Log("활 시위 당김 사운드 테스트");
            BowAudioController.Instance.PlayBowStringPullSound();
        }
        
        if (Input.GetKeyDown(bowStringReleaseKey))
        {
            Debug.Log("활 시위 놓기 사운드 테스트");
            BowAudioController.Instance.PlayBowStringReleaseSound();
        }
        
        // 화살 사운드 테스트
        if (Input.GetKeyDown(arrowShootKey))
        {
            Debug.Log("화살 발사 사운드 테스트");
            BowAudioController.Instance.PlayArrowShootSound();
        }
        
        if (Input.GetKeyDown(arrowFlightKey))
        {
            Debug.Log("화살 비행 사운드 테스트");
            BowAudioController.Instance.PlayArrowFlightSound();
        }
        
        // 타겟 사운드 테스트
        if (Input.GetKeyDown(targetHitKey))
        {
            Debug.Log("타겟 히트 사운드 테스트");
            BowAudioController.Instance.PlayTargetHitSound();
        }
        
        if (Input.GetKeyDown(targetDestroyKey))
        {
            Debug.Log("타겟 파괴 사운드 테스트");
            BowAudioController.Instance.PlayTargetDestroySound();
        }
        
        // 배경 음악 토글
        if (Input.GetKeyDown(toggleMusicKey))
        {
            if (_isMusicPlaying)
            {
                Debug.Log("배경 음악 중지");
                BowAudioController.Instance.StopAllSounds();
                _isMusicPlaying = false;
            }
            else
            {
                Debug.Log("배경 음악 재생");
                BowAudioController.Instance.PlayBackgroundMusic();
                _isMusicPlaying = true;
            }
        }
        
        // 전체 제어
        if (Input.GetKeyDown(pauseAllKey))
        {
            Debug.Log("모든 사운드 일시정지");
            BowAudioController.Instance.PauseAllSounds();
        }
        
        if (Input.GetKeyDown(resumeAllKey))
        {
            Debug.Log("모든 사운드 재개");
            BowAudioController.Instance.ResumeAllSounds();
        }
        
        if (Input.GetKeyDown(stopAllKey))
        {
            Debug.Log("모든 사운드 중지");
            BowAudioController.Instance.StopAllSounds();
            _isMusicPlaying = false;
        }
        
        // 볼륨 조절 테스트
        if (Input.GetKey(volumeUpKey))
        {
            float newVolume = Mathf.Min(1.0f, BowAudioController.Instance.masterVolume + Time.deltaTime);
            BowAudioController.Instance.SetMasterVolume(newVolume);
            Debug.Log($"마스터 볼륨: {newVolume:F2}");
        }
        
        if (Input.GetKey(volumeDownKey))
        {
            float newVolume = Mathf.Max(0.0f, BowAudioController.Instance.masterVolume - Time.deltaTime);
            BowAudioController.Instance.SetMasterVolume(newVolume);
            Debug.Log($"마스터 볼륨: {newVolume:F2}");
        }
        
        if (Input.GetKey(sfxVolumeUpKey))
        {
            float newVolume = Mathf.Min(1.0f, BowAudioController.Instance.sfxVolume + Time.deltaTime);
            BowAudioController.Instance.SetSFXVolume(newVolume);
            Debug.Log($"효과음 볼륨: {newVolume:F2}");
        }
        
        if (Input.GetKey(sfxVolumeDownKey))
        {
            float newVolume = Mathf.Max(0.0f, BowAudioController.Instance.sfxVolume - Time.deltaTime);
            BowAudioController.Instance.SetSFXVolume(newVolume);
            Debug.Log($"효과음 볼륨: {newVolume:F2}");
        }
    }
    
    /// <summary>
    /// 모든 사운드를 순차적으로 테스트합니다.
    /// </summary>
    [ContextMenu("모든 사운드 테스트")]
    public void TestAllSounds()
    {
        if (BowAudioController.Instance == null)
        {
            Debug.LogError("BowAudioController를 찾을 수 없습니다.");
            return;
        }
        
        StartCoroutine(TestAllSoundsCoroutine());
    }
    
    private System.Collections.IEnumerator TestAllSoundsCoroutine()
    {
        Debug.Log("=== 모든 사운드 테스트 시작 ===");
        
        // 활 시위 사운드
        Debug.Log("1. 활 시위 당김 사운드 테스트");
        BowAudioController.Instance.PlayBowStringPullSound();
        yield return new WaitForSeconds(1f);
        
        Debug.Log("2. 활 시위 놓기 사운드 테스트");
        BowAudioController.Instance.PlayBowStringReleaseSound();
        yield return new WaitForSeconds(1f);
        
        // 화살 사운드
        Debug.Log("3. 화살 발사 사운드 테스트");
        BowAudioController.Instance.PlayArrowShootSound();
        yield return new WaitForSeconds(1f);
        
        Debug.Log("4. 화살 비행 사운드 테스트");
        BowAudioController.Instance.PlayArrowFlightSound();
        yield return new WaitForSeconds(1f);
        
        // 타겟 사운드
        Debug.Log("5. 타겟 히트 사운드 테스트");
        BowAudioController.Instance.PlayTargetHitSound();
        yield return new WaitForSeconds(1f);
        
        Debug.Log("6. 타겟 파괴 사운드 테스트");
        BowAudioController.Instance.PlayTargetDestroySound();
        yield return new WaitForSeconds(1f);
        
        Debug.Log("=== 모든 사운드 테스트 완료 ===");
    }
    
    /// <summary>
    /// 현재 오디오 설정을 로그로 출력합니다.
    /// </summary>
    [ContextMenu("현재 오디오 설정 출력")]
    public void LogCurrentAudioSettings()
    {
        if (BowAudioController.Instance == null)
        {
            Debug.LogError("BowAudioController를 찾을 수 없습니다.");
            return;
        }
        
        Debug.Log("=== 현재 오디오 설정 ===");
        Debug.Log($"마스터 볼륨: {BowAudioController.Instance.masterVolume:F2}");
        Debug.Log($"효과음 볼륨: {BowAudioController.Instance.sfxVolume:F2}");
        Debug.Log($"배경음악 볼륨: {BowAudioController.Instance.musicVolume:F2}");
        Debug.Log($"사운드 거리: {BowAudioController.Instance.soundDistance:F1}");
        Debug.Log($"3D 사운드 사용: {BowAudioController.Instance.use3DSound}");
        Debug.Log($"배경 음악 재생 중: {_isMusicPlaying}");
    }
    
    void OnGUI()
    {
        if (!enableTestMode) return;
        
        // 화면에 테스트 정보 표시
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("=== 활게임 오디오 테스트 ===", GUI.skin.box);
        GUILayout.Label($"1: 활 시위 당김");
        GUILayout.Label($"2: 활 시위 놓기");
        GUILayout.Label($"3: 화살 발사");
        GUILayout.Label($"4: 화살 비행");
        GUILayout.Label($"5: 타겟 히트");
        GUILayout.Label($"6: 타겟 파괴");
        GUILayout.Label($"M: 배경음악 토글");
        GUILayout.Label($"P: 일시정지, R: 재개, S: 중지");
        GUILayout.Label($"↑/↓: 마스터 볼륨");
        GUILayout.Label($"←/→: 효과음 볼륨");
        
        if (BowAudioController.Instance != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"마스터 볼륨: {BowAudioController.Instance.masterVolume:F2}");
            GUILayout.Label($"효과음 볼륨: {BowAudioController.Instance.sfxVolume:F2}");
            GUILayout.Label($"배경음악 볼륨: {BowAudioController.Instance.musicVolume:F2}");
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.Label("오디오 컨트롤러 없음", GUI.skin.box);
        }
        
        GUILayout.EndArea();
    }
}
