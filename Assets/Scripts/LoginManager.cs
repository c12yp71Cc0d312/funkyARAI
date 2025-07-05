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
    IEnumerator Start()
    {
        Debug.Log("App starting");

        loginButton.interactable = false; // Disable login button until Firebase is ready

        // Wait until FirebaseInit signals readiness
        yield return new WaitUntil(() => FirebaseInit.IsFirebaseReady);
        auth = FirebaseInit.auth;

        Debug.Log("Firebase ready inside LoginManager");

        // Set listener after Firebase is ready
        loginButton.onClick.AddListener(LoginUser);
        loginButton.interactable = true;

        arSessionRoot.SetActive(false);
        snapshotButton.SetActive(false);
        messageText.gameObject.SetActive(false);
        Debug.Log("UI initialization completed");

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