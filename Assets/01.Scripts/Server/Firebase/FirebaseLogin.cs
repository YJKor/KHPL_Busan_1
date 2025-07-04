using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLogin : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        // Firebase 인증 객체 초기화
        auth = FirebaseAuth.DefaultInstance;
        LoginAnonymously();
    }

    // 익명 로그인을 비동기 방식으로 시도하는 함수
    public async Task LoginAnonymously()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log($"이미 로그인되어 있습니다: {auth.CurrentUser.UserId}");
            return;
        }

        try
        {
            // 익명 로그인을 시도하고 결과가 올 때까지 기다립니다.
            AuthResult result = await auth.SignInAnonymouslyAsync();
            FirebaseUser newUser = result.User;

            // 성공 시, 고유한 사용자 ID(UID)를 로그로 출력합니다.
            Debug.Log($"익명 로그인 성공! UID: {newUser.UserId}");
            PhotonManager.Instance.ConnectToLobby();
        }
        catch (System.Exception ex)
        {
            // 실패 시, 에러 메시지를 출력합니다.
            Debug.LogError($"익명 로그인 실패: {ex.Message}");
        }
    }
}
