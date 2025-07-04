using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// INetworkRunnerCallbacks: Fusion �������� �߻��ϴ� �̺�Ʈ(�÷��̾� ����/���� ��)�� �ޱ� ���� �ʿ��մϴ�.
public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // �̱��� �ν��Ͻ�: ��� ��ũ��Ʈ������ PhotonManager.Instance �� �� ��ũ��Ʈ�� ������ �� �ֽ��ϴ�.
    public static PhotonManager Instance { get; private set; }

    private NetworkRunner _runner; // Fusion ��Ʈ��ũ�� �����ϰ� �����ϴ� �ٽ� ������Ʈ

    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �� ������Ʈ�� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {

    }

    // Firebase �͸� �α��� ���� �� ȣ��� �Լ�
    public async void ConnectToLobby()
    {
        // _runner�� ������ ���� �߰�
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
            // ���� ����!
            Debug.Log("Photon Fusion ������ ����");
            // ���⿡ �κ� ������ �̵��ϰų�, �κ� UI�� Ȱ��ȭ�ϴ� �ڵ带 �߰��� �� �ֽ��ϴ�.
        }
        else
        {
            // ���� ����
            Debug.LogError($" Fusion ���� ���� ����: {result.ShutdownReason}");
        }
    }

    // --- INetworkRunnerCallbacks �������̽� ���� ---
    // �� �Ʒ� �Լ����� Fusion �������� Ư�� �̺�Ʈ�� �߻����� �� �ڵ����� ȣ��˴ϴ�.

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {
            Debug.Log($"OnPlayerJoined, Spawning player for {player.PlayerId}");

            // ������ ��ġ�� _playerPrefab�� ��Ʈ��ũ�� �����ϰ�, 'player'���� �������� �ο��մϴ�.
            runner.Spawn(_playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"�÷��̾� {player.PlayerId}�� �κ񿡼� �������ϴ�.");
    }

    // ������ �ݹ� �Լ��� (���� ���� �ʿ� ������ �������̽� ������ ���� ���ܵӴϴ�)
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