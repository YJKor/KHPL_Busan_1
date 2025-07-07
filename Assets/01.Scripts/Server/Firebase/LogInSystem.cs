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
            // �α��� ����
            outputText.text = "�α��� : " + FirebaseAuthManager.Instance.UserId;

            //  �߰�: �α��� ���� �� "GameScene"���� �̵��մϴ�.
            // "GameScene"�� ConnManager�� �ִ� ���� �̸����� �����մϴ�.
            // Unity�� File > Build Settings�� �ش� ���� �߰��Ǿ� �־�� �մϴ�.
            SceneManager.LoadScene("LobbyMap");
        }
        else
        {
            // �α׾ƿ� ����
            outputText.text = "�α׾ƿ� �����Դϴ�.";
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
