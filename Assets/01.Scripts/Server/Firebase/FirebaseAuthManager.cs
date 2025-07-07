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
                Debug.Log("로그아웃");
                LoginState?.Invoke(false);
            }

            user = auth.CurrentUser;
            
            if (isSigned)
            {
                Debug.Log("로그인");
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
                Debug.LogError("취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("실패");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("회원가입 성공");

        });
    
    }


    public void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("로그인 성공");

        });

       
    }

    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("LogOut");
    }




}
