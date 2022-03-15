using Firebase;
using Firebase.Analytics;
using Firebase.Auth;

using TMPro;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    FirebaseApp app;
    FirebaseAuth auth;
    System.EventHandler AuthStateChanged;
    public Firebase.Auth.FirebaseUser currentUser;

    public TMP_Text register_email;
    public TMP_Text register_password;

    public TMP_Text login_email;
    public TMP_Text login_password;

    // Start is called before the first frame update
    void Start()
    {
        InitializeFirebase();
    }

    // Initialize firebase
    void InitializeFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                // Get auth instance
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Registro
    public void RegisterUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(register_email.text, register_password.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(login_email.text, login_password.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            currentUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                currentUser.DisplayName, currentUser.UserId);
        });
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
