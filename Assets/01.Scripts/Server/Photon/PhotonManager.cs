using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// INetworkRunnerCallbacks: Fusion 서버에서 발생하는 이벤트(플레이어 입장/퇴장 등)를 받기 위해 필요합니다.
public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // 싱글톤 인스턴스: 어느 스크립트에서든 PhotonManager.Instance 로 이 스크립트에 접근할 수 있습니다.
    public static PhotonManager Instance { get; private set; }

    private NetworkRunner _runner; // Fusion 네트워크를 실행하고 관리하는 핵심 컴포넌트

    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 이 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {

    }

    // Firebase 익명 로그인 성공 후 호출될 함수
    public async void ConnectToLobby()
    {
        // _runner가 없으면 새로 추가
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "Lobby-Session",
            //Scene = SceneManager.GetActiveScene().buildIndex,
            PlayerCount = 20,


            
        });

        if (result.Ok)
        {
            // 접속 성공!
            Debug.Log("Photon Fusion 서버에 접속");
            // 여기에 로비 씬으로 이동하거나, 로비 UI를 활성화하는 코드를 추가할 수 있습니다.
        }
        else
        {
            // 접속 실패
            Debug.LogError($" Fusion 서버 접속 실패: {result.ShutdownReason}");
        }
    }

    // --- INetworkRunnerCallbacks 인터페이스 구현 ---
    // 이 아래 함수들은 Fusion 서버에서 특정 이벤트가 발생했을 때 자동으로 호출됩니다.

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {
            Debug.Log($"OnPlayerJoined, Spawning player for {player.PlayerId}");

            // 지정된 위치에 _playerPrefab을 네트워크상에 생성하고, 'player'에게 소유권을 부여합니다.
            runner.Spawn(_playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 {player.PlayerId}가 로비에서 나갔습니다.");
    }

    // 나머지 콜백 함수들 (지금 당장 필요 없지만 인터페이스 구현을 위해 남겨둡니다)
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
}