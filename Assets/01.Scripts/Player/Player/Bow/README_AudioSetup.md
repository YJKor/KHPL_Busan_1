# 활게임 오디오 시스템 설정 가이드

이 문서는 Defense_Map 씬의 활게임에 오디오 소스를 추가하는 방법을 설명합니다.

## 📋 목차

1. [시스템 개요](#시스템-개요)
2. [필요한 오디오 파일](#필요한-오디오-파일)
3. [설치 및 설정](#설치-및-설정)
4. [스크립트 설명](#스크립트-설명)
5. [UI 설정](#ui-설정)
6. [문제 해결](#문제-해결)

## 🎯 시스템 개요

활게임 오디오 시스템은 다음과 같은 기능을 제공합니다:

### 주요 기능
- **활 시위 사운드**: 시위 당김/놓기 사운드
- **화살 사운드**: 발사 및 비행 사운드
- **타겟 사운드**: 히트 및 파괴 사운드
- **배경 음악**: 게임 배경 음악
- **3D 사운드**: 공간적 오디오 효과
- **볼륨 제어**: 개별 볼륨 조절
- **설정 저장**: PlayerPrefs를 통한 설정 저장

### 지원하는 사운드 타입
- 활 시위 당김 사운드
- 활 시위 놓기 사운드
- 화살 발사 사운드
- 화살 비행 사운드
- 타겟 히트 사운드
- 타겟 파괴 사운드
- 배경 음악

## 🎵 필요한 오디오 파일

다음 오디오 파일들을 `Assets/05.AudioClip/` 폴더에 추가하세요:

### 필수 오디오 파일
```
Assets/05.AudioClip/
├── bow_string_pull.wav      # 활 시위 당김 사운드
├── bow_string_release.wav   # 활 시위 놓기 사운드
├── arrow_shoot.wav          # 화살 발사 사운드
├── arrow_flight.wav         # 화살 비행 사운드
├── target_hit.wav           # 타겟 히트 사운드
├── target_destroy.wav       # 타겟 파괴 사운드
└── background_music.mp3     # 배경 음악
```

### 오디오 파일 권장 사양
- **포맷**: WAV (효과음), MP3 (배경음악)
- **샘플레이트**: 44.1kHz
- **비트 깊이**: 16-bit
- **채널**: Mono (효과음), Stereo (배경음악)
- **압축**: 무손실 (효과음), 손실 압축 (배경음악)

## ⚙️ 설치 및 설정

### 1단계: 오디오 컨트롤러 추가

1. **Defense_Map 씬을 엽니다**
2. **빈 GameObject 생성**:
   - Hierarchy에서 우클릭 → Create Empty
   - 이름을 "BowAudioController"로 변경
3. **BowAudioController 스크립트 추가**:
   - 생성한 GameObject에 `BowAudioController` 스크립트 추가
4. **오디오 클립 할당**:
   - Inspector에서 각 오디오 슬롯에 해당하는 오디오 파일 할당

### 2단계: 기존 스크립트 업데이트

#### XRPullInteractable 업데이트
- 기존 `XRPullInteractable` 스크립트가 자동으로 오디오 기능을 사용합니다
- 활 시위 당김/놓기 시 자동으로 사운드가 재생됩니다

#### ArrowController 업데이트
- 기존 `ArrowController`를 `ArrowControllerWithAudio`로 교체하거나
- 기존 스크립트에 오디오 기능이 통합되어 있습니다

### 3단계: 오디오 설정 UI 추가 (선택사항)

1. **Canvas 생성**:
   - Hierarchy에서 우클릭 → UI → Canvas
2. **오디오 설정 패널 생성**:
   - Canvas 하위에 Panel 생성
   - 이름을 "AudioSettingsPanel"로 변경
3. **UI 요소 추가**:
   - 볼륨 슬라이더들
   - 3D 사운드 토글
   - 제어 버튼들
4. **BowAudioSettingsUI 스크립트 추가**:
   - AudioSettingsPanel에 스크립트 추가
   - Inspector에서 UI 요소들을 연결

## 📜 스크립트 설명

### BowAudioController.cs
활게임의 모든 오디오를 관리하는 메인 컨트롤러입니다.

#### 주요 기능
- **싱글톤 패턴**: 씬 전체에서 하나의 인스턴스만 존재
- **다중 오디오 소스**: 효과음, 배경음악, 활 사운드 분리
- **3D 사운드 지원**: 공간적 오디오 효과
- **볼륨 제어**: 개별 볼륨 조절 기능

#### 사용법
```csharp
// 사운드 재생
BowAudioController.Instance.PlayBowStringPullSound();
BowAudioController.Instance.PlayArrowShootSound();
BowAudioController.Instance.PlayTargetHitSound();

// 볼륨 설정
BowAudioController.Instance.SetMasterVolume(0.8f);
BowAudioController.Instance.SetSFXVolume(0.9f);
BowAudioController.Instance.SetMusicVolume(0.6f);
```

### ArrowControllerWithAudio.cs
화살 충돌 시 오디오 효과를 처리하는 스크립트입니다.

#### 주요 기능
- **자동 오디오 재생**: 충돌 시 자동으로 사운드 재생
- **기존 시스템 호환**: 기존 ArrowController와 호환
- **다양한 타겟 지원**: Enemy, Target, DestructibleObject 등

### BowAudioSettingsUI.cs
오디오 설정을 위한 UI 스크립트입니다.

#### 주요 기능
- **실시간 볼륨 조절**: 슬라이더를 통한 즉시 반영
- **설정 저장/불러오기**: PlayerPrefs를 통한 영구 저장
- **3D 사운드 토글**: 3D 사운드 활성화/비활성화
- **전체 제어**: 일시정지, 재개, 중지 기능

## 🎨 UI 설정

### 기본 UI 구조
```
Canvas
└── AudioSettingsPanel
    ├── MasterVolumeSlider
    ├── SFXVolumeSlider
    ├── MusicVolumeSlider
    ├── SoundDistanceSlider
    ├── Use3DSoundToggle
    ├── PauseAllButton
    ├── ResumeAllButton
    ├── StopAllButton
    └── SettingsToggleButton
```

### UI 요소 설정

#### 슬라이더 설정
- **Master Volume Slider**:
  - Min Value: 0
  - Max Value: 1
  - Default Value: 1
- **SFX Volume Slider**:
  - Min Value: 0
  - Max Value: 1
  - Default Value: 0.8
- **Music Volume Slider**:
  - Min Value: 0
  - Max Value: 1
  - Default Value: 0.6
- **Sound Distance Slider**:
  - Min Value: 1
  - Max Value: 50
  - Default Value: 10

#### 토글 설정
- **Use 3D Sound Toggle**:
  - Default Value: true
  - Is On: true

## 🔧 문제 해결

### 일반적인 문제들

#### 1. 사운드가 재생되지 않음
**원인**: 오디오 파일이 할당되지 않았거나 AudioSource가 없음
**해결책**:
- Inspector에서 오디오 클립이 할당되었는지 확인
- AudioSource 컴포넌트가 있는지 확인
- 볼륨이 0이 아닌지 확인

#### 2. 3D 사운드가 작동하지 않음
**원인**: Spatial Blend 설정 문제
**해결책**:
- AudioSource의 Spatial Blend가 1.0으로 설정되었는지 확인
- Max Distance가 적절히 설정되었는지 확인

#### 3. 배경 음악이 반복되지 않음
**원인**: Loop 설정 문제
**해결책**:
- Music AudioSource의 Loop가 체크되었는지 확인

#### 4. 볼륨 설정이 저장되지 않음
**원인**: PlayerPrefs 저장 문제
**해결책**:
- BowAudioSettingsUI의 SaveSettings() 함수가 호출되는지 확인
- PlayerPrefs.Save()가 호출되는지 확인

### 디버깅 팁

#### 로그 확인
```csharp
// 오디오 컨트롤러 존재 확인
if (BowAudioController.Instance != null)
{
    Debug.Log("오디오 컨트롤러가 정상적으로 작동 중입니다.");
}
else
{
    Debug.LogError("오디오 컨트롤러를 찾을 수 없습니다.");
}
```

#### 볼륨 확인
```csharp
// 현재 볼륨 값 확인
Debug.Log($"마스터 볼륨: {BowAudioController.Instance.masterVolume}");
Debug.Log($"효과음 볼륨: {BowAudioController.Instance.sfxVolume}");
Debug.Log($"배경음악 볼륨: {BowAudioController.Instance.musicVolume}");
```

## 📝 추가 정보

### 성능 최적화
- 오디오 파일은 적절한 압축을 사용하세요
- 불필요한 오디오 소스는 제거하세요
- 3D 사운드는 필요한 경우에만 사용하세요

### 확장 가능성
- 새로운 사운드 타입 추가 가능
- 커스텀 오디오 이벤트 시스템 구축 가능
- 실시간 오디오 믹싱 시스템 추가 가능

### 호환성
- 기존 활게임 시스템과 완전 호환
- VR 및 데스크톱 환경 모두 지원
- 다양한 Unity 버전에서 작동

---

**참고**: 이 시스템은 Defense_Map 씬의 활게임에 특화되어 있지만, 다른 씬에서도 재사용할 수 있습니다.
