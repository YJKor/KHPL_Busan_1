using System.Collections;
using UnityEngine;
using TMPro;
using Firebase;
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

    public string UserId => user.UserId;


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


    void OnChanged(object sender,EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            bool isSigned = (auth.CurrentUser != user && auth.CurrentUser != null);
            if (!isSigned && user != null)
            {
                Debug.Log("�α׾ƿ�");
                LoginState?.Invoke(false);
            }

            user = auth.CurrentUser;
            
            if (isSigned)
            {
                Debug.Log("�α���");
                LoginState?.Invoke(true);


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
        auth.SignOut();
        Debug.Log("LogOut");
    }




}
