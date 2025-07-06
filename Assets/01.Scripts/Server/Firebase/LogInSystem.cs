using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        outputText.text = sign ? "로그인 : " : "로그아웃 : ";
        outputText.text += FirebaseAuthManager.Instance.UserId;

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
