# 활쏘기 시스템 사용 가이드

이 문서는 Unity VR에서 사용할 수 있는 완전한 활쏘기 시스템의 사용법을 설명합니다.

## 시스템 구성 요소

### 1. ArrowLauncher.cs
- **역할**: 화살 발사 및 비행을 담당
- **주요 기능**:
  - XR Pull Interactable과 연동
  - 화살 발사 물리 처리
  - 비행 중 회전 및 트레일 효과
  - 발사 이펙트 및 사운드

### 2. ArrowImpactHandler.cs
- **역할**: 화살 충돌 처리 및 데미지 시스템
- **주요 기능**:
  - 충돌 감지 및 처리
  - 데미지 계산 및 적용
  - 이펙트 및 사운드 재생
  - 점수 시스템 연동

### 3. BowController.cs
- **역할**: 활 시위 조작 및 전체 시스템 관리
- **주요 기능**:
  - 시위 시각적 표현 (LineRenderer)
  - 화살 장착/해제 감지
  - 자동 화살 생성
  - 이벤트 시스템

## 설정 방법

### 1. 활 오브젝트 설정

1. **활 오브젝트 생성**:
   ```
   - 빈 GameObject 생성
   - BowController 스크립트 추가
   - XRGrabInteractable 컴포넌트 추가
   ```

2. **시위 설정**:
   ```
   - LineRenderer 컴포넌트 추가
   - 시위 시작점/끝점 Transform 설정
   - XRSocketInteractor 추가 (화살 장착용)
   ```

3. **화살 소켓 설정**:
   ```
   - 활에 자식 오브젝트로 소켓 생성
   - XRSocketInteractor 컴포넌트 추가
   - 소켓 위치를 시위 중앙에 배치
   ```

### 2. 화살 프리팹 설정

1. **화살 오브젝트 생성**:
   ```
   - 3D 모델 또는 기본 오브젝트 사용
   - Rigidbody 컴포넌트 추가
   - Collider 컴포넌트 추가
   ```

2. **스크립트 추가**:
   ```
   - ArrowLauncher 스크립트 추가
   - ArrowImpactHandler 스크립트 추가
   - XRGrabInteractable 컴포넌트 추가
   ```

3. **프리팹으로 저장**:
   ```
   - 설정 완료 후 Prefab으로 저장
   - BowController의 Arrow Prefab에 할당
   ```

### 3. 타겟 설정

1. **타겟 오브젝트 생성**:
   ```
   - 3D 모델 또는 기본 오브젝트 사용
   - Collider 컴포넌트 추가
   - "Target" 태그 설정
   ```

2. **컴포넌트 추가**:
   ```
   - HealthSystem 스크립트 추가
   - TargetController 스크립트 추가 (선택사항)
   ```

## 사용법

### 1. 기본 활쏘기

1. **활 잡기**: VR 컨트롤러로 활을 잡습니다
2. **화살 장착**: 화살을 활의 소켓에 끼웁니다
3. **시위 당기기**: 다른 손으로 시위를 당깁니다
4. **발사**: 시위를 놓으면 화살이 발사됩니다

### 2. 고급 기능

- **자동 화살 생성**: 설정된 간격으로 자동 생성
- **점수 시스템**: 타겟 맞추기 시 점수 획득
- **이펙트 시스템**: 발사/충돌 시 시각/청각 효과
- **물리 시뮬레이션**: 현실적인 화살 비행

## 주요 설정 옵션

### ArrowLauncher 설정
```csharp
[Header("Launch Settings")]
public float _speed = 15f;           // 발사 속도
public float _spinForce = 10f;       // 회전력
public float _maxDistance = 50f;     // 최대 거리

[Header("Visual Effects")]
public GameObject _trailSystem;      // 트레일 시스템
public GameObject _launchEffectPrefab; // 발사 이펙트
```

### ArrowImpactHandler 설정
```csharp
[Header("Arrow Settings")]
public int damage = 15;              // 데미지
public float destroyDelay = 3f;      // 파괴 지연 시간

[Header("Effects")]
public GameObject hitEffectPrefab;   // 충돌 이펙트
public AudioClip hitSound;          // 충돌 사운드
```

### BowController 설정
```csharp
[Header("Shooting")]
public float shootingForceMultiplier = 25f; // 발사 힘 배수
public float maxPullDistance = 0.6f;        // 최대 당김 거리

[Header("Arrow System")]
public float arrowSpawnInterval = 2f;       // 화살 생성 간격
public int maxArrows = 10;                  // 최대 화살 수
```

## 이벤트 시스템

### 사용 가능한 이벤트
```csharp
// BowController 이벤트
OnPullStrengthChanged(float strength)  // 당김 강도 변경
OnArrowReleased()                      // 화살 발사
OnArrowCountChanged(int count)         // 화살 수 변경

// ArrowLauncher 이벤트
OnArrowLaunched()                      // 화살 발사 시작
OnArrowStopped()                       // 화살 비행 중지
```

### 이벤트 사용 예시
```csharp
void Start()
{
    bowController.OnPullStrengthChanged += (strength) => {
        Debug.Log($"당김 강도: {strength}");
    };
    
    bowController.OnArrowReleased += () => {
        Debug.Log("화살이 발사되었습니다!");
    };
}
```

## 문제 해결

### 일반적인 문제들

1. **화살이 발사되지 않음**:
   - XRPullInteractable 컴포넌트 확인
   - 시위 소켓 설정 확인
   - 화살 프리팹의 Rigidbody 설정 확인

2. **충돌이 감지되지 않음**:
   - 화살과 타겟의 Collider 설정 확인
   - ArrowImpactHandler 컴포넌트 확인
   - 태그 설정 확인

3. **시위가 표시되지 않음**:
   - LineRenderer 컴포넌트 확인
   - 시위 시작점/끝점 Transform 설정 확인
   - Material 설정 확인

### 디버그 모드
```csharp
// BowController에서 디버그 로그 활성화
[SerializeField] private bool enableDebugLogs = true;
```

## 성능 최적화

1. **오브젝트 풀링**: 화살 재사용을 위한 오브젝트 풀링 구현
2. **LOD 시스템**: 거리에 따른 렌더링 최적화
3. **이펙트 제한**: 동시 재생 이펙트 수 제한
4. **메모리 관리**: 사용하지 않는 화살 자동 파괴

## 확장 가능성

이 시스템은 다음과 같이 확장할 수 있습니다:

- **다양한 활 타입**: 다른 특성을 가진 활들
- **특수 화살**: 폭발, 얼음, 화염 등 특수 효과
- **멀티플레이어**: 네트워크 멀티플레이어 지원
- **AI 적**: 자동으로 움직이는 적 타겟
- **미션 시스템**: 다양한 미션과 목표

## 라이센스

이 시스템은 Unity 프로젝트에서 자유롭게 사용할 수 있습니다. 