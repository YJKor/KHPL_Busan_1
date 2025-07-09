using UnityEngine;
using Firebase.Auth;
using System;



public class FirebaseAuthManager
{

    private static FirebaseAuthManager _instance = null;

    public static FirebaseAuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FirebaseAuthManager();
            }

            return _instance;
        }
    }


    public FirebaseAuth auth;
    public FirebaseUser user;



    public static string LoggedInUserID { get; private set; }
    public string UserId => user != null ? user.UserId : "";


    public Action<bool> LoginState;
    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            LogOut();
        }

        auth.StateChanged += OnChanged;
    }


    void OnChanged(object sender, EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            bool isSigned = (auth.CurrentUser != null); // ���� �ܼ�ȭ
            user = auth.CurrentUser;

            if (isSigned)
            {
                Debug.Log($"�α���: {user.UserId}");
                LoggedInUserID = user.UserId; 
                LoginState?.Invoke(true);
            }
            else
            {
                Debug.Log("�α׾ƿ�");
                LoggedInUserID = null; 
                LoginState?.Invoke(false);
            }
        }
    }



    public void Create(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("����");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("ȸ������ ����");

        });

    }


    public void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�α��� ���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("�α��� ����");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("�α��� ����");

        });


    }

    public void LogOut()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("�α׾ƿ�");
            LoggedInUserID = null;
        }
    }


    public void GameExit()
    {
        Application.Quit();
        Debug.Log("��������");
        LoggedInUserID = null;
    }

}
