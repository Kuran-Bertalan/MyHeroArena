using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Newtonsoft.Json.Linq;
using Photon.Pun;

public class AuthenticationFieldsManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;


    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("RefreshPassword")]
    public TMP_InputField resetPasswordEmailInput;

    [Header("AfterLoginUserData")]
    public TMP_Text userNameAfterLogin;

    [Header("UserData")]
    public TMP_InputField userNameText;
    public TMP_Text xpField;

    [Header("MultiplayerNameHolder")]
    const string playerNamePrefKey = "PlayerName";

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void ProfileButton()
    {
        StartCoroutine(LoadUserData());
    }

    public void HeroesButton()
    {
        StartCoroutine(LoadHeroesData());
    }

    public void ResetPasswordButton()
    {
        StartCoroutine(ResetPassword(resetPasswordEmailInput.text));
        AuthenticationUIManager.instance.ResetPasswordButton();
    }

    public void SaveHeroesButton()
    {
        //StartCoroutine(UpdateHeroes());
    }

    public void ProfileBackButton()
    { 
        if(PhotonNetwork.NickName != userNameText.text)
        {
            Debug.Log("Different name"); 
            StartCoroutine(UpdateUsernameAuth(userNameText.text));
            StartCoroutine(UpdateUsernameDatabase(userNameText.text));

            StartCoroutine(UpdateXp(int.Parse(xpField.text)));
            SetPlayerName(User.DisplayName);
            userNameAfterLogin.text = userNameText.text;
        }
        
    }

    public void SignOutButton()
    {
        auth.SignOut();
        AuthenticationUIManager.instance.LoginScreen();
        AuthenticationUIManager.instance.SignOutButton();
        ClearRegisterFields();
        ClearLoginFields();
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;


        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    // Clear the login fields
    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    // Clear the register fields
    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
            yield return new WaitForSeconds(2);
            warningLoginText.text = "";
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logging In...";
            yield return new WaitForSeconds(2);
            confirmLoginText.text = "";
            // User Name set for menu and for multiplayer
            SetPlayerName(User.DisplayName);
            string defaultName = string.Empty;
            if (User.DisplayName != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                }
            }
            //Multiplayerhez a név beállitás
            PhotonNetwork.NickName = defaultName;
            Debug.Log(PhotonNetwork.NickName);
            userNameAfterLogin.text = User.DisplayName;

            // ui kezelése
            AuthenticationUIManager.instance.LoginSuccess();
            
            // Clear adatok
            ClearRegisterFields();
            ClearLoginFields();
        }
    }

    

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
                yield return new WaitForSeconds(2);
                warningRegisterText.text = "";
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        StartCoroutine(UpdateUsernameDatabase(_username));
                        StartCoroutine(UpdateXp(1));
                        //Now return to login screen
                        AuthenticationUIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearRegisterFields();
                        ClearLoginFields();
                    }
                }
            }
        }
    }

    private IEnumerator ResetPassword(string _email)
    {
        var ResetPasswordTask = auth.SendPasswordResetEmailAsync(_email);
        yield return new WaitUntil(predicate: () => ResetPasswordTask.IsCompleted);
        if (ResetPasswordTask.Exception != null)
        {
            warningLoginText.text = "Error sending reset";
        }
        else
        {
            confirmLoginText.text = "Password reset sent";
        }
        resetPasswordEmailInput.text = "";
        yield return new WaitForSeconds(2);
        confirmLoginText.text = "";
        warningLoginText.text = "";
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateXp(int _xp)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("xp").SetValueAsync(_xp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
        }
    }

    //private IEnumerator UpdateHeroes()
    //{
    //    //Set the currently logged in user xp
    //    var DBTask = DBreference.Child("users").Child(User.UserId).Child("heroes").SetValueAsync(heroes);

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else
    //    {
    //        //heroes are now updated
    //    }
    //}


    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            userNameText.text = "No username";
            xpField.text = "1";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            userNameText.text = snapshot.Child("username").Value.ToString();
            xpField.text = snapshot.Child("xp").Value.ToString();
        }
    }

    private IEnumerator LoadHeroesData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("heroes").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet, Load default
            var DefaultTask = DBreference.Child("heroes").GetValueAsync();
            yield return new WaitUntil(predicate: () => DefaultTask.IsCompleted);
            DataSnapshot snapshot = DefaultTask.Result;
            var SetDefaultTask = DBreference.Child("users").Child(User.UserId).Child("heroes").SetRawJsonValueAsync(snapshot.GetRawJsonValue().ToString());
            yield return new WaitUntil(predicate: () => SetDefaultTask.IsCompleted);
            Debug.Log("Don't have heroes");
        }
        else
        {
            HeroSelectionDatabase heroSelection = new HeroSelectionDatabase();
            //Data has been retrieved, load the current heroes
            Debug.Log("U have heroes");
            JObject json = JObject.Parse(DBTask.Result.GetRawJsonValue());
            heroSelection.AddHeroesToList(json);
        }
    }
}
