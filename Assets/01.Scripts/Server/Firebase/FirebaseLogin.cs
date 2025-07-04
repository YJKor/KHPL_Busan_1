using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLogin : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        // Firebase ���� ��ü �ʱ�ȭ
        auth = FirebaseAuth.DefaultInstance;
        LoginAnonymously();
    }

    // �͸� �α����� �񵿱� ������� �õ��ϴ� �Լ�
    public async Task LoginAnonymously()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log($"�̹� �α��εǾ� �ֽ��ϴ�: {auth.CurrentUser.UserId}");
            return;
        }

        try
        {
            // �͸� �α����� �õ��ϰ� ����� �� ������ ��ٸ��ϴ�.
            AuthResult result = await auth.SignInAnonymouslyAsync();
            FirebaseUser newUser = result.User;

            // ���� ��, ������ ����� ID(UID)�� �α׷� ����մϴ�.
            Debug.Log($"�͸� �α��� ����! UID: {newUser.UserId}");
            PhotonManager.Instance.ConnectToLobby();
        }
        catch (System.Exception ex)
        {
            // ���� ��, ���� �޽����� ����մϴ�.
            Debug.LogError($"�͸� �α��� ����: {ex.Message}");
        }
    }
}
