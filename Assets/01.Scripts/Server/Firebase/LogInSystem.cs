using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogInSystem : MonoBehaviour
{

    public TMP_InputField email;
    public TMP_InputField password;

    public Text outputText;
    

    void Start()
    {

        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
    }

    private void OnChangedState(bool sign)
    {
        if (sign)
        {
            // 로그인 성공
            outputText.text = "로그인 : " + FirebaseAuthManager.Instance.UserId;

            //  추가: 로그인 성공 시 "GameScene"으로 이동합니다.
            // "GameScene"은 ConnManager가 있는 씬의 이름으로 가정합니다.
            // Unity의 File > Build Settings에 해당 씬이 추가되어 있어야 합니다.
            SceneManager.LoadScene("LobbyMap");
        }
        else
        {
            // 로그아웃 상태
            outputText.text = "로그아웃 상태입니다.";
        }
    }

    public void Create()
    {

        FirebaseAuthManager.Instance.Create(email.text, password.text);
    }

    public void LogIn()
    {
        FirebaseAuthManager.Instance.LogIn(email.text, password.text);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }
}
