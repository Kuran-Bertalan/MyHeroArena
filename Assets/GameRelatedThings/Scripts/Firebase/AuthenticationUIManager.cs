using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthenticationUIManager : MonoBehaviour
{
    public static AuthenticationUIManager instance;

    //Sections UI
    public GameObject loginUI;
    public GameObject registerUI;

    [Header("Logo")]
    public GameObject logo;

    [Header("RefreshPassword")]
    public GameObject resetPasswordPanel; 
    public GameObject resetPasswordResetButton;
    public GameObject resetPasswordConfrimResetButton;
    public GameObject resetPasswordBackButton;

    [Header("AfterLoginUI")]
    public GameObject gamenameLogin;
    public GameObject backgroundLogin;
    public GameObject menuUI;
    public GameObject authenticationUI;

    [Header("UserData")]
    public GameObject signOutButton;
    public GameObject profileButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ResetPasswordButton()
    {
        resetPasswordPanel.SetActive(false);
        loginUI.SetActive(true);
    }

    public void SignOutButton()
    {
        menuUI.SetActive(false);
        authenticationUI.SetActive(true);
    }

    public void LoginSuccess()
    {
        authenticationUI.SetActive(false);
        menuUI.SetActive(true);
    }

    public void LoginScreen()
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
        resetPasswordResetButton.SetActive(true);
    }
    public void RegisterScreen()
    {
        loginUI.SetActive(false);
        resetPasswordResetButton.SetActive(false);
        registerUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
