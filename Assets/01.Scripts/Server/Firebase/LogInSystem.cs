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

    
    

    void Start()
    {
        FirebaseAuthManager.Instance.user = null;
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
    }

    private void OnChangedState(bool sign)
    {
        if (sign)
        {
            
            SceneManager.LoadScene("LobbyMap");
        }
        else
        {
            // 로그아웃 상태
            
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
