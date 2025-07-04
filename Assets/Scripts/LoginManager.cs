using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro Input Fields
using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Threading.Tasks;

public class LoginManager : MonoBehaviour
{
    // UI references (assign these from the Inspector)
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI messageText;
    public GameObject toastMessageText;
    public GameObject loginPanel;     // Assign your LoginPanel
    public GameObject arSessionRoot;  // Assign XR Origin or a parent GameObject containing AR stuff
    public GameObject snapshotButton; // Assign Capture Button

    // Firebase auth instance
    private FirebaseAuth auth;

    // Called once at the start
    void Start()
    {
        Debug.Log("App starting");
        // Initialize Firebase dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                // Get the default instance of FirebaseAuth
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth ready");
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved: " + status);
            }
        });

        // Set up button click listener
        loginButton.onClick.AddListener(LoginUser);

        arSessionRoot.SetActive(false);      // Hide AR stuff at first
        snapshotButton.SetActive(false);     // Hide Capture button
        messageText.gameObject.SetActive(false); // Hide message text initially

    }

    // Method called when user clicks login
    void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Basic input validation
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowToast("Email or password cannot be empty!", 5f));
            return;
        }

        // Sign in with Firebase
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            // This code runs on a background thread!
            if (task.IsCanceled || task.IsFaulted)
            {
                // Switch to main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    StartCoroutine(ShowToast("Login failed. Check your credentials.", 5f));
                    Debug.LogError("Login error: " + task.Exception);
                });
            }
            else
            {
                FirebaseUser user = task.Result.User;
                // Switch to main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    StartCoroutine(ShowToast("Login successful! Welcome, " + user.Email, 5f));
                    Debug.Log("User logged in: " + user.Email);
                    loginPanel.SetActive(false);
                    arSessionRoot.SetActive(true);
                    snapshotButton.SetActive(true);
                });
            }
        });
    }


    IEnumerator ShowToast(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageText.gameObject.SetActive(false);
    }

}